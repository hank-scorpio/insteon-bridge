using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Globalization;
using System.Xml.Linq;
using System.Threading;
using System.Diagnostics;

namespace Homer.Insteon
{

    public class BufferStatusHttpStream : InsteonBridgeStream
    {
        NetworkCredential Credentials { get; }
        public string Host { get; }
        public int Port { get;  }

        public const int DefaultPort = 25105;
        public TimeSpan PollInterval { get; } = TimeSpan.FromMilliseconds(25);

        public BufferStatusHttpStream(string host, string username, string password, int port = DefaultPort)
            : this(host, new NetworkCredential(username, password), port)
        {
        }
        public BufferStatusHttpStream(string host, NetworkCredential credentials, int port = DefaultPort)
        {
            Host = host;
            Port = port;
            Credentials = credentials;
        }


        //Task<string> HttpPost(string url) 
        //    => HttpRequest(c => c.UploadStringTaskAsync(url, ""));
        Task<string> HttpGet(string url)
            => HttpRequest(c => c.DownloadStringTaskAsync($"http://{Host}:{Port}/{url}"));

        Task<T> HttpRequest<T>(Func<WebClient, Task<T>> request)
        {
            using (var c = new WebClient())
            {
                c.Credentials = Credentials; 
                return request(c);
            }
        }

   
        int bufferPos = 0;
        int bufferLen = 0;
        byte[] buffer = new byte[4096];
        int BytesAvailable => bufferLen - bufferPos;
       
        async Task<string> GetBufferStatus() 
            => XElement.Parse(await HttpGet("buffstatus.xml"))?.Element("BS")?.Value.Trim();
  

        async Task UpdateBuffer()
        {
            var bs = await GetBufferStatus();
            if (bs == null) return;
            int len = byte.Parse(bs.Substring(bs.Length - 2), NumberStyles.HexNumber) / 2;

            //Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff]} UpdateBuffer | {len,-2} | {bs}");
            while (bufferLen < len)
            {
                buffer[bufferLen] = byte.Parse(bs.Substring(bufferLen * 2, 2), NumberStyles.HexNumber);
                bufferLen++;
            }
        }
       
        public override byte[] ReadBytes(int n, int timeout = DefaultReadTimeout)
        {
            var sw = Stopwatch.StartNew();
            while (BytesAvailable == 0)
            {
                UpdateBuffer().Wait();
                if (BytesAvailable > 0) break;
                Task.Delay(PollInterval).Wait();
                if (sw.ElapsedMilliseconds > timeout) break;
            }
            if (BytesAvailable < n) n = BytesAvailable;
            byte[] ret = new byte[n];
            Array.Copy(buffer, bufferPos, ret, 0, n);
            bufferPos += n;
            return ret;

        }

        public override bool WriteBytes(byte[] msg, int timeout = DefaultWriteTimeout)
        {
            if (HttpGet($"3?{String.Concat(msg.Select(x => x.ToString("X2")))}=I=3") == null) return false;
            bufferPos = 0;
            bufferLen = 0;
            return true;
        }

        public override bool DiscardExisting()
            //=> GetUrl("1?XB=M=1") != null;
            => true;


    }

}