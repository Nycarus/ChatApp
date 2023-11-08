using ChatApp.DtoLibrary;
using Newtonsoft.Json;

namespace ChatApp.Client.Data
{
    public interface IAuthenticationService
    {
        public Task<bool> Login(string username, string password);
        public Task<bool> Register(string username, string password);

        public Task<string> GetUserSession();
    }
    public class AuthenticationService : IAuthenticationService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        public AuthenticationService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<bool> Login(string username, string password)
        {
            UserLoginDTO loginDTO = new UserLoginDTO()
            {
                Username = username,
                Password = password
            };

            var data = new StringContent(JsonConvert.SerializeObject(loginDTO), System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_configuration.GetSection("API").Value}/api/user/login", data);


            return response.IsSuccessStatusCode;
        }

        public async Task<bool> Register(string username, string password)
        {
            UserRegistrationDTO registerDTO = new UserRegistrationDTO()
            {
                Username = username,
                Password = password
            };

            var data = new StringContent(JsonConvert.SerializeObject(registerDTO), System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_configuration.GetSection("API").Value}/api/user/register", data);

            return response.IsSuccessStatusCode;
        }

        public async Task<string> GetUserSession()
        {
            var response = await _httpClient.GetAsync($"{_configuration.GetSection("API").Value}/api/user/auth");

            if (response.IsSuccessStatusCode)
            {
                return response.Content.ReadAsStringAsync().Result;
            }
            else
            {
                return null;
            }
        }
    }
}