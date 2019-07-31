
using MyGame.Engine.GameState.InstantObjectSet;

namespace MyGame.Engine.GameState.Instants
{
    public class CurrentInstant
    {
        private InstantSet instant;

        internal CurrentInstant(InstantSet instant)
        {
            this.instant = instant;
        }

        internal int InstantID
        {
            get
            {
                return instant.InstantID;
            }
        }

        internal InstantSet InstantSet
        {
            get
            {
                return instant;
            }
        }

    }
}
