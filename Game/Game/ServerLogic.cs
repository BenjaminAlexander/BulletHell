using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.GameStateObjects;
using MyGame.GameStateObjects.Ships;
using MyGame.IO;
namespace MyGame
{
    class ServerLogic : GameStateObjects.IUpdateable
    {
        private Random random = new Random(5);
        private Vector2 worldSize;
        public ServerLogic(Vector2 worldSize, InputManager inputManager)
        {
            this.worldSize = worldSize;
            MyGame.GameStateObjects.Ships.PlayerShip ship = MyGame.GameStateObjects.Ships.PlayerShip.Factory(worldSize / 2, inputManager);
            GameObject.Collection.Add(ship);
        }

        public void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (GameObject.Collection.GetMasterList().GetList<NPCShip>().Count < 100)
            {
                NPCShip npcShip = NPCShip.Factory(RandomPosition(), random);
                GameObject.Collection.Add(npcShip);
            }
        }

        private Vector2 RandomPosition()
        {
            return new Vector2((float)(random.NextDouble() * worldSize.X), (float)(random.NextDouble() * worldSize.Y));
        }
    }
}
