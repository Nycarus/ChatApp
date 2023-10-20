using ChatApp.Data;
using ChatApp.DtoLibrary;
using ChatApp.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ChatApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserServices _userServices;

        public UserController(ILogger<UserController> logger, IUserServices userServices)
        {
            _logger = logger;
            _userServices = userServices;
        }

        [AllowAnonymous]
        [HttpPost("Register")]
        public async Task<IActionResult> Register(UserRegistrationDTO userRegistrationDTO)
        {
            try
            {
                _logger.LogInformation("User Registration.");
                await _userServices.PasswordRegister(userRegistrationDTO);
                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return BadRequest(e.Message);
            }
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> Login(UserLoginDTO userLoginDTO)
        {
            try
            {
                _logger.LogInformation("User Login", userLoginDTO.Username);
                string token = await _userServices.PasswordLogin(userLoginDTO);

                if (token == null)
                {
                    return BadRequest();
                }

                Response.Cookies.Append("Chat-Access-Token", token, new CookieOptions() { HttpOnly = true, SameSite = SameSiteMode.Strict });
                return Ok();
            }
            catch (Exception e) {
                return BadRequest(e.Message);
            }
        }

        [Authorize]
        [HttpGet("Auth")]
        public async Task<IActionResult> Auth()
        {
            try 
            {
                _logger.LogInformation("Auth");
                return Ok();
            } 
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                return StatusCode(500);
            }
        }
    }
}
