using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WorkforceManagementAPI.BLL.Managers;
using WorkforceManagementAPI.DAL.Repositories;

namespace WorkforceManagementAPI.Web.Policies
{
    public class IsAdminOrTimeOffRequestHandler : AuthorizationHandler<IsAdminOrTimeOffRequestCreator>
    {
        private readonly IUserManager _userManager;
        private readonly ITimeOffRequestRepository _timeOffRequestRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public IsAdminOrTimeOffRequestHandler(IUserManager userManager, ITimeOffRequestRepository repository, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _timeOffRequestRepository = repository;
            _httpContextAccessor = httpContextAccessor;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, IsAdminOrTimeOffRequestCreator requirement)
        {
            var currentUserId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUser = await _userManager.FindByIdAsync(currentUserId);
            
            var hasValue = _httpContextAccessor.HttpContext.Request.Query.TryGetValue("timeOffRequestId", out var requestIdData);
            if (!hasValue) await Task.FromException(new BadHttpRequestException("No Id provided!"));

            var timeOffRequestId = int.Parse(requestIdData.FirstOrDefault());
            var timeOffRequest = await _timeOffRequestRepository.GetSingle(r => r.Id == timeOffRequestId);
            if (timeOffRequest is null) await Task.FromException(new KeyNotFoundException());

            var currentUserRoles = await _userManager.GetUserRolesAsync(currentUser);
            var isUserNotAdmin = !currentUserRoles.Exists(r => r.Equals("admin"));
            var isNotOwner = !timeOffRequest.Creator.Id.Equals(currentUser.Id);
            if (isUserNotAdmin && isNotOwner) await Task.FromException(new UnauthorizedAccessException());

            context.Succeed(requirement);
            await Task.CompletedTask;
        }
    }
}
