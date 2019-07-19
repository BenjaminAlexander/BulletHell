using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.Engine.Serialization;

namespace MyGame.Engine.GameState.FieldValues
{
    //This interface is here just in case FieldValue needs to define more methods than are in Serializable
    public interface FieldValue : Serializable, Deserializable
    {

    }
}
