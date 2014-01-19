using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MyGame.GameStateObjects.QuadTreeUtils
{
    public class Leaf : Node
    {

        private int max_count = 10;

        private Rectangle mapSpace;
        private List<CompositePhysicalObject> unitList;

        public override void Inveriant()
        {
            if (this.Parent == null)
            {
                throw new Exception("must have parent");
            }

            if (!(this.Parent is InternalNode))
            {
                throw new Exception("only internal nodes can be parents");
            }

            if (!(this.Parent.nw == this ||
                this.Parent.ne == this ||
                this.Parent.sw == this ||
                this.Parent.se == this))
            {
                throw new Exception("child not with correct parent");
            }
        }

        public override int ObjectCount()
        {
            return unitList.Count;
        }

        public Leaf(InternalNode parent, Rectangle mapSpace)
            : base(parent)
        {
            this.mapSpace = mapSpace;
            unitList = new List<CompositePhysicalObject>();
        }

        public override bool Add(CompositePhysicalObject unit)
        {
            if (this.Contains(unit.Position))
            {
                unitList.Add(unit);
                unit.SetLeaf(this);

                if (unitList.Count > max_count)
                {
                    this.Expand();
                }
                return true;
            }
            return false;
        }

        public override bool Remove(CompositePhysicalObject unit)
        {
            if (unitList.Contains(unit))
            {
                unitList.Remove(unit);
                unit.SetLeaf(null);
                this.Parent.Collapse();
                return true;
            }
            return false;
        }

        public override bool Contains(Vector2 point)
        {
            return Node.Contains(mapSpace, point);
        }

        public override List<CompositePhysicalObject> GetObjectsInCircle(Vector2 center, float radius)
        {
            List<CompositePhysicalObject> returnList = new List<CompositePhysicalObject>();
            if (unitList.Count > 0)
            {
                Vector2 rectangleCenter = new Vector2((((float)mapSpace.X) + ((float)mapSpace.Width) / 2), (((float)mapSpace.Y) + ((float)mapSpace.Height) / 2));
                float rectangleRadius = Vector2.Distance(rectangleCenter, new Vector2(mapSpace.X, mapSpace.Y));

                if (Vector2.Distance(rectangleCenter, center) <= radius + rectangleRadius)
                {

                    foreach (CompositePhysicalObject unit in unitList)
                    {
                        if (Vector2.Distance(unit.Position, center) <= radius)
                        {
                            returnList.Add(unit);
                        }
                    }
                }
                else
                {
                    int x = 0;
                }
            }
            return returnList;
        }

        public override List<CompositePhysicalObject> CompleteList()
        {
            return unitList;
        }

        public override Rectangle GetRectangle()
        {
            return mapSpace;
        }

        public override CompositePhysicalObject GetClosestObject(Vector2 position)
        {
            if (unitList.Count < 1)
            {
                return null;
            }

            CompositePhysicalObject closestUnit = unitList.ElementAt(0);
            float distance = Vector2.Distance(position, closestUnit.Position);
            foreach (CompositePhysicalObject unit in unitList)
            {
                float newDistance = Vector2.Distance(position, unit.Position);
                if (newDistance < distance)
                {
                    distance = newDistance;
                    closestUnit = unit;
                }
            }
            return closestUnit;
        }

        public override CompositePhysicalObject GetClosestObjectWithinDistance(Vector2 position, float distance)
        {
            if (unitList.Count < 1)
            {
                return null;
            }

            CompositePhysicalObject closestUnit = null;
            foreach (CompositePhysicalObject unit in unitList)
            {
                float newDistance = Vector2.Distance(position, unit.Position);
                if (newDistance < distance)
                {
                    distance = newDistance;
                    closestUnit = unit;
                }

            }

            return closestUnit;
        }

        public override void Move(CompositePhysicalObject obj)
        {
            if(unitList.Contains(obj))
            {
                if (!this.Contains(obj.Position))
                {
                    unitList.Remove(obj);
                    this.Parent.Move(obj);
                    if (unitList.Contains(obj))
                    {
                        throw new Exception("Move failed");
                    }
                    this.Parent.Collapse();
                }
            }
            else
            {
                throw new ObjectNotFound();
            }
        }

        private void Expand()
        {
            int preCount = this.unitList.Count();
            int addOps = 0;
            Node newNode = new InternalNode(false, this.Parent, this.mapSpace);// (this.Parent, this.mapSpace, 2);
            this.Parent.Replace(this, newNode);
            foreach (CompositePhysicalObject obj in unitList)
            {
                addOps++;
                if (!newNode.Add(obj))
                {
                    throw new Exception("Failed to add after move");
                }
            }

            if (addOps != preCount)
            {
                throw new Exception("addOps bad");
            }

            if (newNode.ObjectCount() != preCount)
            {
                throw new Exception("incorrect count");
            }
        }

        private class ObjectNotFound : Exception { }
    }
}
