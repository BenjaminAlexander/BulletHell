using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.Engine.GameState.InstantObjectSet;

namespace MyGame.Engine.GameState
{
    class TypeSetInterfaceStub : TypeSetInterface
    {
        public TypeManager.TypeMetadataInterface GetMetaData
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IEnumerator<GameObject> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public GameObject GetObject(int id)
        {
            throw new NotImplementedException();
        }

        public InstantTypeSetInterface GetInstantTypeSet(int instantId)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public InstantTypeSetInterface GetInstantTypeSetInterface(int instantId)
        {
            throw new NotImplementedException();
        }
    }
}
