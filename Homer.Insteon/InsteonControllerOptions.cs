using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace Homer.Insteon
{
    public class InsteonControllerOptions
    {
        public static InsteonControllerOptions Default { get; } = new InsteonControllerOptions();

        public int MaxRetries        { get; set; } = 6;
        public int RetryStartDelayMs { get; set; } = 200;
        public int TimeoutStartMs    { get; set; } = 1500;
        public int TimeoutStepMs     { get; set; } = 500;
        public int RetryStepMs       { get; set; } = 100;
        public int CommandQueueSize  { get; set; } = 128;

        public ExceptionHandling SendCommandExceptionHandling { get; set; }  = ExceptionHandling.Throw;

        public enum ExceptionHandling
        {
            Throw,
            Rethrow,
            ReturnDefault
        }
    }
}