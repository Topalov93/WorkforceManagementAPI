using Moq;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using WorkforceManagementAPI.BLL.Managers;
using WorkforceManagementAPI.BLL.Services;
using WorkforceManagementAPI.Common.Enums;
using WorkforceManagementAPI.DAL.Entities;
using WorkforceManagementAPI.DAL.Repositories;
using Xunit;

namespace WorkforceManagementAPI.BLL.Tests.Services
{
    public class TimeOffServiceTests
    {
        [Fact]
        public async Task GetAll_Default_ShouldReturnAllTimeOffRequests()
        {
            //arrange
            var timeOffRequestRepository = new Mock<ITimeOffRequestRepository>();
            var userManager = new Mock<IUserManager>();
            var approvalRespository = new Mock<IApprovalRespository>();
            var holidaysRepository = new Mock<IHolidaysRepository>();
            var emailService = new Mock<IEmailService>();
            var scheduler = new Mock<IScheduler>();

            var sut = new TimeOffService(timeOffRequestRepository.Object,
                                         approvalRespository.Object,
                                         userManager.Object,
                                         holidaysRepository.Object,
                                         emailService.Object,
                                         scheduler.Object);

            List<TimeOffRequest> list = new() { new TimeOffRequest { }, new TimeOffRequest { } };

            timeOffRequestRepository.Setup(x => x.All()).ReturnsAsync(list);

            //act
            var result = await sut.GetAll();

            //assert
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task Get_WithValidInput_ShouldReturnTimeOffRequests()
        {
            //arrange
            var timeOffRequestRepository = new Mock<ITimeOffRequestRepository>();
            var userManager = new Mock<IUserManager>();
            var approvalRespository = new Mock<IApprovalRespository>();
            var holidaysRepository = new Mock<IHolidaysRepository>();
            var emailService = new Mock<IEmailService>();
            var scheduler = new Mock<IScheduler>();

            var sut = new TimeOffService(timeOffRequestRepository.Object,
                                         approvalRespository.Object,
                                         userManager.Object,
                                         holidaysRepository.Object,
                                         emailService.Object,
                                         scheduler.Object);

            List<TimeOffRequest> list = new() { new TimeOffRequest { Id = 1 } };

            timeOffRequestRepository.Setup(x => x.Get(It.IsAny<Expression<Func<TimeOffRequest, bool>>>()))
                .ReturnsAsync(list);

            //act
            var result = await sut.Get(t => t.Id == 1);

            //assert
            Assert.Single(result);
        }

        [Fact]
        public async Task Get_WithInvalidInput_ShouldReturnTimeEmptyCollection()
        {
            //arrange
            var timeOffRequestRepository = new Mock<ITimeOffRequestRepository>();
            var userManager = new Mock<IUserManager>();
            var approvalRespository = new Mock<IApprovalRespository>();
            var holidaysRepository = new Mock<IHolidaysRepository>();
            var emailService = new Mock<IEmailService>();
            var scheduler = new Mock<IScheduler>();

            var sut = new TimeOffService(timeOffRequestRepository.Object,
                                         approvalRespository.Object,
                                         userManager.Object,
                                         holidaysRepository.Object,
                                         emailService.Object,
                                         scheduler.Object);

            List<TimeOffRequest> list = new() { };

            timeOffRequestRepository.Setup(x => x.Get(It.IsAny<Expression<Func<TimeOffRequest, bool>>>()))
                .ReturnsAsync(list);

            //act
            var result = await sut.Get(t => t.Id == 1);

            //assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetSingle_WithValidInput_ShouldReturnTimeOffRequest()
        {
            //arrange
            var timeOffRequestRepository = new Mock<ITimeOffRequestRepository>();
            var userManager = new Mock<IUserManager>();
            var approvalRespository = new Mock<IApprovalRespository>();
            var holidaysRepository = new Mock<IHolidaysRepository>();
            var emailService = new Mock<IEmailService>();
            var scheduler = new Mock<IScheduler>();

            var sut = new TimeOffService(timeOffRequestRepository.Object,
                                         approvalRespository.Object,
                                         userManager.Object,
                                         holidaysRepository.Object,
                                         emailService.Object,
                                         scheduler.Object);

            TimeOffRequest timeOffRequest = new() { Id = 1 };

            timeOffRequestRepository.Setup(x => x.GetSingle(It.IsAny<Expression<Func<TimeOffRequest, bool>>>()))
                .ReturnsAsync(timeOffRequest);

            //act
            var result = await sut.GetSingle(t => t.Id == 1);

            //assert
            Assert.Equal(1, result.Id);
        }

        [Fact]
        public async Task GetSingle_WithInvalidInput_ShouldReturnTimeOffRequest()
        {
            //arrange
            var timeOffRequestRepository = new Mock<ITimeOffRequestRepository>();
            var userManager = new Mock<IUserManager>();
            var approvalRespository = new Mock<IApprovalRespository>();
            var holidaysRepository = new Mock<IHolidaysRepository>();
            var emailService = new Mock<IEmailService>();
            var scheduler = new Mock<IScheduler>();

            var sut = new TimeOffService(timeOffRequestRepository.Object,
                                         approvalRespository.Object,
                                         userManager.Object,
                                         holidaysRepository.Object,
                                         emailService.Object,
                                         scheduler.Object);

            TimeOffRequest timeOffRequest = new() { Id = 2 };

            timeOffRequestRepository.Setup(x => x.GetSingle(It.IsAny<Expression<Func<TimeOffRequest, bool>>>()))
                .ReturnsAsync(timeOffRequest);

            //act
            var result = await sut.GetSingle(t => t.Id == 1);

            //assert
            Assert.NotEqual(1, result.Id);
        }

        [Fact]
        public async Task Create_WithValidPaidDaysOff_ShouldReturnTrue()
        {
            //arrange
            var timeOffRequestRepository = new Mock<ITimeOffRequestRepository>();
            var userManager = new Mock<IUserManager>();
            var approvalRespository = new Mock<IApprovalRespository>();
            var holidaysRepository = new Mock<IHolidaysRepository>();
            var emailService = new Mock<IEmailService>();
            var scheduler = new Mock<IScheduler>();

            var sut = new TimeOffService(timeOffRequestRepository.Object,
                                         approvalRespository.Object,
                                         userManager.Object,
                                         holidaysRepository.Object,
                                         emailService.Object,
                                         scheduler.Object);

            User user = new()
            {
                RemainingPaidDaysOff = 20
            };

            userManager.Setup(t => t.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(user);


            TimeOffRequest timeOffRequest = new()
            {
                Type = RequestType.Paid,
                Duration = 5
            };

            //act
            var result = await sut.Create(timeOffRequest, user.Id);

            //assert
            Assert.True(result.IsSuccessful);
        }

        [Fact]
        public async Task Create_WithInValidSickDaysOff_ShouldReturnFalse()
        {
            //arrange
            var timeOffRequestRepository = new Mock<ITimeOffRequestRepository>();
            var userManager = new Mock<IUserManager>();
            var approvalRespository = new Mock<IApprovalRespository>();
            var holidaysRepository = new Mock<IHolidaysRepository>();
            var emailService = new Mock<IEmailService>();
            var scheduler = new Mock<IScheduler>();

            var sut = new TimeOffService(timeOffRequestRepository.Object,
                                         approvalRespository.Object,
                                         userManager.Object,
                                         holidaysRepository.Object,
                                         emailService.Object,
                                         scheduler.Object);

            User user = new()
            {
                RemainingSickDaysOff = 0
            };

            userManager.Setup(t => t.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(user);

            

            TimeOffRequest timeOffRequest = new()
            {
                Type = RequestType.Sick,
                Duration = 1
            };

            //act
            var result = await sut.Create(timeOffRequest, user.Id);

            //assert
            Assert.False(result.IsSuccessful);
        }

        [Fact]
        public async Task Delete_WithValidTimeOffRequest_ShouldReturnTrue()
        {
            //arrange
            var timeOffRequestRepository = new Mock<ITimeOffRequestRepository>();
            var userManager = new Mock<IUserManager>();
            var approvalRespository = new Mock<IApprovalRespository>();
            var holidaysRepository = new Mock<IHolidaysRepository>();
            var emailService = new Mock<IEmailService>();
            var scheduler = new Mock<IScheduler>();

            var sut = new TimeOffService(timeOffRequestRepository.Object,
                                         approvalRespository.Object,
                                         userManager.Object,
                                         holidaysRepository.Object,
                                         emailService.Object,
                                         scheduler.Object);

            TimeOffRequest timeOffRequest = new()
            {
                Id = 1,
                Creator = new User(),
                Duration = 5,
                Status = RequestStatus.Created,
                Type = RequestType.Paid,
                Approvals = new List<Approval>()
            };

            timeOffRequestRepository.Setup(x => x.GetSingle(It.IsAny<Expression<Func<TimeOffRequest, bool>>>()))
                .ReturnsAsync(timeOffRequest);

            //act
            var result = await sut.Delete(timeOffRequest.Id);

            //assert
            Assert.True(result.IsSuccessful);
        }

        [Fact]
        public async Task Delete_WithInValidTimeOffRequest_ShouldReturnFalse()
        {
            //arrange
            var timeOffRequestRepository = new Mock<ITimeOffRequestRepository>();
            var userManager = new Mock<IUserManager>();
            var approvalRespository = new Mock<IApprovalRespository>();
            var holidaysRepository = new Mock<IHolidaysRepository>();
            var emailService = new Mock<IEmailService>();
            var scheduler = new Mock<IScheduler>();

            var sut = new TimeOffService(timeOffRequestRepository.Object,
                                         approvalRespository.Object,
                                         userManager.Object,
                                         holidaysRepository.Object,
                                         emailService.Object,
                                         scheduler.Object);

            timeOffRequestRepository.Setup(x => x.GetSingle(It.IsAny<Expression<Func<TimeOffRequest, bool>>>()))
               .ReturnsAsync(new TimeOffRequest());

            //act
            var result = await sut.Delete(2);

            //assert
            Assert.False(result.IsSuccessful);
        }

        [Fact]
        public async Task Delete_WithValidTimeOffRequest_ShouldReturnEmptyApprovalsCollection()
        {
            //arrange
            var timeOffRequestRepository = new Mock<ITimeOffRequestRepository>();
            var userManager = new Mock<IUserManager>();
            var approvalRespository = new Mock<IApprovalRespository>();
            var holidaysRepository = new Mock<IHolidaysRepository>();
            var emailService = new Mock<IEmailService>();
            var scheduler = new Mock<IScheduler>();

            var sut = new TimeOffService(timeOffRequestRepository.Object,
                                         approvalRespository.Object,
                                         userManager.Object,
                                         holidaysRepository.Object,
                                         emailService.Object,
                                         scheduler.Object);

            User teamLeader = new() { Id = "2", IsWorking = true };
            Team team = new() { TeamLeader = teamLeader };
            User currentUser = new() { Id = "1", Teams = new() { team } };

            userManager.Setup(t => t.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(currentUser);

            TimeOffRequest timeOffRequest = new()
            {
                Id = 1,
                Type = RequestType.Paid,
                Status = RequestStatus.Awaited,
                Creator = currentUser,
                Duration = 7
            };

            timeOffRequestRepository.Setup(x => x.GetSingle(It.IsAny<Expression<Func<TimeOffRequest, bool>>>()))
                .ReturnsAsync(timeOffRequest);

            //act
            var result = await sut.Delete(1);

            //assert
            Assert.Empty(timeOffRequest.Approvals);
        }

        [Fact]
        public async Task Update_WithValidTimeOffRequestStatus_ShouldReturnTrue()
        {
            //arrange
            var timeOffRequestRepository = new Mock<ITimeOffRequestRepository>();
            var userManager = new Mock<IUserManager>();
            var approvalRespository = new Mock<IApprovalRespository>();
            var holidaysRepository = new Mock<IHolidaysRepository>();
            var emailService = new Mock<IEmailService>();
            var scheduler = new Mock<IScheduler>();

            var sut = new TimeOffService(timeOffRequestRepository.Object,
                                         approvalRespository.Object,
                                         userManager.Object,
                                         holidaysRepository.Object,
                                         emailService.Object,
                                         scheduler.Object);

            TimeOffRequest timeOffRequest = new() { Status = RequestStatus.Created, Creator = new() };

            timeOffRequestRepository.Setup(x => x.GetSingle(It.IsAny<Expression<Func<TimeOffRequest, bool>>>())).ReturnsAsync(timeOffRequest);
            //act
            var result = await sut.Update(1, timeOffRequest);

            //assert
            Assert.True(result.IsSuccessful);
        }

        [Fact]
        public async Task Update_WithInvalidTimeOffRequestStatus_ShouldReturnFalse()
        {
            //arrange
            var timeOffRequestRepository = new Mock<ITimeOffRequestRepository>();
            var userManager = new Mock<IUserManager>();
            var approvalRespository = new Mock<IApprovalRespository>();
            var holidaysRepository = new Mock<IHolidaysRepository>();
            var emailService = new Mock<IEmailService>();
            var scheduler = new Mock<IScheduler>();

            var sut = new TimeOffService(timeOffRequestRepository.Object,
                                         approvalRespository.Object,
                                         userManager.Object,
                                         holidaysRepository.Object,
                                         emailService.Object,
                                         scheduler.Object);

            TimeOffRequest timeOffRequest = new() { Status = RequestStatus.Approved };

            //act
            var result = await sut.Update(1, timeOffRequest);

            //assert
            Assert.False(result.IsSuccessful);
        }


        [Fact]
        public async Task SetApprovalStatus_WithValidInput_ShouldReturnTrue()
        {
            //arrange
            var timeOffRequestRepository = new Mock<ITimeOffRequestRepository>();
            var userManager = new Mock<IUserManager>();
            var approvalRespository = new Mock<IApprovalRespository>();
            var holidaysRepository = new Mock<IHolidaysRepository>();
            var emailService = new Mock<IEmailService>();
            var scheduler = new Mock<IScheduler>();

            var sut = new TimeOffService(timeOffRequestRepository.Object,
                                         approvalRespository.Object,
                                         userManager.Object,
                                         holidaysRepository.Object,
                                         emailService.Object,
                                         scheduler.Object);

            Approval approval = new()
            {
                Id = 1,
                ApproverId = "1",
                Status = ApprovalStatus.Approved
            };

            List<Approval> approvals = new() { approval };

            TimeOffRequest timeOffRequest = new()
            {
                Id = 1,
                Status = RequestStatus.Awaited,
                Creator = new User(),
                Type = RequestType.Paid,
                Approvers = new List<User>(),
                Approvals = approvals,
                Duration = 5,
                LeadersEmails = new List<string>()
            };

            timeOffRequestRepository.Setup(x => x.GetSingle(It.IsAny<Expression<Func<TimeOffRequest, bool>>>()))
               .ReturnsAsync(timeOffRequest);

            approvalRespository.Setup(x => x.GetUserApproval(It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync(approval);

            approvalRespository.Setup(x => x.GetApprovals(It.IsAny<int>()))
                .ReturnsAsync(approvals);

            //act
            var result = await sut.SetApprovalStatus("1", 1, true);

            //assert
            Assert.True(result.IsSuccessful);
        }

        [Fact]
        public async Task SetApprovalStatus_WithInvalidTimeOffRequest_ShouldReturnFalse()
        {
            //arrange
            var timeOffRequestRepository = new Mock<ITimeOffRequestRepository>();
            var userManager = new Mock<IUserManager>();
            var approvalRespository = new Mock<IApprovalRespository>();
            var holidaysRepository = new Mock<IHolidaysRepository>();
            var emailService = new Mock<IEmailService>();
            var scheduler = new Mock<IScheduler>();

            var sut = new TimeOffService(timeOffRequestRepository.Object,
                                         approvalRespository.Object,
                                         userManager.Object,
                                         holidaysRepository.Object,
                                         emailService.Object,
                                         scheduler.Object);

            timeOffRequestRepository.Setup(x => x.GetSingle(It.IsAny<Expression<Func<TimeOffRequest, bool>>>()))
               .ReturnsAsync(new TimeOffRequest());

            //act
            var result = await sut.SetApprovalStatus("1", 1, true);

            //assert
            Assert.False(result.IsSuccessful);
        }

        [Fact]
        public async Task SetApprovalStatus_WithInvalidTimeOffStatus_ShouldReturnErrorMessage()
        {
            //arrange
            var timeOffRequestRepository = new Mock<ITimeOffRequestRepository>();
            var userManager = new Mock<IUserManager>();
            var approvalRespository = new Mock<IApprovalRespository>();
            var holidaysRepository = new Mock<IHolidaysRepository>();
            var emailService = new Mock<IEmailService>();
            var scheduler = new Mock<IScheduler>();

            var sut = new TimeOffService(timeOffRequestRepository.Object,
                                         approvalRespository.Object,
                                         userManager.Object,
                                         holidaysRepository.Object,
                                         emailService.Object,
                                         scheduler.Object);

            TimeOffRequest timeOffRequest = new() { Id = 1, Status = RequestStatus.Approved };

            timeOffRequestRepository.Setup(x => x.GetSingle(It.IsAny<Expression<Func<TimeOffRequest, bool>>>()))
               .ReturnsAsync(timeOffRequest);

            //act
            var result = await sut.SetApprovalStatus("1", 1, true);

            //assert
            Assert.Equal("Request is not available for modification!", result.Message);
        }

        [Fact]
        public async Task SetApprovalStatus_WithInexistingApproval_ShouldReturnErrorMessage()
        {
            //arrange
            var timeOffRequestRepository = new Mock<ITimeOffRequestRepository>();
            var userManager = new Mock<IUserManager>();
            var approvalRespository = new Mock<IApprovalRespository>();
            var holidaysRepository = new Mock<IHolidaysRepository>();
            var emailService = new Mock<IEmailService>();
            var scheduler = new Mock<IScheduler>();

            var sut = new TimeOffService(timeOffRequestRepository.Object,
                                         approvalRespository.Object,
                                         userManager.Object,
                                         holidaysRepository.Object,
                                         emailService.Object,
                                         scheduler.Object);

            TimeOffRequest timeOffRequest = new() { Id = 1, Status = RequestStatus.Awaited };

            timeOffRequestRepository.Setup(x => x.GetSingle(It.IsAny<Expression<Func<TimeOffRequest, bool>>>()))
               .ReturnsAsync(timeOffRequest);

            //act
            var result = await sut.SetApprovalStatus("1", 1, true);

            //assert
            Assert.Equal("Approval not found!", result.Message);
        }

        [Fact]
        public async Task SendApprovals_WithInvalidTimeOffRequestId_ShouldReturnFalseAndErrorMessage()
        {
            //arrange
            var timeOffRequestRepository = new Mock<ITimeOffRequestRepository>();
            var userManager = new Mock<IUserManager>();
            var approvalRespository = new Mock<IApprovalRespository>();
            var holidaysRepository = new Mock<IHolidaysRepository>();
            var emailService = new Mock<IEmailService>();
            var scheduler = new Mock<IScheduler>();

            var sut = new TimeOffService(timeOffRequestRepository.Object,
                                         approvalRespository.Object,
                                         userManager.Object,
                                         holidaysRepository.Object,
                                         emailService.Object,
                                         scheduler.Object);

            TimeOffRequest timeOffRequest = new() { Id = 2 };

            //act
            var result = await sut.SendApprovals(timeOffRequest.Id, "1");

            //assert
            Assert.False(result.IsSuccessful);
            Assert.Equal($"There is no Time Off Request with ID:{timeOffRequest.Id}", result.Message);
        }

        [Fact]
        public async Task SendApprovals_WithInvalidTimeOffRequestStatus_ShouldReturnFalseAndErrorMessage()
        {
            //arrange
            var timeOffRequestRepository = new Mock<ITimeOffRequestRepository>();
            var userManager = new Mock<IUserManager>();
            var approvalRespository = new Mock<IApprovalRespository>();
            var holidaysRepository = new Mock<IHolidaysRepository>();
            var emailService = new Mock<IEmailService>();
            var scheduler = new Mock<IScheduler>();

            var sut = new TimeOffService(timeOffRequestRepository.Object,
                                         approvalRespository.Object,
                                         userManager.Object,
                                         holidaysRepository.Object,
                                         emailService.Object,
                                         scheduler.Object);

            TimeOffRequest timeOffRequest = new() { Id = 1, Status = RequestStatus.Approved };

            timeOffRequestRepository.Setup(x => x.GetSingle(It.IsAny<Expression<Func<TimeOffRequest, bool>>>()))
              .ReturnsAsync(timeOffRequest);

            //act
            var result = await sut.SendApprovals(timeOffRequest.Id, "1");

            //assert
            Assert.False(result.IsSuccessful);
            Assert.Equal($"The Time Off Request is already sent", result.Message);
        }

        [Fact]
        public async Task SendApprovals_WithInvalidUser_ShouldReturnFalseAndErrorMessage()
        {
            //arrange
            var timeOffRequestRepository = new Mock<ITimeOffRequestRepository>();
            var userManager = new Mock<IUserManager>();
            var approvalRespository = new Mock<IApprovalRespository>();
            var holidaysRepository = new Mock<IHolidaysRepository>();
            var emailService = new Mock<IEmailService>();
            var scheduler = new Mock<IScheduler>();

            var sut = new TimeOffService(timeOffRequestRepository.Object,
                                         approvalRespository.Object,
                                         userManager.Object,
                                         holidaysRepository.Object,
                                         emailService.Object,
                                         scheduler.Object);
            User currentUser = new();
            User approver = new();
            User creator = new() { Id = "2", Teams = new() { new Team() { TeamLeader = new()} } };
            TimeOffRequest timeOffRequest = new()
            {
                Id = 1,
                Status = RequestStatus.Created,
                Creator = creator,
                Approvers = new() { approver },
                LeadersEmails = new List<string>()
            };
            userManager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(currentUser);
            timeOffRequestRepository.Setup(x => x.GetSingle(It.IsAny<Expression<Func<TimeOffRequest, bool>>>()))
              .ReturnsAsync(timeOffRequest);

            //act
            var result = await sut.SendApprovals(timeOffRequest.Id, "1");

            //assert
            Assert.False(result.IsSuccessful);
            Assert.Equal($"Only Crеator of the Time Off Request is able to send it", result.Message);
        }

        [Fact]
        public async Task SendApprovals_WithSickRequest_ShouldReturnTrue()
        {
            //arrange
            var timeOffRequestRepository = new Mock<ITimeOffRequestRepository>();
            var userManager = new Mock<IUserManager>();
            var approvalRespository = new Mock<IApprovalRespository>();
            var holidaysRepository = new Mock<IHolidaysRepository>();
            var emailService = new Mock<IEmailService>();
            var scheduler = new Mock<IScheduler>();

            var sut = new TimeOffService(timeOffRequestRepository.Object,
                                         approvalRespository.Object,
                                         userManager.Object,
                                         holidaysRepository.Object,
                                         emailService.Object,
                                         scheduler.Object);
            User currentUser = new();
            User approver = new();
            User creator = new()
            {
                Id = "1",
                RemainingSickDaysOff = 13
            };

            TimeOffRequest timeOffRequest = new()
            {
                Id = 1,
                Status = RequestStatus.Created,
                Type = RequestType.Sick,
                Duration = 7,
                Creator = creator,
                Approvers = new() { approver },
                LeadersEmails = new List<string>()
            };

            userManager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(currentUser);
            timeOffRequestRepository.Setup(x => x.GetSingle(It.IsAny<Expression<Func<TimeOffRequest, bool>>>()))
              .ReturnsAsync(timeOffRequest);

            //act
            var result = await sut.SendApprovals(timeOffRequest.Id, "1");

            //assert
            Assert.True(result.IsSuccessful);
        }

        [Fact]
        public async Task SendApprovals_WithPaidRequest_ShouldReturnTrue()
        {
            //arrange
            var timeOffRequestRepository = new Mock<ITimeOffRequestRepository>();
            var userManager = new Mock<IUserManager>();
            var approvalRespository = new Mock<IApprovalRespository>();
            var holidaysRepository = new Mock<IHolidaysRepository>();
            var emailService = new Mock<IEmailService>();
            var scheduler = new Mock<IScheduler>();

            var sut = new TimeOffService(timeOffRequestRepository.Object,
                                         approvalRespository.Object,
                                         userManager.Object,
                                         holidaysRepository.Object,
                                         emailService.Object,
                                         scheduler.Object);
            User currentUser = new();
            User approver = new();
            User creator = new()
            {
                Id = "1",
                RemainingPaidDaysOff = 13
            };

            TimeOffRequest timeOffRequest = new()
            {
                Id = 1,
                Status = RequestStatus.Created,
                Type = RequestType.Paid,
                Duration = 7,
                Creator = creator,
                Approvers = new() { approver },
                LeadersEmails = new List<string>()
            };
            userManager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(currentUser);
            timeOffRequestRepository.Setup(x => x.GetSingle(It.IsAny<Expression<Func<TimeOffRequest, bool>>>()))
              .ReturnsAsync(timeOffRequest);

            //act
            var result = await sut.SendApprovals(timeOffRequest.Id, "1");

            //assert
            Assert.True(result.IsSuccessful);
        }

        [Fact]
        public async Task CancelUserPendindRequests_WithValidRequestType_ShouldReturnTrue()
        {
            //arrange
            var timeOffRequestRepository = new Mock<ITimeOffRequestRepository>();
            var userManager = new Mock<IUserManager>();
            var approvalRespository = new Mock<IApprovalRespository>();
            var holidaysRepository = new Mock<IHolidaysRepository>();
            var emailService = new Mock<IEmailService>();
            var scheduler = new Mock<IScheduler>();

            var sut = new TimeOffService(timeOffRequestRepository.Object,
                                         approvalRespository.Object,
                                         userManager.Object,
                                         holidaysRepository.Object,
                                         emailService.Object,
                                         scheduler.Object);

            TimeOffRequest timeOffRequest = new()
            {
                Status = RequestStatus.Created
            };

            User user = new()
            {
                CreatedRequests = new List<TimeOffRequest>() { timeOffRequest }
            };

            //act
            var result = await sut.CancelUserPendindRequests(user);

            //assert
            Assert.True(result.IsSuccessful);
        }

        [Fact]
        public async Task CancelUserPendindRequests_WithInvalidRequestType_ShouldReturnFalse()
        {
            //arrange
            var timeOffRequestRepository = new Mock<ITimeOffRequestRepository>();
            var userManager = new Mock<IUserManager>();
            var approvalRespository = new Mock<IApprovalRespository>();
            var holidaysRepository = new Mock<IHolidaysRepository>();
            var emailService = new Mock<IEmailService>();
            var scheduler = new Mock<IScheduler>();

            var sut = new TimeOffService(timeOffRequestRepository.Object,
                                         approvalRespository.Object,
                                         userManager.Object,
                                         holidaysRepository.Object,
                                         emailService.Object,
                                         scheduler.Object);


            User user = new()
            {
                CreatedRequests = new List<TimeOffRequest>()
            };

            //act
            var result = await sut.CancelUserPendindRequests(user);

            //assert
            Assert.False(result.IsSuccessful);
        }
    }
}
