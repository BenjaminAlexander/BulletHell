using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGame.PlayerControllers;
using Microsoft.Xna.Framework;
using MyGame.Networking;
using MyGame.Engine.Networking;
using System.Net.Sockets;

namespace MyGame.GameServer
{
    public class ControlStateUpdate : UdpMessage
    {
        public ControlStateUpdate(GameTime currentGameTime, ControlState controlState)
            : base(currentGameTime)
        {
            this.Append(controlState.Aimpoint);
            this.Append(controlState.AngleControl);
            this.Append(controlState.TargetAngle);
            this.Append(controlState.MovementControl);
            this.Append(controlState.Fire);
        }

        public ControlStateUpdate(UdpTcpPair pair)
            : base(pair)
        {
        }

        internal void Apply(ControlState controlState)
        {
            this.ResetReader();
            controlState.Aimpoint = this.ReadVector2();
            controlState.AngleControl = this.ReadFloat();
            controlState.TargetAngle = this.ReadFloat();
            controlState.MovementControl = this.ReadFloat();
            controlState.Fire = this.ReadBoolean();
        }
    }
}
