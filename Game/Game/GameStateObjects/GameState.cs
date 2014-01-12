using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.GameStateObjects.Ships;
using MyGame.Utils;
using MyGame.DrawingUtils;
namespace MyGame.GameStateObjects
{
    public class GameState
    {

        private List<Drawable> stars = new List<Drawable>();

        private List<GameObject> addList = new List<GameObject>();
        private List<GameObject> removeList = new List<GameObject>();
        private List<GameObject> gameObjects = new List<GameObject>();

        private Random random = new Random();

        private Utils.RectangleF worldRectangle;

        public List<PhysicalObject> GetPhysicalObjects()
        {
            List<PhysicalObject> returnList = new List<PhysicalObject>();
            foreach (GameObject obj in gameObjects)
            {
                if (obj.IsPhysicalObject)
                {
                    returnList.Add((PhysicalObject)obj);
                }
            }
            return returnList;
        }

        public List<FlyingGameObject> GetFlyingGameObjects()
        {
            List<FlyingGameObject> returnList = new List<FlyingGameObject>();
            foreach (PhysicalObject obj in GetPhysicalObjects())
            {
                if (obj.IsFlyingGameObject)
                {
                    returnList.Add((FlyingGameObject)obj);
                }
            }
            return returnList;
        }

        public List<Ship> GetShips()
        {
            List<Ship> returnList = new List<Ship>();
            foreach (FlyingGameObject obj in GetFlyingGameObjects())
            {
                if (obj.IsShip)
                {
                    returnList.Add((Ship)obj);
                }
            }
            return returnList;
        }

        public List<PlayerShip> GetPlayerShips()
        {
            List<PlayerShip> returnList = new List<PlayerShip>();
            foreach (Ship obj in GetShips())
            {
                if (obj.IsPlayerShip)
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
                if (obj.IsNPCShip)
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

        public GameState(MyGame.IO.InputManager inputManager, Vector2 worldSize)
        {
            worldRectangle = new Utils.RectangleF(new Vector2(0), worldSize);

            Random random = new Random();
            for (int i = 0; i < (int)(worldSize.X * worldSize.Y / 50000); i++)
            {
                stars.Add(new Drawable(Textures.Star, RandomPosition(), Color.SteelBlue, (float)(random.NextDouble() * Math.PI * 2), new Vector2(25), .1f));
            }

            PlayerShip ship = new MyGame.GameStateObjects.Ships.PlayerShip(new Vector2(50), inputManager);
            this.AddShip(ship);
            NPCShip npcShip = new MyGame.GameStateObjects.Ships.NPCShip(new Vector2(260));
            this.AddShip(npcShip);
            //this.AddGameObject(new Bullet(new Vector2(0), 2f));
        }

        public void AddGameObject(GameObject obj)
        {
            if (obj != null && !gameObjects.Contains(obj))
            {
                addList.Add(obj);
            }
        }

        public void RemoveGameObject(GameObject obj)
        {
            if (obj != null && gameObjects.Contains(obj))
            {
                removeList.Add(obj);
            }
        }

        public void AddShip(Ship ship)
        {
            AddGameObject(ship);
        }

        
        public void RemoveShip(Ship ship)
        {
            RemoveGameObject(ship);
        }

        public void Update(GameTime gameTime)
        {
            foreach(GameObject obj in gameObjects)
            {
                obj.Update(gameTime);
            }

            if (this.GetNPCShips().Count < 10)
            {
                AddGameObject(new NPCShip(RandomPosition()));
            }

            foreach(GameObject obj in addList)
            {
                if(obj != null)
                {
                    obj.GameState = this;
                    gameObjects.Add(obj);
                }
            }
            addList.Clear();

            foreach (GameObject obj in removeList)
            {
                if (obj != null && gameObjects.Contains(obj))
                {
                    gameObjects.Remove(obj);
                    obj.GameState = null;
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
        }

        public PlayerShip GetPlayerShip()
        {
            foreach (GameObject obj in gameObjects)
            {
                if (obj.IsPhysicalObject && ((PhysicalObject)obj).IsFlyingGameObject && ((FlyingGameObject)obj).IsShip && ((Ship)obj).IsPlayerShip)
                {
                    return (PlayerShip)obj;
                }
            }
            return null; // TODO: Exception?
        }
    }
}
