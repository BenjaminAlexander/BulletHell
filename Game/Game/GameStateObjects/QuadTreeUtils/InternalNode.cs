using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MyGame.GameStateObjects.QuadTreeUtils
{
    public class InternalNode : Node
    {
        private Rectangle mapSpace;
        private Boolean root;
        public Node nw;
        public Node ne;
        public Node sw;
        public Node se;
        private int unitCount = 0;

        public override int ObjectCount()
        {
            return unitCount;
        }

        public override void Inveriant()
        {
            if (root && this.Parent != null)
            {
                throw new Exception("Root cannot have parent");
            }
            if (!root)
            {
                if(this.Parent == null)
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

                int otherC = this.CompleteList().Count;
                if (unitCount != otherC)
                {
                    throw new Exception("inaccurate count");
                }
            }

            nw.Inveriant();
            ne.Inveriant();
            sw.Inveriant();
            se.Inveriant();
        }

        public InternalNode(Boolean root, InternalNode parent, Rectangle mapSpace)
            : base(parent)
        {
            this.root = root;
            this.mapSpace = mapSpace;

            int halfWidth = mapSpace.Width / 2;
            int halfHeight = mapSpace.Height / 2;

            Rectangle swRectangle = new Rectangle(mapSpace.X, mapSpace.Y, halfWidth, halfHeight);
            Rectangle seRectangle = new Rectangle(mapSpace.X + halfWidth, mapSpace.Y, mapSpace.Width - halfWidth, halfHeight);
            Rectangle nwRectangle = new Rectangle(mapSpace.X, mapSpace.Y + halfHeight, halfWidth, mapSpace.Height - halfHeight);
            Rectangle neRectangle = new Rectangle(mapSpace.X + halfWidth, mapSpace.Y + halfHeight, mapSpace.Width - halfWidth, mapSpace.Height - halfHeight);

            nw = new Leaf(this, nwRectangle);//Node.ConstructBranch(this, nwRectangle, height - 1);
            ne = new Leaf(this, neRectangle);//Node.ConstructBranch(this, neRectangle, height - 1);
            sw = new Leaf(this, swRectangle);//Node.ConstructBranch(this, swRectangle, height - 1);
            se = new Leaf(this, seRectangle);//Node.ConstructBranch(this, seRectangle, height - 1);

        }

        public override bool Add(CompositePhysicalObject obj)
        {
            this.Inveriant();
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
                    this.Inveriant();
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

        public override bool Remove(CompositePhysicalObject obj)
        {
            this.Inveriant();
            if (this.Contains(obj.Position))
            {
                bool removed = false;
                Node removedFrom = null;

                if (nw.Remove(obj))
                {
                    removed = true;
                    removedFrom = nw;
                }
                else if (ne.Remove(obj))
                {
                    removed = true;
                    removedFrom = ne;
                }
                else if (sw.Remove(obj))
                {
                    removed = true;
                    removedFrom = sw;
                }
                else if (se.Remove(obj))
                {
                    removed = true;
                    removedFrom = se;
                }

                

                if (removed)
                {
                    unitCount--;
                    //Collapse();

                    if (removedFrom.ObjectCount() < Node.max_count && removedFrom is InternalNode)
                    {
                        //throw new Exception("Node did not collapse");
                    }

                    //this.Inveriant();
                    return true;
                }
                else
                {
                    this.Inveriant();
                    return false;
                }
            }
            else
            {
                this.Inveriant();
                return false;
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
                        Node newNode = new Leaf(this.Parent, this.mapSpace);
                        this.Parent.Replace(this, newNode);
                        foreach (CompositePhysicalObject myObjects in this.CompleteList())
                        {
                            newNode.Add(myObjects);
                        }
                        this.Parent.Collapse();
                        //Console.WriteLine("asking " + Parent.id + (" to replace " + this.id) + (" with " + newNode.id));
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
            unitCount--;
            if (this.Contains(obj.Position))
            {
                this.Add(obj);
                /*this.Remove(obj);
                if (root)
                {
                    if (!this.Add(obj))
                    {
                        throw new Exception("Failed to add after move");
                    }
                }
                else
                {
                    if (!this.Parent.Add(obj))
                    {
                        throw new Exception("Failed to add after move");
                    }
                }*/
            }
            else
            {
                if (this.Parent != null)
                {
                    this.Parent.Move(obj);

                    if (this.CompleteList().Contains(obj))
                    {
                        throw new Exception("Move failed");
                    }
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
            Console.WriteLine("my children are " + ne.id +", " + nw.id +", " + se.id +", " + sw.id);


            if(nw.Equals(current))
            {
                nw = newNode;
                //current.DisconnectFromParent();
            } 
            else if(ne.Equals(current))
            {
                ne = newNode;
                //current.DisconnectFromParent();
            }
            else if (sw.Equals(current))
            {
                sw = newNode;
                //current.DisconnectFromParent();
            }
            else if (se.Equals(current))
            {
                se = newNode;
                //current.DisconnectFromParent();
            }
            else
            {
                throw new Exception("Cannot replace a non child");
            }
        }

        

        private class MoveOutOfBound : Exception { }
    }
}