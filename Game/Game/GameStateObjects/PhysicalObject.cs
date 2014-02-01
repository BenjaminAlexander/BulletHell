using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.Utils;
using MyGame.Networking;

namespace MyGame.GameStateObjects
{
    public abstract class PhysicalObject : GameObject
    {
        public PhysicalObject(int id) : base(id) { }

        public PhysicalObject() : base() { }

        private List<MemberPhysicalObject> members = new List<MemberPhysicalObject>();

        public abstract Vector2 WorldPosition();

        public abstract float WorldDirection();

        public abstract PhysicalObject Root();

        public virtual void Add(MemberPhysicalObject obj)
        {
            members.Add(obj);
        }

        protected override void UpdateSubclass(GameTime gameTime)
        {
            foreach (MemberPhysicalObject obj in members)
            {
                //obj.UpdateSubclass(gameTime);
            }
        }

        public override void Draw(GameTime gameTime, DrawingUtils.MyGraphicsClass graphics)
        {
            foreach (MemberPhysicalObject obj in members)
            {
                obj.Draw(gameTime, graphics);
            }
        }

        public override void Destroy()
        {
            base.Destroy();
            foreach (MemberPhysicalObject m in members)
            {
                m.Destroy();
            }

        }

        //using MyGame.Networking;
        public override void UpdateMemberFields(GameObjectUpdate message)
        {
            base.UpdateMemberFields(message);
            members = message.ReadGameObjectList().Cast<MemberPhysicalObject>().ToList();
        }

        public override GameObjectUpdate MemberFieldUpdateMessage(GameObjectUpdate message)
        {
            message = base.MemberFieldUpdateMessage(message);
            message.Append(members.Cast<GameObject>().ToList());
            return message;
        }

        public override void SendUpdateMessage()
        {
            if (Game1.IsServer)
            {
                Game1.outgoingQue.Enqueue(this.GetUpdateMessage());
                foreach (MemberPhysicalObject obj in members)
                {
                    obj.SendUpdateMessage();
                }
                base.SendUpdateMessage();
            }
        }
    }
}
