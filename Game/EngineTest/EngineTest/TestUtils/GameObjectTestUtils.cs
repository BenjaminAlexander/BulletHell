using Microsoft.Xna.Framework;
using MyGame.Engine.GameState;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineTest.EngineTest.TestUtils
{
    class GameObjectTestUtils
    {
        public SimpleObjectA expectedA;
        public SimpleObjectB expectedB;
        public InstantSelector.InstantController instantController;

        public GameObjectTestUtils()
        {
            instantController = new InstantSelector.InstantController();
            expectedA = SimpleObjectA.Factory(instantController, 1234, new Vector2(656.34f, 345.4f), 787.9f);
            expectedB = GameObject.Construct<SimpleObjectB>(instantController);

        }
    }
}
