using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyGame.GameStateObjects
{
    public interface IDrawableUpdatable: MyGame.GameStateObjects.IUpdateable, MyGame.GameStateObjects.IDrawable
    {
    }
}
