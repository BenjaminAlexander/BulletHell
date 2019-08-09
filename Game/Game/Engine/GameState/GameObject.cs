using System;
using System.Collections.Generic;
using MyGame.Engine.GameState.Instants;
using MyGame.Engine.Utils;
using MyGame.Engine.GameState.GameObjectUtils;
using static MyGame.Engine.GameState.GameObjectUtils.InstantInfo;
using MyGame.Engine.GameState.InstantObjectSet;
using System.Collections.Concurrent;

namespace MyGame.Engine.GameState
{
    public abstract class GameObject
    {
        delegate void SetFieldValue();

        //TODO: rename and reogrinize files
        //TODO: remove deserializedobjecttracker
        //TODO: start GameObject threadsafeness
        //TODO: add creation Instant
        //TODO: the plan
        //have breaks in interpolation

        private static Logger log = new Logger(typeof(GameObject));

        //TODO: Change id to initial instant, and type sequence
        private TypeSetInterface globalTypeSet = null;
        private Nullable<int> id = null;
        private List<AbstractField> fieldDefinitions;
        //private InstantInfo instantInfo = new InstantInfo();
        private ConcurrentDictionary<int, Info> infoDict = new ConcurrentDictionary<int, Info>();

        private Info GetOrCreateInfo(int instantId)
        {
            Info info;
            if (infoDict.ContainsKey(instantId))
            {
                info = infoDict[instantId];
            }
            else
            {
                info = new Info();
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
                Info info = GetOrCreateInfo(instantTypeSet.InstantID);
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

        //TODO: threadsafe this
        internal void CopyFields(int fromInstant, InstantTypeSetInterface toInstant)
        {
            lock (infoDict)
            {
                Info fromInfo;
                infoDict.TryGetValue(fromInstant, out fromInfo);
                if (fromInfo == null)
                {
                    log.Warn("CopyFields: fromInstantDoes not exist");
                    return;
                }

                Info toInfo = GetOrCreateInfo(toInstant.InstantID);

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
                Info info;
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
                Info info;
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
                Info info = GetOrCreateInfo(instantTypeSet.InstantID);
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
                Info info;
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

        internal void SetValue<T>(int instantId, GenericField<T> field, T value)
        {
            lock (infoDict)
            {
                Info info;
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
                Info info;
                infoDict.TryGetValue(current.InstantID, out info);
                if (info == null)
                {
                    log.Warn("CallUpdate: instant does not exist");
                    return;
                }
                this.Update(current, next);
            }
        }

        internal protected abstract void Update(CurrentInstant current, NextInstant next);

        internal protected abstract void DefineFields(CreationToken creationToken);
    }
}
