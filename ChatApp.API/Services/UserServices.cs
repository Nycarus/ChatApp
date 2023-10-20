using AutoMapper;
using ChatApp.Data;
using ChatApp.DtoLibrary;
using ChatApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ChatApp.Services
{
    public interface IUserServices
    {
        public Task PasswordRegister(UserRegistrationDTO userRegistrationDTO);
        public Task<String> PasswordLogin(UserLoginDTO userLoginDTO);
    }
    public class UserServices : IUserServices
    {
        private readonly ILogger<UserServices> _logger;
        private readonly IConfiguration _configuration;
        private readonly DataContext _dataContext;
        private readonly IMapper _mapper;

        public UserServices(ILogger<UserServices> logger, IConfiguration configuration, DataContext dataContext, IMapper mapper)
        {
            _logger = logger;
            _configuration = configuration;
            _dataContext = dataContext;
            _mapper = mapper;
        }

        public async Task PasswordRegister(UserRegistrationDTO userRegistrationDTO)
        {   
            // Search if User Exists
            var existingUser = _dataContext.Users.Any(User => User.Username == userRegistrationDTO.Username);

            if (existingUser)
            {
                throw new Exception("User Already Exists.");
            }

            // Create new user and profile
            User userModel = new User();
            userModel.Username = userRegistrationDTO.Username; 
            userModel.Password = BCrypt.Net.BCrypt.HashPassword(userRegistrationDTO.Password);
            _dataContext.Users.Add(userModel);
            await _dataContext.SaveChangesAsync();

            UserProfile profileModel = new UserProfile();
            profileModel.Username = userRegistrationDTO.Username;
            profileModel.User = userModel;
            _dataContext.UserProfiles.Add(profileModel);
            await _dataContext.SaveChangesAsync();
        }

        public async Task<String> PasswordLogin(UserLoginDTO userLoginDTO)
        {
            // Search for existing user
            var existingUser = _dataContext.Users.SingleOrDefault(user =>  user.Username == userLoginDTO.Username);

            // Validate credentials
            if (existingUser == null || !BCrypt.Net.BCrypt.Verify(userLoginDTO.Password, existingUser.Password))
            {
                throw new Exception("Username or Password is incorrect.");
            }

            var userProfile = _dataContext.UserProfiles.SingleOrDefault(o => o.UserId == existingUser.Id);

            if (userProfile == null)
            {
                _logger.LogError("Unable to retrieve user profile:", userLoginDTO.Username);
                throw new Exception("Something went wrong retrieving user profile.");
            }

            // Create JWT
            var claim = new List<Claim>
            {
                new Claim(ClaimTypes.Name, userLoginDTO.Username),
                new Claim("UserProfileId", userProfile.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                expires: DateTime.Now.AddHours(3),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("JwtSettings:Key").Value)), SecurityAlgorithms.HmacSha512),
                claims: claim
                );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }
    }
}
