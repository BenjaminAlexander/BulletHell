using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGame.IO;
using Microsoft.Xna.Framework;

namespace MyGame.GameStateObjects.Ships
{
    class PlayerRotatingGun : Turret, IOObserver
    {
        IOEvent space = new MyGame.IO.Events.LeftMouseDown();

        public PlayerRotatingGun(GameState gameState, Ship parent, Vector2 positionRelativeToParent, InputManager inputManager)
            : base(gameState, parent, positionRelativeToParent)
        {
            inputManager.Register(space, this);
        }

        protected override void UpdateSubclass(GameTime gameTime)
        {
            base.UpdateSubclass(gameTime);
            Vector2 mouseWorldPosition = Vector2.Transform(IOState.MouseScreenPosition(), this.GameState.Camera.GetScreenToWorldTransformation());
            PointAt(mouseWorldPosition);
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
