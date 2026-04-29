using Microsoft.AspNetCore.Mvc;
using StockSentinal.Services;
using StockSentinal.DTOs;

namespace StockSentinal.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _service;
        private readonly IUserService _userService;
        public AuthController(IAuthService service, IUserService userService)
        {
            _service     = service;
            _userService = userService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
        {
            var result = await _service.LoginAsync(request);
            return Ok(result);
        }

        [HttpPost]
        [Route("register")]
        public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
        {
            var result = await _service.RegisterAsync(request);
            if (null == result)
            {
                return Conflict("User already exists!");
            }

            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<List<UserResponse>>> GetAllUsers()
        {
            var result = await _userService.GetAllUsersAsync();
            if (!result.Any())
            {
                return NotFound("No Users found! Add users");
            }

            return Ok(result);
        }
    }
}