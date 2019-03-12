using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.Engine.Serialization;

namespace MyGame.Engine.GameState
{
    class StateAtInstant : Serializable
    {
        List<GameObject.Field> fields;
        Dictionary<GameObject.Field, Serializable> fieldsToValues = new Dictionary<GameObject.Field, Serializable>();

        public StateAtInstant(List<GameObject.Field> fields)
        {
            this.fields = fields;
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

        public int SerializationSize
        {
            get
            {
                int sum = 0;
                foreach (GameObject.Field field in fields)
                {
                    sum = sum + fieldsToValues[field].SerializationSize;
                }
                return sum;
            }
        }

        public void Deserialize(byte[] buffer, ref int bufferOffset)
        {
            foreach (GameObject.Field field in fields)
            {
                fieldsToValues[field].Deserialize(buffer, ref bufferOffset);
            }
        }

        public void Serialize(byte[] buffer, int bufferOffset)
        {
            foreach (GameObject.Field field in fields)
            {
                fieldsToValues[field].Serialize(buffer, bufferOffset);
                bufferOffset = bufferOffset + fieldsToValues[field].SerializationSize;
            }
        }
    }
}
