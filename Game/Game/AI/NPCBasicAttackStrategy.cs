using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGame.Utils;
using MyGame.GameStateObjects;
using MyGame.GameStateObjects.Ships;

namespace MyGame.AI
{
    class NPCBasicAttackStrategy : FlyingStrategy
    {
        public NPCBasicAttackStrategy(NPCShip obj, FlyingGameObject target)
            : base(obj)
        {
            AddState(new AttackingState(this, obj, target));
        }

        private class AttackingState : FlyingFollowState
        {
            public AttackingState(NPCBasicAttackStrategy context, NPCShip obj, FlyingGameObject target)
                : base(context, obj, target)
            {
            }

            public override void Handle(Microsoft.Xna.Framework.GameTime elapsedTime)
            {
                base.Handle(elapsedTime);
                if (Vector2Utils.ShortestAngleDistance(obj.Direction, Vector2Utils.Vector2Angle(followObj.Position - obj.Position)) < (Math.PI / 4))
                {

                        ((NPCShip)obj).FireCoaxialWeapon();

                }

                // Switch states if we are too close for engagement.
                if ((followObj.Position - obj.Position).Length() < 500)
                {
                    // TODO: Dangerous convert...fix this
                    this.Context.AddState(new ClearTargetState((NPCBasicAttackStrategy)this.Context, (NPCShip)this.obj, this.followObj));
                    this.Context.RemoveState(this);
                }
            }
        }

        private class ClearTargetState : FlyingFleeState
        {
            public ClearTargetState(NPCBasicAttackStrategy context, NPCShip obj, FlyingGameObject target)
                : base(context, obj, target)
            {
            }

            public override void Handle(Microsoft.Xna.Framework.GameTime elapsedTime)
            {
                base.Handle(elapsedTime);
                // Switch states if we are far enough away for engagement.
                if ((followObj.Position - obj.Position).Length() > 750)
                {
                    this.Context.AddState(new AttackingState((NPCBasicAttackStrategy)this.Context, (NPCShip)this.obj, this.followObj));
                    this.Context.RemoveState(this);
                }
            }
        }
    }
}
