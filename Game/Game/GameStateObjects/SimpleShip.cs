using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGame.DrawingUtils;
using MyGame.Networking;
using Microsoft.Xna.Framework;

namespace MyGame.GameStateObjects
{
    class SimpleShip : GameObject
    {
        private static Collidable collidable = new Collidable(Textures.Bullet, new Vector2(0), Color.White, 0, new Vector2(20, 5), 1);

        struct State
        {
            public Vector2 position;
            public Vector2 velocity;
            public State(Vector2 position, Vector2 velocity)
            {
                this.velocity = velocity;
                this.position = position;
            }
        }
        public SimpleShip(int id) : base(id)
        {

        }

        public SimpleShip() : base()
        {

        }
        
        
        protected override void UpdateSubclass(Microsoft.Xna.Framework.GameTime gameTime)
        {
            throw new NotImplementedException();
        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime, DrawingUtils.MyGraphicsClass graphics)
        {
            collidable.Draw(graphics);
        }

        public override void UpdateMemberFields(GameObjectUpdate message)
        {
            base.UpdateMemberFields(message);
            /*
            message.ResetReader();
            if (!(this.GetType() == GameObject.GetType(message.ReadInt()) && this.id == message.ReadInt()))
            {
                throw new Exception("this message does not belong to this object");
            }
            this.state.destroy = message.ReadBoolean();
             * */
        }

        public override GameObjectUpdate MemberFieldUpdateMessage(GameObjectUpdate message)
        {
            message = base.MemberFieldUpdateMessage(message);
            /*
            message.Append(this.state.destroy);
             */
            return message;
        }
    }
}
