using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
namespace MyGame.Networking
{
    class VectorMessage : TCPMessage
    {
        private Vector2 size;
        public Vector2 Size
        {
            get { return size; }
        }

        public VectorMessage(Vector2 size)
        {
            this.size = size;
            this.Append(size);
        }

        public VectorMessage(byte[] b, int lenght)
            : base(b, lenght)
        {
            this.ResetReader();
            size = this.ReadVector2();
            this.AssertMessageEnd();
        }
    }
}
