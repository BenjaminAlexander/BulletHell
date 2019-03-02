using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.Engine.GameState.ObjectState
{
    class GenericStateManager<T> where T : ObjectState
    {
        Dictionary<int, T> statesAtInstant = new Dictionary<int, T>();

    }
}
