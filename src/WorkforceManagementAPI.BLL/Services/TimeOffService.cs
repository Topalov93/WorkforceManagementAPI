using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using WorkforceManagementAPI.BLL.Managers;
using WorkforceManagementAPI.BLL.Services.Background;
using WorkforceManagementAPI.Common.Enums;
using WorkforceManagementAPI.Common.ResultState;
using WorkforceManagementAPI.DAL.Entities;
using WorkforceManagementAPI.DAL.Repositories;

namespace WorkforceManagementAPI.BLL.Services
{
    public class TimeOffService : ITimeOffService
    {
        private ITimeOffRequestRepository _timeOffRequestRepository;
        private IUserManager _userManager;
        private IApprovalRespository _approvalRespository;
        private IHolidaysRepository _holidaysRepository;
        private readonly IEmailService _emailService;
        private IScheduler _scheduler;

        public TimeOffService(
            ITimeOffRequestRepository timeOffRequestRepository,
            IApprovalRespository approvalRespository,
            IUserManager userManager,
            IHolidaysRepository holidaysRepository,
            IEmailService emailService,
            IScheduler scheduler)
        {
            _timeOffRequestRepository = timeOffRequestRepository;
            _userManager = userManager;
            _approvalRespository = approvalRespository;
            _holidaysRepository = holidaysRepository;
            _emailService = emailService;
            _scheduler = scheduler;
        }

        public async Task<ResultState> Create(TimeOffRequest timeOffRequest, string currentUserId)
        {
            if (timeOffRequest.StartDate > timeOffRequest.EndDate)
                return new ResultState(false, "The start date of the request is after the end date!");

            User currentUser = await _userManager.FindByIdAsync(currentUserId);
            bool isRequestOverlapping = await IsOverlaping(currentUser, timeOffRequest);
            if (isRequestOverlapping) return new ResultState(false, "Can not create overlapping requests!");

            timeOffRequest.Creator = currentUser;
            timeOffRequest.Duration = await GetTimeOffWorkingDays(timeOffRequest.StartDate.Date, timeOffRequest.EndDate.Date);
            if (timeOffRequest.Duration == 0) return new ResultState(false, "Can not create request with duration of 0 days");
            timeOffRequest.Status = RequestStatus.Created;
            bool hasDecreasedDays = await DecreaseTimeOffDays(currentUser, timeOffRequest.Duration, timeOffRequest.Type);
            if (!hasDecreasedDays) return new ResultState(false, "Not enough days off!");

            try
            {
                await _timeOffRequestRepository.Create(timeOffRequest);
                return new ResultState(true, "Time off request created!");
            }
            catch (Exception ex)
            {
                return new ResultState(false, "Failed to create the time off request!", ex);
            }
        }

        public async Task<ResultState> Update(int timeOffRequestId, TimeOffRequest newRequest)
        {
            if (newRequest.Status != RequestStatus.Created) return new ResultState(false, "Can not update request!");
            if (newRequest.StartDate > newRequest.EndDate)
                return new ResultState(false, "The start date of the request is after the end date");

            try
            {
                var originalRequest = await _timeOffRequestRepository.GetSingle(r => r.Id == timeOffRequestId);
                if (originalRequest.Status != RequestStatus.Created)
                    return new ResultState(false, "Can only update requests with status created!");

                var user = originalRequest.Creator;
                bool isNewRequestOverlapping = await IsOverlaping(user, newRequest, originalRequest.Id);
                if (isNewRequestOverlapping) return new ResultState(false, "Can not create overlapping requests!");

                await _userManager.IncreaseRemainingDaysOff(user.Id, originalRequest.Duration, originalRequest.Type);

                var newDuration = await GetTimeOffWorkingDays(newRequest.StartDate.Date, newRequest.EndDate.Date);
                if (newDuration == 0)
                {
                    await _userManager.DecreaseRemainingDaysOff(user.Id, originalRequest.Duration, originalRequest.Type);
                    return new ResultState(false, "Can not create request with duration of 0 days");
                }
                var hasEnoughDays = await DecreaseTimeOffDays(user, newDuration, originalRequest.Type);
                if (hasEnoughDays)
                {
                    newRequest.Duration = newDuration;
                    await _timeOffRequestRepository.Update(timeOffRequestId, newRequest);
                }
                else
                {
                    await _userManager.DecreaseRemainingDaysOff(user.Id, originalRequest.Duration, originalRequest.Type);
                }
                return new ResultState(true, "Time off request updated!");
            }
            catch (Exception ex)
            {
                return new ResultState(false, "Failed to update the time off request!", ex);
            }
        }

