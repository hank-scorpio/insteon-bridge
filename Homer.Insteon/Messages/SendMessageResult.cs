using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Homer.Insteon
{
	[Flags]
    public enum SendMessageResult : byte
    {
		None			= 0x00,
        Invalid         = 0x01,
		OK				= 0x08,
        WriteException  = 0x11,
		EchoInvalid		= 0x21,
        ResponseInvalid	= 0x31,
		ResponseNak		= 0x32,
        ResponseNull    = 0x33,
    }
}
