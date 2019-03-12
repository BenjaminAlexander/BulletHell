using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.Engine.Serialization;

namespace MyGame.Engine.GameState
{
    class GenericMetaField<T> : GameObject.Field where T : Serializable, new()
    {
        private Dictionary<int, T> fieldAtInstant = new Dictionary<int, T>();

        public GenericMetaField(GameObject obj) : base(obj)
        {
        }

        public T this[int instant]
        {
            get
            {
                return fieldAtInstant[instant];
            }

            set
            {
                fieldAtInstant[instant] = value;
            }
        }

        public T this[StateAtInstant state]
        {
            get
            {
                return (T)state[this];
            }

            set
            {
                state[this] = value;
            }
        }

        protected override void Copy(GameObject.Field other, int instant)
        {
            this[instant] = ((GenericMetaField<T>)other)[instant];
        }

        public override int Size
        {
            get
            {
                //TODO: this is ugly and needs to be fixed
                return new T().SerializationSize;
            }
        }

        public override void Deserialize(int instant, byte[] buffer, ref int bufferOffset)
        {
            if(!fieldAtInstant.ContainsKey(instant))
            {
                this[instant] = new T();
            }
            this[instant].Deserialize(buffer, ref bufferOffset);
        }

        public override void Serialize(int instant, byte[] buffer, int bufferOffset)
        {
            this[instant].Serialize(buffer, bufferOffset);
        }

        public override Serializable DefaultValue()
        {
            return new T();
        }
    }
}
