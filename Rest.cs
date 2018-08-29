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
            Uri uriResult = null;
            var ret = Uri.TryCreate(uri, UriKind.Absolute, out uriResponse)
                && (uriResponse.Scheme == Uri.UriSchemeHttp || uriResponse.Scheme == Uri.UriSchemeHttps);
            return ret;
        }

        internal static async Task<T> GetAsync<T>(string host
            , string service
            , System.Collections.Generic.Dictionary<string, object> query = null
            , AuthenticationHeaderValue authenticationHeader = null
            , ushort timeoutSec = TimeoutSeconds)
        {
            if (!CheckUri(host))
                throw new ArgumentException("host");

            if (string.IsNullOrWhiteSpace(service))
                throw new ArgumentException("service");

            if (query != null)
            {
                var queryArray = query.Select(x => $"{x.Key}={x.Value?.ToString()}").ToArray();
                service += '?' + string.Join("&", queryArray);
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
                        var responseModel = JsonConvert.DeserializeObject<T>(json);
                        return responseModel;
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
                        var responseModel = JsonConvert.DeserializeObject<T>(json);
                        return responseModel;
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
                {
                    stringContent = new StringContent(JsonConvert.SerializeObject(model)
                        , System.Text.Encoding.UTF8
                        , ApplicationJson);
                }

                using (var response = await httpClient.PostAsync(service, stringContent))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        var responseModel = JsonConvert.DeserializeObject<T>(json);
                        return responseModel;
                    }

                    throw new HttpRequestException(response.ReasonPhrase); //return default(T);
                }
            }
        }
        #endregion
    }
}
















using ModernHttpClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Agronegocio.Services
{
    public class Rest
    {
        #region Constannts
        private const String ApplicationJson = "application/json";
        #endregion

        #region Propriedades

        private HttpClient client = null;

        #endregion

        #region Construtores
        
        public Rest(string baseUrl, ushort timeOut = 60, string applicationJson = "application/json")
        {
			if (!CheckUri(baseUrl)
				throw new ArgumentException("baseUrl");
			
            client = new HttpClient
            {
                BaseAddress = new Uri(baseUrl),
                Timeout = TimeSpan.FromSeconds(TimeOut)
            };

            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(applicationJson));
        }

        #endregion

        #region Functions
        private bool CheckUri(string uri)
        {
            Uri uriResult = null;
            bool ret = Uri.TryCreate(uri, UriKind.Absolute, out uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
            return ret;
        }
            if (string.IsNullOrWhiteSpace(service))
            {
                throw new ArgumentException("service");
            }

            if (query != null
                && query.Count > 0)
            {
                var queryArray = query.Select(x => $"{x.Key}={x.Value?.ToString()}").ToArray();
                service += '?' + string.Join("&", queryArray);
            }

            if (headerCollection != null)
            {
                foreach (KeyValuePair<String, String> kvp in headerCollection)
                {
                    Client.DefaultRequestHeaders.Add(kvp.Key, kvp.Value);
                }
            }

            using (var response = await Client.GetAsync(service).ConfigureAwait(false))
            {
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var responseModel = JsonConvert.DeserializeObject<T>(json);
                    return responseModel;
                }

                throw new HttpRequestException(response.ReasonPhrase); //return default(T);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="service"></param>
        /// <param name="query"></param>
        /// <param name="authenticationHeader"></param>
        /// <returns></returns>
        public async Task<T> GetAsync<T>(string service
            , Dictionary<string, object> query = null
            , AuthenticationHeaderValue authenticationHeader = null
            , Dictionary<String, String> headerCollection = null)
        {
            if (string.IsNullOrWhiteSpace(service))
            {
                throw new ArgumentException("service");
            }

            if (query != null
                && query.Count > 0)
            {
                var queryArray = query.Select(x => $"{x.Key}={x.Value?.ToString()}").ToArray();
                service += '?' + string.Join("&", queryArray);
            }

            if (headerCollection != null)
            {
                foreach (KeyValuePair<String, String> kvp in headerCollection)
                {
                    Client.DefaultRequestHeaders.Add(kvp.Key, kvp.Value);
                }
            }

            using (var response = await Client.GetAsync(service).ConfigureAwait(false))
            {
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var responseModel = JsonConvert.DeserializeObject<T>(json);
                    return responseModel;
                }

                throw new HttpRequestException(response.ReasonPhrase); //return default(T);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="host"></param>
        /// <param name="service"></param>
        /// <param name="formUrlEncodedContent"></param>
        /// <param name="authenticationHeader"></param>
        /// <param name="timeoutSec"></param>
        /// <returns></returns>
        public async Task<T> PostAsync<T>(string service
            , FormUrlEncodedContent formUrlEncodedContent = null
			, Dictionary<String, String> headerCollection = null)
        {
            if (string.IsNullOrWhiteSpace(service))
            {
                throw new ArgumentException("service");
            }
			
			if (headerCollection != null)
            {
                foreach (KeyValuePair<String, String> kvp in headerCollection)
                {
                    Client.DefaultRequestHeaders.Add(kvp.Key, kvp.Value);
                }
            }

            using (var response = await Client.PostAsync(service, formUrlEncodedContent))
            {
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var responseModel = JsonConvert.DeserializeObject<T>(json);
                    return responseModel;
                }

                throw new HttpRequestException(response.ReasonPhrase); //return default(T);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="host"></param>
        /// <param name="service"></param>
        /// <param name="model"></param>
        /// <param name="authenticationHeader"></param>
        /// <param name="timeoutSec"></param>
        /// <returns></returns>
        public async Task<T> PostAsync<T>(string service
            , object model = null
            , Dictionary<String, String> headerCollection = null)
        {
            if (string.IsNullOrWhiteSpace(service))
            {
                throw new ArgumentException("service");
            }

            StringContent stringContent = null;

            if (model != null)
            {
                stringContent = new StringContent(JsonConvert.SerializeObject(model)
                    , System.Text.Encoding.UTF8
                    , ApplicationJson);
            }

            if (headerCollection != null)
            {
                foreach (KeyValuePair<String, String> kvp in headerCollection)
                {
                    Client.DefaultRequestHeaders.Add(kvp.Key, kvp.Value);
                }
            }

            using (var response = await Client.PostAsync(service, stringContent))
            {
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var responseModel = JsonConvert.DeserializeObject<T>(json);
                    return responseModel;
                }

                throw new HttpRequestException(response.ReasonPhrase); //return default(T);
            }
        }
            StringContent stringContent = null;
            stringContent = new StringContent(Json, System.Text.Encoding.UTF8, ApplicationJson);

            if (headerCollection != null)
            {
                foreach (KeyValuePair<String, String> kvp in headerCollection)
                {
                    Client.DefaultRequestHeaders.Add(kvp.Key, kvp.Value);
                }
            }

            using (var response = await Client.PostAsync(service, stringContent))
            {
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var responseModel = JsonConvert.DeserializeObject<T>(json);
                    return responseModel;
                }

                throw new HttpRequestException(response.ReasonPhrase); //return default(T);
            }
        }

        public async Task<T> DeleteAsync<T>(string service
            , Dictionary<string, object> query = null
            , Dictionary<String, String> headerCollection = null)
        {
            if (string.IsNullOrWhiteSpace(service))
            {
                throw new ArgumentException("service");
            }

            if (query != null
                && query.Count > 0)
            {
                var queryArray = query.Select(x => $"{x.Key}={x.Value?.ToString()}").ToArray();
                service += '?' + string.Join("&", queryArray);
            }

			if (headerCollection != null)
            {
                foreach (KeyValuePair<String, String> kvp in headerCollection)
                {
                    Client.DefaultRequestHeaders.Add(kvp.Key, kvp.Value);
                }
            }
			
            using (var response = await Client.DeleteAsync(service).ConfigureAwait(false))
            {
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var responseModel = JsonConvert.DeserializeObject<T>(json);
                    return responseModel;
                }

                throw new HttpRequestException(response.ReasonPhrase);
            }
        }
        #endregion
    }
}
