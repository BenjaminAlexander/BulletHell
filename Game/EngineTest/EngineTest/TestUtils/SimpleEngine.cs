using MyGame.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.Engine.GameState;

namespace EngineTest.EngineTest.TestUtils
{
    class SimpleEngine : Engine
    {
        public override void AddTypes(TypeManagerInterface typeManager)
        {
            typeManager.AddType<SimpleObjectA>();
            typeManager.AddType<SimpleObjectB>();
        }
    }
}
