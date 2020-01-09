using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Homer.Insteon
{
    public class AwaitableSynchronuousQueue : IDisposable
    {
        private Task consumer = null;

        BlockingCollection<Task>    Queue           { get; } = new BlockingCollection<Task>();
        CancellationTokenSource     Cancellation    { get; } = new CancellationTokenSource();

        void EnsureConsumerInitialized()
            => LazyInitializer.EnsureInitialized(ref consumer, InitializeConsumer);

        Task InitializeConsumer()
            => Task.Factory.StartNew(ConsumeUntilCancelled, Cancellation.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);

        void ConsumeUntilCancelled()
        {
            try
            {
                foreach (Task task in Queue.GetConsumingEnumerable(Cancellation.Token))
                { 
                    task.RunSynchronously();
                }
            }
            catch (OperationCanceledException) { }
        }

        public Task<R> Enqueue<R>(Func<R> fn)
        {
            EnsureConsumerInitialized();
            Task<R> task = new Task<R>(fn, Cancellation.Token);
            Queue.Add(task);
            return task;
        }

        public async Task Wait() 
            => await consumer;

        public void Cancel()
        {
            if (Cancellation.IsCancellationRequested) return;
            Cancellation.Cancel();
        }

        public void Dispose() 
            => Cancel();
    }
}