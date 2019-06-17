using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.Engine.GameState;
using Microsoft.Xna.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyGame.Engine.GameState.FieldValues;
using MyGame.Engine.GameState.Instants;

namespace EngineTest.EngineTest.TestUtils
{
    class SimpleObjectA : GameObject
    {
        Field<IntegerValue> integerMember;
        Field<Vector2Value> vector2Member;
        Field<FloatValue> floatMember;

        internal override void DefineFields(InitialInstant instant)
        {
            integerMember = new Field<IntegerValue>(instant);
            vector2Member = new Field<Vector2Value>(instant);
            floatMember = new Field<FloatValue>(instant);
        }

        public static SimpleObjectA Factory(Instant container, int integer, Vector2 vector, float floatingPoint)
        {
            //TODO: clean up this factory buisness
            SimpleObjectA newObj = GameObject.Construct<SimpleObjectA>(container);
            newObj.integerMember[container.AsNext] = integer;
            newObj.vector2Member[container.AsNext] = vector;
            newObj.floatMember[container.AsNext] = floatingPoint;
            return newObj;
        }

        public Vector2 Vector2Member(CurrentInstant current)
        {
            return vector2Member[current];
        }

        public void Vector2Member(NextInstant current, Vector2 value)
        {
            vector2Member[current] = value;
        }

        public override void Update(CurrentInstant current, NextInstant next)
        {
            this.vector2Member[next] = this.vector2Member[current] + new Vector2(1f);
        }
    }
}