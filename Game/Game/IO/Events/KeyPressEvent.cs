using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace MyGame.IO.Events
{
    public class KeyPressEvent : IOEvent
    {
        Keys key;

        public KeyPressEvent(Keys key)
        {
            this.key = key;
        }

        public override bool hasOccured(IOState ioState)
        {
            return ioState.isKeyPressed(key);
        }

        public override bool Equals(object obj)
        {
            return obj.GetType() == this.GetType() && ((KeyPressEvent)obj).key == this.key;
        }

        public override int GetHashCode()
        {
            return key.GetHashCode();
        }
    }
}
