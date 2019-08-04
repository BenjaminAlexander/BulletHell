using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.Engine.Reflection;
using MyGame.Engine.Serialization;
using static MyGame.Engine.GameState.GameObject;
using MyGame.Engine.GameState.Instants;
using MyGame.Engine.Utils;
using MyGame.Engine.GameState.InstantObjectSet;
using static MyGame.Engine.GameState.TypeManager;
using MyGame.Engine.GameState.GameObjectUtils;
using System.Collections;
using System.Collections.Concurrent;
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

        internal void SetDefaultValue(int instantId)
        {
            if (!instantInfo.IsInstantDeserialized(instantId))
            {
                instantInfo[instantId].IsDeserialized = false;
                foreach (AbstractField field in fieldDefinitions)
                {
                    field.SetDefaultValue(instantId);
                }
            }
        }

        internal void CopyFields(int fromInstant, int toInstant)
        {
            bool fromLockTaken = false;
            bool toLockTaken = false;
            try
            {
                instantInfo[fromInstant].Lock.Enter(ref fromLockTaken);
                instantInfo[fromInstant].Lock.Enter(ref fromLockTaken);
                if (!instantInfo.IsInstantDeserialized(toInstant))
                {
                    instantInfo[toInstant].IsDeserialized = false;
                    foreach (AbstractField field in fieldDefinitions)
                    {
                        field.CopyFieldValues(fromInstant, toInstant);sdfs
                    }
                }
            }
            finally
            {
                if (lockTaken) instantInfo[fromInstant].Lock.Exit();
            }

        }

        internal bool RemoveForUpdate(int instantId)
        {
            bool isDeserialized = false;
            Info info = instantInfo[instantId];
            bool lockTaken = false;
            try
            {
                info.Lock.Enter(ref lockTaken);
                isDeserialized = info.IsDeserialized;
                if (!isDeserialized)
                {
                    instantInfo.RemoveInstant(instantId);
                    foreach (AbstractField field in fieldDefinitions)
                    {
                        field.RemoveInstant(instantId);
                    }
                }
            }
            finally
            {
                if (lockTaken) info.Lock.Exit();
            }
            return lockTaken && !isDeserialized;
        }
        
        //Can this get special consideration because it is only use for deserialization?
        internal void DeserializeRemove(int instantId)
        {
            bool lockTaken = false;
            Info info = instantInfo[instantId];
            try
            {
                info.Lock.PriorityEnter(ref lockTaken);

                instantInfo.RemoveInstant(instantId);
                foreach (AbstractField field in fieldDefinitions)
                {
                    field.RemoveInstant(instantId);
                }
            }
            finally
            {
                if (lockTaken) info.Lock.Exit();
            }
        }

        internal void AddField(AbstractField field)
        {
            fieldDefinitions.Add(field);
        }

        //TODO: can't depend on this method/ its not thread safe
        internal bool IsInstantDeserialized(int instantId)
        {
            return instantInfo.IsInstantDeserialized(instantId);
        }

        internal Serializable GetSerializable(int instantId)
        {
            return new SerializableClosure(instantId, this);
        }

        //Returns true if the value has changed
        //TODO: Unit Test this
        internal bool Deserialize(int instantId, byte[] buffer, ref int bufferOffset)
        {
            if(instantInfo.IsInstantDeserialized(instantId))
            {
                log.Debug("Deserializeing an object into an instant that has already been deserialized.");
            }

            bool isChanged = false;

            bool lockTaken = false;
            Info info = instantInfo[instantId];
            try
            {
                info.Lock.PriorityEnter(ref lockTaken);
                foreach (AbstractField field in fieldDefinitions)
                {
                    isChanged = isChanged | field.Deserialize(instantId, buffer, ref bufferOffset);
                }
                instantInfo[instantId].IsDeserialized = true;
            }
            finally
            {
                if (lockTaken) info.Lock.Exit();
            }
            return isChanged;
        }

        private class SerializableClosure : Serialization.Serializable
        {
            private int instantId;
            private GameObject obj;

            public SerializableClosure(int instantId, GameObject obj)
            {
                this.instantId = instantId;
                this.obj = obj;
            }

            public int SerializationSize
            {
                get
                {
                    int serializationSize = 0;
                    foreach (AbstractField field in obj.fieldDefinitions)
                    {
                        serializationSize = serializationSize + field.SerializationSize(instantId);
                    }
                    return serializationSize;
                }
            }

            public void Serialize(byte[] buffer, ref int bufferOffset)
            {
                if (buffer.Length - bufferOffset < this.SerializationSize)
                {
                    throw new Exception("Buffer length does not match expected state length");
                }

                foreach (AbstractField field in obj.fieldDefinitions)
                {
                    field.Serialize(instantId, buffer, ref bufferOffset);
                }
            }
        }

        //TODO: return a value to signal that this object should not move into the next state
        //TODO: make sure we drop the next state in this object as well as in the instant
        internal protected abstract void Update(CurrentInstant current, NextInstant next);

        internal protected abstract void DefineFields(CreationToken creationToken);
    }
}
