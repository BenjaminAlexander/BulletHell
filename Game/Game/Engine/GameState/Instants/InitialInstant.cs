
namespace MyGame.Engine.GameState.Instants
{
    //TODO: make it imposible to use an initial instant to create a Field outside of the expected method
    //TODO: this needs to be renamed
    public class InitialInstant
    {
        private GameObject obj;
        private Instant instant;

        internal InitialInstant(Instant instant, GameObject obj)
        {
            this.obj = obj;
            this.instant = instant;
        }

        internal GameObject Object
        {
            get
            {
                return obj;
            }
        }

        internal Instant Instant
        {
            get
            {
                return instant;
            }
        }
    }
}
