using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.Engine.GameState.Instants;
using static MyGame.Engine.GameState.GameObject;

namespace MyGame.Engine.GameState.GameObjectUtils
{
    public class ReferenceField<SubType> : GenericField<GameObjectReference<SubType>> where SubType : GameObject
    {
        public ReferenceField(CreationToken creationToken) : base(creationToken)
        {
        }

        public SubType this[CurrentInstant current]
        {
            get
            {
                return this.GetValue(current).Dereference(current.InstantSet);
            }
        }

        public SubType this[NextInstant next]
        {
            set
            {
                this.SetValue(next, value);
            }
        }

        internal override GameObjectReference<SubType> Copy(GameObjectReference<SubType> value)
        {
            return value;
        }

        internal override GameObjectReference<SubType> Deserialize(byte[] buffer, ref int bufferOffset)
        {
            return new GameObjectReference<SubType>(buffer, ref bufferOffset);
        }

        internal override GameObjectReference<SubType> NewDefaultValue()
        {
            return new GameObjectReference<SubType>(null);
        }

        internal override int SerializationSize(GameObjectReference<SubType> value)
        {
            return value.SerializationSize;
        }

        internal override void Serialize(GameObjectReference<SubType> value, byte[] buffer, ref int bufferOffset)
        {
            value.Serialize(buffer, ref bufferOffset);
        }
    }
}
