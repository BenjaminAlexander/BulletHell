using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.Engine.Serialization;

namespace MyGame.Engine.GameState
{
    class StateAtInstant
    {
        Dictionary<GameObject.Field, Serializable> fieldsToValues = new Dictionary<GameObject.Field, Serializable>();

        public StateAtInstant(List<GameObject.Field> fields)
        {
            foreach(GameObject.Field field in fields)
            {
                fieldsToValues[field] = field.DefaultValue();
            }
        }

        public Serializable this[GameObject.Field field]
        {
            get
            {
                return fieldsToValues[field];
            }

            set
            {
                fieldsToValues[field] = value;
            }
        }
    }
}
