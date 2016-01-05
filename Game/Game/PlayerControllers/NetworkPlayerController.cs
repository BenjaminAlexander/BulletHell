using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.Networking;
using MyGame.GameStateObjects;
using MyGame.GameStateObjects.PhysicalObjects.MovingGameObjects.Ships;
using MyGame.GameServer;

namespace MyGame.PlayerControllers
{
    public class NetworkPlayerController : ControlState
    {
        private int id;

        public int ID
        {
            get { return id; }
        }

        public NetworkPlayerController(int id)
        {
            this.id = id;
        }
    }
}
