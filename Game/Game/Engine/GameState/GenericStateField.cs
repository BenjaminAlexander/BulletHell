using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.Engine.GameState
{
    abstract class GenericStateField<T> : StateField
    {
        public static implicit operator T(GenericStateField<T> field)
        {
            return field.Value;
        }

        private T value;

        public GenericStateField(StateAtInstant state, T value) : base(state)
        {
            this.value = value;
        }

        public T Value
        {
            get
            {
                return this.value;
            }
            set
            {
                this.value = value;
            }
        }
    }
}
