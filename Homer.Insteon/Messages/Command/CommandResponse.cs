using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Homer.Insteon
{
	public class CommandResponse : Response
	{
        public const int        Length = 6;
        public CommandMessage   Message { get; protected set; }
        public InsteonId        Source => Message.Destination;

        public CommandResponse Initialize(CommandMessage msg, SendMessageResult result, byte[] data = null)
		{
            Initialize(result, data);
			Message = msg;
            return this;
		}
    }
}
