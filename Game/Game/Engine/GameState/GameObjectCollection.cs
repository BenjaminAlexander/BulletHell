using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.Engine.Serialization;
using MyGame.Engine.Reflection;

namespace MyGame.Engine.GameState
{
    class GameObjectCollection
    {


        NewConstraintTypeFactory<GameObject> factory;
        FullSerializableCollection<GameObject> serializableCollection;
        InstantSelector.InstantController instantController;

        public GameObjectCollection()
        {
            factory = new NewConstraintTypeFactory<GameObject>();
            serializableCollection = new FullSerializableCollection<GameObject>(new TypeSerializer<GameObject>(factory));
            instantController = new InstantSelector.InstantController();
            instantController.SetReadWriteInstant(0);

        }

        public void AddGameObjectType<Type>() where Type : GameObject, new()
        {
            factory.AddItem<Type>();
        }
    }
}
