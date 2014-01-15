using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.IO;
using MyGame.GameStateObjects.Ships;

namespace MyGame.GameStateObjects
{
    public class Gun : MemberPhysicalObject, IOObserver
    {
        IOEvent space = new MyGame.IO.Events.KeyPressEvent(Microsoft.Xna.Framework.Input.Keys.Space);
        private Boolean fire = false;


        public Gun(GameState gameState,Ship parent, Vector2 positionRelativeToParent, float directionRelativeToParent, InputManager inputManager)
            : base(gameState, parent, positionRelativeToParent, directionRelativeToParent)
        {
            inputManager.Register(space, this);
        }

        protected override void UpdateSubclass(GameTime gameTime)
        {
            if (fire && this.Root() is Ship)
            {
                this.GameState.AddGameObject(new Bullet(this.GameState, (Ship)this.Root(), this.WorldPosition(), this.WorldDirection()));
            }
            fire = false;
        }

        public override void Draw(GameTime gameTime, DrawingUtils.MyGraphicsClass graphics)
        {
        }

        public void UpdateWithIOEvent(IOEvent ioEvent)
        {
            if (ioEvent.Equals(space))
            {
                fire = true;
            }
        }
    }
}
