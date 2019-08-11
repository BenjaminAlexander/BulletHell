
using MyGame.Engine.GameState.Instants;

namespace MyGame.Engine.GameState.Instants
{
    public class CurrentInstant
    {
        private Instant instant;

        internal CurrentInstant(Instant instant)
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

        internal Instant InstantSet
        {
            get
            {
                return instant;
            }
        }

    }
}
