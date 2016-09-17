using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Homer.Insteon
{
    public class InsteonTelnetStream : InsteonBridgeStream
    {
        public string Host { get; protected set; }
        public int Port { get; protected set; }

        public const int DefaultPort = 9761;

		public InsteonTelnetStream(string host, int port = DefaultPort)
		{
            Host = host;
            Port = port;
		}

        private TcpClient client;

        NetworkStream GetStream()
        {
            // Client disconnected: dispose
            if (client != null && !client.Connected)
            {
                client.Close();
                client = null;
            }
            // Client not created: create + connect
            if (client == null)
            {
                client = new TcpClient();
                client.Connect(Host, Port);

            }
            return client.GetStream();
        }


        public override byte[] ReadBytes(int n, int timeout = DefaultReadTimeout)
        {

            try
            {
                Stream stream = GetStream();
                byte[] rx = new byte[n];
                stream.ReadTimeout = timeout;
                n = stream.Read(rx, 0, n);
                if (n < rx.Length) Array.Resize(ref rx, n);
                return rx;
            }
            catch (TimeoutException) { }
            catch (IOException) { }
     
            return null;
        }

        public override bool WriteBytes(byte[] msg, int timeout = DefaultWriteTimeout)
        {
            try
            {
                var stream = GetStream();
                stream.WriteTimeout = timeout;
                stream.Write(msg, 0, msg.Length);
                return true;
            }
            catch (TimeoutException) { }
            catch (IOException) { }
     
            return false;
        }

        public override bool DiscardExisting()
        {
            try
            {
                var stream = GetStream();
                while (stream.DataAvailable)
                {
                    stream.ReadByte();
                }
                return true;
            }
            catch (TimeoutException) { }
            catch (IOException) { }

            return false;
        }


    }
}
