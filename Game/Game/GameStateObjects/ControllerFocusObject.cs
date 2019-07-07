using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.GameStateObjects.PhysicalObjects.MovingGameObjects.Ships;
using MyGame.GameStateObjects.DataStuctures;
using MyGame.GameServer;
using Microsoft.Xna.Framework;
using MyGame.Engine.GameState;
using MyGame.Engine.GameState.Instants;

namespace MyGame.GameStateObjects
{
    public class ControllerFocusObject : GameObject
    {
        //TODO: this class is super jank
        //Namely, I'm not sure if the player ids will always be a sequential list of numbers, perhaps a dictionary is better.
        //TODO: make it so this things sends update message only when changed, and sends them over TCP
        private ReferenceListField<Ship> focusList;
        private bool sendUpdate = true;

        //TODO: Need to add a new Subclass of GameObject to fix this
        /*
        protected override float SecondsBetweenUpdateMessage
        {
            get
            {
                return float.MaxValue;
            }
        }*/

        internal override void DefineFields(InitialInstant instant)
        {
            focusList = new ReferenceListField<Ship>(instant);
        }

        public override void Update(CurrentInstant current, NextInstant next)
        {
        }

        public static void ServerInitialize(NextInstant next, ControllerFocusObject obj, int numberOfPlayers)
        {
            List<Ship> focusList = new List<Ship>();
            for (int i = 0; i <= numberOfPlayers; i++)
            {
                //GameObjectReferenceListField<Ship> reflist = obj.focusList[new NextInstant(new Instant(0))];
                //reflist.Add(null);
                //obj.focusList[new NextInstant(new Instant(0))] = reflist;
                focusList.Add(null);
            }
            obj.focusList.SetList(next, focusList);
        }

        /*
        public override void SendUpdateMessage(Lobby lobby, GameTime gameTime)
        {
            //TODO: what about if a new player joins?  This is similar to other game set up data like world-size
            if (sendUpdate)
            {
                sendUpdate = false;
                lobby.BroadcastTCP(new GameObjectUpdate(gameTime, this));
            }
        }*/

        public void SetFocus(NextInstant next, Player player, Ship obj)
        {
            //GameObjectReferenceListField<Ship> reflist = focusList[new NextInstant(new Instant(0))];
            //reflist.Set(player.Id, obj);
            List<Ship> fList = focusList.GetList(next);
            fList[player.Id] = obj;
            focusList.SetList(next, fList);
            sendUpdate = true;
        }

        public Ship GetFocus(CurrentInstant instant, int i)
        {
            List<Ship> fList = focusList.GetList(instant);
            return fList[i];
        }
    }
}
