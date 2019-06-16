using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.Engine.GameState;
using Microsoft.Xna.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyGame.Engine.GameState.FieldValues;

namespace EngineTest.EngineTest.TestUtils
{
    class SimpleObjectA : GameObject
    {
        Field<IntegerValue> integerMember;
        Field<Vector2Value> vector2Member;
        Field<FloatValue> floatMember;

        internal override void DefineFields(NextContainer container)
        {
            integerMember = new Field<IntegerValue>(this, container);
            vector2Member = new Field<Vector2Value>(this, container);
            floatMember = new Field<FloatValue>(this, container);
        }

        public static GameObjectContainer Factory(int instant, int integer, Vector2 vector, float floatingPoint)
        {
            //TODO: clean up this factory buisness
            SimpleObjectA newObj = new SimpleObjectA();
            GameObjectContainer container = new GameObjectContainer(newObj, instant);
            newObj.integerMember[container.Next] = integer;
            newObj.vector2Member[container.Next] = vector;
            newObj.floatMember[container.Next] = floatingPoint;
            return container;
        }

        public Vector2 Vector2Member(CurrentContainer current)
        {
            return vector2Member[current];
        }

        public void Vector2Member(NextContainer current, Vector2 value)
        {
            vector2Member[current] = value;
        }

        public override void Update(CurrentContainer current, NextContainer next)
        {
            this.vector2Member[next] = this.vector2Member[current] + new Vector2(1f);
        }
    }
}