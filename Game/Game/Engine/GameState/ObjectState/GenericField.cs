using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.Engine.GameState.ObjectState
{
    abstract class GenericField<T> : ObjectState.Field
    {
        private T value;

        public GenericField(ObjectState obj) : base(obj)
        {
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
