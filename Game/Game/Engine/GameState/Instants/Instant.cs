using MyGame.Engine.DataStructures;
using MyGame.Engine.Serialization;
using MyGame.Engine.Utils;
using System;
using System.Collections.Generic;

namespace MyGame.Engine.GameState.Instants
{
    class Instant
    {
        private static Logger log = new Logger(typeof(Instant));

        public static void Update(Instant current, Instant next)
        {
            //drop the set of non-deserializedObjects, we are going to recalculate them
            next.objects = new SortedList<int, GameObject>(new IntegerComparer());

            IEnumerator<KeyValuePair<int, GameObject>> deserializedObjects = current.deserializedObjects.GetEnumerator();
            IEnumerator<KeyValuePair<int, GameObject>> objects = current.objects.GetEnumerator();

            bool deserializedHasNext = deserializedObjects.MoveNext();
            bool objectsHasNext = objects.MoveNext();

            while (deserializedHasNext || objectsHasNext)
            {
                KeyValuePair<int, GameObject> pairToUpdate;

                if (deserializedHasNext && objectsHasNext)
                {
                    if (deserializedObjects.Current.Key < objects.Current.Key)
                    {
                        pairToUpdate = deserializedObjects.Current;
                        deserializedHasNext = deserializedObjects.MoveNext();
                    }
                    else
                    {
                        pairToUpdate = objects.Current;
                        objectsHasNext = objects.MoveNext();
                    }
                }
                else if (deserializedHasNext)
                {
                    pairToUpdate = deserializedObjects.Current;
                    deserializedHasNext = deserializedObjects.MoveNext();
                }
                else if (objectsHasNext)
                {
                    pairToUpdate = objects.Current;
                    objectsHasNext = objects.MoveNext();
                }
                else
                {
                    throw new Exception("Imposible logic error");
                }

                if(!next.deserializedObjects.ContainsKey(pairToUpdate.Key))
                {
                    pairToUpdate.Value.CallUpdate(current.AsCurrent, next.AsNext);
                    //TODO: should these Add to instant operations be all in instant or all in game object?
                    //TODO: probably should go into game object, that would work well with game objects deciding if they advance
                    next.AddObject(pairToUpdate.Value);
                }
            }
        }

        private int instant;
        private SortedList<int, GameObject> deserializedObjects = new SortedList<int, GameObject>(new IntegerComparer());
        private SortedList<int, GameObject> objects = new SortedList<int, GameObject>(new IntegerComparer());

        public Instant(int instant)
        {
            this.instant = instant;
        }

        //TODO: re-evaluate warnings to maybe throwing exceptions
        public void AddDeserializedObject(GameObject obj)
        {
            if (obj.ID != null)
            {
                int id = (int)obj.ID;
                if (objects.ContainsKey(id))
                {
                    //TODO: We are deseralizing over an existing object.  Should this return an indicator?
                    objects.Remove(id);
                }
                if (deserializedObjects.ContainsKey(id))
                {
                    log.Warn("An object that is already contained in the deserialized set is being added again.");
                    deserializedObjects.Remove(id);
                }
                deserializedObjects.Add((int)obj.ID, obj);
            }
            else
            {
                throw new Exception("Objects in instants must have IDs");
            }
        }

        //TODO: re-evaluate warnings to maybe throwing exceptions
        public void AddObject(GameObject obj)
        {
            if (obj.ID != null)
            {
                int id = (int)obj.ID;
                if (!deserializedObjects.ContainsKey(id))
                {
                    if (objects.ContainsKey(id))
                    {
                        log.Warn("An object that is already contained in the object set is being added again.");
                        objects.Remove(id);
                    }
                    objects.Add((int)obj.ID, obj);
                }
                else
                {
                    //TODO: when updating objects, check if the next deserialized state exists so this never happens
                    log.Warn("An attempt was made to add an object that is already contained in the deserialized set to the object set.");
                }
            }
            else
            {
                throw new Exception("Objects in instants must have IDs");
            }
        }

        public bool ContainsAsDeserialized(GameObject obj)
        {
            return obj != null && obj.ID != null && deserializedObjects.ContainsKey((int)obj.ID) && deserializedObjects[(int)obj.ID] == obj;
        }

        public bool ContainsAsNonDeserialized(GameObject obj)
        {
            return obj != null && obj.ID != null && objects.ContainsKey((int)obj.ID) && objects[(int)obj.ID] == obj;
        }

        public bool Contains(GameObject obj)
        {
            return this.ContainsAsDeserialized(obj) || this.ContainsAsNonDeserialized(obj);
        }

        public GameObject GetObject(int id)
        {
            if(deserializedObjects.ContainsKey(id))
            {
                return deserializedObjects[id];
            }
            else if (objects.ContainsKey(id))
            {
                return objects[id];
            }
            else
            {
                return null;
            }
        }

        public CurrentInstant AsCurrent
        {
            get
            {
                return new CurrentInstant(this);
            }
        }

        public NextInstant AsNext
        {
            get
            {
                return new NextInstant(this);
            }
        }

        public override bool Equals(object obj)
        {
            if(obj != null && obj is Instant)
            {
                return instant == ((Instant)obj).instant;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return instant;
        }

        public int ID
        {
            get
            {
                return instant;
            }
        }

        public bool CheckIntegrety(TwoWayMap<int, GameObject> officialObjects)
        {
            foreach(KeyValuePair<int, GameObject> pair in objects)
            {
                if(officialObjects[pair.Key] != pair.Value)
                {
                    log.Error("Object key in instant does not match Object key in collection");
                    return false;
                }

                if (deserializedObjects.ContainsKey(pair.Key))
                {
                    log.Error("Object is in both deserialized and non-deserialized sets");
                    return false;
                }
            }

            foreach (KeyValuePair<int, GameObject> pair in deserializedObjects)
            {
                if (officialObjects[pair.Key] != pair.Value)
                {
                    log.Error("Object key in instant does not match Object key in collection");
                    return false;
                }

                if (objects.ContainsKey(pair.Key))
                {
                    log.Error("Object is in both deserialized and non-deserialized sets");
                    return false;
                }
            }
            return true;
        }
    }
}
