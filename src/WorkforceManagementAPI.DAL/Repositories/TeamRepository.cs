using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using WorkforceManagementAPI.DAL.Entities;

namespace WorkforceManagementAPI.DAL.Repositories
{
    public class TeamRepository : ITeamRepository
    {
        private readonly WorkforceManagementAPIDbContext _context;

        public TeamRepository(WorkforceManagementAPIDbContext context)
        {
            _context = context;
        }

        public async Task<Team> GetByName(string teamName)
        {
            return await _context.Teams.FirstOrDefaultAsync(t => t.Name == teamName);
        }

        public async Task<Team> GetById(int teamId)
        {
            return await _context.Teams.FirstOrDefaultAsync(t => t.Id == teamId);
        }

        public async Task Create(Team newTeam)
        {
            await _context.Teams.AddAsync(newTeam);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(Team team)
        {
            _context.Teams.Remove(team);
            await _context.SaveChangesAsync();
        }

        public async Task SaveAsync(Team teamFromDb)
        {
            await _context.SaveChangesAsync();
        }

        public async Task<List<Team>> GetTeams()
        {
            return await _context.Teams.ToListAsync(); ;
        }
    }
}
