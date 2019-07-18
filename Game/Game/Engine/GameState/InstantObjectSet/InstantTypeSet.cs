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
        private int instantId;

        public InstantTypeSet(TypeSet<SubType> globalSet, int instantId)
        {
            this.globalSet = globalSet;
            this.metaData = globalSet.GetMetaData;
            this.instantId = instantId;
        }

        public ObjectTypeFactoryInterface NewObjectTypeFactory(int nextInstantId)
        {
            return new ObjectTypeFactory<SubType>(globalSet, this, nextInstantId);
        }

        public void Add(GameObject obj)
        {
            objects[obj.ID] = (SubType)obj;
        }

        public SubType GetObject(int id)
        {
            if (objects.ContainsKey(id))
            {
                return objects[id];
            }
            return null;
        }

        public int GreatestID
        {
            get
            {
                return objects.GreatestKey;
            }
        }

        public TypeMetadataInterface GetMetaData
        {
            get
            {
                return metaData;
            }
        }

        public int InstantID
        {
            get
            {
                return instantId;
            }
        }
    }
}
