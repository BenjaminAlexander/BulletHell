using System;
using System.Collections.Generic;
using MyGame.Engine.GameState.Instants;
using MyGame.Engine.Utils;
using MyGame.Engine.GameState.GameObjectUtils;
using static MyGame.Engine.GameState.GameObjectUtils.InstantInfo;

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
        private InstantInfo instantInfo = new InstantInfo();

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

        internal void SetDefaultValue(int instantId)
        {
            Info info = null;
            try
            {
                info = instantInfo.CreateAndTryEnter(instantId);
                if (!info.IsDeserialized)
                {
                    info.IsDeserialized = false;
                    foreach (AbstractField field in fieldDefinitions)
                    {
                        field.SetDefaultValue(instantId);
                    }
                }
            }
            finally
            {
                if (info != null)
                {
                    instantInfo.Exit(info);
                }
                else
                {
                    log.Error("SetDefaultValue: Failed to obtain the lock");
                }
            }
        }

        //TODO: threadsafe this
        internal void CopyFields(int fromInstant, int toInstant)
        {
            Info fromInfo = null;
            Info toInfo = null;
            try
            {
                fromInfo = instantInfo.TryEnter(fromInstant);
                if(fromInfo != null)
                {
                    toInfo = instantInfo.CreateAndTryEnter(toInstant);

                    if(!toInfo.IsDeserialized)
                    {
                        foreach (AbstractField field in fieldDefinitions)
                        {
                            field.CopyFieldValues(fromInstant, toInstant);
                        }
                    }
                }
                
            }
            finally
            {
                if (fromInfo != null)
                {
                    instantInfo.Exit(fromInfo);
                }
                else
                {
                    log.Debug("CopyFields: Failed to obtain the lock for fromInstant or lock did not exist");
                }

                if (toInfo != null)
                {
                    instantInfo.Exit(toInfo);
                }
                else
                {
                    log.Debug("CopyFields: Failed to obtain the lock for toInstant or lock did not exist");
                }
            }
        }

        internal bool RemoveForUpdate(int instantId)
        {
            bool instantRemoved = true;
            Info info = null;
            try
            {
                info = instantInfo.TryEnter(instantId);
                if (info != null)
                {
                    if (!info.IsDeserialized)
                    {
                        foreach (AbstractField field in fieldDefinitions)
                        {
                            field.RemoveInstant(instantId);
                        }
                        instantInfo.RemoveInstant(instantId);
                    }
                    else
                    {
                        instantRemoved = false;
                    }
                }
            }
            finally
            {
                if (info != null)
                {
                    instantInfo.Exit(info);
                }
                else
                {
                    log.Debug("RemoveForUpdate: Failed to obtain the lock or lock did not exist");
                }
            }
            return instantRemoved;
        }

        //Can this get special consideration because it is only use for deserialization?
        internal void DeserializeRemove(int instantId)
        {
            Info info = null;
            try
            {
                info = instantInfo.TryEnter(instantId);
                if (info != null)
                {
                    foreach (AbstractField field in fieldDefinitions)
                    {
                        field.RemoveInstant(instantId);
                    }
                    instantInfo.RemoveInstant(instantId);
                }
            }
            finally
            {
                if (info != null)
                {
                    instantInfo.Exit(info);
                }
                else
                {
                    log.Debug("DeserializeRemove: Failed to obtain the lock or lock did not exist");
                }
            }
        }

        //Returns true if the value has changed
        internal bool Deserialize(int instantId, byte[] buffer, ref int bufferOffset)
        {
            bool isChanged = false;
            Info info = null;
            try
            {
                info = instantInfo.CreateAndTryEnter(instantId);
                if (info.IsDeserialized)
                {
                    log.Debug("Deserializeing an object into an instant that has already been deserialized.");
                }
                foreach (AbstractField field in fieldDefinitions)
                {
                    isChanged = isChanged | field.Deserialize(instantId, buffer, ref bufferOffset);
                }
                info.IsDeserialized = true;
            }
            finally
            {
                if (info != null)
                {
                    instantInfo.Exit(info);
                }
                else
                {
                    log.Error("Deserialize: Failed to obtain the lock");
                }
            }
            return isChanged;
        }

        public byte[] Serialize(int instantId)
        {
            byte[] buffer = null;
            Info info = null;
            try
            {
                info = instantInfo.TryEnter(instantId);
                if (info != null)
                {
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
            }
            finally
            {
                if (info != null)
                {
                    instantInfo.Exit(info);
                }
                else
                {
                    log.Debug("Serialize: Failed to obtain the lock or lock did not exist");
                }
            }
            return buffer;
        }

        internal void SetValue<T>(int instantId, GenericField<T> field, T value)
        {
            Info info = null;
            try
            {
                info = instantInfo.TryEnter(instantId);
                if (info != null)
                {
                    if(!info.IsDeserialized)
                    {
                        field.ForceSet(instantId, value);
                    }
                }
            }
            finally
            {
                if (info != null)
                {
                    instantInfo.Exit(info);
                }
                else
                {
                    log.Debug("Serialize: Failed to obtain the lock or lock did not exist");
                }
            }
        }

        internal void CallUpdate(CurrentInstant current, NextInstant next)
        {
            Info info = null;
            try
            {
                info = instantInfo.TryEnter(current.InstantID);
                if(info != null)
                {
                    this.Update(current, next);
                }
            }
            finally
            {
                if (info != null)
                {
                    instantInfo.Exit(info);
                }
                else
                {
                    log.Error("CallUpdate: Failed to obtain the lock or lock did not exist");
                }
            }
        }

        internal protected abstract void Update(CurrentInstant current, NextInstant next);

        internal protected abstract void DefineFields(CreationToken creationToken);
    }
}
