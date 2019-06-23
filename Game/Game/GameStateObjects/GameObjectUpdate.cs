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
        static ConstructorTypeFactory<GameObject> factory = new ConstructorTypeFactory<GameObject>(constuctorParamsTypes);

        public GameObjectUpdate(GameTime currentGameTime, GameObject obj)
            : base(currentGameTime)
        {
            int typeID = factory.GetTypeID(obj);
            this.Append(typeID);
            this.Append(obj.ID);

            this.Append(obj.Serialize(new Instant(0)));
        }

        public GameObjectUpdate(UdpTcpPair pair)
            : base(pair)
        {

        }

        public void Apply(ClientGame game, GameTime gameTime)
        {
            this.ResetReader();
            Type typeFromMessage = factory.GetTypeFromID(this.ReadInt());
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
                obj = factory.Construct(typeFromMessage, constuctorParams);
                obj.ID = idFromMessage;
                obj.SetUp(idFromMessage, new Instant(0));
                collection.Add(obj);
            }

            if (!(obj.GetType() == typeFromMessage && obj.ID == idFromMessage))
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
