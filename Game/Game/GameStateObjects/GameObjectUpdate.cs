using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.GameStateObjects.DataStuctures;
using MyGame.GameClient;
using System.Net.Sockets;

namespace MyGame.GameStateObjects
{
    public class GameObjectUpdate : ClientUpdate
    {
        public GameObjectUpdate(GameTime currentGameTime, GameObject obj)
            : base(currentGameTime)
        {
            int typeID = GameObjectTypes.GetTypeID(obj.GetType());
            this.Append(typeID);
            this.Append(obj.ID);
            this.Append(obj.IsDestroyed);

            foreach (GameObjectField field in obj.Fields)
            {
                field.ConstructMessage(this);
            }
        }

        public GameObjectUpdate(UdpClient udpClient)
            : base(udpClient)
        {

        }

        public override void Apply(ClientGame game, GameTime gameTime)
        {
            this.ResetReader();
            Type typeFromMessage = GameObjectTypes.GetType(this.ReadInt());
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
                obj = GameObjectTypes.Construct(typeFromMessage, game, idFromMessage);
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
