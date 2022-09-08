using ExchangeSimulatorBackend.Dtos;
using ExchangeSimulatorBackend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace ExchangeSimulatorBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUserContextService _userContextService;
        public AuthController(IAuthService authService, IUserContextService userContextService)
        {
            _authService = authService;
            _userContextService = userContextService;
        }

        [HttpPost("register")]
        public async Task<ActionResult> RegisterUser([FromBody] RegisterUserDto dto)
        {
            await _authService.Register(dto);

            return Ok();
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> LoginUser([FromBody] LoginUserDto dto)
        {
            var token = await _authService.GenerateJwt(dto);
            var appUser = await _authService.GetUser(dto.Email);

            Response.Cookies.Append("X-Access-Token", token, new CookieOptions() { HttpOnly = true, Secure = true, SameSite = SameSiteMode.None });

            return Ok(new UserDto { Email = appUser.Email, AccountBalance = appUser.AccountBalance});
        }

        [Authorize]
        [HttpPost("logout")]
        public ActionResult Logout()
        {
            Response.Cookies.Delete("X-Access-Token", new CookieOptions() { HttpOnly = true, Secure = true, SameSite=SameSiteMode.None});

            return Ok();
        }

        [Authorize]
        [HttpGet("user")]
        public async Task<ActionResult<UserDto>> GetUser()
        {
            var appUser = await _authService.GetUser(_userContextService.GetUserEmail!);
            return Ok(new UserDto { Email = appUser.Email, AccountBalance = appUser.AccountBalance });
        }
    }
}
