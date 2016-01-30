using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.GameStateObjects.PhysicalObjects.MovingGameObjects.Ships;
using MyGame.GameStateObjects.DataStuctures;
using MyGame.GameServer;
using Microsoft.Xna.Framework;

namespace MyGame.GameStateObjects
{
    class ControllerFocusObject : GameObject
    {
        //TODO: this class is super jank
        //Namely, I'm not sure if the player ids will always be a sequential list of numbers, perhaps a dictionary is better.
        //TODO: make it so this things sends update message only when changed, and sends them over TCP
        private GameObjectReferenceListField<Ship> focusList;
        private bool sendUpdate = true;

        //TODO: Need to add a new Subclass of GameObject to fix this
        protected override float SecondsBetweenUpdateMessage
        {
            get
            {
                return float.MaxValue;
            }
        }

        public ControllerFocusObject(Game1 game)
            : base(game)
        {
            focusList = new GameObjectReferenceListField<Ship>(this); 
        }

        public static void ServerInitialize(ControllerFocusObject obj, int numberOfPlayers)
        {
            for (int i = 0; i <= numberOfPlayers; i++)
            {
                obj.focusList.Value.Add(null);
            }
        }

        public override void SendUpdateMessage(Lobby lobby, GameTime gameTime)
        {
            //TODO: what about if a new player joins?  This is similar to other game set up data like world-size
            if (sendUpdate)
            {
                sendUpdate = false;
                lobby.BroadcastTCP(new GameObjectUpdate(gameTime, this));
            }
        }

        public void SetFocus(Player player, Ship obj)
        {
            focusList.Value[player.Id] = obj;
            sendUpdate = true;
        }

        public Ship GetFocus(int i)
        {
            return focusList.Value[i];
        }
    }
}
