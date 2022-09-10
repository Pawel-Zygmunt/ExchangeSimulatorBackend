using AutoMapper;
using ExchangeSimulatorBackend.Dtos;
using ExchangeSimulatorBackend.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ExchangeSimulatorBackend.Services
{
    public interface IAuthService
    {
        public Task<IdentityResult> Register(RegisterUserDto dto);
        public Task<string> GenerateJwt(LoginUserDto dto);
        public Task<UserDto> GetUserByEmail(string email);
    }


    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly AuthenticationSettings _authenticationSettings;
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;

        public AuthService(UserManager<AppUser> userManager, AuthenticationSettings authenticationSettings, AppDbContext dbContext, IMapper mapper)
        {
            _userManager = userManager;
            _authenticationSettings = authenticationSettings;
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<IdentityResult> Register(RegisterUserDto dto)
        {
            var newUser = new AppUser()
            {
                Email = dto.Email,
                UserName = dto.Email
            };

            var result = await _userManager.CreateAsync(newUser, dto.Password);

            if (result.Errors.Any())
            {
                throw new BadHttpRequestException("Błąd podczas rejestracji");
            }


            return result;
        }

        public async Task<string> GenerateJwt(LoginUserDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user is null)
            {
                throw new BadHttpRequestException("Invalid username or password");
            }

            var result = await _userManager.CheckPasswordAsync(user, dto.Password);

            if (!result)
            {
                throw new BadHttpRequestException("Invalid username or password");
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authenticationSettings.JwtKey));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(_authenticationSettings.JwtExpireDays);

            var token = new JwtSecurityToken(
                _authenticationSettings.JwtIssuer,
                _authenticationSettings.JwtIssuer,
                claims,
                expires: expires,
                signingCredentials: cred);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    
        public async Task<UserDto> GetUserByEmail(string email)
        {
            var user = await _dbContext.Users
                         .Include(u=>u.Transactions)
                         .FirstOrDefaultAsync(u => u.Email == email);
                         
            if (user is null)
            {
                throw new BadHttpRequestException("Invalid username or password");
            }

            return _mapper.Map<UserDto>(user);
        }
    
    }
}
