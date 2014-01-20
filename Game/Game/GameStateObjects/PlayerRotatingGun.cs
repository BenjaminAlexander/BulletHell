using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGame.PlayerControllers;
using Microsoft.Xna.Framework;

namespace MyGame.GameStateObjects.Ships
{
    class PlayerRotatingGun : Turret, GunnerObserver
    {
        //IOEvent space = new MyGame.IO.Events.LeftMouseDown();
        Boolean pointedAtTarget = false;
        GunnerController controller;
        public PlayerRotatingGun(Ship parent, Vector2 positionRelativeToParent, float directionRelativeToParent, GunnerController controller)
            : base(parent, positionRelativeToParent, directionRelativeToParent, (float)(Math.PI * .5))
        {
            //inputManager.Register(space, this);
            controller.Register(this);
            this.controller = controller;
        }

        protected override void UpdateSubclass(GameTime gameTime)
        {
            base.UpdateSubclass(gameTime);
            //Vector2 mouseWorldPosition = Vector2.Transform(IOState.MouseScreenPosition(), this.GameState.Camera.GetScreenToWorldTransformation());
            //PointAt(mouseWorldPosition);
            Vector2 target = controller.AimPointInWorld;
            this.Target = target;
            pointedAtTarget = IsPointedAt(target, 50);
        }

        public void UpdateWithEvent(GunnerEvent e)
        {
            //if (pointedAtTarget && e.Equals(space))
            if (e is GunnerFire && pointedAtTarget)
            {
                Fire();
            }
        }
    }
}
