using MyGame.Engine.DataStructures;
using MyGame.Engine.GameState.InstantObjectSet;
using MyGame.Engine.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.Engine.GameState.GameObjectFactory
{
    class ObjectFactory
    {
        private TypeManager typeManager;
        private TwoWayMap<int, ObjectTypeFactoryInterface> typeFactories = new TwoWayMap<int, ObjectTypeFactoryInterface>(new IntegerComparer());

        public ObjectFactory(InstantSet instantSet)
        {
            this.typeManager = instantSet.TypeManager;
            foreach (InstantTypeSetInterface typeSet in instantSet)
            {
                typeFactories[typeSet.GetMetaData.TypeID] = typeSet.NewObjectTypeFactory();
            }
        }

        public SubType NewObject<SubType>() where SubType : GameObject, new()
        {
            int typeId = typeManager.GetMetaData(typeof(SubType)).TypeID;
            ObjectTypeFactory<SubType> typeFactory = (ObjectTypeFactory<SubType>)typeFactories[typeId];
            SubType newObject = typeFactory.NewObject();
            return newObject;
        }
    }
}
