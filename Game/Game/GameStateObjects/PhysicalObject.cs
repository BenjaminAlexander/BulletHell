using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.Utils;

namespace MyGame.GameStateObjects
{
    public abstract class PhysicalObject : GameObject
    {
        public PhysicalObject() : base(){}

        private List<MemberPhysicalObject> members = new List<MemberPhysicalObject>();

        public abstract Vector2 WorldPosition();

        public abstract float WorldDirection();

        public abstract PhysicalObject Root();

        public virtual void Add(MemberPhysicalObject obj)
        {
            members.Add(obj);
        }

        public override Boolean IsPhysicalObject
        {
            get { return true; }
        }

        public virtual Boolean IsCompositePhysicalObject
        {
            get { return false; }
        }

        public virtual Boolean IsMemberPhysicalObject
        {
            get { return false; }
        }

        protected override void UpdateSubclass(GameTime gameTime)
        {
            foreach (MemberPhysicalObject obj in members)
            {
                obj.UpdateSubclass(gameTime);
            }
        }

        public override void Draw(GameTime gameTime, DrawingUtils.MyGraphicsClass graphics)
        {
            foreach (MemberPhysicalObject obj in members)
            {
                obj.Draw(gameTime, graphics);
            }
        }
    }
}