        public async Task<ResultState> Delete(int timeOffRequestId)
        {
            try
            {
                var request = await _timeOffRequestRepository.GetSingle(r => r.Id == timeOffRequestId);
                if (request.Approvals.Count != 0)
                {
                    await _approvalRespository.RemoveRange(request.Approvals);
                }
                if (request.Status == RequestStatus.Created || request.Status == RequestStatus.Awaited)
                {
                    await _userManager.IncreaseRemainingDaysOff(request.Creator.Id, request.Duration, request.Type);
                }
                await _timeOffRequestRepository.Delete(timeOffRequestId);
                return new ResultState(true, "Time off request deleted!");
            }
            catch (Exception ex)
            {
                return new ResultState(false, "Failed to delete the time off request!", ex);
            }
        }

        public async Task<ResultState> SetApprovalStatus(string currentUserId, int timeOffRequestId, bool isApproved)
        {
            var timeOffRequest = await GetSingle(r => r.Id.Equals(timeOffRequestId));
            if (timeOffRequest is null) return new ResultState(false, "Invalid id!");
            if (timeOffRequest.Status != RequestStatus.Awaited) return new ResultState(false, "Request is not available for modification!");

            var approval = await _approvalRespository.GetUserApproval(timeOffRequestId, currentUserId);
            if (approval == null) return new ResultState(false, "Approval not found!");
            try
            {
                if (isApproved)
                {
                    await _approvalRespository.SetStatus(approval, ApprovalStatus.Approved);
                }
                else
                {
                    await _approvalRespository.SetStatus(approval, ApprovalStatus.Rejected);
                }

                var approvalsForCurrentRequest = await _approvalRespository.GetApprovals(timeOffRequest.Id);

                if (approvalsForCurrentRequest.Any(a => a.Status == ApprovalStatus.Rejected))
                {
                    await _userManager.IncreaseRemainingDaysOff(currentUserId, timeOffRequest.Duration, timeOffRequest.Type);

                    await _timeOffRequestRepository.SetStatus(timeOffRequest, RequestStatus.Rejected);

                    await _emailService
                        .SendEmailNotificationToTeamLeaders(timeOffRequest.Approvers, timeOffRequest, false);

                    await _emailService
                        .SendEmailNotificationToUser(timeOffRequest.Creator, timeOffRequest, false);

                    await _approvalRespository.RemoveRange(approvalsForCurrentRequest);
                }

                if (approvalsForCurrentRequest.All(a => a.Status == ApprovalStatus.Approved))
                {
                    var teamLeaders = timeOffRequest.LeadersEmails.Select(e => _userManager.FindByEmailAsync(e).Result).ToList();

                    await _timeOffRequestRepository.SetStatus(timeOffRequest, RequestStatus.Approved);

                    await _emailService.SendEmailNotificationToUser(timeOffRequest.Creator, timeOffRequest, true);
                    await _emailService.SendEmailNotificationToTeamLeaders(teamLeaders, timeOffRequest, true);
                    await _emailService.SendEmailNotificationToTeamsMembers(
                        timeOffRequest.Creator.Teams,
                        timeOffRequest.Creator.Id,
                        timeOffRequest,
                        true);
                }

                if (approvalsForCurrentRequest.Any(a => a.Status == ApprovalStatus.Pending))
                {
                    await _approvalRespository.RemoveApproval(approval);
                }

                return new ResultState(true, "Approval status submited!");
            }
            catch (Exception ex)
            {
                return new ResultState(false, "Failed to set the approval status!", ex);
            }
        }

        public async Task<ResultState> SendApprovals(int timeOffRequestId, string currentUserId)
        {
            TimeOffRequest timeOffRequest = await GetSingle(r => r.Id.Equals(timeOffRequestId));

            if (timeOffRequest == null)
            {
                return new ResultState(false, $"There is no Time Off Request with ID:{timeOffRequestId}");
            }

            if (timeOffRequest.Status != RequestStatus.Created)
            {
                return new ResultState(false, "The Time Off Request is already sent");
            }

            var currentUser = await _userManager.FindByIdAsync(currentUserId);
            var teamLeaders = currentUser.Teams.Select(t => t.TeamLeader).Distinct().ToList();
            teamLeaders.RemoveAll(l => l.Id.Equals(currentUser.Id));
            var approvers = teamLeaders.Where(l => l.IsWorking).ToList();

            timeOffRequest.Approvers = approvers;
            timeOffRequest.LeadersEmails = teamLeaders.Select(l => l.Email).ToList();
            SetApprovals(timeOffRequest, approvers);

            if (currentUserId != timeOffRequest.Creator.Id)
            {
                return new ResultState(false, "Only Crеator of the Time Off Request is able to send it");
            }

            if (timeOffRequest.Type == RequestType.Sick || timeOffRequest.Approvals.Count == 0)
            {
                await _timeOffRequestRepository.SetStatus(timeOffRequest, RequestStatus.Approved);

                await _emailService
                    .SendEmailNotificationToTeamLeaders(teamLeaders, timeOffRequest, true);

                await _emailService
                    .SendEmailNotificationToTeamsMembers(currentUser.Teams, currentUser.Id, timeOffRequest, true);

                await _emailService
                    .SendEmailNotificationToUser(timeOffRequest.Creator, timeOffRequest, true);

                return new ResultState(true, "The request has been successfully sent");
            }

            await _emailService.SendEmailRequestToTeamLeaders(approvers, timeOffRequest);

            await _timeOffRequestRepository.SetStatus(timeOffRequest, RequestStatus.Awaited);
            return new ResultState(true, "The Time Off Request has been successfully sent");
        }

