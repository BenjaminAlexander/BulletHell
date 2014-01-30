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
        private List<Drawable> stars = new List<Drawable>();

        private List<IDrawableUpdatable> outOfWorldGameObjects = new List<IDrawableUpdatable>();

        Camera camera;
        public static Random random = new Random(5);

        private Utils.RectangleF worldRectangle;

        public Camera Camera
        {
            get { return camera; }
        }

        public List<Ship> GetShips()
        {
            return GameObject.Collection.UpdateList.GetList<Ship>();
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
            return GameObject.Collection.UpdateList.GetList<Mine>();
        }

        public RectangleF GetWorldRectangle()
        {
            return worldRectangle;
        }

        private Vector2 RandomPosition()
        {
            
            return new Vector2((float)(random.NextDouble() * worldRectangle.Size.X),(float)(random.NextDouble() * worldRectangle.Size.Y));
        }

        public GameState(Vector2 worldSize, Camera camera)
        {

            GameObject.LocalGameState = this;
            this.camera = camera;
            worldRectangle = new Utils.RectangleF(new Vector2(0), worldSize);
            Random random = new Random();
            for (int i = 0; i < (int)(worldSize.X * worldSize.Y / 50000); i++)
            {
                stars.Add(new Drawable(Textures.Star, RandomPosition(), Color.SteelBlue, (float)(random.NextDouble() * Math.PI * 2), new Vector2(25), .1f));
            }

            
        }

        public List<Bullet> GetBullets(Vector2 position, float radius)
        {
            List<Bullet> returnList = new List<Bullet>();

            foreach (CompositePhysicalObject obj in GameObject.Collection.Tree.GetObjectsInCircle(position, radius))
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
            return GameObject.Collection.Tree.GetObjectsInCircle(c.Center, c.Radius);
        }
     
        public void Update(GameTime gameTime)
        {
            foreach (IDrawableUpdatable obj in outOfWorldGameObjects)
            {
                obj.Update(gameTime);
            }

            foreach (GameObject obj in GameObject.Collection.GetUpdateList())
            {
                obj.Update(gameTime);
            }

            if (this.GetNPCShips().Count < 20)
            {

                NPCShip npcShip = new NPCShip(GameObject.NextID);
                npcShip.Initialize(RandomPosition(), random);
                GameObject.Collection.AddToUpdateList(npcShip);
            }

            GameObject.Collection.CleanUp();
        }

        public void Draw(GameTime gameTime, MyGraphicsClass graphics)
        {
            graphics.BeginWorld();
            foreach (GameObject obj in GameObject.Collection.UpdateList.GetList<GameObject>())
            {
                obj.Draw(gameTime, graphics);
            }
            foreach (Drawable obj in stars)
            {
                obj.Draw(graphics);
            }
            graphics.EndWorld();

            graphics.Begin(Matrix.Identity);
            foreach (IDrawableUpdatable obj in outOfWorldGameObjects)
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
            GameObject.Collection.AddToUpdateList(b);
        }

        public Boolean AddOutOfWorldGameObject(IDrawableUpdatable obj)
        {
            if (obj != null)//&& !addList.Contains(obj) && !gameObjects.Contains(obj))
            {
                outOfWorldGameObjects.Add(obj);
                return true;
            }
            return false;
        }
    }
}
