using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.Utils;
using MyGame.GameStateObjects.PhysicalObjects;
using MyGame.DrawingUtils;
using MyGame.GameServer;
using MyGame.Engine.GameState.Instants;
using MyGame.GameStateObjects.PhysicalObjects.MovingGameObjects.Ships;
using MyGame.GameStateObjects.PhysicalObjects.MovingGameObjects;

namespace MyGame.GameStateObjects.DataStuctures
{
    public class GameObjectCollection : Engine.GameState.GameObjectCollection
    {
        internal static Instant SingleInstant = new Instant(0);

        private GameObjectListManager listManager = new GameObjectListManager();
        private ControllerFocusObject controllerObject;

        private Instant currentInstant;
        private Instant nextInstant;

        private Instant CurrentInstant
        {
            get
            {
                return currentInstant;
            }
        }

        public ControllerFocusObject ControllerFocusObject
        {
            get
            {
                return controllerObject;
            }
        }

        public GameObjectCollection()
        {
            Engine.GameState.GameObject.AddType<BigShip>();
            Engine.GameState.GameObject.AddType<SmallShip>();
            Engine.GameState.GameObject.AddType<Tower>();
            Engine.GameState.GameObject.AddType<ControllerFocusObject>();
            SingleInstant = base.GetInstant(SingleInstant);
            currentInstant = base.GetInstant(0);
            nextInstant = currentInstant;
        }

        internal new SubType NewGameObject<SubType>(NextInstant next) where SubType : GameObject, new()
        {
            SubType obj = base.NewGameObject<SubType>(next);
            this.Add(obj);
            return obj;
        }

        public new GameObject Deserialize(byte[] buffer)
        {
            GameObject obj = (GameObject)base.Deserialize(buffer);
            this.Add(obj);
            return obj;
        }

        public void Add(GameObject obj)
        {
            if(obj is ControllerFocusObject)
            {
                this.controllerObject = (ControllerFocusObject)obj;
            }

            if (!listManager.Contains(obj))
            {
                listManager.Add(obj);
            }
        }

        public void ServerUpdate(Lobby lobby, GameTime gameTime)
        {
            foreach (GameObject obj in this.listManager.GetList<GameObject>())
            {
                obj.Update(currentInstant.AsCurrent, nextInstant.AsNext);
            }

            foreach (GameObject obj in this.listManager.GetList<GameObject>())
            {
                obj.SendUpdateMessage(lobby, gameTime, this, nextInstant);
            }
        }

        public void ClientUpdate(GameTime gameTime)
        {
            foreach (GameObject obj in this.listManager.GetList<GameObject>())
            {
                obj.Update(currentInstant.AsCurrent, nextInstant.AsNext);
            }
        }

        public void Draw(MyGraphicsClass graphics)
        {
            graphics.BeginWorld();
            foreach (GameObject obj in listManager.GetList<GameObject>())
            {
                obj.Draw(currentInstant.AsCurrent, graphics);
            }
            graphics.EndWorld();
        }
    }
}
