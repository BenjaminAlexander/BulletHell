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
        private static Logger log = new Logger(typeof(GameObject));
        private static NewConstraintTypeFactory<GameObject> factory = new NewConstraintTypeFactory<GameObject>();

        internal static void AddType<DerivedType>() where DerivedType : GameObject, new()
        {
            factory.AddType<DerivedType>();
        }

        internal static SubType Construct<SubType>(int id, Instant instant) where SubType : GameObject, new()
        {
            SubType obj = new SubType();
            obj.SetUp(id, instant);
            return obj;
        }

        //TODO: remove the deserialize call from this to make it more uniform
        internal static GameObject Construct(int id, Instant instant, byte[] buffer, int bufferOffset)
        {
            int typeID = Serialization.Utils.ReadInt(buffer, ref bufferOffset);
            GameObject obj = factory.Construct(typeID);
            obj.SetUp(id, instant);
            return obj;
        }

        //TODO: remove this method
        internal static int GetTypeID(GameObject obj)
        {
            return factory.GetTypeID(obj);
        }

        //TODO: remove this method
        internal static GameObject Construct(int id, Instant instant, int typeID)
        {
            GameObject obj = factory.Construct(typeID);
            obj.SetUp(id, instant);
            return obj;
        }

        private Nullable<int> id = null;
        private List<AbstractField> fieldDefinitions = new List<AbstractField>();
        private Dictionary<Instant, bool> isInstantDeserialized = new Dictionary<Instant, bool>();

        internal void SetUp(int id, Instant instant)
        {
            this.id = id;
            this.DefineFields(new InitialInstant(instant, this));
            instant.AddObject(this);
            isInstantDeserialized[instant] = false;
        }

        internal int TypeID
        {
            get
            {
                return factory.GetTypeID(this);
            }
        }

        internal Nullable<int> ID
        {
            get
            {
                return id;
            }
        }

        internal bool IsInstantDeserialized(Instant instant)
        {
            return isInstantDeserialized[instant];
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

        internal byte[] Serialize(Instant container)
        {
            byte[] buffer = new byte[this.SerializationSize(container)];
            int bufferOffset = 0;
            this.Serialize(container, buffer, ref bufferOffset);
            return buffer;
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
            if(isInstantDeserialized.ContainsKey(instant) && isInstantDeserialized[instant])
            {
                log.Warn("Deserializeing an object into an instant that has already been deserialized.");
            }

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
            instant.AddDeserializedObject(this);
            isInstantDeserialized[instant] = true;
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
            if (isInstantDeserialized.ContainsKey(next.Instant) && isInstantDeserialized[next.Instant])
            {
                //TODO: unit test this case
                log.Warn("Attempting to update an object into a deserialized instant.");
                return;
            }

            foreach (AbstractField field in fieldDefinitions)
            {
                field.CopyFieldValues(current, next);
            }
            this.Update(current, next);
            isInstantDeserialized[next.Instant] = false;
        }

        //TODO: return a value to signal that this object should not move into the next state
        //TODO: make sure we drop the next state in this object as well as in the instant
        public abstract void Update(CurrentInstant current, NextInstant next);

        internal abstract void DefineFields(InitialInstant instant);

        public bool CheckThatInstantKeysContainThis()
        {
            foreach(AbstractField field in fieldDefinitions)
            {
                foreach (Instant instant in field.GetInstantSet())
                {
                    if ((isInstantDeserialized[instant]))
                    {
                        if(!instant.ContainsAsDeserialized(this))
                        {
                            log.Error("This GameObject contains an instant key and is deserialized but is not contained in that instants deserialized set");
                            return false;
                        }
                    }
                    else
                    {
                        if(!instant.ContainsAsNonDeserialized(this))
                        {
                            log.Error("This GameObject contains an instant key but is not contained in that instants object set");
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        public abstract class AbstractField
        {
            public AbstractField(InitialInstant instant)
            {
                instant.Object.fieldDefinitions.Add(this);
            }

            internal abstract void CopyFieldValues(CurrentInstant current, NextInstant next);

            internal abstract int SerializationSize(Instant container);

            internal abstract void Serialize(Instant container, byte[] buffer, ref int bufferOffset);

            /**
             * Returns True if Values were changed
             */
            internal abstract bool Deserialize(Instant container, byte[] buffer, ref int bufferOffset);

            internal abstract bool IsIdentical(Instant container, AbstractField other, Instant otherContainer);

            internal abstract List<Instant> GetInstantSet();
        }
    }
}
