using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyGame.GameStateObjects
{
    public abstract class GameObjectField
    {

        public GameObjectField(GameObject obj)
        {
            obj.Fields.Add(this);
        }

        //TODO: is this the best way?
        public abstract void ApplyMessage(GameObjectUpdate message);

        public abstract GameObjectUpdate ConstructMessage(GameObjectUpdate message);
    }
}
