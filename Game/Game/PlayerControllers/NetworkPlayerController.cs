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
        private Game1 game;

        public Ship Focus
        {
            set { game.ControllerFocus.SetFocus(id, value, this.game.GameObjectCollection); }
            get { return game.ControllerFocus.GetFocus(id); }
        }

        public NetworkPlayerController(int id, ServerGame game)
        {
            this.id = id;
            this.game = game;
        }

        public void Update(float secondsElapsed)
        {

        }
    }
}
