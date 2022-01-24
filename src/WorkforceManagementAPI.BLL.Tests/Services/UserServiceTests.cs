        using Microsoft.AspNetCore.Identity;
using Moq;
using System;
using System.Collections.Generic;
using WorkforceManagementAPI.BLL.Managers;
using WorkforceManagementAPI.BLL.Services;
using WorkforceManagementAPI.DAL.Entities;
using Xunit;

namespace WorkforceManagementAPI.BLL.Tests.Services
{
    public class UserServiceTests
    {
        [Fact]
        public async void CreateUser_ExistingUsername_ReturnFalse()
        {
            // arrange
            var userRepositoryStub = new Mock<IUserManager>();
            var teamRepositoryStub = new Mock<ITeamService>();
            var timeOffRepositoryStub = new Mock<ITimeOffService>();

            userRepositoryStub
                .Setup(userRepository => userRepository.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(new User());

            var sut = new UserService(userRepositoryStub.Object, teamRepositoryStub.Object, timeOffRepositoryStub.Object);

            // act
            var result = await sut.CreateUser(new User(), "", "");

            // assert
            Assert.False(result.IsSuccessful);
            Assert.Equal("User already exist!", result.Message);
        }

        [Fact]
        public async void CreateUser_RoleNotSupported_ReturnFalse()
        {
            // arrange
            var userRepositoryStub = new Mock<IUserManager>();
            var teamRepositoryStub = new Mock<ITeamService>();
            var timeOffRepositoryStub = new Mock<ITimeOffService>();

            userRepositoryStub
                .Setup(userRepository => userRepository.GetAllRoles())
                .ReturnsAsync(new List<IdentityRole>());

            var sut = new UserService(userRepositoryStub.Object, teamRepositoryStub.Object, timeOffRepositoryStub.Object);

            // act
            var result = await sut.CreateUser(new User(), "", "");

            // assert
            Assert.False(result.IsSuccessful);
            Assert.Equal("Role not supported!", result.Message);
        }

        [Fact]
        public async void CreateUser_ThrowExeption_ReturnFalse()
        {
            // arrange
            var userRepositoryStub = new Mock<IUserManager>();
            var teamRepositoryStub = new Mock<ITeamService>();
            var timeOffRepositoryStub = new Mock<ITimeOffService>();

            userRepositoryStub
                .Setup(userRepository => userRepository.GetAllRoles())
                .ReturnsAsync(new List<IdentityRole>() { new IdentityRole() { Name = "admin", NormalizedName = "ADMIN" } });

            userRepositoryStub
               .Setup(userRepository => userRepository.CreateUserAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()))
               .Throws(new Exception());


            var sut = new UserService(userRepositoryStub.Object, teamRepositoryStub.Object, timeOffRepositoryStub.Object);

            // act
            var result = await sut.CreateUser(new User(), "", "admin");

            // assert
            Assert.False(result.IsSuccessful);
            Assert.Equal("Unable to create user!", result.Message);
        }

        [Fact]
        public async void CreateUser_SuccessfulCreation_ReturnTrue()
        {
            // arrange
            var userRepositoryStub = new Mock<IUserManager>();
            var teamRepositoryStub = new Mock<ITeamService>();
            var timeOffRepositoryStub = new Mock<ITimeOffService>();

            userRepositoryStub
                .Setup(userRepository => userRepository.GetAllRoles())
                .ReturnsAsync(new List<IdentityRole>() { new IdentityRole() { Name = "admin", NormalizedName = "ADMIN" } });

            var sut = new UserService(userRepositoryStub.Object, teamRepositoryStub.Object, timeOffRepositoryStub.Object);

            // act
            var result = await sut.CreateUser(new User(), "", "admin");

            // assert
            Assert.True(result.IsSuccessful);
            Assert.Equal("User was successfully created!", result.Message);
        }

        [Fact]
        public async void EditUser_UserDoesNotExist_ReturnFalse()
        {
            // arrange
            var userRepositoryStub = new Mock<IUserManager>();
            var teamRepositoryStub = new Mock<ITeamService>();
            var timeOffRepositoryStub = new Mock<ITimeOffService>();

            var sut = new UserService(userRepositoryStub.Object, teamRepositoryStub.Object, timeOffRepositoryStub.Object);

            // act
            var result = await sut.EditUser(new Guid(), new User(), new Guid());

            // assert
            Assert.False(result.IsSuccessful);
            Assert.Equal("User does not exist!", result.Message);
        }

