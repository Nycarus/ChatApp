using ChatApp.API.Controllers;
using ChatApp.API.Services;
using System.Security.Claims;

namespace ChatApp.API.Utils
{
    public interface IUserContext
    {
        public int getUserProfileId();
        public string getUsername();
    }
    public class UserContext : IUserContext
    {
        private readonly ILogger<UserContext> _logger;
        private readonly IHttpContextAccessor _contextAccessor;
        public UserContext(ILogger<UserContext> logger, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _contextAccessor = httpContextAccessor;
        }

        public int getUserProfileId()
        {
            var user = _contextAccessor.HttpContext.User;

            if (user == null)
            {
                throw new Exception("User does not exists.");
            }

            var userProfileClaim = user.FindFirst("UserProfileId");

            if (userProfileClaim == null)
            {
                throw new Exception("JWT user has raised an error.");
            }
            else if (!int.TryParse(userProfileClaim.Value, out int n))
            {
                throw new Exception("JWT user is not the correct type.");
            }

            return Int32.Parse(userProfileClaim.Value);
        }

        public string getUsername()
        {
            var user = _contextAccessor.HttpContext.User;

            if (user == null)
            {
                throw new Exception("User does not exists.");
            }

            var userNameClaim = user.FindFirst(ClaimTypes.Name);

            if (userNameClaim == null)
            {
                throw new Exception("JWT user has raised an error.");
            }

            return userNameClaim.Value;
        }
    }
}
