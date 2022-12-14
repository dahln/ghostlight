using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components;
using ghostlight.Shared;
using Blazored.Toast.Services;
using BlazorSpinner;

namespace ghostlight.Client.Services
{
    public class API
    {
        private HttpClient _httpClient { get; set; }
        private NavigationManager _navigationManger { get; set; }
        private IToastService _toastService { get; set; }
        private SpinnerService _spinnerService { get; set; }
        private IAccessTokenProvider _authenticationService { get; set; }
        public API(HttpClient httpClient, IAccessTokenProvider authenticationService, NavigationManager navigationManager, IToastService toastService, SpinnerService spinnerService)
        {
            _httpClient = httpClient;

            _authenticationService = authenticationService;
            _navigationManger = navigationManager;
            _toastService = toastService;
            _spinnerService = spinnerService;
        }

        #region Customer CRUD/Search
        async public Task<Customer> CustomerCreate(Customer content)
        {
            return await PostAsync<Customer>("api/v1/customer", content);
        }
        async public Task<Customer> CustomerGetById(string id)
        {
            return await GetAsync<Customer>($"api/v1/customer/{id}");
        }
        async public Task<Customer> CustomerUpdateById(Customer content, string id)
        {
            return await PutAsync<Customer>($"api/v1/customer/{id}", content);
        }
        async public Task CustomerDeleteById(string id)
        {
            await DeleteAsync($"api/v1/customer/{id}");
        }
        async public Task<SearchResponse<CustomerSlim>> CustomerSearch(Search content)
        {
            return await PostAsync<SearchResponse<CustomerSlim>>("api/v1/customers", content);
        }
        #endregion

        async public Task SeedDB(int number)
        {
            await GetAsync($"api/v1/seed/create/{number}");
        }
        async public Task SeedDBClear()
        {
            await GetAsync($"api/v1/seed/clear");
        }


        #region HTTP Methods
        private async Task GetAsync(string path)
        {
            await Send(HttpMethod.Get, path);
        }
        private async Task<T> GetAsync<T>(string path)
        {
            var response = await Send(HttpMethod.Get, path);
            T result = await ParseResponseObject<T>(response);
            return result;
        }
        private async Task PostAsync(string path, object content)
        {
            await Send(HttpMethod.Post, path, content);
        }
        private async Task<T> PostAsync<T>(string path, object content)
        {
            var response = await Send(HttpMethod.Post, path, content);
            return await ParseResponseObject<T>(response);
        }
        private async Task PutAsync(string path, object content)
        {
            await Send(HttpMethod.Put, path, content);
        }
        private async Task<T> PutAsync<T>(string path, object content)
        {
            var response = await Send(HttpMethod.Put, path, content);
            return await ParseResponseObject<T>(response);
        }
        private async Task PutAsync(string path)
        {
            await Send(HttpMethod.Put, path);
        }
        private async Task DeleteAsync(string path)
        {
            await Send(HttpMethod.Delete, path);
        }
        private async Task DeleteAsync(string path, object content)
        {
            await Send(HttpMethod.Delete, path, content);
        }
        
        
        private async Task<HttpResponseMessage> Send(HttpMethod method, string path, object content = null)
        {
            _spinnerService.Show();

            var httpWebRequest = new HttpRequestMessage(method, path);

            if (content != null)
            {
                string json = JsonConvert.SerializeObject(content);
                StringContent postContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

                httpWebRequest.Content = postContent;
            }

            HttpResponseMessage response = new HttpResponseMessage()
            {
                StatusCode = System.Net.HttpStatusCode.BadRequest
            };
            try
            {
                response = await _httpClient.SendAsync(httpWebRequest);

                if (response.IsSuccessStatusCode == false)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    if(!string.IsNullOrEmpty(responseContent))
                    {
                        _toastService.ShowError(responseContent);
                    }                    
                }

                _spinnerService.Hide();
            }
            catch (AccessTokenNotAvailableException exception)
            {
                exception.Redirect();
            }
            return response;
        }

        private async Task<T> ParseResponseObject<T>(HttpResponseMessage response)
        {
            if (response != null && response.IsSuccessStatusCode && response.Content != null)
            {
                string responseContent = await response.Content.ReadAsStringAsync();

                //Can't deseriazlize a string unless it starts with a "
                if (typeof(T) == typeof(string))
                    responseContent = $"\"{responseContent}\"";

                return JsonConvert.DeserializeObject<T>(responseContent);
            }
            else
            {
                return default(T);
            }
        }
        #endregion
    }
}
