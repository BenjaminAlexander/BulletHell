﻿using MyGame.Engine.GameState.GameObjectFactory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MyGame.Engine.GameState.TypeManager;

namespace MyGame.Engine.GameState.InstantObjectSet
{
    interface InstantTypeSetInterface
    {
        ObjectTypeFactoryInterface NewObjectTypeFactory(int nextInstantId);
        void Add(GameObject obj);

        TypeMetadataInterface GetMetaData
        {
            get;
        }

        int InstantID
        {
            get;
        }
    }
}
