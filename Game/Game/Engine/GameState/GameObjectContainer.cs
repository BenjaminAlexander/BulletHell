using MyGame.Engine.Reflection;
using MyGame.Engine.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MyGame.Engine.GameState.GameObject;
using System.Collections.Specialized;
using System.Collections;

namespace MyGame.Engine.GameState
{
    //TODO: define equals and hash for this class to get indexing in gameObject to work correctly
    class GameObjectContainer
    {
        private static NewConstraintTypeFactory<GameObject> factory = new NewConstraintTypeFactory<GameObject>();

        public static void AddType<DerivedType>() where DerivedType : GameObject, new()
        {
            factory.AddType<DerivedType>();
        }

        private int instant;
        private GameObject gameObject;

        public GameObjectContainer(GameObject gameObject, int instant)
        {
            this.gameObject = gameObject;
            this.instant = instant;
            //this.AddFields(gameObject.FieldDefinitions);
            foreach(AbstractField field in gameObject.FieldDefinitions)
            {
                field.SetInitialValue(this);
            }
        }

        public GameObjectContainer(GameObjectContainer current)
        {
            this.gameObject = current.gameObject;
            this.instant = instant + 1;

            //Copy existing fields
            this.gameObject.CopyFieldValues(current, this);

            gameObject.Update(new CurrentContainer(current), new NextContainer(this));
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

        internal List<FieldValue> GetFieldValues()
        {
            return gameObject.GetFieldValues(this);
        }

        public GameObjectContainer(byte[] buffer)
        {
            int bufferOffset = 0;
            int typeID = Serialization.Utils.ReadInt(buffer, ref bufferOffset);
            if (gameObject == null || factory.GetTypeID(gameObject) != typeID)
            {
                gameObject = factory.Construct(typeID);
            }
            instant = Serialization.Utils.ReadInt(buffer, ref bufferOffset);

            gameObject.Deserialize(this, buffer, ref bufferOffset);
        }

        public Type GetGameObjectType()
        {
            return gameObject.GetType();
        }

        public int SerializationSize
        {
            get
            {
                return sizeof(int) * 2 + gameObject.SerializationSize(this);
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

            gameObject.Serialize(this, buffer, ref bufferOffset);
        }

        public static bool IsEqual(GameObjectContainer obj1, GameObjectContainer obj2)
        {
            //TODO: relook all instances of GetFIeldValues and this method
            if(factory.GetTypeID(obj1.gameObject) == factory.GetTypeID(obj2.gameObject))
            {
                List<FieldValue> list1 = obj1.GetFieldValues();
                List<FieldValue> list2 = obj1.GetFieldValues();
                if (list1.Count == list2.Count)
                {
                    for (int i = 0; i < list1.Count; i++)
                    {
                        if (!list1[i].Equals(list2[i]))
                        {
                            return false;
                        }
                    }
                    return true;
                }
                return false;
            }
            return false;
        }
    }
}