        [Fact]
        public async void EditUser_EmailAlreadyExist_ReturnFalse()
        {
            // arrange
            var userRepositoryStub = new Mock<IUserManager>();
            var teamRepositoryStub = new Mock<ITeamService>();
            var timeOffRepositoryStub = new Mock<ITimeOffService>();

            userRepositoryStub
             .Setup(userRepository => userRepository.FindByIdAsync(It.IsAny<string>()))
             .ReturnsAsync(new User());

            userRepositoryStub
             .Setup(userRepository => userRepository.FindByNameAsync(It.IsAny<string>()))
             .ReturnsAsync(new User());

            var sut = new UserService(userRepositoryStub.Object, teamRepositoryStub.Object, timeOffRepositoryStub.Object);

            // act
            var result = await sut.EditUser(new Guid(), new User(), new Guid());

            // assert
            Assert.False(result.IsSuccessful);
            Assert.Equal("Email already exist!", result.Message);
        }

        [Fact]
        public async void EditUser_ThrownException_ReturnFalse()
        {
            // arrange
            var userRepositoryStub = new Mock<IUserManager>();
            var teamRepositoryStub = new Mock<ITeamService>();
            var timeOffRepositoryStub = new Mock<ITimeOffService>();

            userRepositoryStub
               .Setup(userRepository => userRepository.FindByIdAsync(It.IsAny<string>()))
               .ReturnsAsync(new User());

            userRepositoryStub
               .Setup(userRepository => userRepository.EditUserById(It.IsAny<string>(), It.IsAny<User>()))
               .Throws(new Exception());


            var sut = new UserService(userRepositoryStub.Object, teamRepositoryStub.Object, timeOffRepositoryStub.Object);

            // act
            var result = await sut.EditUser(new Guid(), new User(), new Guid());

            // assert
            Assert.False(result.IsSuccessful);
            Assert.Equal("Unable to edit user!", result.Message);
        }

        [Fact]
        public async void EditUser_SuccessfulEdit_ReturnTrue()
        {
            // arrange
            var userRepositoryStub = new Mock<IUserManager>();
            var teamRepositoryStub = new Mock<ITeamService>();
            var timeOffRepositoryStub = new Mock<ITimeOffService>();

            userRepositoryStub
               .Setup(userRepository => userRepository.FindByIdAsync(It.IsAny<string>()))
               .ReturnsAsync(new User());

            var sut = new UserService(userRepositoryStub.Object, teamRepositoryStub.Object, timeOffRepositoryStub.Object);

            // act
            var result = await sut.EditUser(new Guid(), new User(), new Guid());

            // assert
            Assert.True(result.IsSuccessful);
            Assert.Equal("User was successfully edited!", result.Message);
        }

        [Fact]
        public async void DeleteUser_UserDoesNotExist_ReturnFalse()
        {
            // arrange
            var userRepositoryStub = new Mock<IUserManager>();
            var teamRepositoryStub = new Mock<ITeamService>();
            var timeOffRepositoryStub = new Mock<ITimeOffService>();

            var sut = new UserService(userRepositoryStub.Object, teamRepositoryStub.Object, timeOffRepositoryStub.Object);

            // act
            var result = await sut.DeleteUser(new Guid(), new Guid());

            // assert
            Assert.False(result.IsSuccessful);
            Assert.Equal("User does not exist!", result.Message);
        }

        [Fact]
        public async void DeleteUser_CantDeleteInitialAdminUser_ReturnFalse()
        {
            // arrange
            var userRepositoryStub = new Mock<IUserManager>();
            var teamRepositoryStub = new Mock<ITeamService>();
            var timeOffRepositoryStub = new Mock<ITimeOffService>();

            userRepositoryStub
               .Setup(userRepository => userRepository.FindByIdAsync(It.IsAny<string>()))
               .ReturnsAsync(new User() { Email="admin@test.test"});

            var sut = new UserService(userRepositoryStub.Object, teamRepositoryStub.Object, timeOffRepositoryStub.Object);

            // act
            var result = await sut.DeleteUser(new Guid(), new Guid());

            // assert
            Assert.False(result.IsSuccessful);
            Assert.Equal("Can not delete initial admin user!", result.Message);
        }

