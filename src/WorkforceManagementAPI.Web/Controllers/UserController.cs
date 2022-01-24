using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WorkforceManagementAPI.BLL.Services;
using WorkforceManagementAPI.Common.Enums;
using WorkforceManagementAPI.Common.ResultState;
using WorkforceManagementAPI.DAL.Entities;
using WorkforceManagementAPI.Models.RequestDTO;
using WorkforceManagementAPI.Models.ResponseDTO;

namespace WorkforceManagementAPI.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class UserController : ControllerBase
    {
        private IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> Post(UserCreationRequestDTO user)
        {
            User userToAdd = new User
            {
                UserName = user.Email,
                Email = user.Email,
            };

            var resultState = await _userService.CreateUser(userToAdd, user.Password, user.Role);

            if (resultState.IsSuccessful)
            {
                return Ok(resultState.Message);
            }

            return BadRequest(resultState.Message);

        }

        [HttpPut]
        [Route("{userId}")]
        public async Task<IActionResult> Update(Guid userId, UserEditingRequestDTO user)
        {
            Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out Guid currentUserId);

            User userToEdit = new User
            {
                UserName = user.Email,
                Email = user.Email,
            };

            var resultState = await _userService.EditUser(userId, userToEdit, currentUserId);

            if (resultState.IsSuccessful)
            {

                return Ok(resultState.Message);
            }

            return BadRequest(resultState.Message);
        }

        [HttpDelete]
        [Route("{userId}")]
        public async Task<IActionResult> Delete(Guid userId)
        {
            Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out Guid currentUserId);

            var resultState = await _userService.DeleteUser(userId, currentUserId);

            if (resultState.IsSuccessful)
            {

                return Ok(resultState.Message);
            }

            return BadRequest(resultState.Message);
        }

        [HttpPatch]
        [Route("setInitialDaysOff/{userId}")]
        public async Task<IActionResult> SetInitialDaysOff(Guid userId, InitialDaysOffDTO initialDaysOff)
        {
            User userInitialDaysOff = new User
            {
                InitialPaidDaysOff = initialDaysOff.InitialPaidDaysOff,
                InitialUnpaidDaysOff = initialDaysOff.InitialUnpaidDaysOff,
                InitialSickDaysOff = initialDaysOff.InitialSickDaysOff,
            };

            var resultState = await _userService.SetInitialDaysOff(userId, userInitialDaysOff);

            if (resultState.IsSuccessful)
            {

                return Ok(resultState.Message);
            }

            return BadRequest(resultState.Message);
        }

        [HttpGet]
        [Route("getDaysOff/{userId}")]
        public async Task<UserDaysOffResponseDTO> GetDaysOffById(Guid userId)
        {
            var user = await _userService.GetUserById(userId);

            if (user is null)
            {
                user = new User() { Email = "User does not exist!" };
            }

            UserDaysOffResponseDTO userResponse = new UserDaysOffResponseDTO()
            {
                User = user.Email,
                InitialPaidDaysOff = user.InitialPaidDaysOff,
                InitialUnpaidDaysOff = user.InitialUnpaidDaysOff,
                InitialSickDaysOff = user.InitialSickDaysOff,
                RemainingPaidDaysOff = user.RemainingPaidDaysOff,
                RemainingUnpaidDaysOff = user.RemainingUnpaidDaysOff,
                RemainingSickDaysOff = user.RemainingSickDaysOff
            };

            return userResponse;
        }

        [HttpGet]
        public async Task<List<UserResponseDTO>> GetAll()
        {
            var users = await _userService.GetUsers();

            List<UserResponseDTO> usersResponse = new List<UserResponseDTO>();

            foreach (var user in users)
            {
                var roles = await _userService.GetUserRole(user);

                usersResponse.Add(new UserResponseDTO()
                {
                    Id = user.Id,
                    Email = user.Email,
                    Role=roles.First(),
                    AddedOn = user.CreatedOn,
                    EditedOn = user.ModifiedOn,
                });
            }
            return usersResponse;
        }

        [HttpGet]
        [Route("getDeleted")]
        public async Task<List<UserResponseDTO>> GetDeleted()
        {
            var users = await _userService.GetDeletedUsers();

            List<UserResponseDTO> usersResponse = new List<UserResponseDTO>();

            foreach (var user in users)
            {
                usersResponse.Add(new UserResponseDTO()
                {
                    Id = user.Id,
                    AddedOn = user.CreatedOn,
                    EditedOn = user.ModifiedOn,
                });
            }
            return usersResponse;
        }
    }
}
