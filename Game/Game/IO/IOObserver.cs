using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyGame.IO
{
    public interface IOObserver
    {
        void UpdateWithIOEvent(IOEvent ioEvent);
    }
}
