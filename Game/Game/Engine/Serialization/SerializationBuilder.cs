using MyGame.Engine.Serialization.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.Engine.Serialization
{
    class SerializationBuilder : Serializable
    {
        private int size = 0;
        private List<Serializable> items = new List<Serializable>();

        public void Append(Serializable item)
        {
            size = size + item.SerializationSize;
            items.Add(item);
        }

        public void Append(int integer)
        {
            Serializable item = new SInteger(integer);
            this.Append(item);
        }

        public void Append(byte[] buffer)
        {
            Serializable item = new SerializableBuffer(buffer);
            this.Append(item);
        }

        public int SerializationSize
        {
            get
            {
                return size;
            }
        }

        public byte[] Serialize()
        {
            return Utils.Serialize(this);
        }

        public void Serialize(byte[] buffer, ref int bufferOffset)
        {
            foreach(Serializable item in items)
            {
                item.Serialize(buffer, ref bufferOffset);
            }
        }
    }
}
