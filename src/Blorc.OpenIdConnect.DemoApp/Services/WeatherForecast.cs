﻿namespace Blorc.OpenIdConnect.DemoApp.Services
{
    using System;

    public class WeatherForecast
    {
        public DateTime Date { get; set; }

        public string Summary { get; set; }

        public int TemperatureC { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    }
}
