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
    abstract partial class GameObject
    {
        private List<Field> fieldDefinitions = new List<Field>();

        private void AddField(Field field)
        {
            this.fieldDefinitions.Add(field);
        }

        public List<Field> FieldDefinitions
        {
            get
            {
                return new List<Field>(fieldDefinitions);
            }
        }

        //TODO: make current and next different types
        public abstract void Update(GameObjectContainer current, GameObjectContainer next);

        //TODO: add abstract method for field creation?
    }
}
