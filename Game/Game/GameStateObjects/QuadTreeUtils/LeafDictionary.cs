using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGame.GameStateObjects.PhysicalObjects;

namespace MyGame.GameStateObjects.QuadTreeUtils
{
    class LeafDictionary
    {
        private Dictionary<CompositePhysicalObject, Leaf> leafDictionary = new Dictionary<CompositePhysicalObject, Leaf>();
        public void SetLeaf(CompositePhysicalObject obj, Leaf leaf)
        {
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
        }
        public Leaf GetLeaf(CompositePhysicalObject obj)
        {
            if (leafDictionary.ContainsKey(obj))
            {
                if (!leafDictionary[obj].Contains(obj))
                {
                    throw new Exception("Incorrect leaf");
                }

                return leafDictionary[obj];
            }
            return null;
        }
    }
}
