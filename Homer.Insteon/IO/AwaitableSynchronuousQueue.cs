using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Homer.Insteon
{
    public class AwaitableSynchronuousQueue : IDisposable
    {
        Task consumer = null;
        readonly BlockingCollection<Task> queue = new BlockingCollection<Task>();
        readonly CancellationTokenSource cancellation = new CancellationTokenSource();

        void EnsureConsumerInitialized()
        {
            if (consumer != null) return;
            consumer = new Task(ConsumeUntilCancelled, cancellation.Token, TaskCreationOptions.LongRunning);
            consumer.Start();
        }

        void ConsumeUntilCancelled()
        {
            try
            {
                foreach (Task t in queue.GetConsumingEnumerable(cancellation.Token))
                { 
                    t.RunSynchronously();
                }
            }
            catch (OperationCanceledException) { }
        }

        public Task<R> Enqueue<R>(Func<R> fn)
        {
            EnsureConsumerInitialized();
            var task = new Task<R>(fn, cancellation.Token);
            queue.Add(task);
            return task;
        }

        public async Task Wait()
        {
            await consumer;
        }

        public void Cancel()
        {
            if (cancellation.IsCancellationRequested) return;
            cancellation.Cancel();
        }

        public void Dispose()
        {
            Cancel();
        }
    }
}