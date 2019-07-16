using MyGame.Engine.DataStructures;
using MyGame.Engine.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MyGame.Engine.GameState.TypeManager;
using System.Collections;
using MyGame.Engine.GameState.InstantObjectSet;

namespace MyGame.Engine.GameState
{
    class GlobalSet : IEnumerable<GameObject>, IEnumerable<TypeSetInterface>
    {
        private static Logger log = new Logger(typeof(GlobalSet));

        private TwoWayMap<int, TypeSetInterface> typeSets = new TwoWayMap<int, TypeSetInterface>(new IntegerComparer());
        private TypeManager typeManager;

        public GlobalSet(TypeManager typeManager)
        {
            this.typeManager = typeManager;
            foreach (TypeMetadataInterface metaData in typeManager)
            {
                typeSets.Set(metaData.TypeID, metaData.NewTypeSet());
            }
        }

        public TypeManager TypeManager
        {
            get
            {
                return typeManager;
            }
        }

        public SubType GetObject<SubType>(int objectId) where SubType : GameObject, new()
        {
            int typeId = typeManager.GetMetaData(typeof(SubType)).TypeID;
            TypeSet<SubType> typeSet = (TypeSet<SubType>)typeSets[typeId];
            return typeSet[objectId];
        }

        public InstantSet NewInstantObjectSet()
        {
            return new InstantSet(this);
        }

        public IEnumerator<GameObject> GetEnumerator()
        {
            IEnumerable<GameObject> enumerable = null;
            foreach(KeyValuePair<int, TypeSetInterface> pair in typeSets)
            {
                if(enumerable == null)
                {
                    enumerable = pair.Value;
                }
                else
                {
                    enumerable = Enumerable.Concat<GameObject>(enumerable, pair.Value);
                }
            }
            return enumerable.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        IEnumerator<TypeSetInterface> IEnumerable<TypeSetInterface>.GetEnumerator()
        {
            return typeSets.Values.GetEnumerator();
        }
    }
}
