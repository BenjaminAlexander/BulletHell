
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

        internal InstantSet Instant
        {
            get
            {
                return instant;
            }
        }

        public SubType GetObject<SubType>(int id) where SubType : GameObject, new()
        {
            return instant.GetObject<SubType>(id);
        }
    }
}
