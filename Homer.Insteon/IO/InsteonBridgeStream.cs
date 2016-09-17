using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Homer.Insteon
{

    public abstract class InsteonBridgeStream
    {
        public const int DefaultReadTimeout = 5000;
        public const int DefaultWriteTimeout = 5000;
        public abstract byte[] ReadBytes(int n, int timeout = DefaultReadTimeout);
        public abstract bool WriteBytes(byte[] bytes, int timeout = DefaultWriteTimeout);
        public abstract bool DiscardExisting();
    }

}