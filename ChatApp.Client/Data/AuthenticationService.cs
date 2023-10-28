using ChatApp.DtoLibrary;
using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using System.Net.Http;

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
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        public AuthenticationService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<bool> Login(string username, string password)
        {
            UserLoginDTO loginDTO = new UserLoginDTO()
            {
                Username = username,
                Password = password
            };

            HttpClient httpClient = _httpClientFactory.CreateClient("ChatAppApi");
            var data = new StringContent(JsonConvert.SerializeObject(loginDTO), System.Text.Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync($"{_configuration.GetSection("API").Value}/api/user/login", data);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> Register(string username, string password)
        {
            UserRegistrationDTO registerDTO = new UserRegistrationDTO()
            {
                Username = username,
                Password = password
            };

            HttpClient httpClient = _httpClientFactory.CreateClient("ChatAppApi");
            var data = new StringContent(JsonConvert.SerializeObject(registerDTO), System.Text.Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync($"{_configuration.GetSection("API").Value}/api/user/register", data);

            return response.IsSuccessStatusCode;
        }

        public async Task<string> GetUserSession()
        {
            HttpClient httpClient = _httpClientFactory.CreateClient("ChatAppApi");
            var response = await httpClient.GetAsync($"{_configuration.GetSection("API").Value}/api/user/auth");

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
