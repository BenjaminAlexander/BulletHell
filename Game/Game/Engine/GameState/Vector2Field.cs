using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.Engine.Serialization;
using MyGame.Engine.Serialization.DataTypes;

namespace MyGame.Engine.GameState
{
    class Vector2Field : GenericField<Vector2>
    {
        public Vector2Field(GameObject obj) : base(obj)
        {
        }

        internal override FieldValue GetInitialField()
        {
            Vector2Value value = new Vector2Value();
            value.Value = this.InitialValue;
            return value;
        }
    }
}
