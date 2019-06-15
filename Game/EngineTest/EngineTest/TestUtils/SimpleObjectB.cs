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
        Field<FloatValue> floatMember1;
        Field<FloatValue> floatMember2;
        Field<FloatValue> floatMember3;
        Field<FloatValue> floatMember4;

        public SimpleObjectB()
        {
            floatMember1 = new Field<FloatValue>(this);
            floatMember2 = new Field<FloatValue>(this);
            floatMember3 = new Field<FloatValue>(this);
            floatMember4 = new Field<FloatValue>(this);
        }

        public static GameObjectContainer Factory(int instant, float floating1, float floating2, float floating3, float floating4)
        {
            SimpleObjectB newObj = new SimpleObjectB();
            newObj.floatMember1.InitialValue = floating1;
            newObj.floatMember2.InitialValue = floating2;
            newObj.floatMember3.InitialValue = floating3;
            newObj.floatMember4.InitialValue = floating4;
            GameObjectContainer container = new GameObjectContainer(newObj, instant);
            return container;
        }

        public static void AssertValuesEqual(SimpleObjectB expected, SimpleObjectB actual)
        {
            Assert.AreEqual(expected.floatMember1.InitialValue, actual.floatMember1.InitialValue);
            Assert.AreEqual(expected.floatMember2.InitialValue, actual.floatMember2.InitialValue);
            Assert.AreEqual(expected.floatMember3.InitialValue, actual.floatMember3.InitialValue);
            Assert.AreEqual(expected.floatMember4.InitialValue, actual.floatMember4.InitialValue);
        }

        public float FloatMember1(GameObjectContainer current)
        {
            return floatMember1.GetValue(current);
        }

        public float FloatMember2(GameObjectContainer current)
        {
            return floatMember2.GetValue(current);
        }

        public float FloatMember3(GameObjectContainer current)
        {
            return floatMember3.GetValue(current);
        }

        public float FloatMember4(GameObjectContainer current)
        {
            return floatMember4.GetValue(current);
        }

        public override void Update(CurrentContainer current, NextContainer next)
        {
            throw new NotImplementedException();
        }
    }
}