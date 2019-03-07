using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.Engine.GameState
{
    abstract class GenericMetaField<T> : GameObject.Field
    {
        private Dictionary<int, T> fieldAtInstant = new Dictionary<int, T>();

        public GenericMetaField(GameObject obj) : base(obj)
        {
        }

        public T this[int instant]
        {
            get
            {
                return fieldAtInstant[instant];
            }

            set
            {
                fieldAtInstant[instant] = value;
            }
        }

        protected override void Copy(GameObject.Field other, int instant)
        {
            this[instant] = ((GenericMetaField<T>)other)[instant];
        }
    }
}
