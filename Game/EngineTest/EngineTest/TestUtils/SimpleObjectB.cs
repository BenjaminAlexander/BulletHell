using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.Engine.GameState;
using Microsoft.Xna.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyGame.Engine.GameState.Instants;
using MyGame.Engine.GameState.Fields;
using MyGame.Engine.Serialization.DataTypes;

namespace EngineTest.EngineTest.TestUtils
{
    class SimpleObjectB : GameObject
    {
        StructField<SFloat> floatMember1;
        StructField<SFloat> floatMember2;
        StructField<SFloat> floatMember3;
        StructField<SFloat> floatMember4;

        internal protected override void DefineFields(CreationToken creationToken)
        {
            floatMember1 = new StructField<SFloat>(creationToken);
            floatMember2 = new StructField<SFloat>(creationToken);
            floatMember3 = new StructField<SFloat>(creationToken);
            floatMember4 = new StructField<SFloat>(creationToken);
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

        internal protected override void Update(CurrentInstant current, NextInstant next)
        {
            throw new NotImplementedException();
        }

        protected internal override void Draw(CurrentInstant current)
        {
            
        }
    }
}