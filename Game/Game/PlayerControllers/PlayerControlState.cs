using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MyGame.PlayerControllers
{
    class PlayerControlState
    {
        private Vector2 aimpoint = new Vector2(0);
        private Vector2 move = new Vector2(0);
        private Boolean fire = false;

        public PlayerControlState(Vector2 aimpoint, Vector2 move, Boolean fire)
        {
            this.aimpoint = aimpoint;
            this.move = move;
            this.fire = fire;
        }

    }
}
