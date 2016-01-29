using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.GameStateObjects.PhysicalObjects;

namespace MyGame.GameStateObjects.QuadTreeUtils
{
    public class QuadTree
    {
        private Vector2 mapSize;
        private Node root;

        LeafDictionary leafDictionary;

        public QuadTree(Vector2 mapSize)
        {
            this.mapSize = mapSize;
            leafDictionary = new LeafDictionary(this);
            Rectangle mapRectangle = new Rectangle(0, 0, (int)Math.Ceiling(mapSize.X), (int)Math.Ceiling(mapSize.Y));
            root = new InternalNode(true, null, mapRectangle, leafDictionary);
        }

        public bool Add(CompositePhysicalObject unit)
        {
            if(root.Add(unit))
            {
                return true;
            }
            else
            {
                //throw new Exception("add failed");
                return false;
            }
        }

        public List<CompositePhysicalObject> GetObjectsInCircle(Vector2 center, float radius)
        {
            return root.GetObjectsInCircle(center, radius);
        }

        public bool Remove(CompositePhysicalObject unit)
        {
            Leaf removeFrom = root.Remove(unit);
            if (removeFrom != null)
            {
                removeFrom.Collapse();
                return true;
            }
            else
            {
                //throw new Exception("No object to remove");
                return false;
            }
        }

        public List<CompositePhysicalObject> CompleteList()
        {
            return root.CompleteList();
        }

        public void Move(CompositePhysicalObject obj)
        {
            leafDictionary.GetLeaf(obj).Move(obj);
        }

        internal Node Root
        {
            get { return root; }
        }

    }
}
