using AuthenticationApi.Application.DTOs;
using AuthenticationApi.Application.Interface;
using ECommerce.SharedLibrary.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationApi.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthenticationController(IUser userInterface) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<ActionResult<Response>> Register(AppUserDTO userDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await userInterface.Register(userDTO);
            return result.Flag ? Ok(result) : BadRequest(result);
        }

        [HttpPost("login")]
        public async Task<ActionResult<Response>> Login(LoginDTO userDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await userInterface.Login(userDTO);
            return result.Flag ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<ActionResult<GetUserDTO>> GetUser(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid user Id");
            }
            var user = await userInterface.GetUser(id);
            return user.Id > 0 ? Ok(user) : NotFound();
        }
    }
}
