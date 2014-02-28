using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.GameStateObjects;
using MyGame.GameStateObjects.DataStuctures;
namespace MyGame.Networking
{
    public class GameObjectUpdate : GameUpdate
    {
        Type type;
        int id;
        public Type GameObjectType
        {
            get { return type; }
        }

        public int ID
        {
            get { return id; }
        }

        public GameObjectUpdate(GameObject obj)
        {
            type = obj.GetType();
            id = obj.ID;
            this.Append(obj.GetTypeID());
            this.Append(obj.ID);
        }

        public GameObjectUpdate(byte[] b, int length)
            : base(b, length)
        {
            this.ResetReader();
            type = GameObjectTypes.GetType(this.ReadInt());
            id = this.ReadInt();
        }

        public override void Apply(Game1 game)
        {
            if (!Game1.IsServer)
            {
                GameObjectCollection collection = StaticGameObjectCollection.Collection;
                if (collection.Contains(this.id))
                {
                    collection.Get(this.id).UpdateMemberFields(this);
                }
                else
                {

                    Type[] constuctorParamsTypes = new Type[1];
                    constuctorParamsTypes[0] = typeof(GameObjectUpdate);

                    System.Reflection.ConstructorInfo constructor = this.GameObjectType.GetConstructor(constuctorParamsTypes);
                    if (constructor == null)
                    {
                        throw new Exception("Game object must have constructor GameObject(int)");
                    }
                    object[] constuctorParams = new object[1];
                    constuctorParams[0] = this;
                    GameObject obj = (GameObject)constructor.Invoke(constuctorParams);
                    //obj.UpdateMemberFields(this);
                    collection.Add(obj);

                }
            }
        }
    }
}
