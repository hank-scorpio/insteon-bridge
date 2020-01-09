using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace Homer.Insteon
{

    public class Hub : InsteonController
    {
        #region Constructors

        public Hub(string host, string username, string password, int port = BufferStatusHttpStream.DefaultPort, InsteonId? address = null, string name = null)
            : base(new BufferStatusHttpStream(host, username, password, port), address, name)
        {

        }

        #endregion
    }

}