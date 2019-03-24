using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.Engine.GameState;
using Microsoft.Xna.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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

        public static SubType Factory<SubType>(float floating1, float floating2, float floating3, float floating4) where SubType : SimpleObjectB, new()
        {
            SubType newObj = GameObject.Factory<SubType>();
            newObj.floatMember1.Write = floating1;
            newObj.floatMember2.Write = floating2;
            newObj.floatMember3.Write = floating3;
            newObj.floatMember4.Write = floating4;
            return newObj;
        }

        public static void AssertValuesEqual(SimpleObjectB expected, SimpleObjectB actual)
        {
            Assert.AreEqual(expected.floatMember1.Write, actual.floatMember1.Write);
            Assert.AreEqual(expected.floatMember2.Write, actual.floatMember2.Write);
            Assert.AreEqual(expected.floatMember3.Write, actual.floatMember3.Write);
            Assert.AreEqual(expected.floatMember4.Write, actual.floatMember4.Write);
        }

        public float FloatMember1()
        {
            return floatMember1.Value;
        }

        public float FloatMember2()
        {
            return floatMember2.Value;
        }

        public float FloatMember3()
        {
            return floatMember3.Value;
        }

        public float FloatMember4()
        {
            return floatMember4.Value;
        }

    }
}