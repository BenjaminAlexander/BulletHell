using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyGame.Engine.GameState.GameObjectUtils
{
    class DeserializedInfo
    {
        bool isDeserialized = false;
        public DeserializedInfo()
        {

        }

        public DeserializedInfo(bool isDeserialized)
        {
            this.isDeserialized = isDeserialized;
        }

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
