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
        private List<AbstractField> fieldDefinitions = new List<AbstractField>();

        private void AddField(AbstractField field)
        {
            this.fieldDefinitions.Add(field);
        }

        internal List<AbstractField> FieldDefinitions
        {
            get
            {
                return new List<AbstractField>(fieldDefinitions);
            }
        }

        internal List<FieldValue> GetFieldValues(GameObjectContainer container)
        {
            List<FieldValue> values = new List<FieldValue>();
            foreach(AbstractField field in fieldDefinitions)
            {
                values.Add(field.GetValue(container));
            }

            return values;
        }

        internal void CopyFieldValues(GameObjectContainer current, GameObjectContainer next)
        {
            List<FieldValue> fieldValues = this.GetFieldValues(current);
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

        public abstract void Update(CurrentContainer current, NextContainer next);

        //TODO: add abstract method for field creation?


        public abstract class AbstractField
        {
            public AbstractField(GameObject owner)
            {
                owner.AddField(this);
            }

            //TODO: do we need all of these?
            //TODO: might need to remove this
            internal abstract FieldValue GetInitialField();

            internal abstract void SetInitialValue(GameObjectContainer container);

            internal abstract FieldValue GetValue(GameObjectContainer container);

            internal abstract void CopyFieldValues(GameObjectContainer current, GameObjectContainer next);

            internal abstract int SerializationSize(GameObjectContainer container);

            internal abstract void Serialize(GameObjectContainer container, byte[] buffer, ref int bufferOffset);

            internal abstract void Deserialize(GameObjectContainer container, byte[] buffer, ref int bufferOffset);
        }
    }
}
