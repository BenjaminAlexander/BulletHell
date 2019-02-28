using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyGame.Engine.GameState;
using Microsoft.Xna.Framework;

namespace EngineTest.EngineTest.GameStateTest
{
    [TestClass]
    public class GameObjectTest
    {
        class SimpleObject : GameObject
        {
            IntegerMetaField integerMember;
            Vector2MetaField vector2Member;
            FloatMetaField floatMember;

            public SimpleObject()
            {
                integerMember = new IntegerMetaField(this);
                vector2Member = new Vector2MetaField(this);
                floatMember = new FloatMetaField(this);
            }

            public static SimpleObject Factory(int instant, int integer, Vector2 vector, float floatingPoint)
            {
                SimpleObject simpleObject = new SimpleObject();
                simpleObject.integerMember[instant] = integer;
                simpleObject.vector2Member[instant] = vector;
                simpleObject.floatMember[instant] = floatingPoint;
                return simpleObject;
            }

            public int IntegerMember(int instant)
            {
                return integerMember[instant];
            }

            public Vector2 Vector2Member(int instant)
            {
                return vector2Member[instant];
            }

            public float FloatMember(int instant)
            {
                return floatMember[instant];
            }

        }


        [TestMethod]
        public void SerializeDeserializeTest()
        {
            SimpleObject expected = SimpleObject.Factory(0, 1234, new Vector2(656.34f, 345.4f), 787.9f);
            byte[] serialization = expected.Serialize(0);
            SimpleObject actual = new SimpleObject();
            actual.Deserialize(0, serialization);

            Assert.AreEqual(expected.IntegerMember(0), actual.IntegerMember(0));
            Assert.AreEqual(expected.FloatMember(0), actual.FloatMember(0));
            Assert.AreEqual(expected.Vector2Member(0), actual.Vector2Member(0));
        }
    }
}
