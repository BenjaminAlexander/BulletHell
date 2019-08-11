using MyGame.Engine.DataStructures;
using MyGame.Engine.Utils;
using System.Collections.Generic;
using static MyGame.Engine.GameState.TypeManager;
using MyGame.Engine.GameState.Instants;
using MyGame.Engine.GameState.TypeSets;

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
        private TwoWayMap<int, Instant> instantSets = new TwoWayMap<int, Instant>(new IntegerComparer());

        public ObjectInstantManager(TypeManager typeManager)
        {
            this.typeManager = typeManager;
            foreach (TypeMetadataInterface metaData in typeManager)
            {
                typeSets.Set(metaData.TypeID, metaData.NewTypeSet());
            }
        }

        private Instant GetInstantSet(int instantId)
        {
            lock (instantSets)
            {
                if (instantSets.ContainsKey(instantId))
                {
                    return instantSets[instantId];
                }
                else
                {
                    Instant instantSet = new Instant(typeManager, instantId);
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
            Instant fromInstant;
            Instant toInstant;
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
            Instant instantSet;
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
            Instant instantSet;
            lock (instantSets)
            {
                instantSet = GetInstantSet(instantId);
            }
            //TODO: do something with this return value
            instantSet.Deserialize(buffer, ref bufferOffset);
        }
    }
}
