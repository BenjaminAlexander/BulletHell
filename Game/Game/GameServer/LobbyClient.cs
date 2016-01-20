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

namespace MyGame.GameServer
{
    public class LobbyClient : Client
    {
        //TODO: this class is unthread safe
        private Lobby lobby;
        private NetworkPlayerController controller;

        public NetworkPlayerController Controller
        {
            get
            {
                return controller;
            }
        }

        public LobbyClient(Lobby lobby, int port, int id) : base(port, id)
        {
            this.lobby = lobby;
            this.controller = new NetworkPlayerController(id);
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
                    if (m is ServerUpdate)
                    {
                        lobby.EnqueueInboundMessage((ServerUpdate)m);
                        if (m is PlayerControllerUpdate)
                        {
                            controller.ApplyUpdate((PlayerControllerUpdate)m);
                        }
                    }
                    else
                    {
                        throw new Exception("the client is sending messages it shouldn't");
                    }
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
                    GameMessage m = this.ReadUDPMessage();
                    if (m is ServerUpdate)
                    {
                        lobby.EnqueueInboundMessage((ServerUpdate)m);
                        if (m is PlayerControllerUpdate)
                        {
                            controller.ApplyUpdate((PlayerControllerUpdate)m);
                        }
                    }
                    else
                    {
                        throw new Exception("the client is sending messages it shouldn't");
                    }
                }
            }
            catch (Exception) { }
            // The thread is ending, this client is done listening.
        }
    }
}
