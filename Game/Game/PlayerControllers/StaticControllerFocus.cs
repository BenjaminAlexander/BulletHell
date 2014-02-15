using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGame.GameStateObjects;
using MyGame.Networking;

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

        public static void SetFocus(int i, Ship obj)
        {
            if (focusDictionary.ContainsKey(i))
            {
                focusDictionary.Remove(i);
            }
            focusDictionary.Add(i, new GameObjectReference<Ship>(obj));
        }

        public static void SendUpdateMessages()
        {
            foreach(int i in focusDictionary.Keys)
            {
                Game1.outgoingQue.Enqueue(new SetControllerFocus(i, focusDictionary[i]));
            }
        }
    }
}
