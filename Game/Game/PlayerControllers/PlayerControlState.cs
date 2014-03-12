using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.Networking;

namespace MyGame.PlayerControllers
{
    public class PlayerControlState
    {
        private Vector2 aimpoint = new Vector2(0);
        private float angleControl = 0;
        private float targetAngle = 0;
        private float movementControl = 0;
        private Boolean fire = false;

        public Vector2 Aimpoint
        {
            get
            {
                return aimpoint;
            }
        }

        public Boolean Fire
        {
            get
            {
                return fire;
            }
        }

        public float TargetAngle
        {
            get
            {
                return targetAngle;
            }
        }

        public float AngleControl
        {
            get
            {
                return angleControl;
            }
        }

        public float MovementControl
        {
            get
            {
                return movementControl;
            }
        }

        public PlayerControlState(float angleControl, float targetAngle, float movementControl, Vector2 aimpoint, Boolean fire)
        {
            this.aimpoint = aimpoint;
            this.targetAngle = targetAngle;
            this.angleControl = angleControl;
            this.movementControl = movementControl;
            if (this.angleControl > 1)
                this.angleControl = 1;
            if (this.angleControl < -1)
                this.angleControl = -1;
            if (this.movementControl > 1)
                this.movementControl = 1;
            if (this.movementControl < -1)
                this.movementControl = -1;

            this.fire = fire;
        }

        public PlayerControlState(PlayerControllerUpdate message)
        {
            message.ResetReader();
            this.aimpoint = message.ReadVector2();
            this.angleControl = message.ReadFloat();
            this.targetAngle = message.ReadFloat();
            this.movementControl = message.ReadFloat();
            this.fire = message.ReadBoolean();
            message.AssertMessageEnd();
        }

        public PlayerControllerUpdate GetStateMessage()
        {
            PlayerControllerUpdate message = new PlayerControllerUpdate();
            message.Append(aimpoint);
            message.Append(angleControl);
            message.Append(targetAngle);
            message.Append(movementControl);
            message.Append(fire);
            return message;
        }

    }
}
