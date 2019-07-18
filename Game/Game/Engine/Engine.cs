using MyGame.Engine.GameState;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.Engine
{
    public abstract class Engine
    {
        private TypeManager typeManager;
        private ObjectInstantManager objectInstantManager;

        public Engine()
        {
            typeManager = new TypeManager();
            this.AddTypes(typeManager);
            objectInstantManager = new ObjectInstantManager(typeManager);
        }

        public abstract void AddTypes(TypeManagerInterface typeManager);
    }
}
