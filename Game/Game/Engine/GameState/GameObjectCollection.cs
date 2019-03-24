using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.Engine.Serialization;
using MyGame.Engine.Reflection;

namespace MyGame.Engine.GameState
{
    class GameObjectCollection : SerializedCollection<GameObject>
    {
        SimpleInstantSelector instantSelector;
        NewConstraintTypeFactory<GameObject> factory;

        private GameObjectCollection(NewConstraintTypeFactory<GameObject> factory, SimpleInstantSelector instantSelector) : base(new TypeSerializer<GameObject>(factory))
        {
            this.factory = factory;
            this.instantSelector = instantSelector;
        }

        public GameObjectCollection() : this(new NewConstraintTypeFactory<GameObject>(), new SimpleInstantSelector())
        {

        }

        public void AddType<SubType>() where SubType : GameObject, new()
        {
            factory.AddType<SubType>();
        }

        public void Update(int read, int write)
        {
            //instantController.SetReadInstant
        }
    }
}
