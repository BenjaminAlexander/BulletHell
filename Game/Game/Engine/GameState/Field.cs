using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MyGame.Engine.GameState.GameObject;

namespace MyGame.Engine.GameState
{
    partial class GameObject
    {
        public abstract class Field
        {
            public Field(GameObject owner)
            {
                owner.AddField(this);
            }

            public abstract FieldValue GetInitialField();
        }
    }
}
