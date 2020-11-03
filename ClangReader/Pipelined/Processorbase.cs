using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ClangReader.Pipelined
{
    public abstract class Processorbase
    {
        private readonly int _parallelTaskCount;

        private protected List<Task> _taskList = new List<Task>();

        //private SemaphoreSlim _syncLock = new SemaphoreSlim(1, 1);
        private volatile bool _isStarted = false;

        private object _syncLock = new object();

        public Processorbase(int parallelTaskCount)
        {
            if (parallelTaskCount < 1)
            {
                throw new ArgumentException("Cannot be smaller than 1", nameof(parallelTaskCount));
            }
            _parallelTaskCount = parallelTaskCount;
        }

        public virtual Task StartAsync(CancellationToken cancellation)
        {
            if (IsStarted)
            {
                throw new Exception("Process already started");
            }

            for (int i = 0; i < _parallelTaskCount; i++)
            {
                _taskList.Add(Task.Run(async () => await RunAsync(cancellation), cancellation));
            }
            return Task.CompletedTask;
        }

        private protected abstract Task RunAsync(CancellationToken cancellation);

        private bool IsStarted
        {
            get
            {
                if (_isStarted)
                {
                    return true;
                }

                lock (_syncLock)
                {
                    return _isStarted;
                }
            }
        }
    }
}