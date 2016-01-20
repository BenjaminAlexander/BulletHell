using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.Networking;
using System.Net.Sockets;
using System.Threading;
using System.Net;

namespace MyGame.GameServer
{
    public class LobbyClient : Client
    {
        private Lobby lobby;

        public LobbyClient(Lobby lobby, int port, int id) : base(port, id)
        {
            this.lobby = lobby;

            Thread clientUDPThread = new Thread(new ThreadStart(InboundUDPClientReader));
            clientUDPThread.Start();

            Thread clientTCPThread = new Thread(new ThreadStart(InboundTCPClientReader));
            clientTCPThread.Start();
        }

        private void InboundUDPClientReader()
        {
            try
            {
                while (this.IsConnected())
                {
                    GameMessage m = this.ReadUDPMessage();
                    lobby.EnqueueInboundMessage(m);
                }
            }
            catch (Exception) { }
            // The thread is ending, this client is done listening.
        }

        private void InboundTCPClientReader()
        {
            try
            {
                while (this.IsConnected())
                {
                    GameMessage m = this.ReadTCPMessage();
                    lobby.EnqueueInboundMessage(m);
                }
            }
            catch (Exception) { }
            // The thread is ending, this client is done listening.
        }
    }
}
