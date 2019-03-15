using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.Engine.GameState
{
    class InstantSelector
    {
        int? readInstant = null;
        int? writeInstant = null;

        private InstantSelector()
        {
            this.readInstant = 0;
            this.writeInstant = 1;
        }

        public int ReadInstant
        {
            get
            {
                return (int)this.readInstant;
            }
        }

        public int WriteInstant
        {
            get
            {
                return (int)this.writeInstant;
            }
        }

        public class InstantController : InstantSelector
        {
            public InstantController() : base()
            {
            }

            public void SetReadInstant(int instant)
            {
                this.readInstant = instant;
            }

            public void SetWriteInstant(int instant)
            {
                this.writeInstant = instant;
            }

            public void DisableWrite()
            {
                this.writeInstant = null;
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
