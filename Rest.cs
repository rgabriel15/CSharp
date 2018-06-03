using ModernHttpClient;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace RgSystems.Services
{
    internal static class Rest
    {
        #region Constants
        private const string ApplicationJson = "application/json";
        private const ushort TimeoutSeconds = 15;
        #endregion

        #region Functions
        private static bool CheckUri(string uri)
        {
            return Uri.TryCreate(uri, UriKind.Absolute, out Uri uriResponse)
                && (uriResponse.Scheme == Uri.UriSchemeHttp || uriResponse.Scheme == Uri.UriSchemeHttps);
        }

        internal static async Task<T> GetAsync<T>(string host
            , string service
            , System.Collections.Generic.Dictionary<string, string> query = null
            , AuthenticationHeaderValue authenticationHeader = null
            , ushort timeoutSec = TimeoutSeconds)
        {
            if (!CheckUri(host))
                throw new ArgumentException("host");
            if (string.IsNullOrWhiteSpace(service))
                throw new ArgumentException("service");

            if (query != null)
            {
                service += '?';
                var queryArray = query.Select(x => $"{x.Key}={x.Value}").ToArray();
                service += String.Join("&", queryArray);
            }

            using (var httpClient = new HttpClient(new NativeMessageHandler()))
            {
                httpClient.BaseAddress = new Uri(host);
                httpClient.Timeout = TimeSpan.FromSeconds(timeoutSec);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(ApplicationJson));
                httpClient.DefaultRequestHeaders.Authorization = authenticationHeader;

                using (var response = await httpClient.GetAsync(service).ConfigureAwait(false))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<T>(json);
                    }

                    throw new HttpRequestException(response.ReasonPhrase); //return default(T);
                }
            }
        }

        internal static async Task<T> PostAsync<T>(string host
            , string service
            , FormUrlEncodedContent formUrlEncodedContent = null
            , AuthenticationHeaderValue authenticationHeader = null
            , ushort timeoutSec = TimeoutSeconds)
        {
            if (!CheckUri(host))
                throw new ArgumentException("host");
            if (string.IsNullOrWhiteSpace(service))
                throw new ArgumentException("service");

            using (var httpClient = new HttpClient(new NativeMessageHandler()))
            {
                httpClient.BaseAddress = new Uri(host);
                httpClient.Timeout = TimeSpan.FromSeconds(timeoutSec);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(ApplicationJson));
                httpClient.DefaultRequestHeaders.Authorization = authenticationHeader;

                using (var response = await httpClient.PostAsync(service, formUrlEncodedContent))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<T>(json);
                    }

                    throw new HttpRequestException(response.ReasonPhrase); //return default(T);
                }
            }
        }

        internal static async Task<T> PostAsync<T>(string host
            , string service
            , object model = null
            , AuthenticationHeaderValue authenticationHeader = null
            , ushort timeoutSec = TimeoutSeconds)
        {
            if (!CheckUri(host))
                throw new ArgumentException("host");
            if (string.IsNullOrWhiteSpace(service))
                throw new ArgumentException("service");

            using (var httpClient = new HttpClient(new NativeMessageHandler()))
            {
                httpClient.BaseAddress = new Uri(host);
                httpClient.Timeout = TimeSpan.FromSeconds(timeoutSec);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(ApplicationJson));
                httpClient.DefaultRequestHeaders.Authorization = authenticationHeader;

                StringContent stringContent = null;

                if (model != null)
                    stringContent = new StringContent(JsonConvert.SerializeObject(model)
                        , System.Text.Encoding.UTF8
                        , ApplicationJson);

                using (var response = await httpClient.PostAsync(service, stringContent))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<T>(json);
                    }

                    throw new HttpRequestException(response.ReasonPhrase); //return default(T);
                }
            }
        }
        #endregion
    }
}
