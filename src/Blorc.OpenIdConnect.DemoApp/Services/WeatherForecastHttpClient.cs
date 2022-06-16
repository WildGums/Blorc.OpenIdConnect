namespace Blorc.OpenIdConnect.DemoApp
{
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Threading.Tasks;

    using Blorc.OpenIdConnect.DemoApp.Services;

    public class WeatherForecastHttpClient
    {
        private readonly HttpClient _http;

        public WeatherForecastHttpClient(HttpClient http)
        {
            _http = http;
        }

        public async Task<WeatherForecast[]> GetForecastAsync()
        {
            var forecasts = Array.Empty<WeatherForecast>();

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
