using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.IO;

namespace MyGame.GameStateObjects.Ships
{
    class PlayerGun : Gun, IOObserver
    {
        IOEvent space = new MyGame.IO.Events.KeyDown(Microsoft.Xna.Framework.Input.Keys.Space);

        public PlayerGun(GameState gameState, Ship parent, Vector2 positionRelativeToParent, float directionRelativeToParent, InputManager inputManager)
            : base(gameState, parent, positionRelativeToParent, directionRelativeToParent)
        {
            inputManager.Register(space, this);
        }

        public void UpdateWithIOEvent(IOEvent ioEvent)
        {
            if (ioEvent.Equals(space))
            {
                Fire();
            }
        }
    }
}
