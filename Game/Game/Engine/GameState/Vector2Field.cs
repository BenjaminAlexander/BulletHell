using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.Engine.Serialization;

namespace MyGame.Engine.GameState
{
    class Vector2Field : GenericMetaField<SerializableVector2>
    {
        public Vector2Field(GameObject obj) : base(obj)
        {
        }
    }
}
