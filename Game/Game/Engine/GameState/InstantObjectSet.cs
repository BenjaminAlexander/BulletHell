using MyGame.Engine.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.Engine.GameState
{
    class InstantObjectSet
    {
        TwoWayMap<int, InstantTypeSetInterface> typeSets;

        public InstantObjectSet(TwoWayMap<int, InstantTypeSetInterface> typeSets)
        {
            this.typeSets = typeSets;
        }
    }
}
