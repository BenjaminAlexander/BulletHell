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

namespace MyGame.GameStateObjects.PhysicalObjects.MovingGameObjects.Ships
{
    class SmallShip : Ship
    {
        private static Collidable collidable = new Collidable(TextureLoader.GetTexture("Enemy"), Color.White, TextureLoader.GetTexture("Enemy").CenterOfMass, .9f);
        public override Collidable Collidable
        {
            get { return collidable; }
        }

        public SmallShip(GameObjectUpdate message) : base(message) { }

        public SmallShip(Vector2 position, Vector2 velocity, IController controller1, IController controller4)
            : base(position, velocity, 40, 400, 900, 1.5f, controller1)
        {
            if (controller4 != null)
            {
                controller4.Focus = this;
            }

            Turret t3 = new Turret(this, new Vector2(25, 25) - TextureLoader.GetTexture("Enemy").CenterOfMass, (float)(0), (float)(Math.PI * 3), controller4);

            StaticGameObjectCollection.Collection.Add(t3);
        }
    }
}