using MyGame.Engine.Reflection;
using MyGame.Engine.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.Engine.GameState
{
    class InstantCollection : SerializedCollection<GameObject>
    {
        private int instant;
        private NewConstraintTypeFactory<GameObject> factory;

        public int Instant
        {
            get
            {
                return instant;
            }
        }

        public InstantCollection(NewConstraintTypeFactory<GameObject> factory, int instant) : base(new TypeSerializer<GameObject>(factory))
        {
            this.instant = instant;
            this.factory = factory;
        }

        public InstantCollection NextInstant()
        {
            InstantCollection next = new InstantCollection(factory, this.instant +1);
            foreach (GameObject obj in this)
            {
                GameObject nextObject = obj.NextInstant();
                if(nextObject != null)
                {
                    next.Add(nextObject);
                }
            }
            return next;
        }
    }
}
