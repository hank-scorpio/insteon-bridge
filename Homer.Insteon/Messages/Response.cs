using System;

namespace Homer.Insteon
{
    public class Response
    {
        const string AlreadyInitializedMessage = "Already initialized; Initialize() cannot be called more than once.";
        const SendMessageResult OK = SendMessageResult.OK;


        public SendMessageResult    Result          { get; protected set; }
        public bool                 Succeeded       => Result == OK;
        protected byte[]            ResponseBytes;

        protected byte? this[int index] 
            => ResponseBytes?[index];


        bool initialized = false;

        public void Initialize(SendMessageResult result, byte[] data = null)
        {
            if (initialized)
                throw new InvalidOperationException(AlreadyInitializedMessage);

            if ((Result = result) == OK)
            {
                ResponseBytes = data;
            }
            initialized = true;
        }


    }
}