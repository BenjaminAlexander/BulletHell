/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.Engine.Serialization;
using MyGame.Engine.Reflection;

namespace MyGame.Engine.GameState
{
    class GameObjectCollection : InstantSelector
    {
        int readInstant = 0;
        int writeInstant = 1;
        NewConstraintTypeFactory<GameObject> factory;

        Dictionary<int, InstantCollection> instants = new Dictionary<int, InstantCollection>();

        public int ReadInstant
        {
            get
            {
                return readInstant;
            }

            set
            {
                readInstant = value;
            }
        }

        public int WriteInstant
        {
            get
            {
                return writeInstant;
            }

            set
            {
                writeInstant = value;
            }
        }

        private GameObjectCollection(NewConstraintTypeFactory<GameObject> factory)
        {
            this.factory = factory;
        }

        public GameObjectCollection() : this(new NewConstraintTypeFactory<GameObject>())
        {

        }

        public void AddType<SubType>() where SubType : GameObject, new()
        {
            factory.AddType<SubType>();
        }

        public void Update(int instant)
        {
            InstantCollection instantCollection = instants[instant];
            if(instantCollection == null)
            {
                throw new Exception("Instant does not exist");
            }

            instants[instant + 1] = instantCollection.NextInstant();
        }
    }
}*/
