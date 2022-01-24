using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Spi;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace WorkforceManagementAPI.BLL.Services.Background
{
    [ExcludeFromCodeCoverage]
    public class QuartzHostedService : IHostedService
    {
        private IScheduler _scheduler;
        private IJobFactory _jobFactory;
        public QuartzHostedService(IScheduler scheduler, IJobFactory jobFactory)
        {
            _scheduler = scheduler;
            _jobFactory = jobFactory;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _scheduler.JobFactory = _jobFactory;
            await _scheduler.Start(cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _scheduler.Shutdown(cancellationToken);
        }
    }
}
