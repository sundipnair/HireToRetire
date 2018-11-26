using Microsoft.Identity.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace HireToRetire.Service
{
    public partial class ApiService
    {

        private readonly HttpClient _httpClient;
        private Uri BaseEndpoint { get; set; }

        public ApiService(Uri baseEndpoint)
        {
            if (baseEndpoint == null)
            {
                throw new ArgumentNullException("baseEndpoint");
            }
            BaseEndpoint = baseEndpoint;
            _httpClient = new HttpClient();
        }

        /// <summary>  
        /// Common method for making GET calls  
        /// </summary>  
        internal async Task<T> GetAsync<T>(Uri requestUrl, AuthenticationResult result)
        {
            AddHeaders(result);
            var response = await _httpClient.GetAsync(requestUrl, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(data);
        }

        /// <summary>  
        /// Common method for making POST calls  
        /// </summary>  
        internal async void PostAsync<T>(Uri requestUrl, T content, AuthenticationResult result)
        {
            AddHeaders(result);
            var response = await _httpClient.PostAsync(requestUrl.ToString(), CreateHttpContent<T>(content));
            response.EnsureSuccessStatusCode();
            //var data = await response.Content.ReadAsStringAsync();
            //return JsonConvert.DeserializeObject<Message<T>>(data);
        }

        /// <summary>  
        /// Common method for making PUT calls  
        /// </summary>  
        internal async void PutAsync<T>(Uri requestUrl, T content, AuthenticationResult result)
        {
            AddHeaders(result);
            var response = await _httpClient.PutAsync(requestUrl.ToString(), CreateHttpContent<T>(content));
            response.EnsureSuccessStatusCode();
            //var data = await response.Content.ReadAsStringAsync();
            //return JsonConvert.DeserializeObject<Message<T>>(data);
        }

        /// <summary>  
        /// Common method for making DELETE calls  
        /// </summary>  
        internal async void DeleteAsync<T>(Uri requestUrl, AuthenticationResult result)
        {
            AddHeaders(result);
            var response = await _httpClient.DeleteAsync(requestUrl.ToString());
            response.EnsureSuccessStatusCode();
            //var data = await response.Content.ReadAsStringAsync();
            //return JsonConvert.DeserializeObject<Message<T>>(data);
        }

        private Uri CreateRequestUri(string relativePath, string queryString = "")
        {
            var endpoint = new Uri(BaseEndpoint, relativePath);
            var uriBuilder = new UriBuilder(endpoint);
            uriBuilder.Query = queryString;
            return uriBuilder.Uri;
        }

        private HttpContent CreateHttpContent<T>(T content)
        {
            var json = JsonConvert.SerializeObject(content, MicrosoftDateFormatSettings);
            return new StringContent(json, Encoding.UTF8, "application/json");
        }

        private static JsonSerializerSettings MicrosoftDateFormatSettings
        {
            get
            {
                return new JsonSerializerSettings
                {
                    DateFormatHandling = DateFormatHandling.MicrosoftDateFormat
                };
            }
        }

        private void AddHeaders(AuthenticationResult result)
        {
            //_httpClient.DefaultRequestHeaders.Remove("userIP");
            //_httpClient.DefaultRequestHeaders.Add("userIP", "192.168.1.1");

            if (result != null)
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);
            _httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "d8f0219eb112429ea89804114d4aff2a");
            _httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Trace", "true");
        }
    }
}
