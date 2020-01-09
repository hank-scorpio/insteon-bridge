using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Homer.Insteon
{
    public static class Message
    {

        public const byte STX  = 0x02;
        public const byte NAK  = 0x15;
        public const byte ACK  = 0x06;
        public const int  END  = -1;

        public static IEnumerable<byte> GetBytes(MessageType type, InsteonId dst, params object[] bytes)
            => new[] { STX, (byte)type, dst.A1, dst.A2, dst.A3 }.Concat(bytes.Select(Convert.ToByte));
    }
}
