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

namespace MyGame.GameStateObjects
{
    public class GameObjectUpdate : UdpMessage
    {
        static Type[] constuctorParamsTypes = { typeof(Game1) };
        static DerivedTypeConstructorFactory<GameObject> factory = new DerivedTypeConstructorFactory<GameObject>(constuctorParamsTypes);

        public GameObjectUpdate(GameTime currentGameTime, GameObject obj)
            : base(currentGameTime)
        {
            int typeID = factory.GetTypeID(obj);
            this.Append(typeID);
            this.Append(obj.ID);
            this.Append(obj.IsDestroyed);

            foreach (GameObjectField field in obj.Fields)
            {
                field.ConstructMessage(this);
            }
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
            bool isDesroyedFromMessage = this.ReadBoolean();

            GameObjectCollection collection = game.GameObjectCollection;

            GameObject obj;
            if (collection.Contains(idFromMessage))
            {
                obj = collection.Get(idFromMessage);
                if (obj.LastMessageTimeStamp > this.TimeStamp)
                {
                    return;
                }
            }
            else
            {
                if (isDesroyedFromMessage)
                {
                    return;
                }

                object[] constuctorParams = new object[1];
                constuctorParams[0] = game;
                obj = factory.Construct(typeFromMessage, constuctorParams);
                obj.ID = idFromMessage;
                collection.Add(obj);
            }

            if (!(obj.GetType() == typeFromMessage && obj.ID == idFromMessage))
            {
                throw new Exception("this message does not belong to this object");
            }

            obj.IsDestroyed = isDesroyedFromMessage;
            foreach (GameObjectField field in obj.Fields)
            {
                field.ApplyMessage(this);
            }
            this.AssertMessageEnd();

            obj.LatencyAdjustment(gameTime, this.TimeStamp);
        }
    }
}
