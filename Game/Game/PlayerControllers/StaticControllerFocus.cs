using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGame.GameStateObjects;
using MyGame.Networking;
using MyGame.GameStateObjects.PhysicalObjects.MovingGameObjects.Ships;
using Microsoft.Xna.Framework;
using MyGame.GameStateObjects.DataStuctures;

namespace MyGame.PlayerControllers
{
    public static class StaticControllerFocus
    {
        private static Dictionary<int, GameObjectReference<Ship>> focusDictionary = new Dictionary<int, GameObjectReference<Ship>>();

        public static Ship GetFocus(int i)
        {
            if (focusDictionary.ContainsKey(i))
            {
                return focusDictionary[i].Dereference();
            }
            return null;
        }

        public static void SetFocus(int i, Ship obj, GameObjectCollection collection)
        {
            if (focusDictionary.ContainsKey(i))
            {
                focusDictionary.Remove(i);
            }
            focusDictionary.Add(i, new GameObjectReference<Ship>(obj, collection));
        }

        public static Queue<GameMessage> SendUpdateMessages(GameTime currentGametime)
        {
            Queue<GameMessage> rtn = new Queue<GameMessage>();
            foreach (int i in focusDictionary.Keys)
            {
                rtn.Enqueue(new SetControllerFocus(currentGametime, i, focusDictionary[i]));
            }
            return rtn;
        }
    }
}
