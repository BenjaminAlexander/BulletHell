using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MyGame.GameStateObjects.QuadTreeUtils
{
    public abstract class Node
    {
        private static int maxLeafArea = 100;
        private static int treeDepth = 10;
        private Node parent;

        public Node(Node parent)
        {
            this.parent = parent;
        }

        protected Node Parent
        {
            get { return parent; }
        }

        public static Node ConstructRoot(Rectangle mapSpace)
        {
            return ConstructBranch(null, mapSpace, treeDepth);
        }

        public static Node ConstructBranch(Node parent, Rectangle mapSpace, int height)
        {
            if (height <= 1)
            {
                return new Leaf(parent, mapSpace);
            }
            else
            {
                return new InternalNode(parent, mapSpace, height);
            }
        }

        public abstract Leaf Add(CompositePhysicalObject unit);

        public abstract bool Remove(CompositePhysicalObject unit);

        public abstract bool Contains(Vector2 point);

        public static bool Contains(Rectangle rectangle, Vector2 point)
        {
            return rectangle.X <= point.X &&
                point.X < rectangle.X + rectangle.Width &&
                rectangle.Y <= point.Y &&
                point.Y < rectangle.Y + rectangle.Height;
        }

        public abstract List<CompositePhysicalObject> GetObjectsInCircle(Vector2 center, float radius);

        public abstract List<CompositePhysicalObject> CompleteList();

        public abstract CompositePhysicalObject GetClosestObject(Vector2 position);

        public abstract Rectangle GetRectangle();

        public abstract CompositePhysicalObject GetClosestObjectWithinDistance(Vector2 position, float distance);

        public abstract void Move(CompositePhysicalObject obj);
    }
}
