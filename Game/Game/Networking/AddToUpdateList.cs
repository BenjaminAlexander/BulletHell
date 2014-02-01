using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGame.GameStateObjects;

namespace MyGame.Networking
{
    class AddToUpdateList : GameObjectCollectionUpdate
    {
        int id;
        public override void Apply(GameStateObjects.DataStuctures.GameObjectCollection collection)
        {
            if (!Game1.IsServer)
            {
                collection.ForceAddToUpdateList(collection.Get(id));
            }
        }

        public AddToUpdateList(GameObject obj)
        {
            id = obj.ID;
            this.Append(obj.ID);
        }

        public AddToUpdateList(byte[] b, int lenght)
            : base(b, lenght)
        {
            this.ResetReader();
            id = this.ReadInt();
            AssertMessageEnd();
        }
    }
}
