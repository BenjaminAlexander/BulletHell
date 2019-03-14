﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.Engine.Serialization;

namespace MyGame.Engine.GameState
{
    class IntegerField : GenericField<int, SerializableInteger>
    {
        public IntegerField(GameObject obj) : base(obj)
        {
        }
    }
}
