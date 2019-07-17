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

namespace MyGame.Engine.GameState
{
    public abstract class GameObject
    {
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

        internal static SubType NewGameObject<SubType>(int id, Instant instant) where SubType : GameObject, new()
        {
            SubType obj = new SubType();
            obj.SetUp(id, new TypeSetInterfaceStub());
            obj.SetDefaultValue(instant);
            return obj;
        }

        internal static GameObject NewGameObject(int id, Instant instant, byte[] buffer, int bufferOffset)
        {
            int typeID = Serialization.Utils.ReadInt(buffer, ref bufferOffset);
            GameObject obj = typeManager.Construct(typeID);
            obj.SetUp(id, new TypeSetInterfaceStub());
            obj.SetDefaultValue(instant);
            return obj;
        }

        //private const int DEFAULT_SERIALIZATION_PERIOD = 5;

        //TODO: Change id to initial instant, and type sequence
        private TypeSetInterface globalTypeSet = null;
        private Nullable<int> id = null;
        private List<AbstractField> fieldDefinitions = new List<AbstractField>();
        private Dictionary<Instant, bool> isInstantDeserialized = new Dictionary<Instant, bool>();
        //private int updatesUntilSerialization = DEFAULT_SERIALIZATION_PERIOD;

        internal void SetUp(int id, TypeSetInterface globalTypeSet)
        {
            this.id = id;
            this.globalTypeSet = globalTypeSet;
            this.DefineFields(new CreationToken(this));
        }

        internal void SetDefaultValue(Instant instant)
        {
            if (!IsInstantDeserialized(instant))
            {
                isInstantDeserialized[instant] = false;
                foreach (AbstractField field in fieldDefinitions)
                {
                    field.SetDefaultValue(instant);
                }
                instant.AddObject(this);
            }
        }

        internal int TypeID
        {
            get
            {
                return typeManager.GetMetaData(this).TypeID;
            }
        }

        internal Nullable<int> ID
        {
            get
            {
                return id;
            }
        }

        internal void AddField(AbstractField field)
        {
            fieldDefinitions.Add(field);
        }

        internal bool IsInstantDeserialized(Instant instant)
        {
            return isInstantDeserialized.ContainsKey(instant) && isInstantDeserialized[instant];
        }

        internal bool HasInstant(Instant instant)
        {
            return isInstantDeserialized.ContainsKey(instant);
        }

        internal bool AllFieldsHasInstant(Instant instant)
        {
            if(!isInstantDeserialized.ContainsKey(instant))
            {
                return false;
            }

            foreach(AbstractField field in fieldDefinitions)
            {
                if(!field.HasInstant(instant))
                {
                    return false;
                }
            }
            return true;
        }

        internal int SerializationSize(Instant instant)
        {
            int serializationSize = sizeof(int);
            foreach (AbstractField field in fieldDefinitions)
            {
                serializationSize = serializationSize + field.SerializationSize(instant);
            }
            return serializationSize;
        }

        internal void Serialize(Instant instant, byte[] buffer, ref int bufferOffset)
        {
            if (buffer.Length - bufferOffset < this.SerializationSize(instant))
            {
                throw new Exception("Buffer length does not match expected state length");
            }

            Serialization.Utils.Write(this.TypeID, buffer, ref bufferOffset);
            foreach (AbstractField field in fieldDefinitions)
            {
                field.Serialize(instant, buffer, ref bufferOffset);
            }
        }

        //Returns true if the value has changed
        //TODO: Unit Test this
        internal bool Deserialize(Instant instant, byte[] buffer, ref int bufferOffset)
        {
            if(IsInstantDeserialized(instant))
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
                bool isFieldValueChanged = field.Deserialize(instant, buffer, ref bufferOffset);
                isValueChanged = isFieldValueChanged || isValueChanged;
            }
            isInstantDeserialized[instant] = true;
            instant.AddDeserializedObject(this);
            return isValueChanged;
        }

        internal bool IsIdentical(Instant container, GameObject other, Instant otherContainer)
        {
            if (!this.GetType().Equals(other.GetType()) || this.fieldDefinitions.Count != other.fieldDefinitions.Count)
            {
                return false;
            }
            for (int i = 0; i < this.fieldDefinitions.Count; i++)
            {
                if (!this.fieldDefinitions[i].IsIdentical(container, other.fieldDefinitions[i], otherContainer))
                {
                    return false;
                }
            }
            return true;
        }

        internal void CallUpdate(CurrentInstant current, NextInstant next)
        {
            if (IsInstantDeserialized(next.Instant))
            {
                //TODO: unit test this case
                log.Warn("Attempting to update an object into a deserialized instant.");
                return;
            }

            foreach (AbstractField field in fieldDefinitions)
            {
                field.CopyFieldValues(current, next);
            }
            next.Instant.AddObject(this);
            this.Update(current, next);
        }

        //TODO: return a value to signal that this object should not move into the next state
        //TODO: make sure we drop the next state in this object as well as in the instant
        public abstract void Update(CurrentInstant current, NextInstant next);

        internal abstract void DefineFields(CreationToken creationToken);

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
        }
    }
}
