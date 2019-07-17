using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.Engine.Serialization;
using MyGame.Engine.Reflection;
using MyGame.Engine.DataStructures;
using MyGame.Engine.GameState.Instants;
using MyGame.Engine.Utils;

namespace MyGame.Engine.GameState
{
    //TODO: Add code to update from one instant to the next
    public class GameObjectCollection
    {
        private Logger log = new Logger(typeof(GameObjectCollection));
        private GameObjectSet objects = new GameObjectSet();
        private TwoWayMap<int, Instant> instantMap = new TwoWayMap<int, Instant>();

        public GameObject GetGameObject(int id)
        {
            return objects[id];
        }

        internal int SerializationSize(GameObject obj, Instant instant)
        {
            return sizeof(int) + instant.SerializationSize(obj);
        }

        internal void Serialize(GameObject obj, Instant instant, byte[] buffer, ref int bufferOffset)
        {
            Serialization.Utils.Write(instant.ID, buffer, ref bufferOffset);
            instant.Serialize(obj, buffer, ref bufferOffset);
        }

        internal byte[] Serialize(GameObject obj, Instant instant)
        {
            byte[] buffer = new byte[this.SerializationSize(obj, instant)];
            int bufferOffset = 0;
            this.Serialize(obj, instant, buffer, ref bufferOffset);
            return buffer;
        }

        //TODO: change return type to void
        public GameObject Deserialize(byte[] buffer)
        {
            int bufferOffset = 0;
            return this.Deserialize(buffer, ref bufferOffset);
        }

        //TODO: change return type to void
        public GameObject Deserialize(byte[] buffer, ref int bufferOffset)
        {
            int instantId = Serialization.Utils.ReadInt(buffer, ref bufferOffset);

            Instant instant = GetInstant(instantId);
            return instant.Deserialize(buffer, ref bufferOffset);
        }

/*        public bool CheckCollectionIntegrety()
        {
            if(!objects.CheckIntegrety())
            {
                log.Error("objects set failed its integrety check");
                return false;
            }

            foreach(GameObject obj in objects)
            {
                if(!obj.CheckThatInstantKeysContainThis())
                {
                    log.Error("A GameObject failed its Instant <-> GameObject containment check");
                    return false;
                }
            }

            foreach (KeyValuePair<int, Instant> pair in instantMap)
            {
                if(!pair.Value.CheckIntegrety())
                {
                    log.Error("An instant failed its integerty check");
                    return false;
                }
            }
            return true;
        }*/

        internal Instant GetInstant(int instant)
        {
            if(instantMap.ContainsKey(instant))
            {
                return instantMap[instant];
            }
            else
            {
                Instant newInstant = new Instant(instant, objects);
                instantMap[instant] = newInstant;
                return newInstant;
            }
        }

        internal Instant GetInstant(Instant instant)
        {
            return GetInstant(instant.GetHashCode());
        }

        //TODO: which is the right update method?
        public void Update(int current)
        {
            Instant currentInstant = GetInstant(current);
            Instant nextInstant = GetInstant(current + 1);
            Instant.Update(currentInstant, nextInstant);
        }

        public void Update(int current, int next)
        {
            Instant currentInstant = GetInstant(current);
            Instant nextInstant = GetInstant(next);
            Instant.Update(currentInstant, nextInstant);
        }

        internal Instant Update(Instant current)
        {
            Instant currentInstant = GetInstant(current);
            Instant nextInstant = GetInstant(current.GetHashCode() + 1);
            Instant.Update(currentInstant, nextInstant);
            return nextInstant;
        }
    }
}
