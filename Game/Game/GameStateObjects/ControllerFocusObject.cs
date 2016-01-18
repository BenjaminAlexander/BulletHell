using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.GameStateObjects.PhysicalObjects.MovingGameObjects.Ships;
using MyGame.GameStateObjects.DataStuctures;

namespace MyGame.GameStateObjects
{
    class ControllerFocusObject : GameObject
    {
        //TODO: this class is super jank
        //Namely, I'm not sure if the player ids will always be a sequential list of numbers, perhaps a dictionary is better.
        //TODO: make it so this things sends update message only when changed, and sends them over TCP
        private GameObjectReferenceListField<Ship> focusList;

        public ControllerFocusObject(Game1 game)
            : base(game)
        {
            focusList = new GameObjectReferenceListField<Ship>(this, this.Game.GameObjectCollection); 
        }

        public static void ServerInitialize(ControllerFocusObject obj, int numberOfPlayers)
        {
            for (int i = 0; i <= numberOfPlayers; i++)
            {
                obj.focusList.Value.Add(new GameObjectReference<Ship>(null, obj.Game.GameObjectCollection));
            }
        }

        public void SetFocus(int i, Ship obj)
        {
            focusList.Value[i] = new GameObjectReference<Ship>(obj, this.Game.GameObjectCollection);
        }

        public Ship GetFocus(int i)
        {
            return focusList.Value[i].Dereference();
        }
    }
}
