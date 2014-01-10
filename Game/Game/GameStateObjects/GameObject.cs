using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MyGame.GameStateObjects
{
    public abstract class GameObject
    {
        GameState gameState = null;

        public GameState GameState
        {
            set { gameState = value; }
            get { return gameState; }
        }

        protected abstract void UpdateSubclass(GameTime gameTime);

        public void Update(GameTime gameTime)
        {
            if (gameState != null)
            {
                this.UpdateSubclass(gameTime);
            }
        }

        public abstract void Draw(GameTime gameTime, DrawingUtils.MyGraphicsClass graphics);

        public virtual Boolean IsFlyingGameObject
        {
            get { return false; }
        }
    }
}
