using MyGame.Engine.DataStructures;
using MyGame.Engine.Serialization;
using MyGame.Engine.Utils;
using System;
using System.Collections.Generic;

namespace MyGame.Engine.GameState.Instants
{
    //TODO: add code to send list of valid object IDs
    class Instant
    {
        private static Logger log = new Logger(typeof(Instant));

        public static void Update(Instant current, Instant next)
        {
            //drop the set of non-deserializedObjects, we are going to recalculate them
            next.objectSet.DropNonDeserializedObjects();

            foreach(GameObject obj in current.objectSet)
            {
                if(!next.objectSet.ContainsAsDeserialized(obj))
                {
                    obj.CallUpdate(current.AsCurrent, next.AsNext);
                    //TODO: should these Add to instant operations be all in instant or all in game object?
                    //TODO: probably should go into game object, that would work well with game objects deciding if they advance
                    next.AddObject(obj);
                }
            }
        }

        private int instant;
        private InstantGameObjectSet objectSet = new InstantGameObjectSet();
        private GameObjectSet globalSet = null;

        public Instant(int instant, GameObjectSet globalSet)
        {
            this.instant = instant;
            this.globalSet = globalSet;
        }

        //TODO: remove this method
        public void AddDeserializedObject(GameObject obj)
        {
            objectSet.AddDeserializedObject(obj);
        }

        //TODO: remove this method
        public void AddObject(GameObject obj)
        {
            objectSet.AddObject(obj);
        }

        public GameObject GetObject(int id)
        {
            return objectSet[id];
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
                return instant == ((Instant)obj).instant && this.globalSet == ((Instant)obj).globalSet;
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

        public bool CheckIntegrety()
        {
            if(!objectSet.CheckIntegrety(globalSet))
            {
                log.Error("objects collection failed its integerety check");
                return false;
            }
            return true;
        }

        public bool CheckContainsIntegrety(GameObject obj)
        {
            if (obj.IsInstantDeserialized(this))
            {
                if (!objectSet.ContainsAsDeserialized(obj))
                {
                    log.Error("deserializedObjects collection does not contain the object");
                    return false;
                }
            }
            else
            {
                if (!objectSet.ContainsAsNonDeserialized(obj))
                {
                    log.Error("Objects collection does not contain the object");
                    return false;
                }
            }
            return true;
        }

        public int SerializationSize(GameObject obj)
        {
            return sizeof(int) + obj.SerializationSize(this);
        }

        public void Serialize(GameObject obj, byte[] buffer, ref int bufferOffset)
        {
            Serialization.Utils.Write((int)obj.ID, buffer, ref bufferOffset);
            obj.Serialize(this, buffer, ref bufferOffset);
        }

        //TODO: change return type to void
        public GameObject Deserialize(byte[] buffer, ref int bufferOffset)
        {
            int objectId = Serialization.Utils.ReadInt(buffer, ref bufferOffset);

            GameObject obj = globalSet.GetGameObject(objectId, this, buffer, bufferOffset);
            //TODO: do something with the return value
            obj.Deserialize(this, buffer, ref bufferOffset);
            this.AddDeserializedObject(obj);
            return obj;
        }

        //TODO: don't call GameObject.Const
        internal SubType NewGameObject<SubType>() where SubType : GameObject, new()
        {
            SubType newObject = globalSet.NewGameObject<SubType>(this);
            this.AddObject(newObject);
            return newObject;
        }
    }
}
