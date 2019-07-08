using MyGame.Engine.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace MyGame.Engine.GameState.Instants
{
    class InstantGameObjectSet : IEnumerable<GameObject>
    {
        private static Logger log = new Logger(typeof(InstantGameObjectSet));

        private GameObjectSet deserializedObjects = new GameObjectSet();
        private GameObjectSet objects = new GameObjectSet();

        //TODO: re-evaluate warnings to maybe throwing exceptions
        public void AddDeserializedObject(GameObject obj)
        {
            if (obj.ID != null)
            {
                int id = (int)obj.ID;
                if (objects.Contains(obj))
                {
                    //TODO: We are deseralizing over an existing object.  Should this return an indicator?
                    objects.Remove(obj);
                }
                if (deserializedObjects.Contains(obj))
                {
                    log.Warn("An object that is already contained in the deserialized set is being added again.");
                    deserializedObjects.Remove(obj);
                }
                deserializedObjects.Add(obj);
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
                if (!deserializedObjects.Contains(obj))
                {
                    if (objects.Contains(obj))
                    {
                        log.Warn("An object that is already contained in the object set is being added again.");
                        objects.Remove(obj);
                    }
                    objects.Add(obj);
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
            return obj != null && obj.ID != null && deserializedObjects.Contains(obj);
        }

        public bool ContainsAsNonDeserialized(GameObject obj)
        {
            return obj != null && obj.ID != null && objects.Contains(obj);
        }

        public bool Contains(GameObject obj)
        {
            return ContainsAsDeserialized(obj) || ContainsAsNonDeserialized(obj);
        }

        public void DropNonDeserializedObjects()
        {
            objects = new GameObjectSet();
        }

        public bool CheckIntegrety(GameObjectSet globalSet)
        {
            if (!objects.CheckIntegrety(globalSet))
            {
                log.Error("objects collection failed its integerety check");
                return false;
            }

            if (!deserializedObjects.CheckIntegrety(globalSet))
            {
                log.Error("deserializedObjects collection failed its integerety check");
                return false;
            }

            foreach (GameObject obj in objects)
            {
                if(deserializedObjects.Contains(obj))
                {
                    log.Error("An object is contained in both deserializedObjects and objects collection");
                    return false;
                }
            }

            foreach (GameObject obj in deserializedObjects)
            {
                if (objects.Contains(obj))
                {
                    log.Error("An object is contained in both deserializedObjects and objects collection");
                    return false;
                }
            }

            return true;
        }

        public IEnumerator<GameObject> GetEnumerator()
        {
            return GameObjectSet.Union(deserializedObjects, objects).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public GameObject this[int id]
        {
            get
            {
                if (deserializedObjects.Contains(id))
                {
                    return deserializedObjects[id];
                }
                else if (objects.Contains(id))
                {
                    return objects[id];
                }
                else
                {
                    throw new Exception("ObjectSet does not contain that id");
                }
            }
        }
    }
}
