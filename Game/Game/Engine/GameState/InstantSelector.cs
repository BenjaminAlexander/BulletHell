using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.Engine.GameState
{
    class InstantSelector
    {
        int readInstant;
        int writeInstant;

        private InstantSelector()
        {
            this.readInstant = 0;
            this.writeInstant = 1;
        }

        public int ReadInstant
        {
            get
            {
                return this.readInstant;
            }
        }

        public int WriteInstant
        {
            get
            {
                return this.writeInstant;
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

            public InstantSelector Selector
            {
                get
                {
                    return this;
                }
            }

            public void SetReadWriteInstant(int readInstant)
            {
                this.SetReadInstant(readInstant);
                this.SetWriteInstant(readInstant + 1);
            }

            public void AdvanceReadWriteInstant()
            {
                this.SetReadWriteInstant(this.ReadInstant + 1);
            }
        }
    }
}
