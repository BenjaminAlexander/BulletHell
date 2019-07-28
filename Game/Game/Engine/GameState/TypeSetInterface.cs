using MyGame.Engine.GameState.GameObjectFactory;
using MyGame.Engine.GameState.InstantObjectSet;
using MyGame.Engine.GameState.Instants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MyGame.Engine.GameState.TypeManager;

namespace MyGame.Engine.GameState
{
    interface TypeSetInterface
    {
        void PrepareForUpdate(int next, ObjectFactory factory);
        void Update(CurrentInstant current, NextInstant next);
        InstantTypeSetInterface GetInstantTypeSetInterface(int instantId);
        int TypeID
        {
            get;
        }
    }
}
