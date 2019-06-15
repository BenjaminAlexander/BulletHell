using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.Engine.Serialization;
using MyGame.Engine.Serialization.DataTypes;

namespace MyGame.Engine.GameState
{
    class IntegerField : GenericField<int>
    {
        public IntegerField(GameObject obj) : base(obj)
        {
        }

        internal override FieldValue GetInitialField()
        {
            IntegerValue value = new IntegerValue();
            value.Value = this.InitialValue;
            return value;
        }
    }
}
