using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.Engine.RealTimeNetworking
{
    class Object
    {
        Dictionary<int, ObjectInstantState> instantStates = new Dictionary<int, ObjectInstantState>();

        public Object(ObjectInstantState initalInstantState)
        {
            instantStates.Add(initalInstantState.Instant, initalInstantState);
        }

        public ObjectInstantState getState(int instant)
        {
            return instantStates[instant];
        }

    }
}
