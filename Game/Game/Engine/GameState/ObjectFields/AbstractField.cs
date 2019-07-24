using System.Collections.Generic;

namespace MyGame.Engine.GameState.ObjectFields
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

        internal bool IsInstantDeserialized(int instantId)
        {
            return gameObject.IsInstantDeserialized(instantId);
        }

        internal abstract bool HasInstant(int instantId);

        internal abstract void SetDefaultValue(int instantId);

        internal abstract void RemoveInstant(int instantId);

        internal abstract void CopyFieldValues(int fromInstantId, int toInstantId);

        internal abstract int SerializationSize(int instantId);

        internal abstract void Serialize(int instantId, byte[] buffer, ref int bufferOffset);

        /**
        * Returns True if Values were changed
        */
        internal abstract bool Deserialize(int instantId, byte[] buffer, ref int bufferOffset);

        internal abstract bool IsIdentical(int instantId, AbstractField other, int otherInstantId);

        internal abstract List<int> GetInstantSet();
    }
}
