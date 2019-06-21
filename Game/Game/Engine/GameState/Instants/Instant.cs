using MyGame.Engine.Serialization;
using System;

namespace MyGame.Engine.GameState.Instants
{
    class Instant
    {
        private int instant;

        public Instant(int instant)
        {
            this.instant = instant;
        }

        //TODO: can this get removed?
        public Instant(byte[] buffer, ref int bufferOffset)
        {
            instant = Serialization.Utils.ReadInt(buffer, ref bufferOffset);
        }

        public Instant GetNext
        {
            get
            {
                return new Instant(instant + 1);
            }
        }

        public CurrentInstant AsCurrent
        {
            get
            {
                return new CurrentInstant(this);
            }
        }

        public NextInstant AsNext
        {
            get
            {
                return new NextInstant(this);
            }
        }

        public int SerializationSize
        {
            get
            {
                return sizeof(int);
            }
        }

        public void Serialize(byte[] buffer, ref int bufferOffset)
        {
            if (buffer.Length - bufferOffset < SerializationSize)
            {
                throw new Exception("Buffer length does not match expected state length");
            }
            Serialization.Utils.Write(instant, buffer, ref bufferOffset);
        }

        public override bool Equals(object obj)
        {
            if(obj != null && obj is Instant)
            {
                return instant == ((Instant)obj).instant;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return instant;
        }
    }
}
