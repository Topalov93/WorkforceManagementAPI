using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WorkforceManagementAPI.BLL.Managers;
using WorkforceManagementAPI.BLL.Services;
using WorkforceManagementAPI.DAL.Entities;
using Xunit;

namespace WorkforceManagementAPI.BLL.Tests.Services
{
    public class TeamServiceTests
    {
        [Fact]
        public async void CreateTeam_ExistingTeamName_ReturnFalse()
        {
            // arrange
            var userManagerStub = new Mock<IUserManager>();
            var teamRepositoryStub = new Mock<DAL.Repositories.ITeamRepository>();
            var timeOffRepositoryStub = new Mock<ITimeOffService>();

            teamRepositoryStub
                .Setup(tr => tr.GetByName(It.IsAny<string>()))
                .ReturnsAsync(new Team());

            var sut = new TeamService(teamRepositoryStub.Object, timeOffRepositoryStub.Object, userManagerStub.Object);

            // act
            var result = await sut.CreateTeam(new Team() , new Guid());

            // assert
            Assert.False(result.IsSuccessful);
        }

        [Fact]
        public async void AddMember_ValidData_CallsSaveAsync()
        {
            // arrange
            var userManagerStub = new Mock<IUserManager>();
            var teamRepositoryMock = new Mock<DAL.Repositories.ITeamRepository>();
            var timeOffRepositoryStub = new Mock<ITimeOffService>();
            Team newTeamInfo = new Team() { };
            Team team = new Team() { Id = 1, TeamLeader = new User() };
            Guid guid = new();
            Guid newMemberGuid = new();

            userManagerStub.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(new User() { Id = guid.ToString() });
            teamRepositoryMock.Setup(tr => tr.GetById(It.IsAny<int>())).ReturnsAsync(team);

            var sut = new TeamService(teamRepositoryMock.Object, timeOffRepositoryStub.Object, userManagerStub.Object);

            // act
            await sut.AddMember(1, newMemberGuid);

            // assert
            teamRepositoryMock.Verify(mock =>
                mock.SaveAsync(team),
                Times.Once);
        }

        [Fact]
        public async void AddMember_TeamIsNUll_ReturnFalse()
        {
            // arrange
            var userManagerStub = new Mock<IUserManager>();
            var teamRepositoryStub = new Mock<DAL.Repositories.ITeamRepository>();
            var timeOffRepositoryStub = new Mock<ITimeOffService>();
            Team newTeamInfo = new Team() { };
            Team team = new Team() { Id = 1, TeamLeader = new User() };
            Guid guid = new();
            Guid newMemberGuid = new();

            userManagerStub.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(new User() { Id = guid.ToString() });

            var sut = new TeamService(teamRepositoryStub.Object, timeOffRepositoryStub.Object, userManagerStub.Object);

            // act
            var result = await sut.AddMember(1, newMemberGuid);

            // assert
            Assert.False(result.IsSuccessful);
        }

        [Fact]
        public async void AddMember_MemberIsNUll_ReturnFalse()
        {
            // arrange
            var userManagerStub = new Mock<IUserManager>();
            var teamRepositoryStub = new Mock<DAL.Repositories.ITeamRepository>();
            var timeOffRepositoryStub = new Mock<ITimeOffService>();
            Team newTeamInfo = new Team() { };
            Team team = new Team() { Id = 1, TeamLeader = new User() };
            Guid newMemberGuid = new();

            teamRepositoryStub.Setup(tr => tr.GetById(It.IsAny<int>())).ReturnsAsync(team);

            var sut = new TeamService(teamRepositoryStub.Object, timeOffRepositoryStub.Object, userManagerStub.Object);

            // act
            var result = await sut.AddMember(1, newMemberGuid);

            // assert
            Assert.False(result.IsSuccessful);
        }

        [Fact]
        public async void RemoveMember_TeamIsNUll_ReturnFalse()
        {
            // arrange
            var userManagerStub = new Mock<IUserManager>();
            var teamRepositoryStub = new Mock<DAL.Repositories.ITeamRepository>();
            var timeOffRepositoryStub = new Mock<ITimeOffService>();
            Team newTeamInfo = new Team() { };
            Team team = new Team() { Id = 1, TeamLeader = new User() };
            Guid guid = new();
            Guid newMemberGuid = new();

            userManagerStub.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(new User() { Id = guid.ToString() });

            var sut = new TeamService(teamRepositoryStub.Object, timeOffRepositoryStub.Object, userManagerStub.Object);

            // act
            var result = await sut.RemoveMember(1, newMemberGuid);

            // assert
            Assert.False(result.IsSuccessful);
        }

