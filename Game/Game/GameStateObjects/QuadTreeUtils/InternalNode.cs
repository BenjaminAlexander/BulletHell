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
        private List<Node> children = new List<Node>();
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

            Node nw = new Leaf(this, nwRectangle, leafDictionary);
            Node ne = new Leaf(this, neRectangle, leafDictionary);
            Node sw = new Leaf(this, swRectangle, leafDictionary);
            Node se = new Leaf(this, seRectangle, leafDictionary);
            children.Add(nw);
            children.Add(ne);
            children.Add(sw);
            children.Add(se);
        }

        public override bool Add(CompositePhysicalObject obj)
        {
            if (this.Contains(obj.Position))
            {
                int adds = 0;
                foreach (Node child in new List<Node>(children))
                {
                    if (child.Add(obj))
                    {
                        adds++;

                    }
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
                throw new Exception("move out of bounds");
            }

            return false;
        }

        public override Leaf Remove(CompositePhysicalObject obj)
        {
            if (this.Contains(obj.Position))
            {
                if (children.Count != 4)
                {
                    throw new Exception("child");
                }

                Leaf removedFrom = null;
                foreach (Node child in children)
                {
                    removedFrom = child.Remove(obj);
                    if (removedFrom != null)
                    {
                        break;
                    }
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
                if (AllChildrenAreLeaves())
                {
                    if (this.Parent != null)
                    {
                        if (!this.Parent.IsChild(this))
                        {
                            throw new Exception("incorrect child/parent");
                        }
                        Leaf newNode = new Leaf(this.Parent, this.MapSpace, leafDictionary);
                        this.Parent.Replace(this, newNode);

                        foreach (Leaf leaf in children)
                        {
                            foreach (CompositePhysicalObject myObjects in leaf.CompleteList())
                            {
                                this.Remove(myObjects);
                                if (!newNode.Add(myObjects))
                                {
                                    this.Parent.Move(myObjects);
                                    //throw new Exception("add failed");
                                }
                                leafDictionary.Invariant(myObjects);
                            }

                        
                                leafDictionary.DestroyLeaf(leaf);
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
                        foreach (Node child in children)
                        {
                            child.GetObjectsInCircle(center, radius, list);
                        }
                    }
                }
                return list;
            }
        }

        public override List<CompositePhysicalObject> CompleteList(ref List<CompositePhysicalObject> list)
        {

            foreach (Node child in children)
            {
                list = child.CompleteList(ref list);
            }
            return list;
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
                    throw new Exception("move out of bounds");
                }
            }
        }

        public void Replace(Node current, Node newNode)
        {
            if (current is InternalNode == newNode is InternalNode)
            {
                throw new Exception("Illegal replacement type");
            }

            if (!children.Contains(current))
            {
                throw new Exception("Cannot replace a non child");
            }

            children.Remove(current);
            children.Add(newNode);

            if (children.Count != 4)
            {
                throw new Exception("incorrect child count");
            }
        }

        private bool AllChildrenAreLeaves()
        {
            foreach (Node child in children)
            {
                if (!(child is Leaf))
                {
                    return false;
                }
            }
            return true;
        }

        public bool IsChild(Node node)
        {
            return this.children.Contains(node);
        }

    }
}