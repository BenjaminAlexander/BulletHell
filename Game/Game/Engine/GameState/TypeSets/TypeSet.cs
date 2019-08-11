using MyGame.Engine.DataStructures;
using MyGame.Engine.GameState.GameObjectFactory;
using MyGame.Engine.GameState.Instants;
using MyGame.Engine.GameState.Instants;
using MyGame.Engine.Reflection;
using MyGame.Engine.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MyGame.Engine.GameState.TypeManager;

namespace MyGame.Engine.GameState.TypeSets
{
    class TypeSet<SubType> : TypeSetInterface where SubType : GameObject, new()
    {
        private static Logger log = new Logger(typeof(TypeSet<SubType>));

        private TwoWayMap<int, SubType> objects = new TwoWayMap<int, SubType>(new IntegerComparer());
        private int typeId;

        public TypeSet(int typeId)
        {
            this.typeId = typeId;
        }

        public SubType GetObject(int id)
        {
            if (objects.ContainsKey(id))
            {
                return objects[id];
            }
            else
            {
                SubType newObject = new SubType();
                newObject.SetUp(id, this);
                objects[id] = newObject;
                return newObject;
            }
        }

        public int TypeID
        {
            get
            {
                return typeId;
            }
        }

        public InstantTypeSetInterface NewInstantTypeSetInterface(int instantId)
        {
            return new InstantTypeSet<SubType>(this, instantId);
        }
    }
}
