namespace Blorc.OpenIdConnect.DemoApp
{
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Threading.Tasks;

    using Blorc.OpenIdConnect.DemoApp.Services;

    public class WeatherForecastHttpClient
    {
        private readonly HttpClient _http;

        private readonly IUserManager _userManager;

        public WeatherForecastHttpClient(HttpClient http, IUserManager userManager)
        {
            _http = http;
            _userManager = userManager;
        }

        public async Task<WeatherForecast[]> GetForecastAsync()
        {
            var user = await _userManager.GetUserAsync();
            _http.SetBearerToken(user.AccessToken);

            var forecasts = new WeatherForecast[0];

            try
            {
                forecasts = await _http.GetFromJsonAsync<WeatherForecast[]>("WeatherForecast");
            }
            catch
            {
            }

            return forecasts;
        }
    }
}
