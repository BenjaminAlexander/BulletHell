using MyGame.Engine.DataStructures;
using MyGame.Engine.GameState.Instants;
using MyGame.Engine.Reflection;
using MyGame.Engine.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.Engine.GameState
{
    class TypeSet<SubType> : IEnumerable<SubType>, TypeSetInterface where SubType : GameObject, new()
    {
        private static Logger log = new Logger(typeof(TypeSet<SubType>));

        private TwoWayMap<int, SubType> objects = new TwoWayMap<int, SubType>(new IntegerComparer());

        public SubType this[int id]
        {
            get
            {
                return objects[id];
            }
        }

        public int Count
        {
            get
            {
                return objects.Count;
            }
        }

        public SubType NewObject(Instant intstant)
        {
            int nextID = objects.GreatestKey + 1;
            SubType newObject = GameObject.NewGameObject<SubType>(nextID, intstant);
            return newObject;
        }

        public bool Contains(SubType item)
        {
            return objects.ContainsValue(item);
        }

        public bool Contains(int id)
        {
            return objects.ContainsKey(id);
        }

        public IEnumerator<SubType> GetEnumerator()
        {
            return objects.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
