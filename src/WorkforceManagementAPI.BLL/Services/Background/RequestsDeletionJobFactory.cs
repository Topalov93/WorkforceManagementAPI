using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Spi;
using System;
using System.Diagnostics.CodeAnalysis;
using WorkforceManagementAPI.BLL.Managers;

namespace WorkforceManagementAPI.BLL.Services.Background
{
    [ExcludeFromCodeCoverage]
    public class RequestsDeletionJobFactory : IJobFactory
    {
        private IServiceProvider _serciveProvider;
        public RequestsDeletionJobFactory(IServiceProvider serviceProvider)
        {
            _serciveProvider = serviceProvider;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            var timeOffService = _serciveProvider.GetService<ITimeOffService>();
            var userManager = _serciveProvider.GetService<IUserManager>();

            return new RequestsDeletionJob(timeOffService, userManager);
        }

        public void ReturnJob(IJob job)
        {

        }
    }
}
