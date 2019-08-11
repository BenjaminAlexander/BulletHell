using MyGame.Engine.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MyGame.Engine.GameState.TypeManager;
using System.Collections;
using MyGame.Engine.GameState.TypeSets;

namespace MyGame.Engine.GameState
{
    class TypeManager : TypeManagerInterface, IEnumerable<TypeMetadataInterface>
    {
        //TODO: move this interface to a seperat file
        public abstract class TypeMetadataInterface
        {
            int id;
            public TypeMetadataInterface(int id)
            {
                this.id = id;
            }

            public int TypeID
            {
                get
                {
                    return id;
                }
            }

            public abstract Type GetObjectType();
            public abstract TypeSetInterface NewTypeSet();
        }

        //TODO: move this class to a seperate file
        public class TypeMetadata<SubType> : TypeMetadataInterface where SubType : GameObject, new()
        {
            public TypeMetadata(int id) : base(id)
            {

            }

            public override TypeSetInterface NewTypeSet()
            {
                return new TypeSet<SubType>(TypeID);
            }

            public override Type GetObjectType()
            {
                return typeof(SubType);
            }
        }

        private Dictionary<int, TypeMetadataInterface> metaData = new Dictionary<int, TypeMetadataInterface>();
        private TwoWayMap<int, Type> map = new TwoWayMap<int, Type>();

        public void AddType<SubType>() where SubType : GameObject, new()
        {
            if (!map.ContainsValue(typeof(SubType)))
            {
                int nextID = map.GreatestKey + 1;
                TypeMetadata<SubType> newMetaData = new TypeMetadata<SubType>(nextID);
                metaData[nextID] = newMetaData;
                map[nextID] = typeof(SubType);
            }
        }

        public TypeMetadataInterface GetMetaData(Type t)
        {
            return metaData[map[t]];
        }

        public IEnumerator<TypeMetadataInterface> GetEnumerator()
        {
            return metaData.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
