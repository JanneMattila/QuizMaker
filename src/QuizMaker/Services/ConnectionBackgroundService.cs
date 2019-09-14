using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using QuizMaker.Data;

namespace QuizMaker.Services
{
    public class ConnectionBackgroundService : IHostedService, IDisposable
    {
        private readonly IQuizDataContext _quizDataContext;
        private readonly ConnectionStorage _connectionStorage;
        private Timer? _timer;
        private int _previousCount = 0;

        public ConnectionBackgroundService(IQuizDataContext quizDataContext, ConnectionStorage connectionStorage)
        {
            _quizDataContext = quizDataContext;
            _connectionStorage = connectionStorage;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(UpdateAsync, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
            return Task.CompletedTask;
        }

        public async void UpdateAsync(object? state)
        {
            var count = _connectionStorage.Count();
            if (count > 0)
            {
                // Prevent current connection count from expiring
                await _quizDataContext.UpsertServerConnectionsAsync(count);
            }
            else if (_previousCount > 0 && count == 0)
            {
                await _quizDataContext.DeleteServerConnectionsAsync();
            }

            _previousCount = count;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            await _quizDataContext.DeleteServerConnectionsAsync();
        }

        #region IDisposable Support
        private bool _disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _timer?.Dispose();
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