        [Fact]
        public async void RemoveMember_MemberIsNUll_ReturnFalse()
        {
            // arrange
            var userManagerStub = new Mock<IUserManager>();
            var teamRepositoryStub = new Mock<DAL.Repositories.ITeamRepository>();
            var timeOffRepositoryStub = new Mock<ITimeOffService>();
            Team newTeamInfo = new Team() { };
            Team team = new Team() { Id = 1, TeamLeader = new User() };
            Guid newMemberGuid = new();

            teamRepositoryStub.Setup(tr => tr.GetById(It.IsAny<int>())).ReturnsAsync(team);

            var sut = new TeamService(teamRepositoryStub.Object, timeOffRepositoryStub.Object, userManagerStub.Object);

            // act
            var result = await sut.RemoveMember(1, newMemberGuid);

            // assert
            Assert.False(result.IsSuccessful);
        }

        [Fact]
        public async void RemoveMember_ValidData_CallsSaveAsync()
        {
            // arrange
            var userManagerStub = new Mock<IUserManager>();
            var teamRepositoryMock = new Mock<DAL.Repositories.ITeamRepository>();
            var timeOffRepositoryStub = new Mock<ITimeOffService>();
            Team newTeamInfo = new Team() ;
            Guid guid = new();
            User member = new() { Id = guid.ToString()}; 
            Team team = new Team() { Id = 1, TeamLeader = new User(), TeamMembers = {member } };

            userManagerStub.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(member);
            teamRepositoryMock.Setup(tr => tr.GetById(It.IsAny<int>())).ReturnsAsync(team);

            var sut = new TeamService(teamRepositoryMock.Object, timeOffRepositoryStub.Object, userManagerStub.Object);

            // act
            await sut.RemoveMember(1, guid);

            // assert
            teamRepositoryMock.Verify(mock =>
                mock.SaveAsync(team),
                Times.Once);
        }

        [Fact]
        public async void UpdateTeam_ValidData_CallsSaveAsync()
        {
            // arrange
            var userManagerStub = new Mock<IUserManager>();
            var teamRepositoryMock = new Mock<DAL.Repositories.ITeamRepository>();
            var timeOffRepositoryStub = new Mock<ITimeOffService>();
            Team newTeamInfo = new Team() { Name = "New Name"};
            Team team = new Team() { Id = 1, Name ="Old name", TeamLeader = new User() };
            Team nullTeam = null;
            Guid guid = new();

            userManagerStub.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(new User() { Id = guid.ToString() });            
            teamRepositoryMock.Setup(tr => tr.GetById(It.IsAny<int>())).ReturnsAsync(team);
            teamRepositoryMock.Setup(tr => tr.GetByName(It.IsAny<string>())).ReturnsAsync(nullTeam);

            var sut = new TeamService(teamRepositoryMock.Object, timeOffRepositoryStub.Object, userManagerStub.Object);

            // act
            await sut.UpdateTeam(newTeamInfo, guid ,1);

            // assert
            teamRepositoryMock.Verify(mock =>
                mock.SaveAsync(team),
                Times.Once);
        }

        [Fact]
        public async void UpdateTeam_NewTeamLeaderIsNull_ReturnFalse()
        {
            // arrange
            var userManagerStub = new Mock<IUserManager>();
            var teamRepositoryStub = new Mock<DAL.Repositories.ITeamRepository>();
            var timeOffRepositoryStub = new Mock<ITimeOffService>();
            Team newTeamInfo = new Team() { };
            Team team = new Team() { Id = 1, TeamLeader = new User() };

            Guid guid = new();
            teamRepositoryStub.Setup(tr => tr.GetById(It.IsAny<int>())).ReturnsAsync(team);

            var sut = new TeamService(teamRepositoryStub.Object, timeOffRepositoryStub.Object, userManagerStub.Object);

            // act
            var result = await sut.UpdateTeam(newTeamInfo, guid, 1);

            // assert
            Assert.False(result.IsSuccessful);
        }

        [Fact]
        public async void CreateTeam_TeamLeaderDoesNotExists_ReturnFalse()
        {
            // arrange
            var teamRepositoryStub = new Mock<DAL.Repositories.ITeamRepository>();
            var userManagerStub = new Mock<IUserManager>();
            var timeOffRepositoryStub = new Mock<ITimeOffService>();
            var sut = new TeamService(teamRepositoryStub.Object, timeOffRepositoryStub.Object, userManagerStub.Object);

            // act
            var result = await sut.CreateTeam(new Team (), new Guid());

            // assert
            Assert.False(result.IsSuccessful);
        }

