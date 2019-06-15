using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.Engine.Serialization;
using MyGame.Engine.Serialization.DataTypes;
using static MyGame.Engine.GameState.GameObjectContainer;

namespace MyGame.Engine.GameState
{
    class FloatField : GenericField<float>
    {
        public FloatField(GameObject obj) : base(obj)
        {
        }

        internal override FieldValue GetInitialField()
        {
            FloatValue value = new FloatValue();
            value.Value = this.InitialValue;
            return value;
        }
    }
}
