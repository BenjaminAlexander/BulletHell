using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.Engine.Serialization;
using MyGame.Engine.Serialization.DataTypes;

namespace MyGame.Engine.GameState
{
    class FloatField : GenericField<float, SFloat>
    {
        public FloatField(GameObject obj) : base(obj)
        {
        }
    }
}
