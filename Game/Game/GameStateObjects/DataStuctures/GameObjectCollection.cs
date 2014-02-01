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
        private GameObjectListManager listManager = new GameObjectListManager();
        private GameObjectListManager updateList = new GameObjectListManager();
        private QuadTree quadTree;
        private Dictionary<int, GameObject> dictionary = new Dictionary<int, GameObject>();


        public GameObjectCollection(Vector2 world)
        {
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
            if (Game1.IsServer)
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
        }

        public void ForceAddToUpdateList(GameObject obj)
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

        public void ApplyMessage(TCPMessage m)
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
                obj.SendUpdateMessage();
            }

            foreach (GameObject obj in objList)
            {
                if (obj.IsDestroyed)
                {
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
