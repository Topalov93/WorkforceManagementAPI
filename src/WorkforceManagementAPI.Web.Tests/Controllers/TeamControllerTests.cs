using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WorkforceManagementAPI.BLL.Services;
using WorkforceManagementAPI.Common.ResultState;
using WorkforceManagementAPI.DAL.Entities;
using WorkforceManagementAPI.Models.RequestDTO;
using WorkforceManagementAPI.Models.ResponseDTO;
using WorkforceManagementAPI.Web.Controllers;
using Xunit;

namespace WorkforceManagementAPI.Web.Tests.Controllers
{
    public class TeamControllerTests
    {
        [Fact]
        public async void PostAction_ValidData_ReturnOk()
        {
            // arrange
            var teamServiceStub = new Mock<ITeamService>();
            var sut = new TeamsController(teamServiceStub.Object);

            teamServiceStub.Setup(ts => ts.CreateTeam(It.IsAny<Team>(), It.IsAny<Guid>()))
                 .ReturnsAsync(new ResultState(true, ""));
                                  
            // act
            var result = await sut.Post(new TeamRequestDTO());

            // assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async void PostAction_InvalidData_ReturnBadRequest()
        {
            // arrange
            var teamServiceStub = new Mock<ITeamService>();
            var sut = new TeamsController(teamServiceStub.Object);

            teamServiceStub.Setup(ts => ts.CreateTeam(It.IsAny<Team>(), It.IsAny<Guid>()))
                 .ReturnsAsync(new ResultState(false, ""));

            // act
            var result = await sut.Post(new TeamRequestDTO());

            // assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async void PutAction_ValidData_ReturnOk()
        {
            // arrange
            var teamServiceStub = new Mock<ITeamService>();
            var sut = new TeamsController(teamServiceStub.Object);

            teamServiceStub.Setup(ts => ts.UpdateTeam(It.IsAny<Team>(), It.IsAny<Guid>(), It.IsAny<int>()))
                 .ReturnsAsync(new ResultState(true, ""));

            // act
            var result = await sut.Put(new TeamRequestDTO(), 1);

            // assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async void PutAction_InvalidData_ReturnBadRequest()
        {
            // arrange
            var teamServiceStub = new Mock<ITeamService>();
            var sut = new TeamsController(teamServiceStub.Object);

            teamServiceStub.Setup(ts => ts.UpdateTeam(It.IsAny<Team>(), It.IsAny<Guid>(), It.IsAny<int>()))
                 .ReturnsAsync(new ResultState(false, ""));

            // act
            var result = await sut.Put(new TeamRequestDTO(), 1);

            // assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async void AddMemberAction_ValidData_ReturnOk()
        {
            // arrange
            var teamServiceStub = new Mock<ITeamService>();
            var sut = new TeamsController(teamServiceStub.Object);

            teamServiceStub.Setup(ts => ts.AddMember(It.IsAny<int>(), It.IsAny<Guid>()))
                 .ReturnsAsync(new ResultState(true, ""));

            // act
            var result = await sut.AddMember(1, new Guid());

            // assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async void AddMemberAction_InvalidData_ReturnBadRequest()
        {
            // arrange
            var teamServiceStub = new Mock<ITeamService>();
            var sut = new TeamsController(teamServiceStub.Object);

            teamServiceStub.Setup(ts => ts.AddMember(It.IsAny<int>(), It.IsAny<Guid>()))
                 .ReturnsAsync(new ResultState(false, ""));

            // act
            var result = await sut.AddMember(1, new Guid());

            // assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async void RemoveMemberAction_ValidData_ReturnOk()
        {
            // arrange
            var teamServiceStub = new Mock<ITeamService>();
            var sut = new TeamsController(teamServiceStub.Object);

            teamServiceStub.Setup(ts => ts.RemoveMember(It.IsAny<int>(), It.IsAny<Guid>()))
                 .ReturnsAsync(new ResultState(true, ""));

            // act
            var result = await sut.RemoveMember(1, new Guid());

            // assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async void RemoveMemberAction_InvalidData_ReturnBadRequest()
        {
            // arrange
            var teamServiceStub = new Mock<ITeamService>();
            var sut = new TeamsController(teamServiceStub.Object);

            teamServiceStub.Setup(ts => ts.RemoveMember(It.IsAny<int>(), It.IsAny<Guid>()))
                 .ReturnsAsync(new ResultState(false, ""));

            // act
            var result = await sut.RemoveMember(1, new Guid());

            // assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async void DeleteAction_ValidData_ReturnOk()
        {
            // arrange
            var teamServiceStub = new Mock<ITeamService>();
            var sut = new TeamsController(teamServiceStub.Object);

            teamServiceStub.Setup(ts => ts.DeleteTeam(It.IsAny<int>()))
                 .ReturnsAsync(new ResultState(true, ""));

            // act
            var result = await sut.Delete(1);

            // assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async void DeleteAction_InvalidData_ReturnBadRequest()
        {
            // arrange
            var teamServiceStub = new Mock<ITeamService>();
            var sut = new TeamsController(teamServiceStub.Object);

            teamServiceStub.Setup(ts => ts.DeleteTeam(It.IsAny<int>()))
                 .ReturnsAsync(new ResultState(false, ""));

            // act
            var result = await sut.Delete(1);

            // assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async void GetAllAction_ReturnList()
        {
            // arrange
            var teamServiceStub = new Mock<ITeamService>();
            var sut = new TeamsController(teamServiceStub.Object);
            var team = new Team() { Id = 1, TeamLeader = new User ()};

            teamServiceStub.Setup(ts => ts.GetTeams())
                 .ReturnsAsync(new List<Team>() { team});

            // act
            var result = await sut.GetAll();

            // assert
            Assert.IsType<List<TeamResponseDTO>>(result);
        }

        [Fact]
        public async void GetAction_TeamIsNull_ReturnNotFound()
        {
            // arrange
            var teamServiceStub = new Mock<ITeamService>();
            var sut = new TeamsController(teamServiceStub.Object);
            var team = new Team();
            team = null;

            teamServiceStub.Setup(ts => ts.GetTeam(It.IsAny<int>()))
                 .ReturnsAsync(team);

            // act
            var result = await sut.Get(1);

            // assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}

