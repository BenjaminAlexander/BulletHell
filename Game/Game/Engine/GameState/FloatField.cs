﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.Engine.Serialization;

namespace MyGame.Engine.GameState
{
    class FloatField : GenericMetaField<SerializableFloat>
    {
        public FloatField(GameObject obj) : base(obj)
        {
        }
    }
}