        public async Task ScheduleRequestsDeletion(string userId)
        {
            ITrigger trigger = RequestsDeletionJob.GetTrigger();
            IJobDetail jobDetail = RequestsDeletionJob.CreateJob(userId);

            await _scheduler.ScheduleJob(jobDetail, trigger);
        }

        public async Task<List<TimeOffRequest>> Get(Expression<Func<TimeOffRequest, bool>> action)
        {
            return await _timeOffRequestRepository.Get(action);
        }

        public async Task<TimeOffRequest> GetSingle(Expression<Func<TimeOffRequest, bool>> action)
        {
            return await _timeOffRequestRepository.GetSingle(action);
        }

        public async Task<List<TimeOffRequest>> GetAll()
        {
            return await _timeOffRequestRepository.All();
        }
        public async Task<ResultState> CancelUserPendindRequests(User user)
        {
            if (user.CreatedRequests.Count == 0)
            {
                return new ResultState(false, "User has no Pending Requests");
            }
            else
            {
                foreach (TimeOffRequest timeOffRequest in user.CreatedRequests)
                {
                    if (timeOffRequest.Status == RequestStatus.Created || timeOffRequest.Status == RequestStatus.Awaited)
                    {
                        await _timeOffRequestRepository.SetStatus(timeOffRequest, RequestStatus.Canceled);
                    }
                }

                return new ResultState(true, "User's Pending Requests have been canceled");
            }
        }

        private async Task<int> GetTimeOffWorkingDays(DateTime startDate, DateTime endDate)
        {
            int totalDays = 0;
            for (var currentDate = startDate; currentDate <= endDate; currentDate = currentDate.AddDays(1))
            {
                var isHoliday = await _holidaysRepository.Exists(currentDate.Date);
                if (!isHoliday) totalDays++;
            }

            return totalDays;
        }

        private void SetApprovals(TimeOffRequest timeOffRequest, List<User> approvers)
        {
            if (timeOffRequest.Type == RequestType.Sick)
            {
                timeOffRequest.Approvals = new();
            }
            else
            {
                timeOffRequest.Approvals = approvers.Select(a => new Approval()
                {
                    ApproverId = a.Id,
                    Status = ApprovalStatus.Pending
                }).ToList();
            }
        }

        private async Task<bool> DecreaseTimeOffDays(User creator, int duration, RequestType type)
        {
            switch (type)
            {
                case RequestType.Paid:
                    if (creator.RemainingPaidDaysOff < duration) return false;
                    break;
                case RequestType.Unpaid:
                    if (creator.RemainingUnpaidDaysOff < duration) return false;
                    break;
                case RequestType.Sick:
                    if (creator.RemainingSickDaysOff < duration) return false;
                    break;
            };

            await _userManager.DecreaseRemainingDaysOff(creator.Id, duration, type);

            return true;
        }

        private async Task<bool> IsOverlaping(User user, TimeOffRequest timeOffRequest, int excludedId = -1)
        {
            var activeVacations = await _timeOffRequestRepository.Get(r => r.Creator.Email.Equals(user.Email));
            if (activeVacations == null) activeVacations = new();
            foreach (var vacation in activeVacations)
            {
                if (vacation.Id == excludedId) continue;
                var earlierVacation = timeOffRequest.StartDate < vacation.StartDate ? timeOffRequest : vacation;
                var laterVacation = timeOffRequest.StartDate >= vacation.StartDate ? timeOffRequest : vacation;
                if (earlierVacation.EndDate >= laterVacation.StartDate) return true;
            }

            return false;
        }
    }
}
