﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.Engine.GameState;
using Microsoft.Xna.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyGame.Engine.GameState.Instants;
using MyGame.Engine.GameState.InstantObjectSet;
using MyGame.Engine.GameState.GameObjectUtils;
using MyGame.Engine.Serialization.DataTypes;

namespace EngineTest.EngineTest.TestUtils
{
    class SimpleObjectA : GameObject
    {
        StructField<SInteger> integerMember;
        StructField<SVector2> vector2Member;
        StructField<SFloat> floatMember;

        internal protected override void DefineFields(CreationToken creationToken)
        {
            integerMember = new StructField<SInteger>(creationToken);
            vector2Member = new StructField<SVector2>(creationToken);
            floatMember = new StructField<SFloat>(creationToken);
        }

        public static SimpleObjectA Factory(NextInstant next, int integer, Vector2 vector, float floatingPoint)
        {
            //TODO: clean up this factory buisness
            SimpleObjectA newObj = next.NewGameObject<SimpleObjectA>();
            newObj.integerMember[next] = integer;
            newObj.vector2Member[next] = vector;
            newObj.floatMember[next] = floatingPoint;
            return newObj;
        }

        public Vector2 Vector2Member(CurrentInstant current)
        {
            return vector2Member[current];
        }

        public void Vector2Member(NextInstant current, Vector2 value)
        {
            vector2Member[current] = value;
        }

        internal protected override void Update(CurrentInstant current, NextInstant next)
        {
            this.vector2Member[next] = this.vector2Member[current] + new Vector2(1f);
        }
    }
}