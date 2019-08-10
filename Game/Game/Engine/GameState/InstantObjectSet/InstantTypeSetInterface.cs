using MyGame.Engine.GameState.GameObjectFactory;
using MyGame.Engine.GameState.Instants;
using MyGame.Engine.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MyGame.Engine.GameState.TypeManager;

namespace MyGame.Engine.GameState.InstantObjectSet
{
    interface InstantTypeSetInterface
    {
        ObjectTypeFactoryInterface PrepareForUpdate(InstantTypeSetInterface nextTypeSet);
        void Update(CurrentInstant current, NextInstant next);

        GameObject GetObject(int id);
        bool DeserializeRemoveAll();
        bool Deserialize(byte[] buffer, ref int bufferOffset);
        List<SerializationBuilder> Serialize();
        void Add(GameObject obj);
        void Remove(GameObject obj);
        
        int TypeID
        {
            get;
        }

        int InstantID
        {
            get;
        }


    }
}
