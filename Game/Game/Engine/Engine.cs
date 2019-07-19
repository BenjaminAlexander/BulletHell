using MyGame.Engine.GameState;
using MyGame.Engine.GameState.InstantObjectSet;
using MyGame.Engine.GameState.Instants;
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
        internal ObjectInstantManager objectInstantManager;

        public Engine()
        {
            typeManager = new TypeManager();
            this.AddTypes(typeManager);
            objectInstantManager = new ObjectInstantManager(typeManager);
            
        }

        internal void ServerInitializeInstantZero()
        {
            InstantSet instantZero = objectInstantManager.GetInstantSet(0);
            //TODO: this line is ugly
            ServerInitializeInstantZero(new NextInstant(instantZero, instantZero.NewObjectFactory(0)));
        }

        public abstract void AddTypes(TypeManagerInterface typeManager);

        public abstract void ServerInitializeInstantZero(NextInstant instantZero);
    }
}
