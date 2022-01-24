using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;
using WorkforceManagementAPI.BLL.Services;
using WorkforceManagementAPI.Common.ResultState;
using WorkforceManagementAPI.DAL.Entities;
using WorkforceManagementAPI.Models.RequestDTO;
using WorkforceManagementAPI.Models.ResponseDTO;
using WorkforceManagementAPI.Web.Controllers;
using Xunit;

namespace WorkforceManagementAPI.Web.Tests.Controllers
{
    public class TimeOffRequestControllerTests
    {
        [Fact]
        public async Task Delete_RequestWithStatusCreated_ReturnOk()
        {
            //arrange
            int id = 1;
            var timeOffService = new Mock<ITimeOffService>();
            var userService = new Mock<IUserService>();
            timeOffService.Setup(s => s.Delete(It.IsAny<int>()))
                .Returns(Task.FromResult(new ResultState(true, "Message")));

            var controller = new TimeOffRequestController(timeOffService.Object, userService.Object);

            //act
            var result = await controller.Delete(id);

            //assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task SetApproval_NonExistingId_ReturnsBadRequest()
        {
            //arrange
            int id = 1;
            var timeOffService = new Mock<ITimeOffService>();
            var userService = new Mock<IUserService>();
            timeOffService.Setup(s => s.SetApprovalStatus(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<bool>()))
                .Returns(Task.FromResult(new ResultState(false, "Message")));

            var controller = new TimeOffRequestController(timeOffService.Object, userService.Object);
            var user = new ClaimsPrincipal();
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext() { User = user };

            //act
            var result = await controller.SetApproval(id, true);

            //assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task SendApprovals_ExistingRequest_ReturnOk()
        {
            //arrange
            int id = 1;
            var timeOffService = new Mock<ITimeOffService>();
            var userService = new Mock<IUserService>();
            timeOffService.Setup(s => s.SendApprovals(It.IsAny<int>(), It.IsAny<string>()))
                .Returns(Task.FromResult(new ResultState(true, "Message")));

            var controller = new TimeOffRequestController(timeOffService.Object, userService.Object);
            var user = new ClaimsPrincipal();
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext() { User = user };

            //act
            var result = await controller.SendApprovals(id);

            //assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task UpdateRequest_InvalidId_ReturnBadRequest()
        {
            //arrange
            int id = 1;
            var timeOffService = new Mock<ITimeOffService>();
            var userService = new Mock<IUserService>();
            timeOffService.Setup(s => s.Update(It.IsAny<int>(), It.IsAny<TimeOffRequest>()))
                .Returns(Task.FromResult(new ResultState(false, "Message")));

            var controller = new TimeOffRequestController(timeOffService.Object, userService.Object);

            //act
            var result = await controller.Put(id, new TimeOffRequestRequestDTO());

            //assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task CreateRequest_ValidDTO_ReturnOk()
        {
            //arrange
            var timeOffService = new Mock<ITimeOffService>();
            var userService = new Mock<IUserService>();
            timeOffService.Setup(s => s.Create(It.IsAny<TimeOffRequest>(), It.IsAny<string>()))
                .Returns(Task.FromResult(new ResultState(true, "Message")));

            var controller = new TimeOffRequestController(timeOffService.Object, userService.Object);
            var user = new ClaimsPrincipal();
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext() { User = user };

            //act
            var result = await controller.Post(new TimeOffRequestRequestDTO());

            //assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetRequests_RequestAsAdmin_ReturnRequestsList()
        {
            //arrange
            var timeOffService = new Mock<ITimeOffService>();
            var userService = new Mock<IUserService>();
            timeOffService.Setup(s => s.GetAll()).ReturnsAsync(new List<TimeOffRequest>());
            timeOffService.Setup(s => s.Get(It.IsAny<Expression<Func<TimeOffRequest,bool>>>())).ReturnsAsync(new List<TimeOffRequest>());

            var controller = new TimeOffRequestController(timeOffService.Object, userService.Object);
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Role, "admin") }));

            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext() { User = user };

            //act
            var result = await controller.Get();

            //assert
            timeOffService.Verify(s => s.GetAll(), Times.Once);
            timeOffService.Verify(s => s.Get(It.IsAny<Expression<Func<TimeOffRequest, bool>>>()), Times.Never);
            Assert.IsType<List<TimeOffRequestResponseDTO>>(result);
        }

        [Fact]
        public async Task GetLeftDaysOff_ValidData_ReturnDTO()
        {
            //arrange
            var timeOffService = new Mock<ITimeOffService>();
            var userService = new Mock<IUserService>();
            userService.Setup(s => s.GetUserById(It.IsAny<Guid>())).ReturnsAsync(new User());

            var controller = new TimeOffRequestController(timeOffService.Object, userService.Object);
            var user = new ClaimsPrincipal();
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext() { User = user };

            //act
            var result = await controller.GetLeftDaysOff();

            //assert
            Assert.IsType<LeftDaysOffResponseDTO>(result);
        }
    }
}
