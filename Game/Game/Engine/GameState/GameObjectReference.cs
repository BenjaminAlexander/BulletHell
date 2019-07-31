using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.Engine.Serialization;
using MyGame.Engine.GameState.InstantObjectSet;

namespace MyGame.Engine.GameState
{
    public struct GameObjectReference<SubType> : Serializable, Deserializable where SubType : GameObject
    {
        public static implicit operator GameObjectReference<SubType>(SubType obj)
        {
            return new GameObjectReference<SubType>(obj);
        }

        private int typeId;
        private int objectId;

        public int SerializationSize
        {
            get
            {
                return sizeof(int) * 2;
            }
        }

        internal GameObjectReference(SubType obj)
        {
            if (obj == null)
            {
                typeId = 0;
                objectId = 0;
            }
            else
            {
                typeId = obj.TypeID;
                objectId = obj.ID;
            }
        }

        internal GameObjectReference(byte[] buffer, ref int bufferOffset)
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

        internal SubType Dereference(InstantSet instantSet)
        {
            if(objectId == 0)
            {
                return null;
            }
            return (SubType)instantSet.GetObject(typeId, objectId);
        }

        public override bool Equals(object obj)
        {
            if(obj is GameObjectReference<SubType>)
            {
                GameObjectReference<SubType> other = (GameObjectReference<SubType>)obj;
                if (other.objectId == objectId && other.typeId == typeId)
                {
                    return true;
                }
            }
            return false;
        }

        public override int GetHashCode()
        {
            int hashcode = 23;
            hashcode = (hashcode * 37) + objectId;
            hashcode = (hashcode * 37) + typeId;
            return hashcode;
        }
    }
}
