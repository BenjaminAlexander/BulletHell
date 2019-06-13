using MyGame.Engine.Reflection;
using MyGame.Engine.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MyGame.Engine.GameState.GameObject;

namespace MyGame.Engine.GameState
{
    class GameObjectContainer : Serializable
    {
        private static NewConstraintTypeFactory<GameObject> factory = new NewConstraintTypeFactory<GameObject>();

        public static void AddType<DerivedType>() where DerivedType : GameObject, new()
        {
            factory.AddType<DerivedType>();
        }

        private List<Field> fields = new List<Field>();
        private int instant;
        private GameObject gameObject;

        public GameObjectContainer(byte[] buffer)
        {
            Serialization.Utils.Deserialize(this, buffer);
        }

        public GameObjectContainer(GameObject gameObject, int instant)
        {
            this.gameObject = gameObject;
            this.instant = instant;
        }

        public Type GetGameObjectType()
        {
            return gameObject.GetType();
        }

        public int SerializationSize
        {
            get
            {
                int serializationSize = sizeof(int) * 2;
                foreach (Field field in fields)
                {
                    serializationSize = serializationSize + field.SerializationSize;
                }
                return serializationSize;
            }
        }

        private void AddField(Field field)
        {
            this.fields.Add(field);
        }

        public void Serialize(byte[] buffer, ref int bufferOffset)
        {
            if (buffer.Length - bufferOffset < this.SerializationSize)
            {
                throw new Exception("Buffer length does not match expected state length");
            }

            Serialization.Utils.Write(factory.GetTypeID(gameObject), buffer, ref bufferOffset);
            Serialization.Utils.Write(instant, buffer, ref bufferOffset);

            foreach (Field field in fields)
            {
                field.Serialize(buffer, ref bufferOffset);
            }
        }

        public void Deserialize(byte[] buffer, ref int bufferOffset)
        {
            int typeID = Serialization.Utils.ReadInt(buffer, ref bufferOffset);
            if (factory.GetTypeID(gameObject) != typeID)
            {
                gameObject = factory.Construct(typeID);
            }
            instant = Serialization.Utils.ReadInt(buffer, ref bufferOffset);

            foreach (Field field in fields)
            {
                field.Deserialize(buffer, ref bufferOffset);
            }
        }
    }
}
