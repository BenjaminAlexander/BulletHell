using MyGame.Engine.DataStructures;
using MyGame.Engine.GameState.GameObjectFactory;
using MyGame.Engine.GameState.InstantObjectSet;
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

namespace MyGame.Engine.GameState
{
    class TypeSet<SubType> : TypeSetInterface where SubType : GameObject, new()
    {
        private static Logger log = new Logger(typeof(TypeSet<SubType>));

        //private TwoWayMap<int, InstantTypeSet<SubType>> instantTypeSets = new TwoWayMap<int, InstantTypeSet<SubType>>(new IntegerComparer());
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
        /*
        public ObjectTypeFactoryInterface PrepareForUpdate(int current)
        {
            InstantTypeSet<SubType> currentSet = GetInstantTypeSet(current);
            InstantTypeSet<SubType> nextSet = GetInstantTypeSet(current + 1);
            return currentSet.PrepareForUpdate(nextSet);
            //factory.AddTypeFactory<SubType>(new ObjectTypeFactory<SubType>(this, currentSet.GreatestID + 1, nextSet));
        }*/

        /*public void Update(CurrentInstant current, NextInstant next)
        {
            InstantTypeSet<SubType> from = GetInstantTypeSet(current.InstantID);
            from.Update(current, next);
        }*/

        public InstantTypeSetInterface NewInstantTypeSetInterface(int instantId)
        {
            return new InstantTypeSet<SubType>(this, instantId);
        }

        /*
        public InstantTypeSetInterface GetInstantTypeSetInterface(int instantId)
        {
            return GetInstantTypeSet(instantId);
        }

        public InstantTypeSet<SubType> GetInstantTypeSet(int instantId)
        {
            if (instantTypeSets.ContainsKey(instantId))
            {
                return instantTypeSets[instantId];
            }
            else
            {
                InstantTypeSet<SubType> instantTypeSet = new InstantTypeSet<SubType>(this, instantId);
                instantTypeSets[instantId] = instantTypeSet;
                return instantTypeSet;
            }
        }*/
    }
}
