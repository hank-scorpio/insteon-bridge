using System;
using static Homer.Insteon.SendMessageResult;

namespace Homer.Insteon
{
    public class Response
    {
        public SendMessageResult Result { get; protected set; }
        public bool Initialized { get; protected set; }

        protected byte[] ResponseBytes { get; set; }

        protected byte? this[int index] 
            => ResponseBytes?[index];

        public bool Succeeded
            => Result == OK;

        public void Initialize(SendMessageResult result, byte[] data = null)
        {
            if (Initialized)
                throw new InvalidOperationException("Already initialized; Initialize() cannot be called more than once.");

            Result = result;
            ResponseBytes = result == OK ? data : null;
            Initialized = true;
        }


    }
}