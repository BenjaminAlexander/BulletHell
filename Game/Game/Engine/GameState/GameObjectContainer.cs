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
    //TODO: consider removing a reference to game object from the container?
    //TODO: look at serialization/deserialization pattern
    //TODO: define equals and hash for this class to get indexing in gameObject to work correctly
    class GameObjectContainer
    {
        private int instant;
        //TODO: should all containers which describe the same object reference the same GameObject?
        private GameObject gameObject;

        public GameObjectContainer(GameObject gameObject, int instant)
        {
            this.gameObject = gameObject;
            this.instant = instant;

            //TODO: its unsafe to call defineFields more than once, this should be fixed
            this.gameObject.DefineFields(this.Next);
        }

        public GameObjectContainer(GameObject gameObject, byte[] buffer)
        {
            this.gameObject = gameObject;

            //TODO: its unsafe to call defineFields more than once, this should be fixed
            this.gameObject.DefineFields(this.Next);

            int bufferOffset = 0;
            this.Deserialize(buffer, ref bufferOffset);
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

        public CurrentContainer Current
        {
            get
            {
                return new CurrentContainer(this);
            }
        }

        public NextContainer Next
        {
            get
            {
                return new NextContainer(this);
            }
        }

        public GameObjectContainer(byte[] buffer)
        {
            int bufferOffset = 0;
            int typeID = PeakGameOjectType(buffer, bufferOffset);
            if (gameObject == null || gameObject.TypeID != typeID)
            {
                this.gameObject = GameObject.Construct(typeID);
                this.gameObject.DefineFields(this.Next);
            }

            bufferOffset = 0;
            this.Deserialize(buffer, ref bufferOffset);
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

            Serialization.Utils.Write(gameObject.TypeID, buffer, ref bufferOffset);
            Serialization.Utils.Write(instant, buffer, ref bufferOffset);

            gameObject.Serialize(this, buffer, ref bufferOffset);
        }

        public static int PeakGameOjectType(byte[] buffer, int bufferOffset)
        {
            return Serialization.Utils.ReadInt(buffer, ref bufferOffset);
        }

        //TODO: is this done correctly?
        public void Deserialize(byte[] buffer, ref int bufferOffset)
        {
            int typeID = Serialization.Utils.ReadInt(buffer, ref bufferOffset);
            if (gameObject == null || gameObject.TypeID != typeID)
            {
                throw new Exception("GameObject type ID mismatch");
            }
            instant = Serialization.Utils.ReadInt(buffer, ref bufferOffset);

            gameObject.Deserialize(this, buffer, ref bufferOffset);
        }

        public static bool IsIdentical(GameObjectContainer obj1, GameObjectContainer obj2)
        {
            if(obj1.instant == obj2.instant)
            {
                return obj1.gameObject.IsIdentical(obj1, obj2.gameObject, obj2);
            }
            return false;
        }
    }
}
