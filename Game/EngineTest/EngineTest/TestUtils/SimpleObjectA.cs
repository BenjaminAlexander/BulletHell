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
    class SimpleObjectA : GameObject
    {
        Field<IntegerValue> integerMember;
        Field<Vector2Value> vector2Member;
        Field<FloatValue> floatMember;

        public SimpleObjectA()
        {
            integerMember = new Field<IntegerValue>(this);
            vector2Member = new Field<Vector2Value>(this);
            floatMember = new Field<FloatValue>(this);
            
        }

        public static GameObjectContainer Factory(int instant, int integer, Vector2 vector, float floatingPoint)
        {
            SimpleObjectA newObj = new SimpleObjectA();
            newObj.integerMember.InitialValue = integer;
            newObj.vector2Member.InitialValue = vector;
            newObj.floatMember.InitialValue = floatingPoint;
            GameObjectContainer container = new GameObjectContainer(newObj, instant);
            return container;
        }

        public static void AssertValuesEqual(SimpleObjectA expected, SimpleObjectA actual)
        {
            Assert.AreEqual(expected.integerMember.InitialValue, actual.integerMember.InitialValue);
            Assert.AreEqual(expected.floatMember.InitialValue, actual.floatMember.InitialValue);
            Assert.AreEqual(expected.vector2Member.InitialValue, actual.vector2Member.InitialValue);
        }

        public Vector2 Vector2Member(GameObjectContainer current)
        {
            return vector2Member[current];
        }

        public void Vector2Member(GameObjectContainer current, Vector2 value)
        {
            vector2Member[current] = value;
        }

        public override void Update(CurrentContainer current, NextContainer next)
        {
            this.vector2Member[next] = this.vector2Member[current] + new Vector2(1f);
        }
    }
}