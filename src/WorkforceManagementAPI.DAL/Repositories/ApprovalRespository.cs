using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using WorkforceManagementAPI.Common.Enums;
using WorkforceManagementAPI.DAL.Entities;

namespace WorkforceManagementAPI.DAL.Repositories
{
    public class ApprovalRespository : IApprovalRespository
    {
        private readonly WorkforceManagementAPIDbContext _context;

        public ApprovalRespository(WorkforceManagementAPIDbContext context)
        {
            _context = context;
        }

        public async Task<Approval> GetById(int id)
        {
            return await _context.Approvals.FindAsync(id);
        }

        public async Task<List<Approval>> GetApprovals(int timeOffRequestId)
        {
            var approvals = await _context.Approvals.ToListAsync();
            return approvals.Where(a => a.TimeOffRequest.Id == timeOffRequestId).ToList();
        }

        public async Task<Approval> GetUserApproval(int timeOffRequestId, string currentUserId)
        {
            return await _context.Approvals.SingleOrDefaultAsync(a => a.ApproverId.Equals(currentUserId) && a.TimeOffRequest.Id == timeOffRequestId);
        }

        public async Task SetStatus(Approval approval, ApprovalStatus status)
        {
            approval.Status = status;
            await _context.SaveChangesAsync();
        }

        public async Task RemoveRange(IEnumerable<Approval> approvals)
        {
            _context.Approvals.RemoveRange(approvals);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveApproval(Approval approval)
        {
            _context.Approvals.Remove(approval);
            await _context.SaveChangesAsync();
        }
    }
}