        [Fact]
        public async void CreateTeam_ValidData_ReturnFalse()
        {
            // arrange
            var userManagerStub = new Mock<IUserManager>();
            var teamRepositoryStub = new Mock<DAL.Repositories.ITeamRepository>();
            var timeOffRepositoryStub = new Mock<ITimeOffService>();
            Guid guid = new ();

            userManagerStub
                .Setup(um => um.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(new User() {Id = guid.ToString() });

            var sut = new TeamService(teamRepositoryStub.Object, timeOffRepositoryStub.Object, userManagerStub.Object);

            // act
            var result = await sut.CreateTeam(new Team(), guid);

            // assert
            Assert.True(result.IsSuccessful);
        }

        [Fact]
        public async void CreateTeam_TeamLeaderIsNull_ReturnFalse()
        {
            // arrange
            var userManagerStub = new Mock<IUserManager>();
            var teamRepositoryStub = new Mock<DAL.Repositories.ITeamRepository>();
            var timeOffRepositoryStub = new Mock<ITimeOffService>();
            Guid guid = new();

            var sut = new TeamService(teamRepositoryStub.Object, timeOffRepositoryStub.Object, userManagerStub.Object);

            // act
            var result = await sut.CreateTeam(new Team(), guid);

            // assert
            Assert.False(result.IsSuccessful);
        }

        [Fact]
        public async void UpdateTeam_TeamFrobDbIsNull_ReturnFalse()
        {
            // arrange
            var userManagerStub = new Mock<IUserManager>();
            var teamRepositoryStub = new Mock<DAL.Repositories.ITeamRepository>();
            var timeOffRepositoryStub = new Mock<ITimeOffService>();
            Team newTeamInfo = new Team() { };

            Guid guid = new();

            var sut = new TeamService(teamRepositoryStub.Object, timeOffRepositoryStub.Object, userManagerStub.Object);

            // act
            var result = await sut.UpdateTeam(newTeamInfo, guid, 1);

            // assert
            Assert.False(result.IsSuccessful);
        }

        [Fact]
        public async void DeleteTeam_TeamFrobDbIsNull_ReturnFalse()
        {
            // arrange
            var userManagerStub = new Mock<IUserManager>();
            var teamRepositoryStub = new Mock<DAL.Repositories.ITeamRepository>();
            var timeOffRepositoryStub = new Mock<ITimeOffService>();

            var sut = new TeamService(teamRepositoryStub.Object, timeOffRepositoryStub.Object, userManagerStub.Object);

            // act
            var result = await sut.DeleteTeam(1);

            // assert
            Assert.False(result.IsSuccessful);
        }

        [Fact]
        public async Task DeleteTeam_CallsRemoveAsync()
        {
            // arrange
            var userManagerStub = new Mock<IUserManager>();
            var teamRepositoryMock = new Mock<DAL.Repositories.ITeamRepository>();
            var timeOffRepositoryStub = new Mock<ITimeOffService>();
            Guid teamMemberGuid = new Guid();
            User teamMember = new User() { Id = teamMemberGuid.ToString() };
            Team team = new Team() { Id = 1, TeamLeader = new User (), TeamMembers = { teamMember } };

            teamRepositoryMock.Setup(tr => tr.GetById(It.IsAny<int>())).ReturnsAsync(team);

            var sut = new TeamService(teamRepositoryMock.Object, timeOffRepositoryStub.Object, userManagerStub.Object);

            // act
            await sut.DeleteTeam(1);

            // assert
            teamRepositoryMock.Verify(mock =>
                mock.Delete(team),
                Times.Once);
        }

        [Fact]
        public async Task GetAll_Default_ShouldReturnRepositoryCollection()
        {
            // arrange
            var userManagerStub = new Mock<IUserManager>();
            var teamRepositoryStub = new Mock<DAL.Repositories.ITeamRepository>();
            var timeOffRepositoryStub = new Mock<ITimeOffService>();
            var teams = new List<Team>
            {
                new Team { Name = "demo1" },
                new Team { Name = "demo2" },
            };

            teamRepositoryStub.Setup(tr => tr.GetTeams())
                .ReturnsAsync(teams);

            var sut = new TeamService(teamRepositoryStub.Object, timeOffRepositoryStub.Object, userManagerStub.Object);

            // act
            var result = await sut.GetTeams();

            // assert
            Assert.Equal(teams, result);
        }

        [Fact]
        public async Task Get_Default_ShouldReturnTeam()
        {
            // arrange
            var userManagerStub = new Mock<IUserManager>();
            var teamRepositoryStub = new Mock<DAL.Repositories.ITeamRepository>();
            var timeOffRepositoryStub = new Mock<ITimeOffService>();
            var team = new Team
            {
                Name = "demo1",
                Id = 2
            };

            teamRepositoryStub.Setup(tr => tr.GetById(It.IsAny<int>()))
                .ReturnsAsync(team);

            var sut = new TeamService(teamRepositoryStub.Object, timeOffRepositoryStub.Object, userManagerStub.Object);

            // act
            var result = await sut.GetTeam(2);

            // assert
            Assert.Equal(team, result);
        }
    }
}
