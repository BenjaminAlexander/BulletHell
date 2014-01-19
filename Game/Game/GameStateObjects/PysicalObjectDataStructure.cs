using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MyGame.GameStateObjects
{
    //structure to Provide spacial lookup of physical objects
    class PysicalObjectDataStructure
    {
        private List<CompositePhysicalObject> objectList = new List<CompositePhysicalObject>();

        public Boolean Add(CompositePhysicalObject obj)
        {
            if (obj != null && !objectList.Contains(obj))
            {
                objectList.Add(obj);
                return true;
            }
            return false;
        }

        public Boolean Remove(CompositePhysicalObject obj)
        {
            if (obj != null && objectList.Contains(obj))
            {
                objectList.Remove(obj);
                return true;
            }
            return false;
        }

        public List<CompositePhysicalObject> Get(Vector2 position, float radius)
        {
            List<CompositePhysicalObject> returnList = new List<CompositePhysicalObject>();

            foreach (CompositePhysicalObject obj in objectList)
            {
                if (Vector2.Distance(obj.Position, position) <= radius)
                {
                    returnList.Add(obj);
                }
            }
            return returnList;
        }
    }
}
