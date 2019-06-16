﻿using MyGame.Engine.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MyGame.Engine.GameState.GameObject;

namespace MyGame.Engine.GameState
{
    public class CurrentContainer
    {
        GameObjectContainer container;

        internal CurrentContainer(GameObjectContainer container)
        {
            this.container = container;
        }

        internal GameObjectContainer Container
        {
            get
            {
                return container;
            }
        }
    }
}
