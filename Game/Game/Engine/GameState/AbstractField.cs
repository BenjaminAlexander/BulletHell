using MyGame.Engine.GameState.Instants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.Engine.GameState
{
    //TODO: see if copied code in field subclasses can be moved into base
    public abstract class AbstractField
    {
        private GameObject gameObject;

        public AbstractField(CreationToken creationToken)
        {
            creationToken.Object.AddField(this);
            this.gameObject = creationToken.Object;
        }

        internal bool IsInstantDeserialized(Instant instant)
        {
            return gameObject.IsInstantDeserialized(instant);
        }

        internal abstract bool HasInstant(Instant instant);

        internal abstract void SetDefaultValue(Instant instant);

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
