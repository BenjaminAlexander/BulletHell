using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.PlayerControllers;
using MyGame.GameStateObjects.PhysicalObjects;
using MyGame.GameStateObjects.PhysicalObjects.MovingGameObjects.Ships;
using MyGame.GameServer;
using MyGame.GameClient;
using MyGame.Engine.GameState.Instants;

namespace MyGame.GameStateObjects.PhysicalObjects.MemberPhysicalObjects
{
    public class Gun : MemberPhysicalObject
    {
        public Gun(Game1 game)
            : base(game)
        {
        }

        public static void ServerInitialize(Gun obj, PhysicalObject parent, Vector2 position, float direction)
        {
            MemberPhysicalObject.ServerInitialize(obj, parent, position, direction);
        }
    }
}
