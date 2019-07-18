using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.Engine.GameState
{
    public interface TypeManagerInterface
    {
        void AddType<SubType>() where SubType : GameObject, new();
    }
}
