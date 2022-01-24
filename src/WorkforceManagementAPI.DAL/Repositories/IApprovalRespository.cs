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
    public interface IApprovalRespository
    {
        Task<Approval> GetById(int id);
        Task<List<Approval>> GetApprovals(int timeOffRequestId);
        Task<Approval> GetUserApproval(int timeOffRequestId, string currentUserId);
        Task SetStatus(Approval approval, ApprovalStatus status);
        Task RemoveRange(IEnumerable<Approval> approvals);
        Task RemoveApproval(Approval approval);
    }
}
