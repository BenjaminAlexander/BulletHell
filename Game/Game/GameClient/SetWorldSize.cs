using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.GameClient;
using System.Net.Sockets;

namespace MyGame.GameClient
{
    public class SetWorldSize : ClientUpdate
    {
        private Vector2 size;
        public Vector2 Size
        {
            get { return size; }
        }

        public SetWorldSize(GameTime currentGameTime, Vector2 size) : base(currentGameTime)
        {
            this.size = size;
            this.Append(size);
        }

        public SetWorldSize(NetworkStream networkStream) : base(networkStream)
        {
            this.ResetReader();
            size = this.ReadVector2();
            this.AssertMessageEnd();
        }

        public override void Apply(ClientGame game, GameTime gameTime)
        {
            //TODO: this smells bad
        }
    }
}
