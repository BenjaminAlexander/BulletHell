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
        static Type[] constuctorParamsTypes = { typeof(Game1) };

        public GameObjectUpdate(GameTime currentGameTime, GameObject obj)
            : base(currentGameTime)
        {
            int typeID = Engine.GameState.GameObject.GetTypeID(obj);
            this.Append(typeID);
            this.Append((int)obj.ID);

            this.Append(obj.Serialize(new Instant(0)));
        }

        public GameObjectUpdate(UdpTcpPair pair)
            : base(pair)
        {

        }

        public void Apply(ClientGame game, GameTime gameTime)
        {
            this.ResetReader();
            int typeFromMessage = this.ReadInt();
            int idFromMessage = this.ReadInt();

            GameObjectCollection collection = game.GameObjectCollection;

            GameObject obj;
            if (collection.Contains(idFromMessage))
            {
                obj = collection.Get(idFromMessage);
            }
            else
            {
                object[] constuctorParams = new object[1];
                constuctorParams[0] = game;

                obj = (GameObject)collection.NewGameObject(idFromMessage, new Instant(0), typeFromMessage);
            }

            if (!(obj.TypeID == typeFromMessage && obj.ID == idFromMessage))
            {
                throw new Exception("this message does not belong to this object");
            }

            byte[] serialization = this.ReadTheRest();
            int offset = 0;
            obj.Deserialize(new Instant(0), serialization, ref offset);

            this.AssertMessageEnd();
        }
    }
}
