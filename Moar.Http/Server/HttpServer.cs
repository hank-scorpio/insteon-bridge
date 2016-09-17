using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Moar.Http
{

	public class HttpServer
    {
		public const string	AnyHost				= "+";
		public const int	HttpPort			= 80;
		public const string	DefaulRealm			= "default-realm";
		public const int	ReceiveQueueSize	= 100;

		public IHttpHandler Handler { get; }
    	public bool			Started { get; protected set; }
    	public int			Port	{ get; }
    	public string		Host	{ get; }
        public string		Realm	{ get; }
        public string       Prefix => $"http://{Host}:{Port}/";



		HttpListener		    listener;
		Task				    processTask;
		CancellationTokenSource cancelSource;

		object startStopLock = new object();

		public HttpServer(IHttpHandler handler, int port = HttpPort, string host = AnyHost, string realm = DefaulRealm)
		{
			Handler = handler;
			Port = port;
			Host = host;
            Realm = realm;
		}

		public void Start()
		{
			lock (startStopLock)
			{
				if (Started) return;

				listener = InitializeListener();
				cancelSource = new CancellationTokenSource();
				processTask = Task.Factory.StartNew(ProcessRequests, cancelSource.Token, 
					TaskCreationOptions.LongRunning, TaskScheduler.Default);

				Started = true;
			}
		}

		HttpListener InitializeListener()
		{
			var l = new HttpListener()
			{
				Realm = this.Realm
			};
			l.Prefixes.Add(Prefix);
			l.Start();
			return l;
		}

		public void Stop()
		{
			lock (startStopLock)
			{
				if (!Started) return;

				listener.Stop();
				listener = null;

				cancelSource.Cancel();
				cancelSource = null;

				processTask.Wait();
				processTask  = null;
			
				Started		= false;
			}
		}

		public async Task Wait()
		{
            if (Started) await processTask;
		}


		void ProcessRequests()
		{
			// Loop while start
			while (!cancelSource.IsCancellationRequested)
			{
				// Wait for request (blocking)
				var listen = listener.GetContextAsync();
				listen.Wait(cancelSource.Token);

				if (!listen.IsCompleted) return;
			
				// Dispatch request handling (async)
				Task.Factory.StartNew(() =>
				{
					var c = new HttpServerContext(this, listen.Result);
					Handler.HandleRequest(c);
					c.Response.Close();
				});
				
			}

		}

	}
}
