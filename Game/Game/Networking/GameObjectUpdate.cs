using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.GameStateObjects;
using MyGame.GameStateObjects.DataStuctures;
using MyGame.GameClient;

namespace MyGame.Networking
{
    public class GameObjectUpdate : ClientUpdate
    {
        private Type type;
        private int id;

        public GameObjectUpdate(GameTime currentGameTime, GameObject obj)
            : base(currentGameTime)
        {
            type = obj.GetType();
            id = obj.ID;
            int typeID = GameObjectTypes.GetTypeID(obj.GetType());
            this.Append(typeID);
            this.Append(obj.ID);
        }

        public GameObjectUpdate(byte[] b, int lenght)
            : base(b, lenght)
        {
            this.ResetReader();
            type = GameObjectTypes.GetType(this.ReadInt());
            id = this.ReadInt();
        }

        public override void Apply(ClientGame game, GameTime gameTime)
        {
            GameObjectCollection collection = game.GameObjectCollection;
            if (collection.Contains(this.id))
            {
                collection.Get(this.id).UpdateMemberFields(this, gameTime);
            }
            else
            {
                GameObject obj = GameObjectTypes.Construct(type, game);
                obj.ClientInitialize(this, gameTime);
                if (!obj.IsDestroyed)
                {
                    collection.Add(obj);
                }
            }
        }
    }
}
