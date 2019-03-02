using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace MyGame.Engine.GameState.ObjectState
{
    class SimpleObjectStateC : ObjectState
    {
        private IntegerField integerMember;
        private Vector2Field vector2Member;
        private FloatField floatMember;

        public SimpleObjectStateC()
        {
            integerMember = new IntegerField(this);
            vector2Member = new Vector2Field(this);
            floatMember = new FloatField(this);
        }

        public static SimpleObjectStateC Factory(int integer, Vector2 vector, float floatingPoint)
        {
            SimpleObjectStateC simpleState = new SimpleObjectStateC();
            simpleState.integerMember.Value = integer;
            simpleState.vector2Member.Value = vector;
            simpleState.floatMember.Value = floatingPoint;
            return simpleState;
        }

        public int IntegerMember
        {
            get
            {
                return integerMember.Value;
            }
        }

        public Vector2 Vector2Member
        {
            get
            {
                return vector2Member.Value;
            }
        }

        public float FloatMember
        {
            get
            {
                return floatMember.Value;
            }
        }
    }
}
