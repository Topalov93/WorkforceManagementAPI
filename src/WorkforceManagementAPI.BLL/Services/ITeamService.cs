using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WorkforceManagementAPI.Common.ResultState;
using WorkforceManagementAPI.DAL.Entities;

namespace WorkforceManagementAPI.BLL.Services
{
    public interface ITeamService
    {
        Task<ResultState> CreateTeam(Team newTeam, Guid teamLeaderId);
        Task<ResultState> UpdateTeam(Team newTeamInfo, Guid teamLeaderId, int teamId);
        Task<ResultState> DeleteTeam(int teamId);
        Task<ResultState> AddMember(int teamId, Guid userId);
        Task<ResultState> RemoveMember(int teamId, Guid userId);
        Task<List<Team>> GetTeams();
        Task<Team> GetTeam(int teamId);
        Task<bool> IsPartFromTeam(Team team, Guid userId);
    }
}
