using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyGame.IO
{
    public abstract class IOEvent
    {
        public abstract bool hasOccured(IOState ioState);

        public override bool Equals(object obj)
        {
            return base.Equals(obj) || obj.GetType() == this.GetType();
        }

        public override int GetHashCode()
        {
            return 0;
        }
    }
}
