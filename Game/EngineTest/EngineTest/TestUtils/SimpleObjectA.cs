using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.Engine.GameState;
using Microsoft.Xna.Framework;

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

        public static SimpleObjectA Factory(int instant, int integer, Vector2 vector, float floatingPoint)
        {
            SimpleObjectA simpleObject = new SimpleObjectA();
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
}