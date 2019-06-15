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
        private List<Field> fieldDefinitions = new List<Field>();

        private void AddField(Field field)
        {
            this.fieldDefinitions.Add(field);
        }

        internal List<Field> FieldDefinitions
        {
            get
            {
                return new List<Field>(fieldDefinitions);
            }
        }

        public abstract void Update(CurrentContainer current, NextContainer next);

        //TODO: add abstract method for field creation?


        public abstract class Field
        {
            public Field(GameObject owner)
            {
                owner.AddField(this);
            }

            internal abstract FieldValue GetInitialField();
        }
    }
}
