using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SWD392_BE.Services.Interfaces;

namespace SWD392_BE.Services
{
    public class StoreStatusBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan[] _scheduledTimes;
        private readonly TimeZoneInfo _vietnamTimeZone;

        public StoreStatusBackgroundService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _scheduledTimes = new[]
            {
                new TimeSpan(4, 1, 0),    // 4:01 AM
                new TimeSpan(8, 36, 0),   // 8:36 AM
                new TimeSpan(11, 31, 0),  // 11:31 AM
                new TimeSpan(13, 00, 0),   // 4:01 PM
                new TimeSpan(23, 1, 0)    // 11:01 PM
            };

            _vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var nextRunTime = GetNextRunTime();
                var delay = nextRunTime - TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, _vietnamTimeZone);

                if (delay.TotalMilliseconds > 0)
                {
                    await Task.Delay(delay, stoppingToken);
                }

                using (var scope = _serviceProvider.CreateScope())
                {
                    var storeService = scope.ServiceProvider.GetRequiredService<IStoreService>();
                    await storeService.UpdateStoreStatusAsync();
                }
            }
        }

        private DateTime GetNextRunTime()
        {
            var now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, _vietnamTimeZone);
            var todayRunTimes = _scheduledTimes.Select(t => DateTime.Today.Add(t)).ToList();
            var nextRun = todayRunTimes.FirstOrDefault(t => t > now);

            if (nextRun == default)
            {
                nextRun = todayRunTimes.First().AddDays(1);
            }

            return nextRun;
        }
    }
}
