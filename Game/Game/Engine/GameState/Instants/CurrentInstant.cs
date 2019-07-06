
namespace MyGame.Engine.GameState.Instants
{
    public class CurrentInstant
    {
        private Instant instant;

        internal CurrentInstant(Instant instant)
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

        public GameObject GetObject(int id)
        {
            return instant.GetObject(id);
        }
    }
}
