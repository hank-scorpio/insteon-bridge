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
using static Homer.Insteon.SendMessageResult;

namespace Homer.Insteon
{
    public class InsteonController : InsteonDevice, IDisposable
    {
        public InsteonControllerOptions         Options         { get; } = InsteonControllerOptions.Default;
        private AwaitableSynchronuousQueue      CommandsQueue   { get; } = new AwaitableSynchronuousQueue();
        private InsteonBridgeStream             Stream          { get; }

        #region Constructors

        public InsteonController(InsteonBridgeStream stream, InsteonId? address = null, string name = null)
            : base(address?.Value ?? InsteonId.Zero, null, name)
        {
            Stream = stream;
    	}

        #endregion


        #region [public] Dispose / Wait

        public void Dispose() 
            => CommandsQueue.Dispose();

        public async Task Run() 
            => await CommandsQueue.Wait();

        #endregion


        #region [public] Light Switch Commands

        public Task<LightStatus> GetStatus(InsteonId dst)
            => Send<LightStatus>(dst, Command.GetLightStatus);

        public Task<LightStatus> SetFullOn(InsteonId dst)
            => Send<LightStatus>(dst, Command.On);

        public Task<LightStatus> SetOff(InsteonId dst)
            => Send<LightStatus>(dst, Command.Off);

        public Task<LightStatus> SetFastOn(InsteonId dst)
            => Send<LightStatus>(dst, Command.FastOn);

        public Task<LightStatus> SetFastOff(InsteonId dst)
            => Send<LightStatus>(dst, Command.FastOff);

        public Task<LightStatus> Brighten(InsteonId dst)
            => Send<LightStatus>(dst, Command.Brighten);

        public Task<LightStatus> Dim(InsteonId dst)
            => Send<LightStatus>(dst, Command.Dim);

        public Task<LightStatus> SetLevelAsync(InsteonId dst, byte level)
            => Send<LightStatus>(dst, Command.SetLevel, level);

        public Task<LightStatus> SetLevelAsync(InsteonId dst, double pct)
            => SetLevelAsync(dst, LightStatus.PctToLevel(pct));

        public Task<LightStatus> StartRampAsync(InsteonId dst, RampDirection dir)
            => Send<LightStatus>(dst, Command.RampStart, (byte)dir);

        public Task<LightStatus> StopRampAsync(InsteonId dst)
            => Send<LightStatus>(dst, Command.RampStop);

        public Task<LightStatus> RampUp(InsteonId dst, int durationMs = 0)
            => RampAsync(dst, RampDirection.Up, durationMs);

        public Task<LightStatus> RampDown(InsteonId dst, int durationMs = 0)
            => RampAsync(dst, RampDirection.Down, durationMs);

        public async Task<LightStatus> RampAsync(InsteonId dst, RampDirection dir, int durationMs = 0)
        {
            LightStatus s = await StartRampAsync(dst, dir);
            if (durationMs > 0)
            {
                await Task.Delay(durationMs);
                s = await StopRampAsync(dst); 
            }
            return s;
        }
        #endregion


        #region [public] Send

        public async Task<R> Send<R>(CommandMessage cmd) where R : CommandResponse, new()
            => await CommandsQueue.Enqueue(() => SendCommand<R>(cmd));

        public Task<CommandResponse> Send(InsteonId dst, Command cmd, byte value = 0, MessageFlags flags = MessageFlags.Default)
            => Send<CommandResponse>(dst, cmd, value, flags);

        public Task<CommandResponse> Send(CommandMessage cmd)
            => Send<CommandResponse>(cmd);

        public Task<R> Send<R>(InsteonId dst, Command cmd, byte value = 0, MessageFlags flags = MessageFlags.Default) where R : CommandResponse, new()
            => Send<R>(new CommandMessage(dst, cmd, value, flags));

        #endregion


        #region [private] SendCommand

        R SendCommand<R>(CommandMessage cmd) where R : CommandResponse, new()
        {
            R r;
            Stopwatch sw = Stopwatch.StartNew();
            try
            {
                r = SendCommand<R>(Stream, cmd, Options);
            }
            catch (Exception e) 
            {
                switch (Options.SendCommandExceptionHandling)
                {
                    case InsteonControllerOptions.ExceptionHandling.ReturnDefault:
                        return null;
                    case InsteonControllerOptions.ExceptionHandling.Rethrow:
                        throw;
                    default:
                        throw new IOException("Failed to send command. See inner exception for more information.", e);
                }
            }
            Debug.WriteLine($@"[{DateTime.Now:HH:mm:ss.fff}] {sw.ElapsedMilliseconds} ms | 
                {Address} -> {cmd.Destination} | {cmd.Command}({cmd.Value}) = [{r?.Result.ToString() ?? "ERROR"}]");

            return r;
        }

        static R SendCommand<R>(InsteonBridgeStream stream, CommandMessage msg, InsteonControllerOptions opt) where R : CommandResponse, new()
        {
            int delay = opt.RetryStartDelayMs;
            int timeout = opt.TimeoutStartMs;
            int retry = 0;
            SendMessageResult r = OK;

            while (retry++ < opt.MaxRetries)
            {
                if ((r = SendCommand(stream, msg, timeout, out byte[] data)) == OK)
                {
                    return msg.CreateResponse<R>(data);
                }
                Thread.Sleep(delay);
                timeout += opt.TimeoutStepMs;
                delay += opt.RetryStepMs;
            }
            return msg.CreateResponse<R>(r);
            
        }

        static SendMessageResult SendCommand(InsteonBridgeStream stream, CommandMessage msg, int timeout, out byte[] data)
        {
            SendMessageResult r = WriteMessage(stream, msg.GetBytes, timeout);
            if (r != OK)
            {
                data = null;
                return r;
            }
            return ReadResponse(stream, msg.GetResponseHeaderBytes, timeout, CommandResponse.Length, out data);
        }

        #endregion


        #region [private] WriteMessage / ReadResponse

        static SendMessageResult WriteMessage(InsteonBridgeStream stream, byte[] msg, int timeout)
        {
            if (!stream.DiscardExisting() || !stream.Wri­teBytes(msg))
                return WriteException;

            SendMessageResult r = ReadResponse(stream, msg, timeout, 1, out byte[] ack);

            if (r != OK)
                return r;
            if (ack?[0] != Message.ACK)
                return EchoInvalid;

            return OK;
        }

        static SendMessageResult ReadResponse(InsteonBridgeStream stream, byte[] msg, int timeout, int responseLen, out byte[] response)
        {
            SendMessageResult r = ReadResponseHeader(stream, msg, timeout);

            if (r != OK)
            {
                response = null;
                return r;
            }

            response = stream.ReadBytes(responseLen, timeout);

            if (response == null)
                return ResponseNull;
            if (response.Length < responseLen)
                return ResponseInvalid;

            return OK;
        }

        static SendMessageResult ReadResponseHeader(InsteonBridgeStream stream, byte[] bytes, int timeout)
        {
            byte[] rx = stream.ReadBytes(bytes.Length, timeout);

            if (rx?.Length == 0)
                return ResponseNull;
            if (rx[0] == Message.NAK)
                return ResponseNak;
            if (!Enumerable.SequenceEqual(bytes, rx))
                return ResponseInvalid;
      
            return OK;
        }

        #endregion

	}

}
