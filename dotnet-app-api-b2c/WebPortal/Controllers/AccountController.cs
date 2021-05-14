using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WebPortal.Controllers
{
    [Route("[controller]")]
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly HttpClient _httpClient;
        private readonly string _WeatherApiScope = string.Empty;
        private readonly string _WeatherApiScopeUrl = string.Empty;
        private readonly ITokenAcquisition _tokenAcquisition;

        public AccountController (ITokenAcquisition tokenAcquisition, HttpClient httpClient,
            IConfiguration configuration, IHttpContextAccessor contextAccessor, ILogger<AccountController> logger)
        {
            _logger = logger;
            _httpClient = httpClient;
            _tokenAcquisition = tokenAcquisition;
            _contextAccessor = contextAccessor;
            _WeatherApiScope = configuration["ServiceApis:weatherApi:weatherApiScope"];
            _WeatherApiScopeUrl = configuration["ServiceApis:weatherApi:weatherApiUrl"];
        }

        private async Task GetToken()
        {
            /* 
                Acquires access token from cache if present, else gets it from AAD B2C.
             */
            var accessToken = await _tokenAcquisition.GetAccessTokenForUserAsync(new[] { _WeatherApiScope });
            _logger.LogInformation($"access token: {accessToken.ToString()}");

            /* 
                Sets the Bearer header with the access token on the HTTP Client used to 
                call the remote APIs
             */
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        [HttpGet("getecho")]
        public async Task<dynamic> GetEcho()
        {
            //await GetToken();

            var response = await _httpClient.GetAsync($"{_WeatherApiScopeUrl}/WeatherForecast/echo");
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var content = await response.Content.ReadAsStringAsync();
                //var data = JsonSerializer.Deserialize<dynamic>(content);

                return content;
            }

            throw new HttpRequestException($"Invalid status code in the HttpResponseMessage: {response.StatusCode}.");
        }

        [HttpGet("getweather")]
        public async Task<dynamic> GetWeather()
        {
            await this.GetToken();

            var response = await _httpClient.GetAsync($"{_WeatherApiScopeUrl}/WeatherForecast");
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var content = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<dynamic>(content);

                return data;
            }

            throw new HttpRequestException($"Invalid status code in the HttpResponseMessage: {response.StatusCode}.");
        }
    }
}