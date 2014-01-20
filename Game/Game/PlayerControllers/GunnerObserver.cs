using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyGame.PlayerControllers
{
    interface GunnerObserver
    {
        void UpdateWithEvent(GunnerEvent e);
    }
}
