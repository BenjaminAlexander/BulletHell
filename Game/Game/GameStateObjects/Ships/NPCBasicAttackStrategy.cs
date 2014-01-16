using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGame.Utils;

namespace MyGame.GameStateObjects.Ships
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
            int ammo = 4;
            public AttackingState(NPCBasicAttackStrategy context, NPCShip obj, FlyingGameObject target)
                : base(context, obj, target)
            {
            }

            public override void Handle(Microsoft.Xna.Framework.GameTime elapsedTime)
            {
                base.Handle(elapsedTime);
                if (Vector2Utils.AngleDistance(obj.Direction, Vector2Utils.Vector2Angle(followObj.Position - obj.Position)) < (Math.PI / 4))
                {
                    if (ammo > 0)
                    {
                        ((NPCShip)obj).FireCoaxialWeapon();
                        ammo = ammo - 1;
                    }
                }

                // Switch states if we are too close for engagement.
                if ((followObj.Position - obj.Position).Length() < 200)
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
                if ((followObj.Position - obj.Position).Length() > 500)
                {
                    this.Context.AddState(new AttackingState((NPCBasicAttackStrategy)this.Context, (NPCShip)this.obj, this.followObj));
                    this.Context.RemoveState(this);
                }
            }
        }
    }
}
