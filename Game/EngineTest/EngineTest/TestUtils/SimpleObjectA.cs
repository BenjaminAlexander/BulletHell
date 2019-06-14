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
        IntegerField integerMember;
        Vector2Field vector2Member;
        FloatField floatMember;

        public SimpleObjectA()
        {
            integerMember = new IntegerField(this);
            vector2Member = new Vector2Field(this);
            floatMember = new FloatField(this);
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
            return vector2Member.GetValue(current);
        }

        public void Vector2Member(GameObjectContainer current, Vector2 value)
        {
            vector2Member.SetValue(current, value);
        }

        public override void Update(GameObjectContainer current, GameObjectContainer next)
        {
            base.Update(current, next);
            Vector2 newVector = this.vector2Member.GetValue(current) + new Vector2(1f);
            this.vector2Member.SetValue(next, newVector);
        }
    }
}