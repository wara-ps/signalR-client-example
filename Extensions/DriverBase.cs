using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace boargame.Extensions
{
    /// <summary>
    /// This class is a variant of BackgroundService from
    /// Microsoft.Extensions.Hosting where the service lifetime is divided
    /// into Startup, Execution and Shutdown phases.
    /// </summary>
    public abstract class DriverBase : IHostedService, IDisposable
    {
        protected readonly ILogger _logger = null;
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        private Task _task = null;

        public DriverBase(ILogger logger)
        {
            _logger = logger;
        }

        protected abstract Task StartupDriverAsync(CancellationToken token);
        protected abstract Task ExecuteDriverAsync(CancellationToken token);
        protected abstract Task ShutdownDriverAsync(CancellationToken token);

        /// <summary>
        /// Triggered when the application host is ready to start the service.
        /// </summary>
        /// <param name="token">Indicates that the start process has been aborted.</param>
        public virtual Task StartAsync(CancellationToken token)
        {
            // Store the task we're executing
            _task = RunServiceAsync(token);

            // If the task is completed then return it, this will bubble cancellation and failure to the caller
            if (_task.IsCompleted)
            {
                return _task;
            }

            // Otherwise it's running
            return Task.CompletedTask;
        }

        /// <summary>
        /// Triggered when the application host is performing a graceful shutdown.
        /// </summary>
        /// <param name="token">Indicates that the shutdown process should no longer be graceful.</param>
        public virtual async Task StopAsync(CancellationToken token)
        {
            // Stop called without start
            if (_task == null)
            {
                return;
            }

            try
            {
                // Signal cancellation to the executing method
                _cts.Cancel();
            }
            finally
            {
                // Wait until the task completes or the stop token triggers
                await Task.WhenAny(_task, Task.Delay(Timeout.Infinite, token));
            }

            await ShutdownInternalAsync(token);
        }

        /// <summary>
        /// Initiate driver startup and execution.
        /// </summary>
        /// <param name="token">Indicates that the startup process has been aborted.</param>
        /// <returns></returns>
        private async Task RunServiceAsync(CancellationToken token)
        {
            await Task.Yield();

            try
            {
                await StartupInternalAsync(token);
                await ExecuteInternalAsync(_cts.Token);
            }
            finally
            {
                await ShutdownInternalAsync(_cts.Token);
            }
        }

        public virtual void Dispose()
        {
            _cts.Cancel();
        }

        private async Task StartupInternalAsync(CancellationToken token)
        {
            _logger.LogTrace("Driver startup initiated.");

            try
            {
                await StartupDriverAsync(token);
            }
            catch (Exception e)
            {
                if (!(e is TaskCanceledException))
                {
                    _logger.LogError(e, "Driver startup threw an unhandled exception.");
                }
                throw;
            }

            _logger.LogTrace("Driver startup finished.");
        }

        private async Task ExecuteInternalAsync(CancellationToken token)
        {
            _logger.LogTrace("Driver execution initiated.");

            try
            {
                await ExecuteDriverAsync(token);
            }
            catch (Exception e)
            {
                if (!(e is TaskCanceledException))
                {
                    _logger.LogError(e, "Driver execution threw an unhandled exception.");
                }
                throw;
            }

            _logger.LogTrace("Driver execution finished.");
        }

        private async Task ShutdownInternalAsync(CancellationToken token)
        {
            _logger.LogTrace("Driver shutdown initiated.");

            try
            {
                await ShutdownDriverAsync(token);
            }
            catch (TaskCanceledException e)
            {
                _logger.LogError(e, $"Driver failed to shutdown gracefully.");
            }

            _logger.LogTrace("Driver shutdown finished.");
        }
    }
}
