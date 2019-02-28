using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.Engine.GameState;
using Microsoft.Xna.Framework;

namespace EngineTest.EngineTest.TestUtils
{
    class SimpleObjectB : GameObject
    {
        FloatField floatMember1;
        FloatField floatMember2;
        FloatField floatMember3;
        FloatField floatMember4;

        public SimpleObjectB()
        {
            floatMember1 = new FloatField(this);
            floatMember2 = new FloatField(this);
            floatMember3 = new FloatField(this);
            floatMember4 = new FloatField(this);
        }

        public static SimpleObjectB Factory(int instant, float floating1, float floating2, float floating3, float floating4)
        {
            SimpleObjectB simpleObject = new SimpleObjectB();
            simpleObject.floatMember1[instant] = floating1;
            simpleObject.floatMember2[instant] = floating2;
            simpleObject.floatMember3[instant] = floating3;
            simpleObject.floatMember4[instant] = floating4;
            return simpleObject;
        }

        public float FloatMember1(int instant)
        {
            return floatMember1[instant];
        }

        public float FloatMember2(int instant)
        {
            return floatMember2[instant];
        }

        public float FloatMember3(int instant)
        {
            return floatMember3[instant];
        }

        public float FloatMember4(int instant)
        {
            return floatMember4[instant];
        }

    }
}