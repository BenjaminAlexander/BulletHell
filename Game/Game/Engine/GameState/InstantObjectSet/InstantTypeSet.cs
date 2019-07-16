using MyGame.Engine.DataStructures;
using MyGame.Engine.GameState.GameObjectFactory;
using MyGame.Engine.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MyGame.Engine.GameState.TypeManager;

namespace MyGame.Engine.GameState.InstantObjectSet
{
    class InstantTypeSet<SubType> : InstantTypeSetInterface where SubType : GameObject, new()
    {
        private TypeSet<SubType> globalSet;
        private TwoWayMap<int, SubType> objects = new TwoWayMap<int, SubType>(new IntegerComparer());
        private TypeMetadataInterface metaData;

        public InstantTypeSet(TypeSet<SubType> globalSet)
        {
            this.globalSet = globalSet;
            this.metaData = globalSet.GetMetaData;
        }

        public ObjectTypeFactoryInterface NewObjectTypeFactory()
        {
            return new ObjectTypeFactory<SubType>(globalSet, objects.GreatestKey + 1);
        }

        public TypeMetadataInterface GetMetaData
        {
            get
            {
                return metaData;
            }
        }
    }
}