        [Fact]
        public async void SetInitialDaysOff_UserDoesNotExist_ReturnFalse()
        {
            // arrange
            var userRepositoryStub = new Mock<IUserManager>();
            var teamRepositoryStub = new Mock<ITeamService>();
            var timeOffRepositoryStub = new Mock<ITimeOffService>();

            var sut = new UserService(userRepositoryStub.Object, teamRepositoryStub.Object, timeOffRepositoryStub.Object);

            // act
            var result = await sut.SetInitialDaysOff(new Guid(), new User());

            // assert
            Assert.False(result.IsSuccessful);
            Assert.Equal("User does not exist!", result.Message);
        }

        [Fact]
        public async void SetInitialDaysOff_InitialDaysOffAlreadyUpdated_ReturnFalse()
        {
            // arrange
            var userRepositoryStub = new Mock<IUserManager>();
            var teamRepositoryStub = new Mock<ITeamService>();
            var timeOffRepositoryStub = new Mock<ITimeOffService>();

            userRepositoryStub
             .Setup(userRepository => userRepository.FindByIdAsync(It.IsAny<string>()))
             .ReturnsAsync(new User() { IsInitialDaysOffSet = true });

            var sut = new UserService(userRepositoryStub.Object, teamRepositoryStub.Object, timeOffRepositoryStub.Object);

            // act
            var result = await sut.SetInitialDaysOff(new Guid(), new User());

            // assert
            Assert.False(result.IsSuccessful);
            Assert.Equal("Initial days off already updated!", result.Message);
        }

        [Fact]
        public async void SetInitialDaysOff_ThrownException_ReturnFalse()
        {
            // arrange
            var userRepositoryStub = new Mock<IUserManager>();
            var teamRepositoryStub = new Mock<ITeamService>();
            var timeOffRepositoryStub = new Mock<ITimeOffService>();

            userRepositoryStub
             .Setup(userRepository => userRepository.FindByIdAsync(It.IsAny<string>()))
             .ReturnsAsync(new User());

            userRepositoryStub
             .Setup(userRepository => userRepository.SetInitialDaysOff(It.IsAny<string>(), It.IsAny<User>()))
             .Throws(new Exception());

            var sut = new UserService(userRepositoryStub.Object, teamRepositoryStub.Object, timeOffRepositoryStub.Object);

            // act
            var result = await sut.SetInitialDaysOff(new Guid(), new User());

            // assert
            Assert.False(result.IsSuccessful);
            Assert.Equal("Unable to set initial days off!", result.Message);
        }

        [Fact]
        public async void SetInitialDaysOff_SuccessfulSet_ReturnTrue()
        {
            // arrange
            var userRepositoryStub = new Mock<IUserManager>();
            var teamRepositoryStub = new Mock<ITeamService>();
            var timeOffRepositoryStub = new Mock<ITimeOffService>();

            userRepositoryStub
             .Setup(userRepository => userRepository.FindByIdAsync(It.IsAny<string>()))
             .ReturnsAsync(new User());

            var sut = new UserService(userRepositoryStub.Object, teamRepositoryStub.Object, timeOffRepositoryStub.Object);

            // act
            var result = await sut.SetInitialDaysOff(new Guid(), new User());

            // assert
            Assert.True(result.IsSuccessful);
            Assert.Equal("Initial days off successfully set!", result.Message);
        }

        [Fact]
        public async void GetAllUsers_SuccessfulGet_ReturnUsers()
        {
            // arrange
            var userRepositoryStub = new Mock<IUserManager>();
            var teamRepositoryStub = new Mock<ITeamService>();
            var timeOffRepositoryStub = new Mock<ITimeOffService>();

            userRepositoryStub
            .Setup(userRepository => userRepository.GetAllAsync())
            .ReturnsAsync(new List<User>() { new User(), new User() });

            var sut = new UserService(userRepositoryStub.Object, teamRepositoryStub.Object, timeOffRepositoryStub.Object);

            // act
            var result = await sut.GetUsers();

            // assert
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async void GetUserById_SuccessfulGet_ReturnUser()
        {
            // arrange
            var userRepositoryStub = new Mock<IUserManager>();
            var teamRepositoryStub = new Mock<ITeamService>();
            var timeOffRepositoryStub = new Mock<ITimeOffService>();

            userRepositoryStub
            .Setup(userRepository => userRepository.FindByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(new User());

            var sut = new UserService(userRepositoryStub.Object, teamRepositoryStub.Object, timeOffRepositoryStub.Object);

            // act
            var result = await sut.GetUserById(new Guid());

            // assert
            Assert.NotNull(result);
        }

    }
}
