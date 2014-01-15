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
            get { return gameState; }
        }

        public GameObject(GameState gameState)
        {
            this.gameState = gameState;
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

        public virtual Boolean IsPhysicalObject
        {
            get { return false; }
        }
    }
}
