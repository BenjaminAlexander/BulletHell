using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.Engine.GameState.Fields
{
    public class CreationToken
    {
        private GameObject obj;

        internal CreationToken(GameObject obj)
        {
            this.obj = obj;
        }

        internal GameObject Object
        {
            get
            {
                return obj;
            }
        }
    }
}
