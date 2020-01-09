using System;
using System.Linq;
using System.Runtime.InteropServices;
using static Homer.Insteon.MessageType;
using static Homer.Insteon.SendMessageResult;

namespace Homer.Insteon
{

    public struct CommandMessage
    {
        public InsteonId       Destination  { get; }
        public MessageFlags    Flags        { get; }
        public Command         Command      { get; }
        public byte            Value        { get; }
      
        public CommandMessage(InsteonId dst, Command cmd, byte value = 0, MessageFlags flags = MessageFlags.Default)
        {
            Destination = dst;
            Command = cmd;
            Value = value;
            Flags = flags;
        }

        public override string ToString() 
            => $" -> {Destination} | {Command.ToString()}({Value:X2}) | flags={Flags}";
 
        public byte[] GetBytes
            => Message.GetBytes(CommandSend, Destination, Flags, Command, Value).ToArray();

        public byte[] GetResponseHeaderBytes
            => Message.GetBytes(CommandReceived, Destination).ToArray();

        public T CreateResponse<T>(byte[] data)  where T : CommandResponse, new()
		    => CreateResponse<T>(data?.Length == CommandResponse.Length ? OK : Invalid, data);

		public T CreateResponse<T>(SendMessageResult result, byte[] data = null)  where T : CommandResponse, new()
		    => (T)new T().Initialize(this, result, data);
        

    }
}