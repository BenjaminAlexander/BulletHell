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

        public static SubType Factory<SubType>(InstantSelector instantSelector, int integer, Vector2 vector, float floatingPoint) where SubType : SimpleObjectA, new()
        {
            SubType newObj = GameObject.Factory<SubType>(instantSelector);
            newObj.integerMember.Write = integer;
            newObj.vector2Member.Write = vector;
            newObj.floatMember.Write = floatingPoint;
            return newObj;
        }

        public static void AssertValuesEqual(SimpleObjectA expected, SimpleObjectA actual)
        {
            Assert.AreEqual(expected.integerMember.Read, actual.integerMember.Read);
            Assert.AreEqual(expected.floatMember.Read, actual.floatMember.Read);
            Assert.AreEqual(expected.vector2Member.Read, actual.vector2Member.Read);
        }

        public Vector2 Vector2Member()
        {
            return vector2Member.Value;
        }

        public void Vector2Member(Vector2 value)
        {
            vector2Member.Value = value;
        }

        protected override void Update()
        {
            base.Update();
            this.vector2Member.Value = this.vector2Member.Value + new Vector2(1f);
        }
    }
}