using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGame.Networking;
using Microsoft.Xna.Framework;
using MyGame.PlayerControllers;
using MyGame.IO;
using MyGame.DrawingUtils;
using MyGame.GameStateObjects.PhysicalObjects.MemberPhysicalObjects;
using MyGame.GameServer;
using MyGame.GameClient;
using Microsoft.Xna.Framework.Graphics;

namespace MyGame.GameStateObjects.PhysicalObjects.MovingGameObjects.Ships
{
    class BigShip : Ship
    {
        private static Collidable collidable = new Collidable(TextureLoader.GetTexture("Ship"), Color.White, TextureLoader.GetTexture("Ship").CenterOfMass, .9f);
        public override Collidable Collidable
        {
            get { return collidable; }
        }

        public BigShip(Game1 game)
            : base(game)
        {
        }

        public void BigShipInit(Vector2 position, Vector2 velocity, ControlState controller1, ControlState controller2, ControlState controller3, ControlState controller4)
        {
            Ship.ServerInitialize(this, position, velocity, 0, 4000, 300, 300, 0.5f, controller1);
            Turret t = new Turret(this.Game);
            Turret.ServerInitialize(t, this, new Vector2(119, 95) - TextureLoader.GetTexture("Ship").CenterOfMass, (float)(Math.PI / 2), (float)(Math.PI / 3), controller2);
            Turret t2 = new Turret(this.Game);
            Turret.ServerInitialize(t2, this, new Vector2(119, 5) - TextureLoader.GetTexture("Ship").CenterOfMass, (float)(-Math.PI / 2), (float)(Math.PI / 3), controller3);
            Turret t3 = new Turret(this.Game);
            Turret.ServerInitialize(t3, this, new Vector2(145, 50) - TextureLoader.GetTexture("Ship").CenterOfMass, (float)(0), (float)(Math.PI / 3), controller4);
            Turret t4 = new Turret(this.Game);
            Turret.ServerInitialize(t4, this, new Vector2(20, 50) - TextureLoader.GetTexture("Ship").CenterOfMass, (float)(-Math.PI), (float)(Math.PI/3), controller4);
            this.Game.GameObjectCollection.Add(t);
            this.Game.GameObjectCollection.Add(t2);
            this.Game.GameObjectCollection.Add(t3);
            this.Game.GameObjectCollection.Add(t4);
        }
    }
}