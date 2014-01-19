using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MyGame.GameStateObjects.QuadTreeUtils
{
    public class Leaf : Node
    {

        private Rectangle mapSpace;
        private List<CompositePhysicalObject> unitList;
        public Leaf(Node parent, Rectangle mapSpace) : base(parent)
        {
            this.mapSpace = mapSpace;
            unitList = new List<CompositePhysicalObject>();
        }

        public override Leaf Add(CompositePhysicalObject unit)
        {
            if (this.Contains(unit.Position))
            {
                unitList.Add(unit);
                unit.SetLeaf(this);
                return this;
            }
            return null;
        }

        public override bool Remove(CompositePhysicalObject unit)
        {
            if (unitList.Contains(unit))
            {
                unitList.Remove(unit);
                unit.SetLeaf(null);
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
                    this.Remove(obj);
                    this.Parent.Move(obj);
                }
            }
            else
            {
                throw new ObjectNotFound();
            }
        }

        private class ObjectNotFound : Exception { }
    }
}
