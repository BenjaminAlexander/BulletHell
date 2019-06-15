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
    class Vector2Field : GenericField<Vector2, Vector2Value>
    {
        public Vector2Field(GameObject obj) : base(obj)
        {
        }
    }
}
