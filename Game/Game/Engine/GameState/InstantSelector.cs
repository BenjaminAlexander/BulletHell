using MyGame.Engine.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.Engine.GameState
{
    interface InstantSelector : Serializer<GameObject>
    {
        int ReadInstant
        {
            get;
        }

        int WriteInstant
        {
            get;
        
        }

        int SerializeInstant
        {
            get;
        }
    }
}
