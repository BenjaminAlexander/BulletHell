using System.Collections.Generic;

namespace MyGame.Engine.GameState.GameObjectUtils
{
    //TODO: see if copied code in field subclasses can be moved into base
    public abstract class AbstractField
    {
        private GameObject gameObject;

        internal AbstractField(CreationToken creationToken)
        {
            creationToken.Object.AddField(this);
            this.gameObject = creationToken.Object;
        }

        internal void SetValue<T>(int instantId, GenericField<T> field, T value)
        {
            gameObject.SetValue<T>(instantId, field, value);
        }

        internal abstract void SetDefaultValue(int instantId);

        internal abstract void RemoveInstant(int instantId);

        internal abstract void CopyFieldValues(int fromInstantId, int toInstantId);

        internal abstract int SerializationSize(int instantId);

        internal abstract void Serialize(int instantId, byte[] buffer, ref int bufferOffset);

        /**
        * Returns True if Values were changed
        */
        internal abstract bool Deserialize(int instantId, byte[] buffer, ref int bufferOffset);
    }
}
