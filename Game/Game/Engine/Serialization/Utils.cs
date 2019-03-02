using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.Engine.Serialization
{
    static class Utils
    {
        public static T Deserialize<T>(byte[] buffer, ref int bufferOffset) where T : Serializable, new()
        {
            T obj = new T();
            bufferOffset = obj.Deserialize(buffer, bufferOffset);
            return obj;
        }
    }
}
