using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyGame.GameStateObjects.DataStuctures
{
    interface GameObjectListInterface
    {
        void Add(GameObject obj);
        void Remove(GameObject obj);
        Type GetListType();
        List<GameObject> GetList();
    }
}
