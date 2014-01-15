using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MyGame.GameStateObjects.Ships
{
    abstract class FlyingStrategy
    {
        public FlyingStrategy(FlyingGameObject obj)
        {
            this.obj = obj;
        }

        public void ExecuteStrategy(GameTime elapsedTime)
        {
            // Copying the states before iterating through, as some states remove themselves from the original collection.
            List<FlyingState> statesCopy = new List<FlyingState>(this.states);
            foreach (FlyingState state in statesCopy)
            {
                state.Handle(elapsedTime);
            }
        }

        public void AddState(FlyingState state)
        {
            states.Add(state);
        }

        public void RemoveState(FlyingState state)
        {
            states.Remove(state);
        }

        public FlyingGameObject GetAttachedObject()
        {
            return this.obj;
        }

        private FlyingGameObject obj;
        private List<FlyingState> states = new List<FlyingState>();
    }
}
