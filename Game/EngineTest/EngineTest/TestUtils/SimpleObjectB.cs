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
using MyGame.Engine.GameState.ObjectFields;

namespace EngineTest.EngineTest.TestUtils
{
    class SimpleObjectB : GameObject
    {
        Field<FloatValue> floatMember1;
        Field<FloatValue> floatMember2;
        Field<FloatValue> floatMember3;
        Field<FloatValue> floatMember4;

        internal override void DefineFields(CreationToken creationToken)
        {
            floatMember1 = new Field<FloatValue>(creationToken);
            floatMember2 = new Field<FloatValue>(creationToken);
            floatMember3 = new Field<FloatValue>(creationToken);
            floatMember4 = new Field<FloatValue>(creationToken);
        }

        public static SimpleObjectB Factory(NextInstant next, float floating1, float floating2, float floating3, float floating4)
        {
            SimpleObjectB newObj = next.NewGameObject<SimpleObjectB>();
            newObj.floatMember1[next] = floating1;
            newObj.floatMember2[next] = floating2;
            newObj.floatMember3[next] = floating3;
            newObj.floatMember4[next] = floating4;
            return newObj;
        }

        public float FloatMember1(CurrentInstant current)
        {
            return floatMember1[current];
        }

        public float FloatMember2(CurrentInstant current)
        {
            return floatMember2[current];
        }

        public float FloatMember3(CurrentInstant current)
        {
            return floatMember3[current];
        }

        public float FloatMember4(CurrentInstant current)
        {
            return floatMember4[current];
        }

        public override void Update(CurrentInstant current, NextInstant next)
        {
            throw new NotImplementedException();
        }
    }
}