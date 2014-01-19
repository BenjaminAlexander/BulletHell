using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.GameStateObjects.Ships;
using MyGame.Utils;
using MyGame.DrawingUtils;
using MyGame.GameStateObjects.QuadTreeUtils;
namespace MyGame.GameStateObjects
{
    public class GameState
    {
        private QuadTree spacialData;

        private List<Drawable> stars = new List<Drawable>();

        private List<GameObject> addList = new List<GameObject>();
        private List<GameObject> removeList = new List<GameObject>();
        private List<GameObject> gameObjects = new List<GameObject>();
        private List<Ship> ships = new List<Ship>();
        Camera camera;
        private Random random = new Random();

        private Utils.RectangleF worldRectangle;

        public Camera Camera
        {
            get { return camera; }
        }

        public QuadTree Tree
        {
            get { return spacialData; }
        }

        public List<Ship> GetShips()
        {
            return ships;
        }

        public List<PlayerShip> GetPlayerShips()
        {
            List<PlayerShip> returnList = new List<PlayerShip>();
            foreach (Ship obj in GetShips())
            {
                if (obj is PlayerShip)
                {
                    returnList.Add((PlayerShip)obj);
                }
            }
            return returnList;
        }

        public List<NPCShip> GetNPCShips()
        {
            List<NPCShip> returnList = new List<NPCShip>();
            foreach (Ship obj in GetShips())
            {
                if (obj is NPCShip)
                {
                    returnList.Add((NPCShip)obj);
                }
            }
            return returnList;
        }

        public RectangleF GetWorldRectangle()
        {
            return worldRectangle;
        }

        private Vector2 RandomPosition()
        {
            
            return new Vector2((float)(random.NextDouble() * worldRectangle.Size.X),(float)(random.NextDouble() * worldRectangle.Size.Y));
        }

        public GameState(MyGame.IO.InputManager inputManager, Vector2 worldSize, Camera camera)
        {

            GameObject.LocalGameState = this;

            this.camera = camera;
            worldRectangle = new Utils.RectangleF(new Vector2(0), worldSize);
            spacialData = new QuadTree(worldSize);
            Random random = new Random();
            for (int i = 0; i < (int)(worldSize.X * worldSize.Y / 50000); i++)
            {
                stars.Add(new Drawable(Textures.Star, RandomPosition(), Color.SteelBlue, (float)(random.NextDouble() * Math.PI * 2), new Vector2(25), .1f));
            }

            PlayerShip ship = new MyGame.GameStateObjects.Ships.PlayerShip(new Vector2(50), inputManager);
            this.AddGameObject(ship);
            NPCShip npcShip = new MyGame.GameStateObjects.Ships.NPCShip(new Vector2(260));
            this.AddGameObject(npcShip);
            //this.AddGameObject(new Bullet(new Vector2(0), 2f));
        }

        public Boolean AddGameObject(GameObject obj)
        {
            if (obj != null && !addList.Contains(obj) && !gameObjects.Contains(obj))
            {
                addList.Add(obj);
                return true;
            }
            return false;
        }

        public void RemoveGameObject(GameObject obj)
        {
            if (obj != null && !removeList.Contains(obj) && gameObjects.Contains(obj))
            {
                removeList.Add(obj);
            }
        }

        public List<Bullet> GetBullets(Vector2 position, float radius)
        {
            List<Bullet> returnList = new List<Bullet>();

            foreach (CompositePhysicalObject obj in spacialData.GetObjectsInCircle(position, radius))
            {
                if (obj is Bullet)
                {
                    returnList.Add((Bullet)obj);
                }
            }
            return returnList;
        }
     
        public void Update(GameTime gameTime)
        {
            foreach(GameObject obj in gameObjects)
            {
                obj.Update(gameTime);
            }

            if (this.GetNPCShips().Count < 50)
            {
                AddGameObject(new NPCShip(RandomPosition()));
            }

            foreach(GameObject obj in addList)
            {
                if(obj != null)
                {
                    gameObjects.Add(obj);
                }

                if (obj is Ship)
                {
                    ships.Add((Ship)obj);
                }
            }
            addList.Clear();

            foreach (GameObject obj in removeList)
            {
                if (obj != null && gameObjects.Contains(obj))
                {
                    gameObjects.Remove(obj);
                }

                if (obj is CompositePhysicalObject)
                {
                    spacialData.Remove((CompositePhysicalObject)obj);
                }

                if (obj is Ship)
                {
                    ships.Remove((Ship)obj);
                }
            }
            removeList.Clear();

        }

        public void Draw(GameTime gameTime, MyGraphicsClass graphics)
        {
            //graphics.DrawRectangle(worldRectangle.Position, worldRectangle.Size, new Vector2(0), 0, Color.Red, 1);
            foreach (GameObject obj in gameObjects)
            {
                obj.Draw(gameTime, graphics);
            }
            foreach (Drawable obj in stars)
            {
                obj.Draw(graphics);
            }

            foreach (GameObject obj in spacialData.CompleteList())
            {
                obj.Draw(gameTime, graphics);
            }
        }

        public PlayerShip GetPlayerShip()
        {
            List<PlayerShip> pShips = GetPlayerShips();
            if (pShips.Count != 0)
            {
                return pShips[0];
            }
            return null; // TODO: Exception?
        }

        public void AddBullet(Bullet b)
        {
            AddGameObject(b);
        }
    }
}
