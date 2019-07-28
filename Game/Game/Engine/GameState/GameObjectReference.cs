using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.Engine.Serialization;

namespace MyGame.Engine.GameState
{
    struct GameObjectReference : Serializable, Deserializable
    {
        private int typeId;
        private int objectId;

        public int SerializationSize
        {
            get
            {
                return sizeof(int) * 2;
            }
        }

        public GameObjectReference(GameObject obj)
        {
            typeId = obj.TypeID;
            objectId = obj.ID;
        }

        public GameObjectReference(byte[] buffer, ref int bufferOffset)
        {
            Serialization.Utils.Read(out typeId, buffer, ref bufferOffset);
            Serialization.Utils.Read(out objectId, buffer, ref bufferOffset);
        }

        public void Serialize(byte[] buffer, ref int bufferOffset)
        {
            Serialization.Utils.Write(typeId, buffer, ref bufferOffset);
            Serialization.Utils.Write(objectId, buffer, ref bufferOffset);
        }

        public void Deserialize(byte[] buffer, ref int bufferOffset)
        {
            Serialization.Utils.Read(out typeId, buffer, ref bufferOffset);
            Serialization.Utils.Read(out objectId, buffer, ref bufferOffset);
        }
    }
}
