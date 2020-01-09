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

    public class SmartLinc : InsteonController
    {
        #region Constructors

        public SmartLinc(string host, InsteonId? address = null, string name = null)
            : base(new InsteonTcpStream(host), address, name)
        {

        }

        #endregion
    }

}