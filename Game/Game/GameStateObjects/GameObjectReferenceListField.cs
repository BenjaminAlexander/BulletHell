using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGame.GameStateObjects.PhysicalObjects;
using Microsoft.Xna.Framework;
using MyGame.Networking;
using MyGame.GameStateObjects.DataStuctures;

namespace MyGame.GameStateObjects
{
    public class GameObjectReferenceListField<T> : GenericGameObjectField<List<GameObjectReference<T>>> where T : GameObject
    {
        private GameObjectCollection collection;

        public GameObjectReferenceListField(GameObject obj, GameObjectCollection collection) : base(obj, new List<GameObjectReference<T>>())
        {
            this.collection = collection;
        }

        public override void ApplyMessage(GameObjectUpdate message)
        {
            //this.simulationValue = message.ReadGameObjectReferenceList<T>(this.collection);

            var rtn = new List<GameObjectReference<T>>();
            int count = message.ReadInt();
            for (int i = 0; i < count; i++)
            {
                GameObjectReference<T> rf = new GameObjectReference<T>(message.ReadInt(), collection);
                rtn.Add(rf);
            }
            this.simulationValue = rtn;
        }

        public override GameObjectUpdate ConstructMessage(GameObjectUpdate message)
        {
            //message.Append(this.simulationValue)

            message.Append(this.simulationValue.Count);
            foreach (GameObjectReference<T> obj in this.simulationValue)
            {
                message.Append(obj.ID);
            }
            return message;
        }

        public override void Interpolate(float smoothing)
        {
            this.drawValue = this.simulationValue;
        }

        public List<T> GetList()
        {
            List <GameObjectReference<T>> referenceList = this.Value;
            List<T> dereferencedObjects = new List<T>();

            foreach (GameObjectReference<T> reference in referenceList)
            {
                if (reference.CanDereference())
                {
                    dereferencedObjects.Add(reference);
                }

            }
            return dereferencedObjects;
        }

        public void Add(T obj)
        {
            this.Value.Add(new GameObjectReference<T>(obj, this.collection));
        }

        public T this[int index]
        {
            get
            {
                return this.Value[index];
            }
        }
    }
}
