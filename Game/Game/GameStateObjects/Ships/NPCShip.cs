using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.Utils;
using MyGame.DrawingUtils;
using MyGame.AI;
using MyGame.Networking;

namespace MyGame.GameStateObjects.Ships
{
    public class NPCShip : Ship
    {

        public static NPCShip Factory(Vector2 position, Random random)
        {
            NPCShip ship = new NPCShip(position, random);
            GameObject.Collection.Add(ship);
            return ship;
        }

        public NPCShip(int id)
            : base(id)
        {
            this.Collidable = new Collidable(Textures.Enemy, new Vector2(0), Color.White, 0, new Vector2(30, 25), .9f);
        }


        //TODO: get random out of here
        public NPCShip(Vector2 position, Random random)
            : base (position, new Collidable(Textures.Enemy, position, Color.White, 0, new Vector2(30, 25), .9f), 600 + random.Next(0, 100))
        {

            this.DoDamage(30);
        }

        protected override void UpdateSubclass(GameTime gameTime)
        {
            if (this.flyingStrategy == null)
            {
                PlayerShip player = null;
                List<PlayerShip> playerShips = GameObject.Collection.GetMasterList().GetList<PlayerShip>();
                if(playerShips.Count > 0)
                {
                    player = playerShips[0];
                }
                
                if (player != null)
                {
                    this.flyingStrategy = new NPCBasicAttackStrategy(this, player);
                }
            }
            else
            {
                if (Game1.IsServer)
                {
                    this.flyingStrategy.ExecuteStrategy(gameTime);
                }
            }
            base.UpdateSubclass(gameTime);
        }

        public override void Draw(GameTime gameTime, MyGraphicsClass graphics)
        {
            base.Draw(gameTime, graphics);
        }

        
        FlyingStrategy flyingStrategy;

        //using MyGame.Networking;
        public override void UpdateMemberFields(GameObjectUpdate message)
        {
            base.UpdateMemberFields(message);

        }

        public override GameObjectUpdate MemberFieldUpdateMessage(GameObjectUpdate message)
        {
            message = base.MemberFieldUpdateMessage(message);
            return message;
        }
    }
}
