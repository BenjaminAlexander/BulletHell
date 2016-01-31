using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGame.IO;
using MyGame.IO.Events;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MyGame.GameStateObjects;
using MyGame.GameStateObjects.PhysicalObjects.MovingGameObjects.Ships;
using MyGame.GameClient;
using MyGame.GameServer;

namespace MyGame.PlayerControllers
{
    public class LocalPlayerController : ControlState, IOObserver
    {
        private ClientGame game;
        private IOEvent forward;
        private IOEvent back;
        private IOEvent left;
        private IOEvent right;
        private IOEvent fire;
        private IOEvent space;

        private float angleControl = 0;
        private float movementControl = 0;
        private Boolean isFire = false;

        public LocalPlayerController(ClientGame game) : base()
        {
            this.game = game;

            forward = new KeyDown(Keys.W);
            back = new KeyDown(Keys.S);
            left = new KeyDown(Keys.A);
            right = new KeyDown(Keys.D);
            fire = new LeftMouseDown();
            space = new KeyDown(Keys.Space);

            this.game.InputManager.Register(forward, this);
            this.game.InputManager.Register(back, this);
            this.game.InputManager.Register(left, this);
            this.game.InputManager.Register(right, this);
            this.game.InputManager.Register(fire, this);
            this.game.InputManager.Register(space, this);
        }

        public void Update()
        {
            Vector2 aimpoint;
            Ship focus = this.game.GetLocalPlayerFocus();
            //this.game.ControllerFocus.GetFocus(this.game.PlayerID);
            if (focus != null)
            {
                aimpoint = this.game.Camera.ScreenToWorldPosition(IOState.MouseScreenPosition()) - focus.Position;
            }
            else
            {
                aimpoint = new Vector2(0);
            }
            this.AngleControl = angleControl;
            this.TargetAngle = (float)(2 * Math.PI + 1);
            this.MovementControl = movementControl;
            this.Aimpoint = aimpoint;
            this.Fire = isFire;

            angleControl = 0;
            movementControl = 0;
            isFire = false;
        }

        public void UpdateWithIOEvent(IOEvent ioEvent)
        {
            if (ioEvent.Equals(forward))
            {
                movementControl = movementControl + 1;
            }
            else if (ioEvent.Equals(back))
            {
                movementControl = movementControl - 1;
            }
            else if (ioEvent.Equals(left))
            {
                angleControl = angleControl - 1;
            }
            else if (ioEvent.Equals(right))
            {
                angleControl = angleControl + 1;
            }
            else if (ioEvent.Equals(fire) || ioEvent.Equals(space))
            {
                isFire = true;
            }
        }
    }
}
