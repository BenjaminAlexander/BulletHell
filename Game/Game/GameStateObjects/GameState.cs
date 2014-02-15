using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.Utils;
using MyGame.DrawingUtils;
using MyGame.Geometry;
using MyGame.GameStateObjects.DataStuctures;

namespace MyGame.GameStateObjects
{
    public class GameState
    {
        private List<IDrawableUpdatable> localUpdateable = new List<IDrawableUpdatable>();


        

        public GameState(Vector2 worldSize, Camera camera)
        {
            
            
        }

        public void Update(GameTime gameTime)
        {
            foreach (IDrawableUpdatable obj in localUpdateable)
            {
                obj.Update(gameTime);
            }

            foreach (GameObject obj in StaticGameObjectCollection.Collection.GetMasterList().GetList<GameObject>())
            {
                obj.Update(gameTime);
            }
            StaticGameObjectCollection.Collection.CleanUp();
        }

        public void Draw(GameTime gameTime, MyGraphicsClass graphics)
        {
            graphics.BeginWorld();
            foreach (GameObject obj in StaticGameObjectCollection.Collection.GetMasterList().GetList<GameObject>())
            {
                obj.Draw(gameTime, graphics);
            }
            graphics.EndWorld();

            graphics.Begin(Matrix.Identity);
            foreach (IDrawableUpdatable obj in localUpdateable)
            {
                obj.Draw(gameTime, graphics);
            }
            graphics.End();
        }

        public Boolean AddLocalUpdateable(IDrawableUpdatable obj)
        {
            if (obj != null)//&& !addList.Contains(obj) && !gameObjects.Contains(obj))
            {
                localUpdateable.Add(obj);
                return true;
            }
            return false;
        }
    }
}
