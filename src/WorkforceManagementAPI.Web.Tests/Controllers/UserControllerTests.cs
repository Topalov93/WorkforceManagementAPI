using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using WorkforceManagementAPI.BLL.Services;
using WorkforceManagementAPI.Common.ResultState;
using WorkforceManagementAPI.DAL.Entities;
using WorkforceManagementAPI.Models.RequestDTO;
using WorkforceManagementAPI.Web.Controllers;
using Xunit;

namespace WorkforceManagementAPI.Web.Tests.Controllers
{
    public class UserControllerTests
    {
        [Fact]
        public async void Post_CreateUserSuccessfully_ReturnOk()
        {
            // arrange
            var userServiceStub = new Mock<IUserService>();

            userServiceStub
                .Setup(userService => userService.CreateUser(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(new ResultState(true, "Message")));

            var sut = new UserController(userServiceStub.Object);
            var user = new ClaimsPrincipal();
            sut.ControllerContext = new ControllerContext();
            sut.ControllerContext.HttpContext = new DefaultHttpContext() { User = user };

            // act
            var result = await sut.Post(new UserCreationRequestDTO());

            // assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async void SetInitialDaysOff_DaysOffSetSuccessfully_ReturnOk()
        {
            // arrange
            var userServiceStub = new Mock<IUserService>();

            userServiceStub
                .Setup(userService => userService.SetInitialDaysOff(It.IsAny<Guid>(), It.IsAny<User>()))
                .Returns(Task.FromResult(new ResultState(true, "Message")));

            var sut = new UserController(userServiceStub.Object);
            var user = new ClaimsPrincipal();
            sut.ControllerContext = new ControllerContext();
            sut.ControllerContext.HttpContext = new DefaultHttpContext() { User = user };

            // act
            var result = await sut.SetInitialDaysOff(new Guid(), new InitialDaysOffDTO());

            // assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async void Update_UpdateUserInfoSuccessfully_ReturnOk()
        {
            // arrange
            var userServiceStub = new Mock<IUserService>();

            userServiceStub
                .Setup(userService => userService.EditUser(It.IsAny<Guid>(), It.IsAny<User>(), It.IsAny<Guid>()))
                .Returns(Task.FromResult(new ResultState(true, "Message")));

            var sut = new UserController(userServiceStub.Object);
            var user = new ClaimsPrincipal();
            sut.ControllerContext = new ControllerContext();
            sut.ControllerContext.HttpContext = new DefaultHttpContext() { User = user };

            // act
            var result = await sut.Update(new Guid(), new UserEditingRequestDTO());

            // assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async void Delete_DeleteUserInfoSuccessfully_ReturnOk()
        {
            // arrange
            var userServiceStub = new Mock<IUserService>();

            userServiceStub
                .Setup(userService => userService.DeleteUser(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .Returns(Task.FromResult(new ResultState(true, "Message")));

            var sut = new UserController(userServiceStub.Object);
            var user = new ClaimsPrincipal();
            sut.ControllerContext = new ControllerContext();
            sut.ControllerContext.HttpContext = new DefaultHttpContext() { User = user };

            // act
            var result = await sut.Delete(new Guid());

            // assert
            Assert.IsType<OkObjectResult>(result);
        }
    }
}
