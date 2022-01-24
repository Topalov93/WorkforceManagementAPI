using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WorkforceManagementAPI.BLL.Services;
using WorkforceManagementAPI.DAL.Entities;
using WorkforceManagementAPI.Models.RequestDTO;
using WorkforceManagementAPI.Models.ResponseDTO;

namespace WorkforceManagementAPI.Web.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TeamsController : ControllerBase
    {
        private readonly ITeamService _teamService;

        public TeamsController(ITeamService teamService)
        {
            _teamService = teamService;
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> Post(TeamRequestDTO teamDto)
        {
            Team newTeam = new Team
            {
                Name = teamDto.Name,
                Description = teamDto.Description
            };

            var resultState = await _teamService.CreateTeam(newTeam, teamDto.TeamLeaderId);

            if (resultState.IsSuccessful)
            {

                return Ok(resultState.Message);
            }

            return BadRequest(resultState.Message);
        }

        [Authorize(Roles = "admin")]
        [HttpPut]
        [Route("{teamId}")]
        public async Task<IActionResult> Put(TeamRequestDTO teamDto, int teamId)
        {
            Team newTeamInfo = new Team
            {
                Name = teamDto.Name,
                Description = teamDto.Description
            };

            var resultState = await _teamService.UpdateTeam(newTeamInfo, teamDto.TeamLeaderId, teamId);

            if (resultState.IsSuccessful)
            {
                return Ok(resultState.Message);
            }

            return BadRequest(resultState.Message);
        }

        [Authorize(Roles = "admin")]
        [HttpPut]
        [Route("{teamId}/AddMember/{userId}")]
        public async Task<IActionResult> AddMember(int teamId, Guid userId)
        {
            var resultState = await _teamService.AddMember(teamId, userId);

            if (resultState.IsSuccessful)
            {
                return Ok(resultState.Message);
            }

            return BadRequest(resultState.Message);
        }

        [Authorize(Roles = "admin")]
        [HttpPut]
        [Route("{teamId}/RemoveMember/{userId}")]
        public async Task<IActionResult> RemoveMember(int teamId, Guid userId)
        {
            var resultState = await _teamService.RemoveMember(teamId, userId);

            if (resultState.IsSuccessful)
            {
                return Ok(resultState.Message);
            }

            return BadRequest(resultState.Message);
        }

        [Authorize(Roles = "admin")]
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var resultState = await _teamService.DeleteTeam(id);

            if (resultState.IsSuccessful)
            {
                return Ok(resultState.Message);
            }

            return BadRequest(resultState.Message);
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public async Task<List<TeamResponseDTO>> GetAll()
        {
            var teams = await _teamService.GetTeams();

            List<TeamResponseDTO> teamsResponse = new List<TeamResponseDTO>();

            foreach (var team in teams)
            {
                teamsResponse.Add(new TeamResponseDTO()
                {
                    Id = team.Id,
                    Name = team.Name,
                    Description = team.Description,
                    TeamLeader = team.TeamLeader.UserName,
                    AddedOn = team.CreatedOn,
                    EditedOn = team.ModifiedOn,
                    Members = team.TeamMembers.Select(m => m.UserName).ToList()
                });
            }

            return teamsResponse;
        }

        [HttpGet]
        [Route("{teamId}")]
        public async Task<IActionResult> Get(int teamId)
        {
            var team = await _teamService.GetTeam(teamId);
            if (team != null)
            {
                Guid currentUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

                if (!await _teamService.IsPartFromTeam(team, currentUserId)) return Forbid();                                

                TeamResponseDTO teamDto = new TeamResponseDTO()
                {
                    Id = team.Id,
                    Name = team.Name,
                    Description = team.Description,
                    TeamLeader = team.TeamLeader.UserName,
                    AddedOn = team.CreatedOn,
                    EditedOn = team.ModifiedOn,
                    Members = team.TeamMembers.Select(m => m.UserName).ToList()
                };

                return Ok(teamDto);

            }

            return NotFound();
        }
    }
}
