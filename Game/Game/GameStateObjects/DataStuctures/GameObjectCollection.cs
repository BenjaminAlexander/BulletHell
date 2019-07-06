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
using MyGame.GameStateObjects.PhysicalObjects.CompositePhysicalObjects;
using MyGame.GameStateObjects.PhysicalObjects.MemberPhysicalObjects;
using MyGame.GameStateObjects.PhysicalObjects.MovingGameObjects.Ships;
using MyGame.GameStateObjects.PhysicalObjects.MovingGameObjects;

namespace MyGame.GameStateObjects.DataStuctures
{
    public class GameObjectCollection : Engine.GameState.GameObjectCollection
    {
        private static GameObjectCollection reference;
        
        public static GameObjectCollection Reference
        {
            get
            {
                return reference;
            }
        }

        private int nextId = 1;
        private GameObjectListManager listManager = new GameObjectListManager();
        private Dictionary<int, GameObject> dictionary = new Dictionary<int, GameObject>();
        private ControllerFocusObject controllerObject;

        public ControllerFocusObject ControllerFocusObject
        {
            get
            {
                return controllerObject;
            }
        }

        public int NextID
        {
            get { return nextId++; }
        }

        public GameObjectCollection()
        {
            Engine.GameState.GameObject.AddType<Moon>();
            Engine.GameState.GameObject.AddType<Turret>();
            Engine.GameState.GameObject.AddType<BigShip>();
            Engine.GameState.GameObject.AddType<SmallShip>();
            Engine.GameState.GameObject.AddType<Tower>();
            Engine.GameState.GameObject.AddType<ControllerFocusObject>();
            reference = this;
        }

        public Boolean Contains(int id)
        {
            return dictionary.ContainsKey(id);
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

            if (!dictionary.ContainsKey((int)obj.ID))
            {
                dictionary.Add((int)obj.ID, obj);
                listManager.Add(obj);
            }
        }

        public GameObject Get(int id)
        {
            if (id == 0)
            {
                return null;
            }
            return dictionary[id];
        }

        public void ServerUpdate(Lobby lobby, GameTime gameTime)
        {
            foreach (GameObject obj in this.listManager.GetList<GameObject>())
            {
                obj.Update((new Instant(0)).AsCurrent, (new Instant(0)).AsNext);
            }

            foreach (GameObject obj in this.listManager.GetList<GameObject>())
            {
                obj.SendUpdateMessage(lobby, gameTime, this, new Instant(0));
            }
        }

        public void ClientUpdate(GameTime gameTime)
        {
            foreach (GameObject obj in this.listManager.GetList<GameObject>())
            {
                obj.Update((new Instant(0)).AsCurrent, (new Instant(0)).AsNext);
            }
        }

        public void Draw(CurrentInstant current, MyGraphicsClass graphics)
        {
            graphics.BeginWorld();
            foreach (GameObject obj in listManager.GetList<GameObject>())
            {
                obj.Draw(current, graphics);
            }
            graphics.EndWorld();
        }
    }
}
