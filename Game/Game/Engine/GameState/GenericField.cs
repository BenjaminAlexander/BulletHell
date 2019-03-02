using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.Engine.GameState
{
    abstract class GenericMetaField<T> : GameObject.Field
    {
        private Dictionary<Instant, T> fieldAtInstant = new Dictionary<Instant, T>();

        public GenericMetaField(GameObject obj) : base(obj)
        {
        }

        public T this[Instant instant]
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
    }
}
