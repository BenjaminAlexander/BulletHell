using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.Engine.RealTimeNetworking
{
    class ObjectInstantState
    {
        int instant;
        public ObjectInstantState(int instant)
        {
             this.instant = instant;
        }

        public int Instant
        {
            get
            {
                return instant;
            }
        }


    }
}
