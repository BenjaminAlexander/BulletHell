using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.Utils;

namespace MyGame.GameStateObjects.Ships
{
    public class NPCShip : Ship
    {
        private Vector2 followPoint;

        public Vector2 FollowPoint
        {
            get { return followPoint; }
            set { followPoint = value; }
        }

        public NPCShip(Vector2 position, Vector2 followPoint) : base(position)
        {
            this.FollowPoint = followPoint;
        }

        protected override void UpdateSubclass(GameTime gameTime)
        {
            
            float secondsElapsed = gameTime.ElapsedGameTime.Milliseconds/1000.0f;

            TurnTowards(gameTime, FollowPoint);
            Acceleration = 100;
            base.UpdateSubclass(gameTime);
        }

        public override void Draw(GameTime gameTime, DrawingUtils.MyGraphicsClass graphics)
        {
            graphics.DrawSolidRectangle(Position, new Vector2(90, 30), new Vector2(45, 15), Direction, Color.Red, 1);
        }

        public override Boolean IsNPCShip
        {
            get { return true; }
        }
    }
}
