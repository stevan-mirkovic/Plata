using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Plata.Services.Settings;

namespace Plata.Services
{
    public class ExchangeRateApi(HttpClient httpClient, IOptions<ExchangeRateApiSettings> settings)
    {
        public async Task<decimal> GetConversionRate(string toCurrency)
        {
            if (toCurrency == settings.Value.BaseCurrency) return 1;

            var url = $"{settings.Value.BaseUrl}/{settings.Value.ApiKey}/latest/{settings.Value.BaseCurrency}";
            var response = await httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode) throw new HttpRequestException();
            var json = await response.Content.ReadAsStringAsync();
            var deserializer = JsonConvert.DeserializeObject<ExchangeRateDeserializer>(json);

            return deserializer.ConversionRates[toCurrency];
        }

        public async Task<decimal> ConvertToBaseCurrency(decimal value, string fromCurrency)
        {
            var conversionRate = await GetConversionRate(fromCurrency);
            return value / conversionRate;
        }

        public string GetCurrencySymbol(string currency)
        {
            return currency == "RSD" ? "RSD" : currency == "EUR" ? "€" : currency == "USD" ? "$" : "";
        }

        private class ExchangeRateDeserializer
        {
            [JsonProperty("conversion_rates")]
            public Dictionary<string, decimal> ConversionRates { get; set; }
        }
    }
}