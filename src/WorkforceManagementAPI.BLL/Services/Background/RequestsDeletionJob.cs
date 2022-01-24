using Quartz;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using WorkforceManagementAPI.BLL.Managers;

namespace WorkforceManagementAPI.BLL.Services.Background
{
    [ExcludeFromCodeCoverage]
    [DisallowConcurrentExecution]
    public class RequestsDeletionJob : IJob
    {
        private static string jobGroupName = "PrintJobs";
        private static string userIdKey = "userId";
        private ITimeOffService _timeOffService;
        private IUserManager _userManager;

        public RequestsDeletionJob(ITimeOffService timeOffService, IUserManager userManager)
        {
            _timeOffService = timeOffService;
            _userManager = userManager;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var userId = context.JobDetail.JobDataMap.GetString(userIdKey);
            var user = await _userManager.FindByIdAsync(userId);
            int requestsCount = user.CreatedRequests.Count;

            for (int i = 0; i < requestsCount; i++)
            {
                await _timeOffService.Delete(user.CreatedRequests[i].Id);
            }
        }

        public static ITrigger GetTrigger()
        {
            string id = Guid.NewGuid().ToString();
            DateTimeOffset startTime = DateTime.Now.AddMonths(6);
            return TriggerBuilder.Create().WithIdentity(id, jobGroupName).StartAt(startTime).Build();
        }

        public static IJobDetail CreateJob(string userId)
        {
            string id = Guid.NewGuid().ToString();
            return JobBuilder.Create<RequestsDeletionJob>().WithIdentity(id, jobGroupName).UsingJobData(userIdKey, userId).Build();
        }
    }
}
