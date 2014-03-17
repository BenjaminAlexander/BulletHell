using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.GameStateObjects.PhysicalObjects;

namespace MyGame.GameStateObjects.QuadTreeUtils
{
    public class Leaf : Node
    {
        
        private Rectangle mapSpace;
        private List<CompositePhysicalObject> unitList;
        private int unitCount = 0;
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
                unitCount++;
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

        public override Leaf Remove(CompositePhysicalObject unit)
        {
            if (unitList.Contains(unit))
            {
                unitCount--;
                unitList.Remove(unit);
                unit.SetLeaf(null);
                return this;
            }
            return null;
        }

        public void Collapse()
        {
            this.Parent.Collapse();
        }

        public override bool Contains(Vector2 point)
        {
            return Node.Contains(mapSpace, point);
        }

        public override List<CompositePhysicalObject> GetObjectsInCircle(Vector2 center, float radius, List<CompositePhysicalObject> list)
        {
            //List<CompositePhysicalObject> returnList = new List<CompositePhysicalObject>();
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
                            list.Add(unit);
                        }
                    }
                }
                else
                {
                    int x = 0;
                }
            }
            return list;
        }

        public override List<CompositePhysicalObject> CompleteList(ref List<CompositePhysicalObject> list)
        {
            foreach (CompositePhysicalObject obj in unitList)
            {
                list.Add(obj);
            }
            return list;
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
                    unitCount--;
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
                throw new Exception("No such object");
            }
        }

        private void Expand()
        {
            if (mapSpace.Width > 1 && mapSpace.Height > 1)
            {
                Node newNode = new InternalNode(false, this.Parent, this.mapSpace);// (this.Parent, this.mapSpace, 2);
                this.Parent.Replace(this, newNode);
                foreach (CompositePhysicalObject obj in unitList)
                {
                    if (!newNode.Add(obj))
                    {
                        throw new Exception("Failed to add after move");
                    }
                }
            }
        }
    }
}
