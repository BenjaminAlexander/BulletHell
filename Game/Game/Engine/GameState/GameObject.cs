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

namespace MyGame.Engine.GameState
{
    public abstract class GameObject
    {
        //TODO: add creation Instant
        //TODO: the plan
        //ID objects with type and sequence in type
        //recycle objects
        //have breaks in interpolation
        //TODO: create objects with no instant, add instant in setup


        //TODO: test serialization period
        private static Logger log = new Logger(typeof(GameObject));
        //TODO: find the right spot for this
        private static TypeManager typeManager = new TypeManager();

        internal static TypeManager TypeManager
        {
            get
            {
                return typeManager;
            }
        }

        internal static void AddType<DerivedType>() where DerivedType : GameObject, new()
        {
            typeManager.AddType<DerivedType>();
        }

        //private const int DEFAULT_SERIALIZATION_PERIOD = 5;

        //TODO: Change id to initial instant, and type sequence
        private TypeSetInterface globalTypeSet = null;
        private Nullable<int> id = null;
        private List<AbstractField> fieldDefinitions = new List<AbstractField>();
        private Dictionary<int, bool> isInstantDeserialized = new Dictionary<int, bool>();
        //private int updatesUntilSerialization = DEFAULT_SERIALIZATION_PERIOD;

        internal void SetUp(int id, TypeSetInterface globalTypeSet)
        {
            this.id = id;
            this.globalTypeSet = globalTypeSet;
            this.DefineFields(new CreationToken(this));
        }

        internal void SetDefaultValue(InstantTypeSetInterface instant)
        {
            if (!IsInstantDeserialized(instant.InstantID))
            {
                isInstantDeserialized[instant.InstantID] = false;
                foreach (AbstractField field in fieldDefinitions)
                {
                    field.SetDefaultValue(instant.InstantID);
                }
                instant.Add(this);
            }
        }

        internal int TypeID
        {
            get
            {
                return typeManager.GetMetaData(this).TypeID;
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

        internal void AddField(AbstractField field)
        {
            fieldDefinitions.Add(field);
        }

        internal bool IsInstantDeserialized(int instantId)
        {
            return isInstantDeserialized.ContainsKey(instantId) && isInstantDeserialized[instantId];
        }

        internal bool HasInstant(int instantId)
        {
            return isInstantDeserialized.ContainsKey(instantId);
        }

        internal bool AllFieldsHasInstant(int instantId)
        {
            if(!isInstantDeserialized.ContainsKey(instantId))
            {
                return false;
            }

            foreach(AbstractField field in fieldDefinitions)
            {
                if(!field.HasInstant(instantId))
                {
                    return false;
                }
            }
            return true;
        }

        internal int SerializationSize(int instantId)
        {
            int serializationSize = sizeof(int);
            foreach (AbstractField field in fieldDefinitions)
            {
                serializationSize = serializationSize + field.SerializationSize(instantId);
            }
            return serializationSize;
        }

        internal void Serialize(int instantId, byte[] buffer, ref int bufferOffset)
        {
            if (buffer.Length - bufferOffset < this.SerializationSize(instantId))
            {
                throw new Exception("Buffer length does not match expected state length");
            }

            Serialization.Utils.Write(this.TypeID, buffer, ref bufferOffset);
            foreach (AbstractField field in fieldDefinitions)
            {
                field.Serialize(instantId, buffer, ref bufferOffset);
            }
        }

        //Returns true if the value has changed
        //TODO: Unit Test this
        internal bool Deserialize(InstantTypeSetInterface instant, byte[] buffer, ref int bufferOffset)
        {
            if(IsInstantDeserialized(instant.InstantID))
            {
                log.Warn("Deserializeing an object into an instant that has already been deserialized.");
            }

            //TODO: typeID needs to be moved out of this object
            int typeID = Serialization.Utils.ReadInt(buffer, ref bufferOffset);
            if (this.TypeID != typeID)
            {
                throw new Exception("GameObject type ID mismatch");
            }

            bool isValueChanged = false;
            foreach (AbstractField field in fieldDefinitions)
            {
                bool isFieldValueChanged = field.Deserialize(instant.InstantID, buffer, ref bufferOffset);
                isValueChanged = isFieldValueChanged || isValueChanged;
            }
            isInstantDeserialized[instant.InstantID] = true;
            instant.Add(this);
            return isValueChanged;
        }

        internal bool IsIdentical(int instantId, GameObject other, int otherInstantId)
        {
            if (!this.GetType().Equals(other.GetType()) || this.fieldDefinitions.Count != other.fieldDefinitions.Count)
            {
                return false;
            }
            for (int i = 0; i < this.fieldDefinitions.Count; i++)
            {
                if (!this.fieldDefinitions[i].IsIdentical(instantId, other.fieldDefinitions[i], otherInstantId))
                {
                    return false;
                }
            }
            return true;
        }

        internal void CallUpdate(CurrentInstant current, NextInstant next)
        {
            foreach (AbstractField field in fieldDefinitions)
            {
                field.CopyFieldValues(current.Instant.ID, next.Instant.ID);
            }
            next.Instant.AddObject(this);
            this.Update(current, next);
        }

        //TODO: return a value to signal that this object should not move into the next state
        //TODO: make sure we drop the next state in this object as well as in the instant
        public abstract void Update(CurrentInstant current, NextInstant next);

        internal abstract void DefineFields(CreationToken creationToken);

        /*
        public bool CheckThatInstantKeysContainThis()
        {
            foreach(AbstractField field in fieldDefinitions)
            {
                foreach (Instant instant in field.GetInstantSet())
                {
                    if(!instant.CheckContainsIntegrety(this))
                    {
                        log.Error("The instant does not correctly contain the object");
                        return false;
                    }
                }
            }
            return true;
        }*/
    }
}
