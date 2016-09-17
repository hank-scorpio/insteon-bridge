using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Homer.Insteon
{
    public enum MessageType : byte
    {
    	Unknown					= 0x00,
    	CommandReceived			= 0x50, // 02 50 | src | dst | flags | cmd1 | cmd2
    	IMGetInfo				= 0x60,
    	SendAllLinkCommand		= 0x61,
    	CommandSend				= 0x62,	// Send INSTEON Standard or Extended Message
    	SendX10Command			= 0x63,
    	IMReset					= 0x67,
    	GetFirstAllLinkRecord	= 0x69,
    	GetNextAllLinkRecord	= 0x6A
    }
}
