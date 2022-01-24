using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkforceManagementAPI.DAL.Entities;

namespace WorkforceManagementAPI.DAL.Repositories
{
    public interface ITeamRepository
    {
        public Task<Team> GetByName(string teamName);
        public Task<Team> GetById(int teamId);
        public Task Create(Team newTeam);
        public Task Delete(Team team);
        public Task SaveAsync(Team teamFromDb);
        public Task<List<Team>> GetTeams();
    }
}
