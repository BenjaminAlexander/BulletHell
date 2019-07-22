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

        public ObjectFactory(InstantSet currentInstantSet, InstantSet nextInstantSet)
        {
            this.typeManager = currentInstantSet.TypeManager;
            foreach (InstantTypeSetInterface typeSet in currentInstantSet)
            {
                InstantTypeSetInterface nextInstantTypeSet = nextInstantSet.GetInstantTypeSet(typeSet.GetMetaData.TypeID);
                typeFactories[typeSet.GetMetaData.TypeID] = typeSet.NewObjectTypeFactory(nextInstantTypeSet);
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
