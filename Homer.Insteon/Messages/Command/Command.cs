using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homer.Insteon
{
    // INSTEON Command Tables, Section 2.1.1 "Standard-length direct commands", p. 2

    // Standard-length direct commands
    public enum Command : byte
    {
        On             = 0x11,
        FastOn         = 0x12,
        Off            = 0x13,
        FastOff        = 0x14,
        Brighten       = 0x15,
        Dim            = 0x16,
        RampStart      = 0x17,
        RampStop       = 0x18,
        GetLightStatus = 0x19,
        SetLevel       = 0x21,
        RampOn         = 0x2E,
        RampOff        = 0x2F,
    
    }

}
