using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.GameStateObjects.Ships;
using MyGame.Utils;
using MyGame.DrawingUtils;
using MyGame.GameStateObjects.QuadTreeUtils;
using MyGame.Geometry;
using MyGame.GameStateObjects.DataStuctures;

namespace MyGame.GameStateObjects
{
    public class GameState
    {
        private QuadTree spacialData;

        private List<Drawable> stars = new List<Drawable>();

        //replaces List<GameObject> gameObjects = new List<GameObject>();
        private GameObjectListManager listManager = new GameObjectListManager();

        private List<GameObject> addList = new List<GameObject>();
        private List<GameObject> removeList = new List<GameObject>();
        private List<GameObject> outOfWorldAddList = new List<GameObject>();
        private List<GameObject> outOfWorldRemoveList = new List<GameObject>();
        private List<GameObject> outOfWorldGameObjects = new List<GameObject>();

        Camera camera;
        public static Random random = new Random();

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
            return listManager.GetList<Ship>();
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

        public List<Mine> GetMines()
        {
            return listManager.GetList<Mine>();
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


            for (int i = 0; i < (int)(worldSize.X * worldSize.Y / 500000); i++)
            {
                //AddGameObject(new Mine(RandomPosition()));
            }

            PlayerShip ship = new MyGame.GameStateObjects.Ships.PlayerShip(worldSize/2, inputManager);
            this.AddGameObject(ship);
        }

        public Boolean AddGameObject(GameObject obj)
        {
            if (obj != null )//&& !addList.Contains(obj) && !gameObjects.Contains(obj))
            {
                addList.Add(obj);
                return true;
            }
            return false;
        }

        public void RemoveGameObject(GameObject obj)
        {
            if (obj != null && !removeList.Contains(obj) && listManager.GetList<GameObject>().Contains(obj))
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

        public List<CompositePhysicalObject> GetObjectsInCircle(Circle c)
        {
            return spacialData.GetObjectsInCircle(c.Center, c.Radius);
        }
     
        public void Update(GameTime gameTime)
        {
            foreach (GameObject obj in outOfWorldGameObjects)
            {
                obj.Update(gameTime);
            }

            foreach (GameObject obj in listManager.GetList<GameObject>())
            {
                obj.Update(gameTime);
            }

            if (this.GetNPCShips().Count < 20)
            {
                AddGameObject(new NPCShip(RandomPosition(), random));
            }
            
            

            foreach(GameObject obj in addList)
            {
                if(obj != null)
                {
                    listManager.Add(obj);
                }
            }
            addList.Clear();

            foreach (GameObject obj in removeList)
            {
                if (obj != null && listManager.GetList<GameObject>().Contains(obj))
                {
                    listManager.Remove(obj);
                }

                if (obj is CompositePhysicalObject)
                {
                    spacialData.Remove((CompositePhysicalObject)obj);
                }
            }
            removeList.Clear();

            foreach (GameObject obj in outOfWorldAddList)
            {
                if (obj != null)
                {
                    outOfWorldGameObjects.Add(obj);
                }
            }
            outOfWorldAddList.Clear();

            foreach (GameObject obj in outOfWorldRemoveList)
            {
                if (obj != null && outOfWorldGameObjects.Contains(obj))
                {
                    outOfWorldGameObjects.Remove(obj);
                }
            }
            outOfWorldRemoveList.Clear();
        }

        public void Draw(GameTime gameTime, MyGraphicsClass graphics)
        {
            //graphics.DrawRectangle(worldRectangle.Position, worldRectangle.Size, new Vector2(0), 0, Color.Red, 1);
            graphics.BeginWorld();
            foreach (GameObject obj in listManager.GetList<GameObject>())
            {
                obj.Draw(gameTime, graphics);
            }
            foreach (Drawable obj in stars)
            {
                obj.Draw(graphics);
            }
            graphics.EndWorld();

            graphics.Begin(Matrix.Identity);
            foreach (GameObject obj in outOfWorldGameObjects)
            {
                obj.Draw(gameTime, graphics);
            }
            graphics.End();
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

        public Boolean AddOutOfWorldGameObject(GameObject obj)
        {
            if (obj != null)//&& !addList.Contains(obj) && !gameObjects.Contains(obj))
            {
                outOfWorldAddList.Add(obj);
                return true;
            }
            return false;
        }

        public void RemoveOutOfWorldGameObject(GameObject obj)
        {
            if (obj != null && !outOfWorldRemoveList.Contains(obj) && outOfWorldGameObjects.Contains(obj))
            {
                outOfWorldRemoveList.Add(obj);
            }
        }
    }
}
