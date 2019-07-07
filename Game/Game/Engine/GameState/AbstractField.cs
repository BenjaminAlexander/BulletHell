using MyGame.Engine.GameState.Instants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.Engine.GameState
{
    public abstract class AbstractField
    {
        public AbstractField(InitialInstant instant)
        {
            instant.Object.AddField(this);
        }

        internal abstract void CopyFieldValues(CurrentInstant current, NextInstant next);

        internal abstract int SerializationSize(Instant container);

        internal abstract void Serialize(Instant container, byte[] buffer, ref int bufferOffset);

        /**
            * Returns True if Values were changed
            */
        internal abstract bool Deserialize(Instant container, byte[] buffer, ref int bufferOffset);

        internal abstract bool IsIdentical(Instant container, AbstractField other, Instant otherContainer);

        internal abstract List<Instant> GetInstantSet();
    }
}
