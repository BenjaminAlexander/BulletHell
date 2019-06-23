﻿using System;
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
    class ControllerFocusObject : GameObject
    {
        //TODO: this class is super jank
        //Namely, I'm not sure if the player ids will always be a sequential list of numbers, perhaps a dictionary is better.
        //TODO: make it so this things sends update message only when changed, and sends them over TCP
        private Field<GameObjectReferenceListField<Ship>> focusList;
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

        public ControllerFocusObject()
        {
        }

        public ControllerFocusObject(Game1 game)
            : base(game)
        {
        }

        internal override void DefineFields(InitialInstant instant)
        {
            base.DefineFields(instant);
            focusList = new Field<GameObjectReferenceListField<Ship>>(instant);
        }

        public static void ServerInitialize(ControllerFocusObject obj, int numberOfPlayers)
        {
            for (int i = 0; i <= numberOfPlayers; i++)
            {
                GameObjectReferenceListField<Ship> reflist = obj.focusList[new NextInstant(new Instant(0))];
                reflist.Add(null);
                obj.focusList[new NextInstant(new Instant(0))] = reflist;
            }
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

        public void SetFocus(Player player, Ship obj)
        {
            GameObjectReferenceListField<Ship> reflist = focusList[new NextInstant(new Instant(0))];
            reflist.Set(player.Id, obj);
            focusList[new NextInstant(new Instant(0))] = reflist;
            sendUpdate = true;
        }

        public Ship GetFocus(int i)
        {
            return focusList[new NextInstant(new Instant(0))].Get(i);
        }
    }
}
