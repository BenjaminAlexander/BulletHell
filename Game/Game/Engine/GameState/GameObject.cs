using System;
using System.Collections.Generic;
using MyGame.Engine.GameState.Instants;
using MyGame.Engine.Utils;
using MyGame.Engine.GameState.Fields;
using System.Collections.Concurrent;
using MyGame.Engine.GameState.Utils;
using MyGame.Engine.GameState.TypeSets;

namespace MyGame.Engine.GameState
{
    public abstract class GameObject
    {
        private static Logger log = new Logger(typeof(GameObject));

        //TODO: Change id to initial instant, and type sequence
        private TypeSetInterface globalTypeSet = null;
        private Nullable<int> id = null;
        private List<AbstractField> fieldDefinitions;
        private ConcurrentDictionary<int, DeserializedInfo> infoDict = new ConcurrentDictionary<int, DeserializedInfo>();

        private DeserializedInfo GetOrCreateInfo(int instantId)
        {
            DeserializedInfo info;
            if (infoDict.ContainsKey(instantId))
            {
                info = infoDict[instantId];
            }
            else
            {
                info = new DeserializedInfo();
                infoDict[instantId] = info;
            }
            return info;
        }

        internal int TypeID
        {
            get
            {
                return globalTypeSet.TypeID;
            }
        }

        internal int ID
        {
            get
            {
                if(id == null)
                {
                    throw new Exception("Game Objects must have SetUp called before use");
                }
                return (int)id;
            }
        }

        internal void SetUp(int id, TypeSetInterface globalTypeSet)
        {
            if (id == 0)
            {
                throw new Exception("0 is cannot be used as an object ID");
            }
            this.id = id;
            this.globalTypeSet = globalTypeSet;
            this.fieldDefinitions = new List<AbstractField>();
            this.DefineFields(new CreationToken(this));
        }

        internal void AddField(AbstractField field)
        {
            fieldDefinitions.Add(field);
        }

        internal void SetDefaultValue(InstantTypeSetInterface instantTypeSet)
        {
            lock(infoDict)
            {
                DeserializedInfo info = GetOrCreateInfo(instantTypeSet.InstantID);
                if (!info.IsDeserialized)
                {
                    info.IsDeserialized = false;
                    foreach (AbstractField field in fieldDefinitions)
                    {
                        field.SetDefaultValue(instantTypeSet.InstantID);
                    }
                    instantTypeSet.Add(this);
                }
            }
        }

        internal void CopyFields(int fromInstant, InstantTypeSetInterface toInstant)
        {
            lock (infoDict)
            {
                DeserializedInfo fromInfo;
                infoDict.TryGetValue(fromInstant, out fromInfo);
                if (fromInfo == null)
                {
                    log.Warn("CopyFields: fromInstantDoes not exist");
                    return;
                }

                DeserializedInfo toInfo = GetOrCreateInfo(toInstant.InstantID);

                if (!toInfo.IsDeserialized)
                {
                    foreach (AbstractField field in fieldDefinitions)
                    {
                        field.CopyFieldValues(fromInstant, toInstant.InstantID);
                    }
                    toInstant.Add(this);
                }
            }
        }

        internal void RemoveForUpdate(InstantTypeSetInterface typeSet)
        {
            lock (infoDict)
            {
                DeserializedInfo info;
                infoDict.TryGetValue(typeSet.InstantID, out info);
                if (info == null)
                {
                    log.Warn("RemoveForUpdate: instant does not exist");
                    return;
                }

                if (!info.IsDeserialized)
                {
                    foreach (AbstractField field in fieldDefinitions)
                    {
                        field.RemoveInstant(typeSet.InstantID);
                    }
                    typeSet.Remove(this);

                    if(!infoDict.TryRemove(typeSet.InstantID, out info))
                    {
                        log.Error("RemoveForUpdate: failed to remove instantInfo");
                    }
                }
            }
        }

        //Can this get special consideration because it is only use for deserialization?
        internal void DeserializeRemove(InstantTypeSetInterface typeSet)
        {
            lock (infoDict)
            {
                DeserializedInfo info;
                infoDict.TryGetValue(typeSet.InstantID, out info);
                if (info == null)
                {
                    log.Warn("DeserializeRemove: instant does not exist");
                    return;
                }

                foreach (AbstractField field in fieldDefinitions)
                {
                    field.RemoveInstant(typeSet.InstantID);
                }
                typeSet.Remove(this);

                if (!infoDict.TryRemove(typeSet.InstantID, out info))
                {
                    log.Error("DeserializeRemove: failed to remove instantInfo");
                }
            }
        }

        //Returns true if the value has changed
        internal bool Deserialize(InstantTypeSetInterface instantTypeSet, byte[] buffer, ref int bufferOffset)
        {
            bool isChanged = false;
            lock (infoDict)
            {
                DeserializedInfo info = GetOrCreateInfo(instantTypeSet.InstantID);
                if (info.IsDeserialized)
                {
                    log.Debug("Deserializeing an object into an instant that has already been deserialized.");
                }
                foreach (AbstractField field in fieldDefinitions)
                {
                    isChanged = isChanged | field.Deserialize(instantTypeSet.InstantID, buffer, ref bufferOffset);
                }
                instantTypeSet.Add(this);
                info.IsDeserialized = true;
            }
            return isChanged;
        }

        public byte[] Serialize(int instantId)
        {
            byte[] buffer = null;

            lock (infoDict)
            {
                DeserializedInfo info;
                infoDict.TryGetValue(instantId, out info);
                if (info == null)
                {
                    log.Warn("Serialize: instant does not exist");
                    return null;
                }

                int serializationSize = 0;
                foreach (AbstractField field in fieldDefinitions)
                {
                    serializationSize = serializationSize + field.SerializationSize(instantId);
                }

                int bufferOffset = 0;
                buffer = new byte[serializationSize];
                foreach (AbstractField field in fieldDefinitions)
                {
                    field.Serialize(instantId, buffer, ref bufferOffset);
                }
            }
            return buffer;
        }

        internal T GetValue<T>(int instantId, GenericField<T> field)
        {
            lock (infoDict)
            {
                DeserializedInfo info;
                infoDict.TryGetValue(instantId, out info);
                if (info != null)
                {
                    return field.ForceGetValue(instantId);
                }
                else
                {
                    throw new ObjectDoesNotExistInInstant();
                }
            }
        }

        internal void SetValue<T>(int instantId, GenericField<T> field, T value)
        {
            lock (infoDict)
            {
                DeserializedInfo info;
                infoDict.TryGetValue(instantId, out info);
                if (info != null && !info.IsDeserialized)
                {
                    field.ForceSet(instantId, value);
                }
            }
        }

        internal void CallUpdate(CurrentInstant current, NextInstant next)
        {
            lock (infoDict)
            {
                DeserializedInfo info;
                infoDict.TryGetValue(current.InstantID, out info);
                if (info == null)
                {
                    log.Warn("CallUpdate: instant does not exist");
                    return;
                }
                this.Update(current, next);
            }
        }

        internal void CallDraw(CurrentInstant current)
        {
            lock (infoDict)
            {
                DeserializedInfo info;
                infoDict.TryGetValue(current.InstantID, out info);
                if (info != null)
                {
                    this.Draw(current);
                }
            }
        }

        internal protected abstract void Update(CurrentInstant current, NextInstant next);

        internal protected abstract void DefineFields(CreationToken creationToken);

        internal protected abstract void Draw(CurrentInstant current);
    }
}
