using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace MyGame.IO.Events
{
    public class KeyDown : IOEvent
    {
        Keys key;

        public KeyDown(Keys key)
        {
            this.key = key;
        }

        public override bool hasOccured(IOState ioState)
        {
            return ioState.isKeyDown(key);
        }

        public override bool Equals(object obj)
        {
            return obj.GetType() == this.GetType() && ((KeyDown)obj).key == this.key;
        }

        public override int GetHashCode()
        {
            return key.GetHashCode();
        }
    }
}