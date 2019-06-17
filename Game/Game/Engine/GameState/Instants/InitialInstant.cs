
namespace MyGame.Engine.GameState.Instants
{
    public class InitialInstant
    {
        private Instant instant;
        private GameObject obj;

        internal InitialInstant(Instant instant, GameObject obj)
        {
            this.instant = instant;
            this.obj = obj;
        }

        internal Instant Instant
        {
            get
            {
                return instant;
            }
        }

        internal GameObject Object
        {
            get
            {
                return obj;
            }
        }
    }
}
