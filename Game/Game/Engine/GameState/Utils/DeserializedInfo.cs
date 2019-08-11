using MyGame.Engine.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyGame.Engine.GameState.Utils
{
    class DeserializedInfo
    {
        private volatile bool isDeserialized = false;

        public bool IsDeserialized
        {
            get
            {
                return isDeserialized;
            }

            set
            {
                isDeserialized = value;
            }
        }
    }
}
