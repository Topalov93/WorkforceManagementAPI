using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
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
    public class TimeOffRequestController : ControllerBase
    {
        private readonly ITimeOffService _timeOffService;
        private readonly IUserService _userService;

        public TimeOffRequestController(ITimeOffService timeOffService, IUserService userService)
        {
            _timeOffService = timeOffService;
            _userService = userService;
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<LeftDaysOffResponseDTO> GetLeftDaysOff()
        {
            Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out Guid currentUserId);
            var currentUser = await _userService.GetUserById(currentUserId);

            LeftDaysOffResponseDTO daysOffDTO = new()
            {
                PaidDaysOff = currentUser.RemainingPaidDaysOff,
                UnpaidDaysOff = currentUser.RemainingUnpaidDaysOff,
                SickDaysOff = currentUser.RemainingSickDaysOff
            };

            return daysOffDTO;
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<List<TimeOffRequestResponseDTO>> Get()
        {
            string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var requests = User.IsInRole("admin") ? await _timeOffService.GetAll() : await _timeOffService.Get(r => r.Creator.Id.Equals(currentUserId));
            var requestDTOs = new List<TimeOffRequestResponseDTO>();

            foreach (var request in requests)
            {
                var approvals = new List<ApprovalResponseDTO>();
                foreach (var approval in request.Approvals)
                {
                    ApprovalResponseDTO approvalDTO = new();
                    approvalDTO.Id = approval.Id;
                    approvalDTO.Status = approval.Status.ToString();
                    approvalDTO.ApproverId = approval.ApproverId;
                    approvals.Add(approvalDTO);
                }

                TimeOffRequestResponseDTO responseDTO = new()
                {
                    Id = request.Id,
                    CreatorId = request.Creator.Id,
                    Status = request.Status.ToString(),
                    Type = request.Type.ToString(),
                    Description = request.Description,
                    Approvals = approvals,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    Duration = request.Duration,
                    CreatedOn = request.CreatedOn,
                    ModifiedOn = request.ModifiedOn,
                };
                requestDTOs.Add(responseDTO);
            }

            return requestDTOs;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Post(TimeOffRequestRequestDTO requestDTO)
        {
            string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            TimeOffRequest timeOffRequest = new()
            {
                Type = requestDTO.Type,
                Description = requestDTO.Description,
                StartDate = requestDTO.StartDate,
                EndDate = requestDTO.EndDate,
            };

            var result = await _timeOffService.Create(timeOffRequest, currentUserId);

            if (result.IsSuccessful)
            {
                return Ok(result.Message);
            }

            return BadRequest(result.Message);
        }

        [HttpPut]
        [Route("[action]")]
        [Authorize(Policy = "IsAdminOrTimeOffRequestCreator")]
        public async Task<IActionResult> Put(int timeOffRequestId, TimeOffRequestRequestDTO requestDTO)
        {
            TimeOffRequest request = new();
            request.Type = requestDTO.Type;
            request.Description = requestDTO.Description;
            request.StartDate = requestDTO.StartDate;
            request.EndDate = requestDTO.EndDate;

            var result = await _timeOffService.Update(timeOffRequestId, request);

            if (result.IsSuccessful)
            {
                return Ok(result.Message);
            }

            return BadRequest(result.Message);
        }

        [HttpDelete]
        [Route("[action]")]
        [Authorize(Policy = "IsAdminOrTimeOffRequestCreator")]
        public async Task<IActionResult> Delete(int timeOffRequestId)
        {
            var result = await _timeOffService.Delete(timeOffRequestId);

            if (result.IsSuccessful)
            {
                return Ok(result.Message);
            }

            return BadRequest(result.Message);
        }

        [HttpPost]
        [Route("[action]/{timeOffRequestId}/{isApproved}")]
        public async Task<IActionResult> SetApproval(int timeOffRequestId, bool isApproved)
        {
            string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _timeOffService.SetApprovalStatus(currentUserId, timeOffRequestId, isApproved);

            if (result.IsSuccessful)
            {
                return Ok(result.Message);
            }

            return BadRequest(result.Message);
        }

        [HttpPost]
        [Route("[action]/{timeOffRequestId}")]
        public async Task<IActionResult> SendApprovals(int timeOffRequestId)
        {
            string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _timeOffService.SendApprovals(timeOffRequestId, currentUserId);

            if (result.IsSuccessful)
            {
                return Ok(result.Message);
            }

            return BadRequest(result.Message);
        }
    }
}
