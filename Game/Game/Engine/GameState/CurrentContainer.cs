using MyGame.Engine.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MyGame.Engine.GameState.GameObject;

namespace MyGame.Engine.GameState
{
    public class CurrentContainer
    {
        GameObjectContainer container;

        internal CurrentContainer(GameObjectContainer container)
        {
            this.container = container;
        }

        internal FieldValueType GetFieldValue<FieldValueType>(AbstractField definition) where FieldValueType : struct, FieldValue
        {
            return container.GetFieldValue<FieldValueType>(definition);
        }
    }
}
