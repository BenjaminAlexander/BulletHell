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
        private Vector2 move = new Vector2(0);
        private Boolean fire = false;

        public Vector2 Move
        {
            get
            {
                return move;
            }
        }

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

        public PlayerControlState(Vector2 aimpoint, Vector2 move, Boolean fire)
        {
            this.aimpoint = aimpoint;
            this.move = move;
            if (this.move.Length() > 1)
            {
                this.move.Normalize();
            }

            this.fire = fire;
        }

        public PlayerControlState(PlayerControllerUpdate message)
        {
            message.ResetReader();
            this.aimpoint = message.ReadVector2();
            this.move = message.ReadVector2();
            this.fire = message.ReadBoolean();
            message.AssertMessageEnd();
        }

        public PlayerControllerUpdate GetStateMessage()
        {
            PlayerControllerUpdate message = new PlayerControllerUpdate();
            message.Append(aimpoint);
            message.Append(move);
            message.Append(fire);
            return message;
        }

    }
}
