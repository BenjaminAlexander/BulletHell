using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.Engine.GameState
{
    class InstantTypeSet<SubType> : InstantTypeSetInterface where SubType : GameObject, new()
    {
        public TypeSet<SubType> globalSet;

        public InstantTypeSet(TypeSet<SubType> globalSet)
        {
            this.globalSet = globalSet;
        }
    }
}
