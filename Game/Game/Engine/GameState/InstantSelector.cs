using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.Engine.GameState
{
    class InstantSelector
    {
        int instant;
        private InstantSelector(int instant)
        {
            this.instant = instant;
        }

        public int CurrentInstant
        {
            get
            {
                return this.instant;
            }
        }

        public int NextInstant
        {
            get
            {
                return this.instant + 1;
            }
        }

        public class InstantController : InstantSelector
        {
            public InstantController(int instant) : base(instant)
            {
            }

            public void SetInstant(int instant)
            {
                this.instant = instant;
            }

            public InstantSelector Selector
            {
                get
                {
                    return this;
                }
            }
        }
    }
}
