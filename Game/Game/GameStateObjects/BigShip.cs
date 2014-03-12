using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGame.Networking;
using Microsoft.Xna.Framework;
using MyGame.PlayerControllers;
using MyGame.IO;
using MyGame.DrawingUtils;

namespace MyGame.GameStateObjects
{
    class BigShip : Ship
    {
        private static Collidable collidable = new Collidable(TextureLoader.GetTexture("Ship"), Color.White, TextureLoader.GetTexture("Ship").CenterOfMass, .9f);

        public new class State : Ship.State
        {
            public State(GameObject obj) : base(obj) { }

            public override void Draw(Microsoft.Xna.Framework.GameTime gameTime, DrawingUtils.MyGraphicsClass graphics)
            {
                collidable.Draw(graphics, this.WorldPosition(), this.WorldDirection());
            }

        }

        public BigShip(GameObjectUpdate message) : base(message) { }

        public BigShip(Vector2 position, Vector2 velocity, IController controller1, IController controller2, IController controller3, IController controller4)
            : base(position, velocity, 40, 300, 300, 0.5f, controller1)
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


            Turret t = new Turret(this, new Vector2(119, 95) - TextureLoader.GetTexture("Ship").CenterOfMass, (float)(Math.PI / 2), (float)(Math.PI / 3), controller2);
            Turret t2 = new Turret(this, new Vector2(119, 5) - TextureLoader.GetTexture("Ship").CenterOfMass, (float)(-Math.PI / 2), (float)(Math.PI / 3), controller3);
            Turret t3 = new Turret(this, new Vector2(145, 50) - TextureLoader.GetTexture("Ship").CenterOfMass, (float)(0), (float)(Math.PI / 4), controller4);
            StaticGameObjectCollection.Collection.Add(t);
            StaticGameObjectCollection.Collection.Add(t2);
            StaticGameObjectCollection.Collection.Add(t3);
        }

        protected override GameObject.State BlankState(GameObject obj)
        {
            return new BigShip.State(obj);
        }
    }
}