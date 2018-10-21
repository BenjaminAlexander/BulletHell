using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.Networking;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using MyGame.PlayerControllers;
using MyGame.GameClient;
using MyGame.Utils;
using MyGame.GameStateObjects;

namespace MyGame.GameServer
{
    public class Player : BasePlayer<ControlStateUpdate>
    {
        internal ControlState controller;

        public ControlState Controller
        {
            get
            {
                return controller;
            }
        }

        public Player()
            : base(new UdpTcpPair())
        {
            this.controller = new ControlState();
        }

        public override ControlStateUpdate GetUDPMessage(UdpTcpPair client)
        {
            return new ControlStateUpdate(client);
        }

        public void UpdateControlState()
        {
            Queue<ControlStateUpdate> messages = this.DequeueAllIncomingUDP();
            while (messages.Count != 0)
            {
                ControlStateUpdate message = messages.Dequeue();
                message.Apply(this.controller);
            }
        }
    }
}
