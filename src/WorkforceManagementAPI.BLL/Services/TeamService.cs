using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WorkforceManagementAPI.BLL.Managers;
using WorkforceManagementAPI.Common.Enums;
using WorkforceManagementAPI.Common.ResultState;
using WorkforceManagementAPI.DAL.Entities;
using WorkforceManagementAPI.DAL.Repositories;


namespace WorkforceManagementAPI.BLL.Services
{
    public class TeamService : ITeamService
    {
        private readonly ITeamRepository _teamsRepository;
        private readonly ITimeOffService _timeOffService;
        private readonly IUserManager _userManager;

        public TeamService(ITeamRepository teamsRepository, ITimeOffService timeOffService, IUserManager userManager)
        {
            _teamsRepository = teamsRepository;
            _timeOffService = timeOffService;
            _userManager = userManager;
        }

        public async Task<ResultState> AddMember(int teamId, Guid userId)
        {
            Team team = await _teamsRepository.GetById(teamId);

            if (team == null)
            {
                return new ResultState(false, "Team does not exist!");
            }

            User member = await _userManager.FindByIdAsync(userId.ToString());

            if (member == null || member.IsDeleted)
            {
                return new ResultState(false, "User does not exist!");
            }

            team.TeamMembers.Add(member);

            try
            {
                await _teamsRepository.SaveAsync(team);
                return new ResultState(true, "User was successfully added in team!");
            }
            catch (Exception ex)
            {
                return new ResultState(false, "Unable to add user in team!", ex);
            }
        }

        public async Task<ResultState> CreateTeam(Team newTeam, Guid teamLeaderId)
        {
            if (await _teamsRepository.GetByName(newTeam.Name) != null)
            {
                return new ResultState(false, "Team already exist!");
            }

            User teamLeader = await _userManager.FindByIdAsync(teamLeaderId.ToString());

            if (teamLeader == null || teamLeader.IsDeleted)
            {
                return new ResultState(false, "Team Leader doesn't exist! Insert valid id.");
            }

            newTeam.TeamLeader = teamLeader;

            try
            {
                await _teamsRepository.Create(newTeam);
                return new ResultState(true, "Team was successfully created!");
            }
            catch (Exception ex)
            {
                return new ResultState(false, "Unable to create team!", ex);
            }
        }

        public async Task<ResultState> DeleteTeam(int teamId)
        {
            Team team = await _teamsRepository.GetById(teamId);

            if (team == null)
            {
                return new ResultState(false, "Team does not exist!");
            }

            foreach (User member in team.TeamMembers.ToList())
            {
                await CloseAwaitedTimeRequests(team, member);
            }
 
            try
            {
                await _teamsRepository.Delete(team);
                return new ResultState(true, "Team was successfully deleted!");
            }
            catch (Exception ex)
            {
                return new ResultState(false, "Unable to delete team!", ex);
            }
        }

        private async Task SendAprrovalToAwaitedRequests(Team team, User member)
        {
            var teamLeaderRequestsForApproval = team.TeamLeader.RequestsForApproval.ToList().FindAll(r => r.Status == RequestStatus.Awaited);
            var memberAwaitedRequests = member.CreatedRequests.ToList().FindAll(r => r.Status == RequestStatus.Awaited);

            foreach (var leaderRequest in teamLeaderRequestsForApproval)
            {
                foreach (var memberRequest in memberAwaitedRequests)
                {
                    if (leaderRequest.Id == memberRequest.Id )
                    {
                        await _timeOffService.SetApprovalStatus(team.TeamLeader.Id, memberRequest.Id, true);
                    }
                }
            }
        }

        public async Task<List<Team>> GetTeams()
        {
            return await _teamsRepository.GetTeams();
        }

        public async Task<ResultState> RemoveMember(int teamId, Guid userId)
        {
            Team team = await _teamsRepository.GetById(teamId);
            User member = await _userManager.FindByIdAsync(userId.ToString());

            if (team == null || member == null || member.IsDeleted)
            {
                return new ResultState(false, "Wrong team and/or member id!");
            }

            if (!team.TeamMembers.ToList().Any(m => m.Id == member.Id))
            {
                return new ResultState(false, "User is not member of this team!");
            }

            team.TeamMembers.Remove(member);
            await CloseAwaitedTimeRequests(team, member);

            try
            {
                await _teamsRepository.SaveAsync(team);
                return new ResultState(true, "User was successfully removed from team!");
            }
            catch (Exception ex)
            {
                return new ResultState(false, "Unable to remove user from team!", ex);
            }
        }

        public async Task<ResultState> UpdateTeam(Team newTeamInfo, Guid teamLeaderId, int teamId)
        {
            Team teamFromDb = await _teamsRepository.GetById(teamId);

            if (teamFromDb == null)
            {
                return new ResultState(false, "Team does not exist!");
            }

            User newTeamLeader = await _userManager.FindByIdAsync(teamLeaderId.ToString());

            if (newTeamLeader == null || newTeamLeader.IsDeleted)
            {
                return new ResultState(false, "The new Team Leader doesn't exist! Insert valid id.");
            }
            
            if(teamFromDb.Name != newTeamInfo.Name && await _teamsRepository.GetByName(newTeamInfo.Name) != null)
                return new ResultState(false, "Team with new name already exist!");

            teamFromDb.Description = newTeamInfo.Description;
            teamFromDb.ModifiedOn = DateTime.UtcNow;
            if (teamFromDb.TeamLeader.Id != newTeamLeader.Id)
            {
                foreach (User member in teamFromDb.TeamMembers.ToList())
                {
                    await CloseAwaitedTimeRequests(teamFromDb, member);
                }
            }

            teamFromDb.TeamLeader = newTeamLeader;

            try
            {
                await _teamsRepository.SaveAsync(teamFromDb);
                return new ResultState(true, "Team was successfully edited!");
            }
            catch (Exception ex)
            {
                return new ResultState(false, "Unable to edit team!", ex);
            }
        }

        private async Task CloseAwaitedTimeRequests(Team team, User member)
        {
            List<Team> otherTeamsWhichLead = team.TeamLeader.LeadTeams.ToList();
            otherTeamsWhichLead.Remove(team);
            if (!otherTeamsWhichLead.Any(t => t.TeamMembers.ToList().Any(m => m.Id == member.Id)))
            {
                await SendAprrovalToAwaitedRequests(team, member);
            }
        }

        public async Task<Team> GetTeam(int teamId)
        {
            return await _teamsRepository.GetById(teamId);
        }

        public async Task<bool> IsPartFromTeam (Team team, Guid userId)
        {
            var currentUser = await _userManager.FindByIdAsync(userId.ToString());
            var currentUserRoles = await _userManager.GetUserRolesAsync(currentUser);
            var isAdmin = currentUserRoles.Exists(r => r.Equals("admin"));

            if (isAdmin || currentUser.Id == team.TeamLeader.Id || team.TeamMembers.ToList().Any(m => m.Id == currentUser.Id))
            {                
                return true;
            }

            return false;
        }
    }
}
