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

        LeafDictionary leafDictionary = new LeafDictionary();

        public QuadTree(Vector2 mapSize)
        {
            this.mapSize = mapSize;
            Rectangle mapRectangle = new Rectangle(0, 0, (int)Math.Ceiling(mapSize.X), (int)Math.Ceiling(mapSize.Y));
            root = new InternalNode(true, null, mapRectangle, leafDictionary);
        }

        public bool Add(CompositePhysicalObject unit)
        {
            return root.Add(unit);
        }

        public List<CompositePhysicalObject> GetObjectsInCircle(Vector2 center, float radius)
        {
            //return new List<Unit>();
            return root.GetObjectsInCircle(center, radius);
        }

        public bool Remove(CompositePhysicalObject unit)
        {
            Leaf removeFrom = root.Remove(unit);
            if (removeFrom != null)
            {
                leafDictionary.SetLeaf(unit, null);
                removeFrom.Collapse();
                return true;
            }
            else
            {
                throw new Exception("No object to remove");
            }
        }

        public List<CompositePhysicalObject> CompleteList()
        {
            return root.CompleteList();
        }

        public CompositePhysicalObject GetClosestObject(Vector2 position)
        {
            return root.GetClosestObject(position);
        }

        public CompositePhysicalObject GetClosestObjectWithinDistance(Vector2 position, float radius)
        {
            return root.GetClosestObjectWithinDistance(position, radius);
        }

        public void Move(CompositePhysicalObject obj)
        {
            leafDictionary.GetLeaf(obj).Move(obj);
        }

    }
}
