using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.GameStateObjects.DataStuctures;
using MyGame.GameClient;
using System.Net.Sockets;
using MyGame.Networking;
using MyGame.Engine.Networking;
using MyGame.Engine.Reflection;
using MyGame.Engine.GameState.Instants;

namespace MyGame.GameStateObjects
{
    public class GameObjectUpdate : UdpMessage
    {
        internal GameObjectUpdate(GameTime currentGameTime, GameObject obj, Engine.GameState.GameObjectCollection collection, Instant instant)
            : base(currentGameTime)
        {
            this.Append(collection.Serialize(obj, instant));
        }

        public GameObjectUpdate(UdpTcpPair pair)
            : base(pair)
        {

        }

        public void Apply(ClientGame game, GameTime gameTime)
        {
            this.ResetReader();
            byte[] serialization = this.ReadTheRest();
            GameObjectCollection collection = game.GameObjectCollection;
            collection.Deserialize(serialization);
            this.AssertMessageEnd();
        }
    }
}
