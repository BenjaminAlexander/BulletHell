using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.Networking;
using MyGame.GameStateObjects.QuadTreeUtils;
using MyGame.Utils;
using MyGame.GameStateObjects.PhysicalObjects;
using MyGame.DrawingUtils;
using MyGame.GameServer;

namespace MyGame.GameStateObjects.DataStuctures
{
    public class GameObjectCollection
    {
        private int nextId = 1;
        private GameObjectListManager listManager = new GameObjectListManager();
        private QuadTree quadTree;
        private Dictionary<int, GameObject> dictionary = new Dictionary<int, GameObject>();
        private Utils.RectangleF worldRectangle;
        
        public int NextID
        {
            get { return nextId++; }
        }

        public QuadTree Tree
        {
            get 
            {
                if (GameObjectField.IsModeSimulation())
                {
                    return quadTree;
                }
                else
                {
                    throw new Exception("Can only access quadtree in simulation mode");
                }
            }
        }

        public RectangleF GetWorldRectangle()
        {
            return worldRectangle;
        }

        public GameObjectCollection(Vector2 world)
        {
            worldRectangle = new Utils.RectangleF(new Vector2(0), world);
            quadTree = new QuadTree(world);
        }

        public Boolean Contains(GameObject obj)
        {
            return dictionary.ContainsKey(obj.ID);
        }

        public Boolean Contains(int id)
        {
            return dictionary.ContainsKey(id);
        }

        public void Add(GameObject obj)
        {
            if (!this.Contains(obj))
            {
                if (obj is CompositePhysicalObject)
                {
                    if (quadTree.Add((CompositePhysicalObject)obj))
                    {
                        dictionary.Add(obj.ID, obj);
                        listManager.Add(obj);
                    }
                }
                else
                {
                    dictionary.Add(obj.ID, obj);
                    listManager.Add(obj);
                }
            }
        }

        private void Remove(GameObject obj)
        {
            listManager.Remove(obj);
            dictionary.Remove(obj.ID);
            if (obj is CompositePhysicalObject)
            {
                quadTree.Remove((CompositePhysicalObject)obj);
            }
        }

        public GameObject Get(int id)
        {
            if (id == 0)
            {
                return null;
            }
            return dictionary[id];
        }

        public GameObjectListManager GetMasterList()
        {
            return listManager;
        }

        public void ServerUpdate(Lobby lobby, GameTime gameTime)
        {
            float secondsElapsed = gameTime.ElapsedGameTime.Milliseconds / 1000.0f;
            foreach (GameObject obj in this.listManager.GetList<GameObject>())
            {
                obj.ServerUpdate(secondsElapsed);
            }

            foreach (GameObject obj in this.listManager.GetList<GameObject>())
            {
                obj.SendUpdateMessage(lobby, gameTime);
                if (obj.IsDestroyed)
                {
                    this.Remove(obj);
                }
            }
        }

        public void ClientUpdate(GameTime gameTime)
        {
            float secondsElapsed = gameTime.ElapsedGameTime.Milliseconds / 1000.0f;
            foreach (GameObject obj in this.listManager.GetList<GameObject>())
            {
                obj.ClientUpdate(secondsElapsed);
            }

            foreach (GameObject obj in this.listManager.GetList<GameObject>())
            {
                if (obj.IsDestroyed)
                {
                    this.Remove(obj);
                }
            }
        }

        public void Draw(GameTime gameTime, MyGraphicsClass graphics)
        {
            graphics.BeginWorld();
            foreach (GameObject obj in listManager.GetList<GameObject>())
            {
                obj.Draw(gameTime, graphics);
            }
            graphics.EndWorld();
        }
    }
}
