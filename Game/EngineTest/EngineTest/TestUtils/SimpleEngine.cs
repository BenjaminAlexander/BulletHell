using MyGame.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.Engine.GameState;
using MyGame.Engine.GameState.Instants;

namespace EngineTest.EngineTest.TestUtils
{
    class SimpleEngine : Engine
    {
        public override void AddTypes(TypeManagerInterface typeManager)
        {
            typeManager.AddType<SimpleObjectA>();
            typeManager.AddType<SimpleObjectB>();
        }

        public override void ServerInitializeInstantZero(NextInstant instantZero)
        {
            SimpleObjectA.Factory(instantZero, 123, new Microsoft.Xna.Framework.Vector2(244), 45.3f);
            SimpleObjectB.Factory(instantZero, 13, 567, 4545, 345);
            SimpleObjectB.Factory(instantZero, 676, 8, 2, 1);
            SimpleObjectB.Factory(instantZero, 13, 23, 43453545, 0908);
            SimpleObjectB.Factory(instantZero, 87, 2, 46, 7686);
        }
    }
}
