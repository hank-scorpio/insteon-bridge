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
    public enum MessageFlags : byte
    {
       	Default		= MaxHop3 | HopLeft3,
    	Broadcast	= 0x80, // bit 8
    	Nak			= 0x80, // bit 8
    	AllLink		= 0x40, // bit 7
    	Ack			= 0x20, // bit 6
    	ExtendedMsg	= 0x10,	// bit 5,
    	HopLeft3	= 0x0C,	// bit 3+4
    	HopLeft2	= 0x08,	// bit 4,
    	HopLeft1	= 0x04,	// bit 3,
    	MaxHop3		= 0x03,	// bit 1+2
    	MaxHop2		= 0x02, // bit 2
    	MaxHop1		= 0x01, // bit 1
    
    	None		= 0x00,
 
    }
}
