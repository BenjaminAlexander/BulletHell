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
    class SimpleObjectB : GameObject
    {
        Field<FloatValue> floatMember1;
        Field<FloatValue> floatMember2;
        Field<FloatValue> floatMember3;
        Field<FloatValue> floatMember4;

        internal override void DefineFields(NextContainer container)
        {
            floatMember1 = new Field<FloatValue>(this, container);
            floatMember2 = new Field<FloatValue>(this, container);
            floatMember3 = new Field<FloatValue>(this, container);
            floatMember4 = new Field<FloatValue>(this, container);
        }

        public static GameObjectContainer Factory(int instant, float floating1, float floating2, float floating3, float floating4)
        {
            SimpleObjectB newObj = new SimpleObjectB();
            GameObjectContainer container = new GameObjectContainer(newObj, instant);
            newObj.floatMember1[container.Next] = floating1;
            newObj.floatMember2[container.Next] = floating2;
            newObj.floatMember3[container.Next] = floating3;
            newObj.floatMember4[container.Next] = floating4;
            return container;
        }

        public float FloatMember1(CurrentContainer current)
        {
            return floatMember1[current];
        }

        public float FloatMember2(CurrentContainer current)
        {
            return floatMember2[current];
        }

        public float FloatMember3(CurrentContainer current)
        {
            return floatMember3[current];
        }

        public float FloatMember4(CurrentContainer current)
        {
            return floatMember4[current];
        }

        public override void Update(CurrentContainer current, NextContainer next)
        {
            throw new NotImplementedException();
        }
    }
}