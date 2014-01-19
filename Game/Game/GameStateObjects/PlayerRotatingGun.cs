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
        Boolean pointedAtTarget = false;
        public PlayerRotatingGun(Ship parent, Vector2 positionRelativeToParent, float directionRelativeToParent, InputManager inputManager)
            : base(parent, positionRelativeToParent, directionRelativeToParent, (float)(Math.PI * .5))
        {
            inputManager.Register(space, this);
        }

        protected override void UpdateSubclass(GameTime gameTime)
        {
            base.UpdateSubclass(gameTime);
            Vector2 mouseWorldPosition = Vector2.Transform(IOState.MouseScreenPosition(), this.GameState.Camera.GetScreenToWorldTransformation());
            //PointAt(mouseWorldPosition);
            this.Target = mouseWorldPosition;
            pointedAtTarget = IsPointedAt(mouseWorldPosition, 50);
        }

        public void UpdateWithIOEvent(IOEvent ioEvent)
        {
            if (pointedAtTarget && ioEvent.Equals(space))
            {
                Fire();
            }
        }
    }
}
