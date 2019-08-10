using MyGame.Engine.DataStructures;
using MyGame.Engine.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MyGame.Engine.GameState.TypeManager;
using System.Collections;
using MyGame.Engine.GameState.InstantObjectSet;
using MyGame.Engine.Serialization.DataTypes;
using MyGame.Engine.Serialization;
using MyGame.Engine.GameState.Instants;
using MyGame.Engine.GameState.GameObjectFactory;

namespace MyGame.Engine.GameState
{
    //TODO: Serialize/deserialize
    //TODO: update
    //TODO: drop
    class ObjectInstantManager
    {
        private static Logger log = new Logger(typeof(ObjectInstantManager));

        private TypeManager typeManager;
        private TwoWayMap<int, TypeSetInterface> typeSets = new TwoWayMap<int, TypeSetInterface>(new IntegerComparer());
        private TwoWayMap<int, InstantSet> instantSets = new TwoWayMap<int, InstantSet>(new IntegerComparer());

        public ObjectInstantManager(TypeManager typeManager)
        {
            this.typeManager = typeManager;
            foreach (TypeMetadataInterface metaData in typeManager)
            {
                typeSets.Set(metaData.TypeID, metaData.NewTypeSet());
            }
        }

        private InstantSet GetInstantSet(int instantId)
        {
            lock (instantSets)
            {
                if (instantSets.ContainsKey(instantId))
                {
                    return instantSets[instantId];
                }
                else
                {
                    InstantSet instantSet = new InstantSet(typeManager, instantId);
                    foreach (TypeSetInterface typeSet in typeSets.Values)
                    {
                        instantSet.Add(typeSet);
                    }
                    instantSets[instantId] = instantSet;
                    return instantSet;
                }
            }
        }

        public NextInstant Update(int fromInstantId)
        {
            InstantSet fromInstant;
            InstantSet toInstant;
            lock (instantSets)
            {
                int toInstantId = fromInstantId + 1;
                fromInstant = instantSets[fromInstantId];
                toInstant = GetInstantSet(toInstantId);
            }
            return fromInstant.Update(toInstant);
        }

        public List<byte[]> Serialize(int instantId, int maximumBufferSize)
        {
            InstantSet instantSet;
            lock (instantSets)
            {
                instantSet = instantSets[instantId];
            }
            return instantSet.Serialize(maximumBufferSize);
        }

        public void Deserialize(byte[] buffer, ref int bufferOffset)
        {
            int instantId;
            Serialization.Utils.Read(out instantId, buffer, ref bufferOffset);
            InstantSet instantSet;
            lock (instantSets)
            {
                instantSet = GetInstantSet(instantId);
            }
            //TODO: do something with this return value
            instantSet.Deserialize(buffer, ref bufferOffset);
        }
    }
}
