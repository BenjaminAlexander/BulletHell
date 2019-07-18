
using MyGame.Engine.GameState.GameObjectFactory;
using MyGame.Engine.GameState.InstantObjectSet;

namespace MyGame.Engine.GameState.Instants
{
    //TODO: make it so next instant can only be use on one object
    public class NextInstant
    {
        private InstantSet instant;
        private ObjectFactory factory;

        internal NextInstant(InstantSet instant, ObjectFactory factory)
        {
            this.instant = instant;
            this.factory = factory;
        }

        internal InstantSet Instant
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
            return factory.NewObject<SubType>();
        }
    }
}
