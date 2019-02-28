using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.Engine.GameState
{
    class GameObject
    {
        //message:
        //objectID,TypeID,InstantID,data...

        //MetaField:
        //abstract buffer Serialize(instant)
        //abstract void Deserialize(buff)

        //MetaField<T>
        //Dictionary<int, T> fieldAtInstant
        //buffer Serialize(instant) - convert T to bytes
        //void Deserialize(buff) - stores T in fieldAtInstant
        //MetaField<T>[instant] - return value

        //Get rid of StateAtInstant

        //MetaField position;
        List<StateMetaField> metaFields = new List<StateMetaField>();

        int stateSize = 0;
        public int StateSize
        {
            get
            {
                return stateSize;
            }
        }

        //Vector2MetaField position;
        //Dictionary<int, Vector2> position
        public GameObject()
        {
            //add meta fields
            //position = new Vector2MetaField(this);
        }

        //TODO: set access modifiers so only StateMetaField can call this
        public int AddField(StateMetaField metaField)
        {
            int bufferAddress = stateSize;
            stateSize = stateSize + metaField.Size;
            metaFields.Add(metaField);
            return bufferAddress;
        }

        public byte[] Serialize(int instant)
        {
            byte[] buffer = new byte[this.stateSize];
            int position = 0;
            foreach (StateMetaField field in metaFields)
            {
                field.Serialize(instant, buffer, position);
                position = position + field.Size;
            }
            return buffer;
        }

        public void Deserialize(int instant, byte[] buffer)
        {
            if (buffer.Length != this.stateSize)
            {
                throw new Exception("Buffer length does not match expected state length");
            }

            int position = 0;
            foreach (StateMetaField field in metaFields)
            {
                field.Deserialize(instant, buffer, position);
                position = position + field.Size;
            }
        }


        //Vector2 pos = position[state]

        /*
         * Deserialize(buffer)
         *  read instant from buffer
         *  for each metafield...
         *      Value = metafield.deserialize(buffer)
         *      X state[metafield] = value
         *      metafield[state] = value     (
         *  states[instant] = state
         *  
         *  
         *  metafield[state] = value
         *      
         *  
         */


    }
}
