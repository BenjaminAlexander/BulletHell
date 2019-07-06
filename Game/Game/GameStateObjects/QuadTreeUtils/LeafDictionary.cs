/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGame.GameStateObjects.PhysicalObjects;

namespace MyGame.GameStateObjects.QuadTreeUtils
{
    class LeafDictionary
    {
        private QuadTree tree;
        private Dictionary<CompositePhysicalObject, Leaf> leafDictionary = new Dictionary<CompositePhysicalObject, Leaf>();

        public LeafDictionary(QuadTree tree)
        {
            this.tree = tree;
        }

        public void SetLeaf(CompositePhysicalObject obj, Leaf leaf)
        {
            //this.Invariant();
            if (leaf != null)
            {
                if (!leafDictionary.ContainsKey(obj))
                {
                    leafDictionary.Add(obj, leaf);
                }
                leafDictionary[obj] = leaf;
            }
            else
            {
                leafDictionary.Remove(obj);
            }
            //this.Invariant();
        }
        public Leaf GetLeaf(CompositePhysicalObject obj)
        {
            //this.Invariant();
            if (leafDictionary.ContainsKey(obj))
            {
                if (!leafDictionary[obj].Contains(obj))
                {
                    throw new Exception("Incorrect leaf");
                }

                return leafDictionary[obj];
            }
            throw new Exception("object does not have leaf");
        }

        public void DestroyLeaf(Leaf l)
        {
            Dictionary<CompositePhysicalObject, Leaf> copy = new Dictionary<CompositePhysicalObject, Leaf>(leafDictionary);
            foreach (CompositePhysicalObject obj in copy.Keys)
            {
                if (copy[obj] == l)
                {
                    leafDictionary.Remove(obj);
                }
            }
        }
    }
}
*/