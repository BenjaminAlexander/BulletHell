using MyGame.Engine.DataStructures;
using MyGame.Engine.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MyGame.Engine.GameState.TypeManager;
using System.Collections;

namespace MyGame.Engine.GameState
{
    class GlobalSet : IEnumerable<GameObject>
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

        public TypeSetInterface GetTypeSet(int typeId)
        {
            return typeSets[typeId];
        }

        public TypeSetInterface GetTypeSet(Type type)
        {
            int typeId = typeManager.GetMetaData(type).TypeID;
            return typeSets[typeId];
        }

        public TypeSet<SubType> GetTypeSet<SubType>() where SubType : GameObject, new()
        {
            int typeId = typeManager.GetMetaData(typeof(SubType)).TypeID;
            return (TypeSet<SubType>)typeSets[typeId];
        }

        public GameObject GetObject(int typeId, int objectId)
        {
            return typeSets[typeId].GetObject(objectId);
        }

        public GameObject GetObject(Type type, int objectId)
        {
            int typeId = typeManager.GetMetaData(type).TypeID;
            return typeSets[typeId].GetObject(objectId);
        }

        public SubType GetObject<SubType>(int objectId) where SubType : GameObject, new()
        {
            int typeId = typeManager.GetMetaData(typeof(SubType)).TypeID;
            TypeSet<SubType> typeSet = (TypeSet<SubType>)typeSets[typeId];
            return typeSet[objectId];
        }

        public InstantObjectSet NewInstantObjectSet()
        {
            TwoWayMap<int, InstantTypeSetInterface> instantTypeSets = new TwoWayMap<int, InstantTypeSetInterface>();
            foreach (KeyValuePair<int, TypeSetInterface> pair in typeSets)
            {
                instantTypeSets[pair.Key] = pair.Value.NewInstantTypeSet();
            }
            return new InstantObjectSet(instantTypeSets);
        }

        public bool CheckIntegrety()
        {
            foreach (KeyValuePair<int, TypeSetInterface> pair in typeSets)
            {
                if (pair.Key != pair.Value.GetMetaData.TypeID)
                {
                    log.Error("Object Set Type ID does not match key in map");
                    return false;
                }

                if(!pair.Value.CheckIntegrety())
                {
                    log.Error("Object Set failed its integrety check");
                    return false;
                }
            }
            return true;
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
            throw new NotImplementedException();
        }
    }
}
