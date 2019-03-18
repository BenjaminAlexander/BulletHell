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
    class Vector2Field : GenericField<Microsoft.Xna.Framework.Vector2, Serialization.DataTypes.SVector2>
    {
        public Vector2Field(GameObject obj) : base(obj)
        {
        }
    }
}
