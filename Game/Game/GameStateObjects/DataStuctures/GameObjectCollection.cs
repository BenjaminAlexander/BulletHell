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
            return dictionary.Contains(new KeyValuePair<int, GameObject>(obj.ID, obj));
        }

        public void Add(GameObject obj)
        {
            dictionary.Add(obj.ID, obj);
            listManager.Add(obj);
            Game1.outgoingQue.Enqueue(new GameObjectUpdate(obj));
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
            listManager.Remove(obj);
            updateList.Remove(obj);
            dictionary.Remove(obj.ID);
            if (obj is CompositePhysicalObject)
            {
                quadTree.Remove((CompositePhysicalObject)obj);
            }
        }

        public void CleanUp()
        {
            List<GameObject> objList = new List<GameObject>(listManager.GetList<GameObject>());
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
    }
}
