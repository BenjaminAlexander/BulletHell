using MyGame.Engine.GameState.InstantObjectSet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MyGame.Engine.GameState.TypeManager;

namespace MyGame.Engine.GameState
{
    interface TypeSetInterface : IEnumerable<GameObject>
    {
        GameObject GetObject(int id);
        bool CheckIntegrety();
        InstantTypeSetInterface NewInstantTypeSet();
        TypeMetadataInterface GetMetaData
        {
            get;
        }
    }
}
