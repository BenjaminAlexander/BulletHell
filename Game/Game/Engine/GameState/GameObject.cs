using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.Engine.Reflection;
using MyGame.Engine.Serialization;
using static MyGame.Engine.GameState.GameObject;

namespace MyGame.Engine.GameState
{
    public abstract class GameObject
    {
        private static NewConstraintTypeFactory<GameObject> factory = new NewConstraintTypeFactory<GameObject>();

        internal static void AddType<DerivedType>() where DerivedType : GameObject, new()
        {
            factory.AddType<DerivedType>();
        }

        internal static GameObject Construct(int typeID)
        {
            return factory.Construct(typeID);
        }

        private List<AbstractField> fieldDefinitions = new List<AbstractField>();

        public GameObject()
        {

        }

        internal int TypeID
        {
            get
            {
                return factory.GetTypeID(this);
            }
        }

        internal void CopyFieldValues(GameObjectContainer current, GameObjectContainer next)
        {
            foreach (AbstractField field in fieldDefinitions)
            {
                field.CopyFieldValues(current, next);
            }
        }

        internal int SerializationSize(GameObjectContainer container)
        {
            int serializationSize = 0;
            foreach (AbstractField field in fieldDefinitions)
            {
                serializationSize = serializationSize + field.SerializationSize(container);
            }
            return serializationSize;
        }

        internal void Serialize(GameObjectContainer container, byte[] buffer, ref int bufferOffset)
        {
            foreach (AbstractField field in fieldDefinitions)
            {
                field.Serialize(container, buffer, ref bufferOffset);
            }
        }

        internal void Deserialize(GameObjectContainer container, byte[] buffer, ref int bufferOffset)
        {
            foreach (AbstractField field in fieldDefinitions)
            {
                field.Deserialize(container, buffer, ref bufferOffset);
            }
        }

        internal bool IsIdentical(GameObjectContainer container, GameObject other, GameObjectContainer otherContainer)
        {
            if(this.GetType().Equals(other.GetType()) && this.fieldDefinitions.Count == other.fieldDefinitions.Count)
            {
                for (int i = 0; i < this.fieldDefinitions.Count; i++)
                {
                    if (!this.fieldDefinitions[i].IsIdentical(container, other.fieldDefinitions[i], otherContainer))
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        public abstract void Update(CurrentContainer current, NextContainer next);

        //TODO: is NextContainer the right type for this?
        internal abstract void DefineFields(NextContainer nextContainer);

        public abstract class AbstractField
        {
            public AbstractField(GameObject owner)
            {
                owner.fieldDefinitions.Add(this);
            }

            internal abstract void CopyFieldValues(GameObjectContainer current, GameObjectContainer next);

            internal abstract int SerializationSize(GameObjectContainer container);

            internal abstract void Serialize(GameObjectContainer container, byte[] buffer, ref int bufferOffset);

            internal abstract void Deserialize(GameObjectContainer container, byte[] buffer, ref int bufferOffset);

            internal abstract bool IsIdentical(GameObjectContainer container, AbstractField other, GameObjectContainer otherContainer);
        }
    }
}
