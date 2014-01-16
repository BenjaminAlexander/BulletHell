using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace MyGame.IO
{
    public class InputManager
    {
        public InputManager()
        {
            IOState.Initilize();
        }

        public void Update()
        {
            IOState.Update();
            foreach(IOEvent e in observerDictionary.Keys)
            {
                if (e.hasOccured())
                {
                    foreach (IOObserver observer in observerDictionary[e])
                    {
                        observer.UpdateWithIOEvent(e);
                    }
                }
            }
        }

        public void Register(IOEvent e, IOObserver observer)
        {
            if (!observerDictionary.ContainsKey(e))
            {
                List<IOObserver> observerList = new List<IOObserver>();
                observerDictionary.Add(e, observerList);
            }
            observerDictionary[e].Add(observer);
        }

        public void Unregister(IOEvent e, IOObserver observer)
        {
            if (observerDictionary.ContainsKey(e))
            {
                if (observerDictionary[e].Contains(observer))
                {
                    observerDictionary[e].Remove(observer);
                }
            }
        }

        public void Unregister(IOObserver observer)
        {
            foreach(List<IOObserver> list in observerDictionary.Values)
            {
                if (list.Contains(observer))
                {
                    list.Remove(observer);
                }
            }
        }

        private Dictionary<IOEvent, List<IOObserver>> observerDictionary = new Dictionary<IOEvent, List<IOObserver>>();
    }
}
