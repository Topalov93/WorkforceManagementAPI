using System.Collections.Generic;
using WorkforceManagementAPI.BLL.Services;
using WorkforceManagementAPI.DAL.Entities;
using Xunit;

namespace WorkforceManagementAPI.BLL.Tests.Services
{
    public class EmailServiceTests
    {
        [Fact]
        public async void SendEmail_WithValidInput_ShouldReturnTrue()
        {
            // arrange
            var sut = new EmailService();

            // act
            var result = await sut.SendEmail("test@test.com", "test", "test");

            // assert
            Assert.True(result.IsSuccessful);
        }

        [Fact]
        public async void SendEmail_WithInvalidEmail_ShouldReturnFalse()
        {
            // arrange
            var sut = new EmailService();

            // act
            var result = await sut.SendEmail("", "test", "test");

            // assert
            Assert.False(result.IsSuccessful);
        }

        [Fact]
        public async void SendEmailRequestToTeamLeaders_WithValidInput_ShouldReturnTrue()
        {
            // arrange
            var sut = new EmailService();


            List<User> teamleaders = new();
            TimeOffRequest timeOffRequest = new();

            // act
            var result = await sut.SendEmailRequestToTeamLeaders(teamleaders, timeOffRequest);

            // assert
            Assert.True(result.IsSuccessful);
        }

        [Fact]
        public async void SendEmailRequestToTeamLeaders_WithInvalidInput_ShouldReturnFalse()
        {
            // arrange
            var sut = new EmailService();

            // act
            var result = await sut.SendEmailRequestToTeamLeaders(null, null);

            // assert
            Assert.False(result.IsSuccessful);
        }

        [Fact]
        public async void SendEmailNotificationToTeamLeaders_WithValidInput_ShouldReturnTrue()
        {
            // arrange
            var sut = new EmailService();


            List<User> teamleaders = new();
            TimeOffRequest timeOffRequest = new();

            // act
            var result = await sut.SendEmailNotificationToTeamLeaders(teamleaders, timeOffRequest, true);

            // assert
            Assert.True(result.IsSuccessful);
        }

        [Fact]
        public async void SendEmailNotificationToTeamLeaders_WithInvalidInput_ShouldReturnFalse()
        {
            // arrange
            var sut = new EmailService();


            // act
            var result = await sut.SendEmailNotificationToTeamLeaders(null, null, false);

            // assert
            Assert.False(result.IsSuccessful);
        }

        [Fact]
        public async void SendEmailNotificationToTeamsMembers_WithValidInput_ShouldReturnTrue()
        {
            // arrange
            var sut = new EmailService();


            List<Team> teams = new();
            TimeOffRequest timeOffRequest = new();
            string userId = "";

            // act
            var result = await sut.SendEmailNotificationToTeamsMembers(teams, userId, timeOffRequest, true);

            // assert
            Assert.True(result.IsSuccessful);
        }

        [Fact]
        public async void SendEmailNotificationToTeamsMembers_WithInvalidInput_ShouldReturnFalse()
        {
            // arrange
            var sut = new EmailService();

            // act
            var result = await sut.SendEmailNotificationToTeamsMembers(null, null, null, true);

            // assert
            Assert.False(result.IsSuccessful);
        }

        [Fact]
        public async void SendEmailNotificationToUser_WithValidInput_ShouldReturnTrue()
        {
            // arrange
            var sut = new EmailService();


            User user = new();
            TimeOffRequest timeOffRequest = new();

            // act
            var result = await sut.SendEmailNotificationToUser(user, timeOffRequest, true);

            // assert
            Assert.True(result.IsSuccessful);
        }

        [Fact]
        public async void SendEmailNotificationToUser_WithInvalidInput_ShouldReturnFalse()
        {
            // arrange
            var sut = new EmailService();

            // act
            var result = await sut.SendEmailNotificationToUser(null, null, true);

            // assert
            Assert.False(result.IsSuccessful);
        }
    }
}
