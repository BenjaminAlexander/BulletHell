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
    class GameObjectContainer
    {
        private static NewConstraintTypeFactory<GameObject> factory = new NewConstraintTypeFactory<GameObject>();

        public static void AddType<DerivedType>() where DerivedType : GameObject, new()
        {
            factory.AddType<DerivedType>();
        }

        private Dictionary<Field, FieldValue> fieldsDict = new Dictionary<Field, FieldValue>();
        private List<FieldValue> fieldsList = new List<FieldValue>();
        private int instant;
        private GameObject gameObject;

        public GameObjectContainer(GameObject gameObject, int instant)
        {
            this.gameObject = gameObject;
            this.instant = instant;
            this.AddFields(gameObject.FieldDefinitions);
        }

        public GameObjectContainer(GameObjectContainer current)
        {
            this.gameObject = current.gameObject;
            this.instant = instant + 1;
            //TODO: is this the right way? or should it copy existing?
            this.AddFields(gameObject.FieldDefinitions);
            gameObject.Update(current, this);
        }

        public GameObject GameObject
        {
            get
            {
                return gameObject;
            }
        }

        public int Instant
        {
            get
            {
                return instant;
            }
        }

        public FieldValue this[Field definition]
        {
            get
            {
                return fieldsDict[definition];
            }
        }

        public GameObjectContainer(byte[] buffer)
        {
            int bufferOffset = 0;
            int typeID = Serialization.Utils.ReadInt(buffer, ref bufferOffset);
            if (gameObject == null || factory.GetTypeID(gameObject) != typeID)
            {
                gameObject = factory.Construct(typeID);
                fieldsDict = new Dictionary<Field, FieldValue>();
                fieldsList = new List<FieldValue>();
                this.AddFields(gameObject.FieldDefinitions);
            }
            instant = Serialization.Utils.ReadInt(buffer, ref bufferOffset);

            foreach (FieldValue field in fieldsList)
            {
                field.Deserialize(buffer, ref bufferOffset);
            }
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
                foreach (FieldValue field in fieldsList)
                {
                    serializationSize = serializationSize + field.SerializationSize;
                }
                return serializationSize;
            }
        }

        private void AddFields(List<Field> fields)
        {
            foreach (Field field in fields)
            {
                FieldValue value = field.GetInitialField();
                this.fieldsList.Add(value);
                this.fieldsDict.Add(field, value);
            }
        }

        public byte[] Serialize()
        {
            byte[] buffer = new byte[SerializationSize];
            int bufferOffset = 0;
            Serialize(buffer, ref bufferOffset);
            return buffer;
        }

        public void Serialize(byte[] buffer, ref int bufferOffset)
        {
            if (buffer.Length - bufferOffset < this.SerializationSize)
            {
                throw new Exception("Buffer length does not match expected state length");
            }

            Serialization.Utils.Write(factory.GetTypeID(gameObject), buffer, ref bufferOffset);
            Serialization.Utils.Write(instant, buffer, ref bufferOffset);

            foreach (FieldValue field in fieldsList)
            {
                field.Serialize(buffer, ref bufferOffset);
            }
        }

        public static bool IsEqual(GameObjectContainer obj1, GameObjectContainer obj2)
        {
            if(factory.GetTypeID(obj1.gameObject) == factory.GetTypeID(obj2.gameObject) && 
                obj1.fieldsList.Count == obj2.fieldsList.Count)
            {
                for(int i = 0; i < obj1.fieldsList.Count; i++)
                {
                    if(!obj1.fieldsList[i].IsEqual(obj2.fieldsList[i]))
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }
    }
}
