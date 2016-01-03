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

namespace MyGame.GameStateObjects.PhysicalObjects.MovingGameObjects.Ships
{
    class BigShip : Ship
    {
        private static Collidable collidable = new Collidable(TextureLoader.GetTexture("Ship"), Color.White, TextureLoader.GetTexture("Ship").CenterOfMass, .9f);
        public override Collidable Collidable
        {
            get { return collidable; }
        }

        public BigShip(ClientGame game, GameObjectUpdate message) : base(game, message) { }

        public BigShip(ServerGame game, Vector2 position, Vector2 velocity, ControlState controller1, ControlState controller2, ControlState controller3, ControlState controller4)
            : base(game, position, velocity, 4000, 300, 300, 0.5f, controller1)
        {
            if (controller4 != null)
            {
                controller4.Focus = this;
            }
            if (controller2 != null)
            {
                controller2.Focus = this;
            }
            if (controller3 != null)
            {
                controller3.Focus = this;
            }

            Turret t = new Turret(game, this, new Vector2(119, 95) - TextureLoader.GetTexture("Ship").CenterOfMass, (float)(Math.PI / 2), (float)(Math.PI / 3), controller2);
            Turret t2 = new Turret(game, this, new Vector2(119, 5) - TextureLoader.GetTexture("Ship").CenterOfMass, (float)(-Math.PI / 2), (float)(Math.PI / 3), controller3);
            Turret t3 = new Turret(game, this, new Vector2(145, 50) - TextureLoader.GetTexture("Ship").CenterOfMass, (float)(0), (float)(Math.PI / 4), controller4);
            Turret t4 = new Turret(game, this, new Vector2(20, 50) - TextureLoader.GetTexture("Ship").CenterOfMass, (float)(-Math.PI), (float)(Math.PI / 4), controller4);
            game.GameObjectCollection.Add(t);
            game.GameObjectCollection.Add(t2);
            game.GameObjectCollection.Add(t3);
            game.GameObjectCollection.Add(t4);
        }
    }
}