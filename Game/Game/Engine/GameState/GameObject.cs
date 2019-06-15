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

        public abstract void Update(CurrentContainer current, NextContainer next);

        //TODO: add abstract method for field creation?


        public abstract class AbstractField
        {
            public AbstractField(GameObject owner)
            {
                owner.AddField(this);
            }

            internal abstract FieldValue GetInitialField();
        }
    }
}
