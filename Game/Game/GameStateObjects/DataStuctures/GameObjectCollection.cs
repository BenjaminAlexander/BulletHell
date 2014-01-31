using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.GameStateObjects.QuadTreeUtils;
using MyGame.Networking;

namespace MyGame.GameStateObjects.DataStuctures
{
    public class GameObjectCollection
    {
        GameObjectListManager listManager = new GameObjectListManager();
        GameObjectListManager updateList = new GameObjectListManager();
        QuadTree quadTree;
        Dictionary<int, GameObject> dictionary = new Dictionary<int, GameObject>();

        public GameObjectCollection(Vector2 worldSize)
        {
            quadTree = new QuadTree(worldSize);
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
            if (Game1.IsServer && !this.Contains(obj))
            {
                dictionary.Add(obj.ID, obj);
                listManager.Add(obj);
                Game1.outgoingQue.Enqueue(obj.GetUpdateMessage());
            }
        }

        public void ForceAdd(GameObject obj)
        {
            dictionary.Add(obj.ID, obj);
            listManager.Add(obj);
            //Game1.outgoingQue.Enqueue(obj.GetUpdateMessage());
        }

        public void AddToUpdateList(GameObject obj)
        {
            if (!this.Contains(obj))
            {
                throw new Exception("object must already be contained");
            }
            updateList.Add(obj);
            if (obj is CompositePhysicalObject)
            {
                AddCompositPhysicalObject((CompositePhysicalObject)obj);
            }
            Game1.outgoingQue.Enqueue(new AddToUpdateList(obj));
        }

        public void AddCompositPhysicalObject(CompositePhysicalObject obj)
        {
            if (this.Contains(obj))
            {
                quadTree.Add(obj);
            }
        }

        public QuadTree Tree
        {
            get { return quadTree; }
        }

        public GameObjectListManager UpdateList
        {
            get { return updateList; }
        }

        private void Remove(GameObject obj)
        {
            if (obj is CompositePhysicalObject && updateList.GetList<GameObject>().Contains(obj))
            {
                quadTree.Remove((CompositePhysicalObject)obj);
            }
            listManager.Remove(obj);
            updateList.Remove(obj);
            dictionary.Remove(obj.ID);
            
        }

        public void ApplyMessages(TCPMessage m)
        {
            if (!Game1.IsServer && m is GameObjectCollectionUpdate)
            {
                GameObjectCollectionUpdate updateMessage = (GameObjectCollectionUpdate)m;
                updateMessage.Apply(this);
                
            }
        }

        public void CleanUp()
        {
            List<GameObject> objList = new List<GameObject>(listManager.GetList<GameObject>());

            foreach (GameObject obj in updateList.GetList<GameObject>())
            {
                if (Game1.IsServer && obj is Ships.Ship && obj.IsDestroyed)
                {
                    int i;
                }
                obj.SendUpdateMessage();
            }

            foreach (GameObject obj in objList)
            {

                

                //Game1.outgoingQue.Enqueue(obj.GetUpdateMessage());
                if (obj.IsDestroyed)
                {
                    if (!Game1.IsServer)
                    {
                        int i;
                    }
                    this.Remove(obj);
                    
                }
            }
        }

        public List<GameObject> GetUpdateList()
        {
            return new List<GameObject>(updateList.GetList<GameObject>());
        }

        public GameObject Get(int id)
        {
            if (id == 0)
            {
                return null;
            }
            return dictionary[id];
        }
    }
}
