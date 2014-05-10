using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.GameStateObjects.PhysicalObjects;
using MyGame.GameStateObjects.DataStuctures;

namespace MyGame.GameStateObjects.QuadTreeUtils
{
    class Leaf : Node
    {
        private GameObjectListManager unitList;
        public override int ObjectCount()
        {
            return unitList.GetMaster().Count;
        }

        public Leaf(InternalNode parent, Rectangle mapSpace, LeafDictionary leafDictionary)
            : base(parent, mapSpace, leafDictionary)
        {
            unitList = new GameObjectListManager();
        }

        public override bool Add(CompositePhysicalObject unit)
        {
            if (this.Contains(unit.Position))
            {
                unitList.Add(unit);
                leafDictionary.SetLeaf(unit, this);

                if (ObjectCount() > max_count)
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
                leafDictionary.SetLeaf(unit, null);
                unitList.Remove(unit);
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
            return Node.Contains(MapSpace, point);
        }

        public override List<CompositePhysicalObject> GetObjectsInCircle(Vector2 center, float radius, List<CompositePhysicalObject> list)
        {
            if (ObjectCount() > 0)
            {
                Vector2 rectangleCenter = new Vector2((((float)MapSpace.X) + ((float)MapSpace.Width) / 2), (((float)MapSpace.Y) + ((float)MapSpace.Height) / 2));
                float rectangleRadius = Vector2.Distance(rectangleCenter, new Vector2(MapSpace.X, MapSpace.Y));

                if (Vector2.Distance(rectangleCenter, center) <= radius + rectangleRadius)
                {

                    foreach (CompositePhysicalObject unit in unitList.GetList<CompositePhysicalObject>())
                    {
                        if (Vector2.Distance(unit.Position, center) <= radius)
                        {
                            list.Add(unit);
                        }
                    }
                }
            }
            return list;
        }

        public override List<CompositePhysicalObject> CompleteList(ref List<CompositePhysicalObject> list)
        {
            foreach (CompositePhysicalObject obj in unitList.GetList<CompositePhysicalObject>())
            {
                list.Add(obj);
            }
            return list;
        }

        public override void Move(CompositePhysicalObject obj)
        {
            if(unitList.Contains(obj))
            {
                if (!this.Contains(obj.Position))
                {
                    this.Remove(obj);
                    this.Parent.Move(obj);
                    if (unitList.Contains(obj))
                    {
                        throw new Exception("Move failed");
                    }
                    if (!this.Parent.IsChild(this))
                    {
                        throw new Exception("incorrect child/parent");
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
            if (MapSpace.Width > 1 && MapSpace.Height > 1)
            {
                Node newNode = new InternalNode(false, this.Parent, this.MapSpace, leafDictionary);// (this.Parent, this.mapSpace, 2);
                this.Parent.Replace(this, newNode);
                foreach (CompositePhysicalObject obj in unitList.GetList<CompositePhysicalObject>())
                {
                    this.Remove(obj);
                    if (!newNode.Add(obj))
                    {
                        this.Parent.Move(obj);
                        //throw new Exception("Failed to add after move");
                    }
                    leafDictionary.Invariant(obj);
                }

                leafDictionary.DestroyLeaf(this);
            }
        }

        public Boolean Contains(CompositePhysicalObject obj)
        {
            return this.unitList.Contains(obj);
        }
    }
}
