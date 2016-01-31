using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.GameStateObjects.PhysicalObjects.MovingGameObjects.Ships;
using MyGame.GameServer;

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

        public ControlStateUpdate GetStateMessage(GameTime currentGameTime)
        {
            return new ControlStateUpdate(currentGameTime, this);
        }
    }
}
