using MyGame.Engine.GameState.GameObjectFactory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MyGame.Engine.GameState.TypeManager;

namespace MyGame.Engine.GameState.InstantObjectSet
{
    interface InstantTypeSetInterface : IEnumerable<GameObject>
    {
        ObjectTypeFactoryInterface NewObjectTypeFactory(InstantTypeSetInterface nextInstantTypeSet);
        void Add(GameObject obj);
        bool DeserializeRemoveAll();
        bool Deserialize(byte[] buffer, ref int bufferOffset);
        
        TypeMetadataInterface GetMetaData
        {
            get;
        }

        int InstantID
        {
            get;
        }

        int TypeID
        {
            get;
        }

        int ObjectCount
        {
            get;
        }
    }
}
