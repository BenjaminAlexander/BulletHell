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
    //TODO: make it so deserialized instant states don't get stepped on when deserializing
    //TODO: log WARN when deserializing a deserialized state
    //TODO: Let the caller know if values changed during deserialization

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

        internal static GameObject Construct(int id, Instant instant, byte[] buffer, ref int bufferOffset)
        {
            int orgininalOffset = bufferOffset;
            int typeID = Serialization.Utils.ReadInt(buffer, ref bufferOffset);
            GameObject obj = factory.Construct(typeID);
            obj.SetUp(id, instant);
            obj.Deserialize(instant, buffer, ref orgininalOffset);
            return obj;
        }

        private Nullable<int> id = null;
        private List<AbstractField> fieldDefinitions = new List<AbstractField>();

        private void SetUp(int id, Instant instant)
        {
            this.id = id;
            this.DefineFields(new InitialInstant(instant, this));
            instant.AddObject(this);
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

        internal void Deserialize(Instant instant, byte[] buffer, ref int bufferOffset)
        {
            int typeID = Serialization.Utils.ReadInt(buffer, ref bufferOffset);
            if (this.TypeID != typeID)
            {
                throw new Exception("GameObject type ID mismatch");
            }

            foreach (AbstractField field in fieldDefinitions)
            {
                field.Deserialize(instant, buffer, ref bufferOffset);
            }
            instant.AddDeserializedObject(this);
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
            foreach (AbstractField field in fieldDefinitions)
            {
                field.CopyFieldValues(current, next);
            }
            this.Update(current, next);
        }

        public abstract void Update(CurrentInstant current, NextInstant next);

        internal abstract void DefineFields(InitialInstant instant);

        public bool CheckThatInstantKeysContainThis()
        {
            foreach(AbstractField field in fieldDefinitions)
            {
                foreach (Instant instant in field.GetInstantSet())
                {
                    //TODO: update this method when the game object know which states are deserialized
                    if(!(instant.Contains(this) || instant.ContainsAsDeserialized(this)))
                    {
                        log.Error("This GameObject contains an instant key that does not also contain that GameObject");
                        return false;
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

            internal abstract void Deserialize(Instant container, byte[] buffer, ref int bufferOffset);

            internal abstract bool IsIdentical(Instant container, AbstractField other, Instant otherContainer);

            internal abstract List<Instant> GetInstantSet();
        }
    }
}
