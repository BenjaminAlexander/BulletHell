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
    class SmallShip : Ship
    {
        private static Collidable collidable = new Collidable(TextureLoader.GetTexture("Enemy"), Color.White, TextureLoader.GetTexture("Enemy").CenterOfMass, .9f);
        public override Collidable Collidable
        {
            get { return collidable; }
        }

        public SmallShip(ClientGame game, GameObjectUpdate message) : base(game, message) { }

        public SmallShip(ServerGame game, Vector2 position, Vector2 velocity, ControlState controller1, ControlState controller4)
            : base(game, position, velocity, 40, 400, 900, 1.5f, controller1)
        {
            if (controller4 != null)
            {
                controller4.Focus = this;
            }

            Turret t3 = new Turret(game, this, new Vector2(25, 25) - TextureLoader.GetTexture("Enemy").CenterOfMass, (float)(0), (float)(Math.PI * 3), controller4);

            game.GameObjectCollection.Add(t3);
        }
    }
}