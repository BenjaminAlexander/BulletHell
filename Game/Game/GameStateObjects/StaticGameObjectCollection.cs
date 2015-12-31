using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGame.GameStateObjects.DataStuctures;
using Microsoft.Xna.Framework;


namespace MyGame.GameStateObjects
{
    public static class StaticGameObjectCollection
    {
        static GameObjectCollection gameObjectCollection;

        public static GameObjectCollection Collection
        {
            get { return gameObjectCollection; }
        }

        public static void Initialize(Vector2 worldSize)
        {
            gameObjectCollection = new GameObjectCollection(worldSize);
        }
    }
}
