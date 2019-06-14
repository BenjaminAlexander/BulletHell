using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.Engine.Reflection;
using MyGame.Engine.Serialization;

namespace MyGame.Engine.GameState
{
    partial class GameObject
    {
        private List<Field> fieldDefinitions = new List<Field>();

        private void AddField(Field field)
        {
            this.fieldDefinitions.Add(field);
        }

        public void GetInitialFields(GameObjectContainer current)
        {
            foreach(Field field in this.fieldDefinitions)
            {
                current.AddField(field, field.GetInitialField());
            }
        }

        //TODO: make current and next different types
        public virtual void Update(GameObjectContainer current, GameObjectContainer next)
        {

        }
    }
}
