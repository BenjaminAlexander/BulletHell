using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGame.PlayerControllers;
using Microsoft.Xna.Framework;
using MyGame.Networking;
using System.Net.Sockets;

namespace MyGame.GameServer
{
    public class PlayerControllerUpdate : GameMessage
    {
        private int playerID;
        public int PlayerID
        {
            get
            {
                return playerID;
            }
        }

        public PlayerControllerUpdate(GameTime currentGameTime, LocalPlayerController controlState)
            : base(currentGameTime)
        {
            this.playerID = controlState.PlayerID;
            this.Append(playerID);

            this.Append(controlState.Aimpoint);
            this.Append(controlState.AngleControl);
            this.Append(controlState.TargetAngle);
            this.Append(controlState.MovementControl);
            this.Append(controlState.Fire);
        }

        public PlayerControllerUpdate(UdpClient udpClient)
            : base(udpClient)
        {
            this.ResetReader();
            this.playerID = this.ReadInt();
        }

        internal void Apply(ControlState controlState)
        {
            this.ResetReader();
            this.playerID = this.ReadInt();
            controlState.Aimpoint = this.ReadVector2();
            controlState.AngleControl = this.ReadFloat();
            controlState.TargetAngle = this.ReadFloat();
            controlState.MovementControl = this.ReadFloat();
            controlState.Fire = this.ReadBoolean();
        }
    }
}
