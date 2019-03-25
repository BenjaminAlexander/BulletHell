using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.Engine.Serialization;
using MyGame.Engine.Reflection;

namespace MyGame.Engine.GameState
{
    class GameObjectCollection : InstantSerializedCollection<GameObject>, InstantSelector
    {
        int readInstant = 0;
        int writeInstant = 1;
        NewConstraintTypeFactory<GameObject> factory;

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

        private GameObjectCollection(NewConstraintTypeFactory<GameObject> factory) : base(new InstantTypeSerializer<GameObject>(factory))
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

        public override void Add(GameObject obj)
        {
            obj.SetDependencies(this);
            base.Add(obj);
        }

        public void Update(int instant)
        {
            readInstant = instant;
            writeInstant = instant + 1;
            foreach(GameObject obj in this)
            {
                obj.Update(readInstant, writeInstant);
            }
        }
    }
}
