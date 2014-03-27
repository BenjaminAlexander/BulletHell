using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.Geometry;
using MyGame.GameStateObjects.PhysicalObjects;

namespace MyGame.GameStateObjects.QuadTreeUtils
{
    class InternalNode : Node
    {
        private Boolean root;
        private Node nw;
        private Node ne;
        private Node sw;
        private Node se;
        private int unitCount = 0;

        public override int ObjectCount()
        {
            return unitCount;
        }

        public InternalNode(Boolean root, InternalNode parent, Rectangle mapSpace, LeafDictionary leafDictionary)
            : base(parent, mapSpace, leafDictionary)
        {
            this.root = root;
            int halfWidth = mapSpace.Width / 2;
            int halfHeight = mapSpace.Height / 2;

            Rectangle swRectangle = new Rectangle(mapSpace.X, mapSpace.Y, halfWidth, halfHeight);
            Rectangle seRectangle = new Rectangle(mapSpace.X + halfWidth, mapSpace.Y, mapSpace.Width - halfWidth, halfHeight);
            Rectangle nwRectangle = new Rectangle(mapSpace.X, mapSpace.Y + halfHeight, halfWidth, mapSpace.Height - halfHeight);
            Rectangle neRectangle = new Rectangle(mapSpace.X + halfWidth, mapSpace.Y + halfHeight, mapSpace.Width - halfWidth, mapSpace.Height - halfHeight);

            nw = new Leaf(this, nwRectangle, leafDictionary);
            ne = new Leaf(this, neRectangle, leafDictionary);
            sw = new Leaf(this, swRectangle, leafDictionary);
            se = new Leaf(this, seRectangle, leafDictionary);

        }

        public override bool Add(CompositePhysicalObject obj)
        {
            if (this.Contains(obj.Position))
            {
                
                int adds = 0;
                bool returnNode = nw.Add(obj);
                if (returnNode)
                {
                    adds++;
                }

                returnNode = ne.Add(obj);
                if (returnNode)
                {
                    adds++;
                }

                returnNode = sw.Add(obj);
                if (returnNode)
                {
                    adds++;
                }

                returnNode = se.Add(obj);
                if (returnNode)
                {
                    adds++;
                }
                if (adds == 1)
                {
                    unitCount++;
                    return true;
                }
                else
                {
                    throw new Exception("failed adds to QuadTree");
                }
            }
            if (Parent == null)
            {
                throw new MoveOutOfBound();
            }

            return false;
        }

        public override Leaf Remove(CompositePhysicalObject obj)
        {
            if (this.Contains(obj.Position))
            {
                Leaf removedFrom = nw.Remove(obj);

                if (removedFrom == null)
                {
                    removedFrom = ne.Remove(obj);
                }

                if (removedFrom == null)
                {
                    removedFrom = se.Remove(obj);
                }

                if (removedFrom == null)
                {
                    removedFrom = sw.Remove(obj);
                }
                if (removedFrom != null)
                {
                    unitCount--;
                    return removedFrom;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
        
        public void Collapse()
        {
            if (this.ObjectCount() < Node.max_count)
            {
                if (nw is Leaf &&
                    ne is Leaf &&
                    sw is Leaf &&
                    se is Leaf)
                {
                    if (this.Parent != null)
                    {
                        Node newNode = new Leaf(this.Parent, this.MapSpace, leafDictionary);
                        this.Parent.Replace(this, newNode);
                        foreach (CompositePhysicalObject myObjects in this.CompleteList())
                        {
                            newNode.Add(myObjects);
                        }
                        this.Parent.Collapse();
                    }
                }
                else
                {
                    throw new Exception("Children did not collapse");
                }
            }
        }

        public override bool Contains(Vector2 point)
        {
            return Node.Contains(MapSpace, point);
        }

        public override List<CompositePhysicalObject> GetObjectsInCircle(Vector2 center, float radius, List<CompositePhysicalObject> list)
        {
            if ((new Circle(center, radius)).Contains(this.MapSpace))
            {
                //return everything
                return this.CompleteList(ref list);
            }
            else
            {
                //List<CompositePhysicalObject> returnList = new List<CompositePhysicalObject>();
                if (unitCount > 0)
                {
                    Vector2 rectangleCenter = new Vector2((((float)MapSpace.X) + ((float)MapSpace.Width) / 2), (((float)MapSpace.Y) + ((float)MapSpace.Height) / 2));
                    float rectangleRadius = Vector2.Distance(rectangleCenter, new Vector2(MapSpace.X, MapSpace.Y));


                    if (Vector2.Distance(rectangleCenter, center) <= radius + rectangleRadius)
                    {
                        nw.GetObjectsInCircle(center, radius, list);
                        ne.GetObjectsInCircle(center, radius, list);
                        sw.GetObjectsInCircle(center, radius, list);
                        se.GetObjectsInCircle(center, radius, list);
                    }
                }
                return list;
            }
        }

        public override List<CompositePhysicalObject> CompleteList(ref List<CompositePhysicalObject> list)
        {
            list = nw.CompleteList(ref list);
            list = ne.CompleteList(ref list);
            list = sw.CompleteList(ref list);
            list = se.CompleteList(ref list);
            return list;
        }

        public override CompositePhysicalObject GetClosestObjectWithinDistance(Vector2 center, float radius)
        {
            CompositePhysicalObject closest = null;
            if (unitCount > 0)
            {
                Vector2 rectangleCenter = new Vector2((((float)MapSpace.X) + ((float)MapSpace.Width) / 2), (((float)MapSpace.Y) + ((float)MapSpace.Height) / 2));
                float rectangleRadius = Vector2.Distance(rectangleCenter, new Vector2(MapSpace.X, MapSpace.Y));

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
            unitCount--;
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

        public void Replace(Node current, Node newNode)
        {
            if (current is InternalNode == newNode is InternalNode)
            {
                throw new Exception("Illegal replacement type");
            }

            if (nw == newNode || ne == newNode || sw == newNode || se == newNode)
            {
                throw new Exception("Cannot replace with current child");
            }

            if(nw.Equals(current))
            {
                nw = newNode;
            } 
            else if(ne.Equals(current))
            {
                ne = newNode;
            }
            else if (sw.Equals(current))
            {
                sw = newNode;
            }
            else if (se.Equals(current))
            {
                se = newNode;
            }
            else
            {
                throw new Exception("Cannot replace a non child");
            }
        }

        

        private class MoveOutOfBound : Exception { }
    }
}