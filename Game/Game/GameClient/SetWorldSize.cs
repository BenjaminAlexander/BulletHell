using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.GameClient;

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

        public SetWorldSize(byte[] b)
            : base(b)
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
