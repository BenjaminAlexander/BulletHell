
using MyGame.Engine.GameState.GameObjectFactory;
using MyGame.Engine.GameState.Instants;

namespace MyGame.Engine.GameState.Instants
{
    //TODO: make it so next instant can only be use on one object
    public class NextInstant
    {
        private Instant instant;
        private ObjectFactory factory;

        internal NextInstant(Instant instant, ObjectFactory factory)
        {
            this.instant = instant;
            this.factory = factory;
        }

        internal int InstantID
        {
            get
            {
                return instant.InstantID;
            }
        }

        //TODO: review this method.  is this right for the end user?
        public SubType NewGameObject<SubType>() where SubType : GameObject, new()
        {
            return factory.NewObject<SubType>();
        }
    }
}
