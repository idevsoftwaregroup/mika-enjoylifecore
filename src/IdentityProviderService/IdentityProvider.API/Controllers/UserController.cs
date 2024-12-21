using IdentityProvider.Application.Contracts.UserUpdate;
using IdentityProvider.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IdentityProvider.API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }
       
        [HttpPut("User/{userId}")]
        public async Task<ActionResult<UpdateUserResponseDTO>> UpdateUser(int userId, UpdateUserRequestDTO requestDTO, CancellationToken cancellationToken = default)
        {
            return Ok(await _userService.UpdateUserAsync(userId, requestDTO, cancellationToken));
        }
       
        [HttpDelete("User/{userId}")]
        public async Task<IActionResult> DeleteUser(int userId, CancellationToken cancellationToken = default)
        {
            await _userService.DeleteUserAsync(userId, cancellationToken);
            return NoContent();
        }


    }
}
