using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.Engine.Reflection;
using MyGame.Engine.Serialization;
using static MyGame.Engine.GameState.GameObject;
using MyGame.Engine.GameState.Instants;

namespace MyGame.Engine.GameState
{
    public abstract class GameObject
    {
        private static NewConstraintTypeFactory<GameObject> factory = new NewConstraintTypeFactory<GameObject>();

        internal static void AddType<DerivedType>() where DerivedType : GameObject, new()
        {
            factory.AddType<DerivedType>();
        }

        internal static SubType Construct<SubType>(Instant instant) where SubType : GameObject, new()
        {
            SubType obj = new SubType();
            obj.DefineFields(new InitialInstant(instant, obj));
            return obj;
        }

        internal static GameObject Construct(byte[] buffer, int bufferOffset)
        {
            Instant instant = new Instant(buffer, ref bufferOffset);
            int typeID = Serialization.Utils.ReadInt(buffer, ref bufferOffset);

            GameObject obj = factory.Construct(typeID);
            obj.DefineFields(new InitialInstant(instant, obj));

            return obj;
        }       

        private List<AbstractField> fieldDefinitions = new List<AbstractField>();

        internal int TypeID
        {
            get
            {
                return factory.GetTypeID(this);
            }
        }

        internal int SerializationSize(Instant container)
        {
            int serializationSize = sizeof(int) + container.SerializationSize;
            foreach (AbstractField field in fieldDefinitions)
            {
                serializationSize = serializationSize + field.SerializationSize(container);
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

        internal void Serialize(Instant container, byte[] buffer, ref int bufferOffset)
        {
            if (buffer.Length - bufferOffset < this.SerializationSize(container))
            {
                throw new Exception("Buffer length does not match expected state length");
            }

            container.Serialize(this, buffer, ref bufferOffset);
            Serialization.Utils.Write(this.TypeID, buffer, ref bufferOffset);
            foreach (AbstractField field in fieldDefinitions)
            {
                field.Serialize(container, buffer, ref bufferOffset);
            }
        }

        internal void Deserialize(byte[] buffer, ref int bufferOffset)
        {
            Instant container = new Instant(buffer, ref bufferOffset);

            int typeID = Serialization.Utils.ReadInt(buffer, ref bufferOffset);
            if (this.TypeID != typeID)
            {
                throw new Exception("GameObject type ID mismatch");
            }

            foreach (AbstractField field in fieldDefinitions)
            {
                field.Deserialize(container, buffer, ref bufferOffset);
            }
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
        }
    }
}
