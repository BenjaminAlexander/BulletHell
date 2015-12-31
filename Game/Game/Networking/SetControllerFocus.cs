using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGame.GameStateObjects;
using MyGame.PlayerControllers;
using MyGame.GameStateObjects.PhysicalObjects.MovingGameObjects.Ships;
using Microsoft.Xna.Framework;
using MyGame.GameClient;

namespace MyGame.Networking
{
    public class SetControllerFocus: ClientUpdate
    {

        public SetControllerFocus(GameTime currentGameTime, int id, GameObjectReference<Ship> ship) : base (currentGameTime)
        {
            Game1.AsserIsServer();
            this.Append(id);
            this.Append<Ship>(ship);
        }

        public SetControllerFocus(byte[] b, int lenght)
            : base(b, lenght)
        {
            Game1.AssertIsNotServer();
        }

        public override void Apply(ClientGame game)
        {
            Game1.AssertIsNotServer();
            this.ResetReader();
            int id = this.ReadInt();
            GameObjectReference<Ship> ship = this.ReadGameObjectReference<Ship>();

            StaticControllerFocus.SetFocus(id, ship.Dereference());
        }
    }
}