using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MyGame.GameStateObjects.QuadTreeUtils
{
    public abstract class Node
    {
        protected static int max_count = 10;
        
        //private static int treeDepth = 10;
        private InternalNode parent;
        public int id;
        public static int nextI = 0;

        public Node(InternalNode parent)
        {
            id = nextI++;
            this.parent = parent;
        }

        public abstract int ObjectCount();

        protected InternalNode Parent
        {
            get { return parent; }
        }

        public void DisconnectFromParent()
        {
            parent = null;
        }

        public abstract bool Add(CompositePhysicalObject unit);

        public abstract Leaf Remove(CompositePhysicalObject unit);

        public abstract bool Contains(Vector2 point);

        public static bool Contains(Rectangle rectangle, Vector2 point)
        {
            return rectangle.X <= point.X &&
                point.X < rectangle.X + rectangle.Width &&
                rectangle.Y <= point.Y &&
                point.Y < rectangle.Y + rectangle.Height;
        }

        public abstract List<CompositePhysicalObject> GetObjectsInCircle(Vector2 center, float radius, List<CompositePhysicalObject> list);

        public abstract List<CompositePhysicalObject> CompleteList(ref List<CompositePhysicalObject> list);

        public List<CompositePhysicalObject> CompleteList()
        {
            List<CompositePhysicalObject> list = new List<CompositePhysicalObject>();
            return this.CompleteList(ref list);
        }

        public List<CompositePhysicalObject> GetObjectsInCircle(Vector2 center, float radius)
        {
            List<CompositePhysicalObject> list = new List<CompositePhysicalObject>();
            return this.GetObjectsInCircle(center, radius, list);
        }

        public abstract CompositePhysicalObject GetClosestObject(Vector2 position);

        public abstract Rectangle GetRectangle();

        public abstract CompositePhysicalObject GetClosestObjectWithinDistance(Vector2 position, float distance);

        public abstract void Move(CompositePhysicalObject obj);
    }
}
