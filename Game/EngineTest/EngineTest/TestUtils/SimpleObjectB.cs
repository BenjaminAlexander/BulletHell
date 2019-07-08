using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.Engine.GameState;
using Microsoft.Xna.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyGame.Engine.GameState.FieldValues;
using MyGame.Engine.GameState.Instants;

namespace EngineTest.EngineTest.TestUtils
{
    class SimpleObjectB : GameObject
    {
        Field<FloatValue> floatMember1;
        Field<FloatValue> floatMember2;
        Field<FloatValue> floatMember3;
        Field<FloatValue> floatMember4;

        internal override void DefineFields(InitialInstant instant)
        {
            floatMember1 = new Field<FloatValue>(instant);
            floatMember2 = new Field<FloatValue>(instant);
            floatMember3 = new Field<FloatValue>(instant);
            floatMember4 = new Field<FloatValue>(instant);
        }

        public static SimpleObjectB Factory(Instant instant, float floating1, float floating2, float floating3, float floating4)
        {
            SimpleObjectB newObj = instant.NewGameObject<SimpleObjectB>();
            newObj.floatMember1[instant.AsNext] = floating1;
            newObj.floatMember2[instant.AsNext] = floating2;
            newObj.floatMember3[instant.AsNext] = floating3;
            newObj.floatMember4[instant.AsNext] = floating4;
            return newObj;
        }

        public float FloatMember1(CurrentInstant current)
        {
            return floatMember1[current];
        }

        public float FloatMember2(CurrentInstant current)
        {
            return floatMember2[current];
        }

        public float FloatMember3(CurrentInstant current)
        {
            return floatMember3[current];
        }

        public float FloatMember4(CurrentInstant current)
        {
            return floatMember4[current];
        }

        public override void Update(CurrentInstant current, NextInstant next)
        {
            throw new NotImplementedException();
        }
    }
}