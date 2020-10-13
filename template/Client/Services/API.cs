using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components;
using template.Shared;
using Blazored.Toast.Services;
using BlazorSpinner;

namespace template.Client.Services
{
    public class API
    {
        private HttpClient _client { get; set; }
        private IAccessTokenProvider _authenticationService { get; set; }
        private NavigationManager _navigationManger { get; set; }
        private IToastService _toastService { get; set; }
        private SpinnerService _spinnerService { get; set; }
        public API(IAccessTokenProvider authenticationService, NavigationManager navigationManager, IToastService toastService, SpinnerService spinnerService)
        {
            _authenticationService = authenticationService;
            _navigationManger = navigationManager;
            _toastService = toastService;
            _spinnerService = spinnerService;

            _client = new HttpClient();
            _client.BaseAddress = new Uri(_navigationManger.BaseUri);
        }



        async public Task<CustomerResponse> AccountCreate(CustomerRequest content)
        {
            return await PostAsync<CustomerResponse>("api/v1/customer", content);
        }
        async public Task<CustomerResponse> CustomerGetById(string id)
        {
            return await GetAsync<CustomerResponse>("api/v1/customer/{id}");
        }
        async public Task<CustomerResponse> CustomerUpdateById(CustomerRequest content)
        {
            return await PutAsync<CustomerResponse>("api/v1/customer/{id}", content);
        }
        async public Task<CustomerSearchResponse> CustomerSearch(Search content)
        {
            return await PostAsync<CustomerSearchResponse>("api/v1/customers", content);
        }



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
            string guid = Guid.NewGuid().ToString();
            _spinnerService.Show();

            var httpWebRequest = new HttpRequestMessage(method, path);

            var tokenResult = await _authenticationService.RequestAccessToken();
            if (tokenResult.TryGetToken(out var token))
            {
                httpWebRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.Value);
            }
            else
            {
                _navigationManger.NavigateTo(tokenResult.RedirectUrl);
            }

            if (content != null)
            {
                string json = JsonConvert.SerializeObject(content);
                StringContent postContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

                httpWebRequest.Content = postContent;
            }

            HttpResponseMessage response = await _client.SendAsync(httpWebRequest);

            if (response.IsSuccessStatusCode == false)
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                _toastService.ShowError(responseContent);
            }

            _spinnerService.Hide();
            return response;
        }

        private async Task<T> ParseResponseObject<T>(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(responseContent);
            }
            else
            {
                return default(T);
            }
        }
    }
}
