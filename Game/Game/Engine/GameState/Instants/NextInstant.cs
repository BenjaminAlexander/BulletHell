
namespace MyGame.Engine.GameState.Instants
{
    public class NextInstant
    {
        private Instant instant;

        internal NextInstant(Instant instant)
        {
            this.instant = instant;
        }

        internal Instant Instant
        {
            get
            {
                return instant;
            }
        }

        internal CurrentInstant AsCurrent
        {
            get
            {
                return new CurrentInstant(instant);
            }
        }
    }
}
