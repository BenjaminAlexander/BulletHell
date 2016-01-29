using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyGame.GameStateObjects.DataStuctures
{
    interface GameObjectListInterface
    {
        void Add(GameObject obj);
        Boolean Remove(GameObject obj);
        Type GetListType();
        List<GameObject> GetList();
    }
}
