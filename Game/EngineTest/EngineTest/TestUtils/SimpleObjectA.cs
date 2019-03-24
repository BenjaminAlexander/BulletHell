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

        public static SubType Factory<SubType>(int integer, Vector2 vector, float floatingPoint) where SubType : SimpleObjectA, new()
        {
            SubType newObj = GameObject.Factory<SubType>();
            newObj.integerMember.Write = integer;
            newObj.vector2Member.Write = vector;
            newObj.floatMember.Write = floatingPoint;
            return newObj;
        }

        public static void AssertValuesEqual(SimpleObjectA expected, SimpleObjectA actual)
        {
            Assert.AreEqual(expected.integerMember.Write, actual.integerMember.Write);
            Assert.AreEqual(expected.floatMember.Write, actual.floatMember.Write);
            Assert.AreEqual(expected.vector2Member.Write, actual.vector2Member.Write);
        }

        public Vector2 Vector2Member()
        {
            return vector2Member.Value;
        }

        protected override void Update()
        {
            base.Update();
            this.vector2Member.Value = this.vector2Member.Value + new Vector2(1f);
        }
    }
}