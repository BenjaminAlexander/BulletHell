using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MyGame.GameStateObjects.QuadTreeUtils
{
    public class QuadTree
    {
        private Vector2 mapSize;
        private Node root;

        public QuadTree(Vector2 mapSize)
        {
            this.mapSize = mapSize;
            //RectangleF mapRectangle = new RectangleF(new Vector2(0), mapSize);
            Rectangle mapRectangle = new Rectangle(0, 0, (int)Math.Ceiling(mapSize.X), (int)Math.Ceiling(mapSize.Y));
            root = Node.ConstructRoot(mapRectangle);
        }

        public Leaf Add(CompositePhysicalObject unit)
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
            if (root.Remove(unit))
            {
                unit.SetLeaf(null);
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
            //return null;
            return root.GetClosestObject(position);
        }

        public CompositePhysicalObject GetClosestObjectWithinDistance(Vector2 position, float radius)
        {
            //return null;
            return root.GetClosestObjectWithinDistance(position, radius);
        }

    }
}
