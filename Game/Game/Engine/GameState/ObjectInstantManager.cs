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
    class ObjectInstantManager : IEnumerable<GameObject>, IEnumerable<TypeSetInterface>, IEnumerable<InstantSet>
    {
        private static Logger log = new Logger(typeof(ObjectInstantManager));

        private TypeManager typeManager;
        private TwoWayMap<int, TypeSetInterface> typeSets = new TwoWayMap<int, TypeSetInterface>(new IntegerComparer());
        //TODO: to deserialize go instant -> type -> id
        //TODO: instant, type, total count of type, offset of type, id, ....data...
        private TwoWayMap<int, InstantSet> instantSets = new TwoWayMap<int, InstantSet>(new IntegerComparer());

        public ObjectInstantManager(TypeManager typeManager)
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

        public InstantSet GetInstantSet(int instantId)
        {
            if (instantSets.ContainsKey(instantId))
            {
                return instantSets[instantId];
            }
            else
            {
                InstantSet instantSet = new InstantSet(this, instantId);
                instantSets[instantId] = instantSet;
                return instantSet;
            }
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

        IEnumerator<InstantSet> IEnumerable<InstantSet>.GetEnumerator()
        {
            return instantSets.Values.GetEnumerator();
        }
    }
}
