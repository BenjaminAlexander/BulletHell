using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGame.GameStateObjects;

namespace MyGame.Networking
{
    public class GameObjectUpdate : TCPMessage
    {
        Type type;
        public Type GameObjectType
        {
            get { return type; }
        }

        public GameObjectUpdate(GameObject obj)
        {
            type = obj.GetType();
            this.Append(obj.GetTypeID());
        }

        public GameObjectUpdate(byte[] b, int lenght)
            : base(b, lenght)
        {
            this.ResetReader();
            type = GameObject.GetType(this.ReadInt());
            AssertMessageEnd();
        }
    }
}
