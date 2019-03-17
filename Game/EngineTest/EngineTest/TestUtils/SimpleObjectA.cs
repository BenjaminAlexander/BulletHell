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

        public static SimpleObjectA Factory(int integer, Vector2 vector, float floatingPoint)
        {
            SimpleObjectA simpleObject = new SimpleObjectA();
            simpleObject.integerMember.Value = integer;
            simpleObject.vector2Member.Value = vector;
            simpleObject.floatMember.Value = floatingPoint;
            return simpleObject;
        }

        public static SimpleObjectA Factory(InstantSelector instantSelector, int integer, Vector2 vector, float floatingPoint)
        {
            SimpleObjectA simpleObject = new SimpleObjectA();
            //TODO: set up instantSelector when creating objects
            simpleObject.InstantSelector = instantSelector;
            simpleObject.integerMember.Value = integer;
            simpleObject.vector2Member.Value = vector;
            simpleObject.floatMember.Value = floatingPoint;
            return simpleObject;
        }

        public static void AssertValuesEqual(SimpleObjectA expected, SimpleObjectA actual)
        {
            Assert.AreEqual(expected.integerMember.Write, actual.integerMember.Write);
            Assert.AreEqual(expected.floatMember.Write, actual.floatMember.Write);
            Assert.AreEqual(expected.vector2Member.Write, actual.vector2Member.Write);
        }

        public int IntegerMember()
        {
            return integerMember.Value;
        }

        public void IntegerMember(int value)
        {
            integerMember.Value = value;
        }

        public Vector2 Vector2Member()
        {
            return vector2Member.Value;
        }

        public float FloatMember()
        {
            return floatMember.Value;
        }

        public override void Update()
        {
            base.Update();
            this.vector2Member.Value = this.vector2Member.Value + new Vector2(1f);
        }
    }
}