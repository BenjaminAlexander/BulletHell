
namespace MyGame.Engine.GameState.Instants
{
    //TODO: make it so next instant can only be use on one object
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

        //TODO: review this method.  is this right for the end user?
        public SubType NewGameObject<SubType>() where SubType : GameObject, new()
        {
            return instant.NewGameObject<SubType>();
        }
    }
}
