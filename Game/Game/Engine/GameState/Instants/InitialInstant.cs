
namespace MyGame.Engine.GameState.Instants
{
    //TODO: make it imposible to use an initial instant to create a Field outside of the expected method
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
