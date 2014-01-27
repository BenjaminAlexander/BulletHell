using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace NetworkLibrary
{
    public class FileIO
    {
        public static FileStream GetStream()
        {
            return new FileStream("MyFile.bin", FileMode.Create, FileAccess.Write, FileShare.None);
        }
    }
}
