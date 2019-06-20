using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.Engine.Serialization;
using MyGame.Engine.Reflection;
using MyGame.Engine.DataStructures;
using MyGame.Engine.GameState.Instants;

namespace MyGame.Engine.GameState
{
    class GameObjectCollection
    {
        int nextId = 0;
        TwoWayMap<int, GameObject> idsToObjects = new TwoWayMap<int, GameObject>();
        TwoWayMap<int, Instant> instantMap = new TwoWayMap<int, Instant>();


        //TODO: what is the right type of instant?
        public SubType NewGameObject<SubType>(Instant instant) where SubType : GameObject, new()
        {
            SubType newObject = GameObject.Construct<SubType>(instant);
            idsToObjects[nextId] = newObject;
            return newObject;
        }

        /*
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
        }*/
    }
}
