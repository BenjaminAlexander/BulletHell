using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.Networking;
using MyGame.GameStateObjects.PhysicalObjects.MovingGameObjects.Ships;

namespace MyGame.PlayerControllers
{
    public class ControlState
    {
        private Vector2 aimpoint = new Vector2(0);  //where u aimin
        private float angleControl = 0;   // rate of turn towards target angle
        private float targetAngle = 0;  //direction you want to turn to
        private float movementControl = 0;  //accelerate, decelerate
        private Boolean fire = false;  // boom boom

        public Vector2 Aimpoint
        {
            get
            {
                return aimpoint;
            }
            set
            {
                aimpoint = value;
            }
        }

        public Boolean Fire
        {
            get
            {
                return fire;
            }
            set
            {
                fire = value;
            }
        }

        public float TargetAngle
        {
            get
            {
                return targetAngle;
            }
            set 
            {
                targetAngle = value;
            }

        }

        public float AngleControl
        {
            get
            {
                return angleControl;
            }
            set
            {
                angleControl = value;
                if (this.angleControl > 1)
                    this.angleControl = 1;
                if (this.angleControl < -1)
                    this.angleControl = -1;
            }
        }

        public float MovementControl
        {
            get
            {
                return movementControl;
            }
            set
            {
                movementControl = value;
                if (this.movementControl > 1)
                    this.movementControl = 1;
                if (this.movementControl < -1)
                    this.movementControl = -1;
            }
        }

        public void ApplyUpdate(PlayerControllerUpdate message)
        {
            message.ResetReader();
            this.aimpoint = message.ReadVector2();
            this.angleControl = message.ReadFloat();
            this.targetAngle = message.ReadFloat();
            this.movementControl = message.ReadFloat();
            this.fire = message.ReadBoolean();
            message.AssertMessageEnd();
        }

        public PlayerControllerUpdate GetStateMessage(GameTime currentGameTime)
        {
            PlayerControllerUpdate message = new PlayerControllerUpdate(currentGameTime);
            message.Append(aimpoint);
            message.Append(angleControl);
            message.Append(targetAngle);
            message.Append(movementControl);
            message.Append(fire);
            return message;
        }
    }
}
