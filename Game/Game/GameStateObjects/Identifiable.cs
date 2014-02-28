using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGame.Networking;

namespace MyGame.GameStateObjects
{
    public abstract class Identifiable
    {
        public Identifiable()
        {
            id = GetNextID();
            nextId++;
        }

        public Identifiable(GameObjectUpdate message)
        {
            id = message.ID;
        }

        public int GetID()
        {
            return id;
        }
        
        private static int GetNextID()
        {
            return nextId;
        }

        // The id of this Identifiable
        private int id;

        //this is the id for the next game object
        private static int nextId = 1;
    }
}
