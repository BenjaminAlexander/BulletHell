using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MyGame.GameStateObjects.QuadTreeUtils
{
    class InternalNode : Node
    {
        private Rectangle mapSpace;

        private Node nw;
        private Node ne;
        private Node sw;
        private Node se;
        private int unitCount = 0;

        public InternalNode(Node parent, Rectangle mapSpace, int height) : base(parent)
        {
            this.mapSpace = mapSpace;

            int halfWidth = mapSpace.Width / 2;
            int halfHeight = mapSpace.Height / 2;

            Rectangle swRectangle = new Rectangle(mapSpace.X, mapSpace.Y, halfWidth, halfHeight);
            Rectangle seRectangle = new Rectangle(mapSpace.X + halfWidth, mapSpace.Y, mapSpace.Width - halfWidth, halfHeight);
            Rectangle nwRectangle = new Rectangle(mapSpace.X, mapSpace.Y + halfHeight, halfWidth, mapSpace.Height - halfHeight);
            Rectangle neRectangle = new Rectangle(mapSpace.X + halfWidth, mapSpace.Y + halfHeight, mapSpace.Width - halfWidth, mapSpace.Height - halfHeight);

            nw = Node.ConstructBranch(this, nwRectangle, height - 1);
            ne = Node.ConstructBranch(this, neRectangle, height - 1);
            sw = Node.ConstructBranch(this, swRectangle, height - 1);
            se = Node.ConstructBranch(this, seRectangle, height - 1);

        }

        public override Leaf Add(CompositePhysicalObject obj)
        {
            if (this.Contains(obj.Position))
            {
                unitCount++;
                Leaf returnNode = nw.Add(obj);
                if (returnNode != null)
                {
                    return returnNode;
                }

                returnNode = ne.Add(obj);
                if (returnNode != null)
                {
                    return returnNode;
                }

                returnNode = sw.Add(obj);
                if (returnNode != null)
                {
                    return returnNode;
                }

                returnNode = se.Add(obj);
                if (returnNode != null)
                {
                    return returnNode;
                }                
            }
            if (Parent == null)
            {
                throw new MoveOutOfBound();
            }

            return null;
        }

        public override bool Remove(CompositePhysicalObject obj)
        {
            if (this.Contains(obj.Position))
            {
                if (nw.Remove(obj) ||
                    ne.Remove(obj) ||
                    sw.Remove(obj) ||
                    se.Remove(obj))
                {
                    unitCount--;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public override bool Contains(Vector2 point)
        {
            return Node.Contains(mapSpace, point);
        }

        public override List<CompositePhysicalObject> GetObjectsInCircle(Vector2 center, float radius)
        {
            List<CompositePhysicalObject> returnList = new List<CompositePhysicalObject>();
            if (unitCount > 0)
            {
                Vector2 rectangleCenter = new Vector2((((float)mapSpace.X) + ((float)mapSpace.Width) / 2), (((float)mapSpace.Y) + ((float)mapSpace.Height) / 2));
                float rectangleRadius = Vector2.Distance(rectangleCenter, new Vector2(mapSpace.X, mapSpace.Y));


                if (Vector2.Distance(rectangleCenter, center) <= radius + rectangleRadius)
                {
                    foreach (CompositePhysicalObject unit in nw.GetObjectsInCircle(center, radius))
                    {
                        returnList.Add(unit);
                    }
                    foreach (CompositePhysicalObject unit in ne.GetObjectsInCircle(center, radius))
                    {
                        returnList.Add(unit);
                    }
                    foreach (CompositePhysicalObject unit in sw.GetObjectsInCircle(center, radius))
                    {
                        returnList.Add(unit);
                    }
                    foreach (CompositePhysicalObject unit in se.GetObjectsInCircle(center, radius))
                    {
                        returnList.Add(unit);
                    }
                }
            }
            return returnList;
        }

        public override List<CompositePhysicalObject> CompleteList()
        {
            List<CompositePhysicalObject> returnList = new List<CompositePhysicalObject>();
            if (unitCount > 0)
            {
                foreach (CompositePhysicalObject unit in nw.CompleteList())
                {
                    returnList.Add(unit);
                }
                foreach (CompositePhysicalObject unit in ne.CompleteList())
                {
                    returnList.Add(unit);
                }
                foreach (CompositePhysicalObject unit in sw.CompleteList())
                {
                    returnList.Add(unit);
                }
                foreach (CompositePhysicalObject unit in se.CompleteList())
                {
                    returnList.Add(unit);
                }
            }
            return returnList;
        }

        public override Rectangle GetRectangle()
        {
            return mapSpace;
        }

        public override CompositePhysicalObject GetClosestObjectWithinDistance(Vector2 center, float radius)
        {
            CompositePhysicalObject closest = null;
            if (unitCount > 0)
            {
                Vector2 rectangleCenter = new Vector2((((float)mapSpace.X) + ((float)mapSpace.Width) / 2), (((float)mapSpace.Y) + ((float)mapSpace.Height) / 2));
                float rectangleRadius = Vector2.Distance(rectangleCenter, new Vector2(mapSpace.X, mapSpace.Y));


                if (Vector2.Distance(rectangleCenter, center) <= radius + rectangleRadius)
                {
                    CompositePhysicalObject newClosest;
                    newClosest = nw.GetClosestObjectWithinDistance(center, radius);
                    if (newClosest != null)
                    {
                        radius = Vector2.Distance(newClosest.Position, center);
                        closest = newClosest;
                    }

                    newClosest = ne.GetClosestObjectWithinDistance(center, radius);
                    if (newClosest != null)
                    {
                        radius = Vector2.Distance(newClosest.Position, center);
                        closest = newClosest;
                    }

                    newClosest = se.GetClosestObjectWithinDistance(center, radius);
                    if (newClosest != null)
                    {
                        radius = Vector2.Distance(newClosest.Position, center);
                        closest = newClosest;
                    }

                    newClosest = sw.GetClosestObjectWithinDistance(center, radius);
                    if (newClosest != null)
                    {
                        radius = Vector2.Distance(newClosest.Position, center);
                        closest = newClosest;
                    }
                }
            }
            return closest;
        }

        public override CompositePhysicalObject GetClosestObject(Vector2 position)
        {

            return GetClosestObjectWithinDistance(position, float.MaxValue);
        }

        public override void Move(CompositePhysicalObject obj)
        {
            if (this.Contains(obj.Position))
            {
                this.Add(obj);
            }
            else
            {
                if (this.Parent != null)
                {
                    this.Parent.Move(obj);
                }
                else
                {
                    throw new MoveOutOfBound();
                }
            }
        }

        private class MoveOutOfBound : Exception { }
    }
}