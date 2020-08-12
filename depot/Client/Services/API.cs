using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components;
using depot.Shared.RequestModels;
using depot.Shared.ResponseModels;
using Blazored.Toast.Services;
using BlazorSpinner;

namespace depot.Client.Services
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



        async public Task<ResponseId> FolderCreate(FolderCreateEditRequestModel content)
        {
            return await PostAsync<ResponseId>("api/v1/folder", content);
        }

        async public Task FolderEditName(string folderId, FolderCreateEditRequestModel content)
        {
            await PutAsync($"api/v1/folder/{folderId}", content);
        }

        async public Task<ResponseFolder> FolderGetById(string folderId)
        {
            return await GetAsync<ResponseFolder>($"api/v1/folder/{folderId}");
        }

        async public Task<List<ResponseFolderShort>> GetFoldersByAuthorizedUser()
        {
            return await GetAsync<List<ResponseFolderShort>>("api/v1/folder/user/authorized");
        }

        async public Task UpdateFolderSetUserAuthorized(string folderId, FolderAddAuthorizedEmailModel model)
        {
            await PutAsync($"api/v1/folder/{folderId}/user/authorized", model);
        }

        async public Task UpdateFolderToggleUserAdministrator(string folderId, string applicationUserId, FolderToggleAuthorizedModel model)
        {
            await PutAsync($"api/v1/folder/{folderId}/user/{applicationUserId}/authorized/toggle", model);
        }

        async public Task UpdateFolderRemoveUserAuthorized(string folderId, string applicationUserId)
        {
            await DeleteAsync($"api/v1/folder/{folderId}/user/{applicationUserId}/authorized");
        }

        async public Task DeleteFolder(string folderId)
        {
            await DeleteAsync($"api/v1/folder/{folderId}");
        }

        async public Task<ResponseId> CreateFolderType(string folderId, ResponseDataType content )
        {
            return await PostAsync<ResponseId>($"api/v1/folder/{folderId}/type", content);
        }

        async public Task<ResponseId> UpdateFolderTypeName(string folderId, string dataTypeId, ResponseDataType content)
        {
            return await PostAsync<ResponseId>($"api/v1/folder/{folderId}/type/{dataTypeId}", content);
        }

        async public Task<ResponseDataType> GetFolderTypeById(string folderId, string dataTypeId)
        {
            return await GetAsync<ResponseDataType>($"api/v1/folder/{folderId}/type/{dataTypeId}");
        }
        async public Task DeleteFolderTypeById(string folderId, string dataTypeId)
        {
            await DeleteAsync($"api/v1/folder/{folderId}/type/{dataTypeId}");
        }


        async public Task<List<ResponseDataType>> GetFolderTypeAsList(string folderId)
        {
            return await GetAsync<List<ResponseDataType>>($"api/v1/folder/{folderId}/type");
        }

        async public Task<List<ResponseDataType>> GetFolderTypeAsMenuOptionList(string folderId)
        {
            return await GetAsync<List<ResponseDataType>>($"api/v1/folder/{folderId}/type/menu");
        }

        async public Task<ResponseId> CreateFolderDataTypeField(string folderId, string dataTypeId, ResponseField content)
        {
            return await PostAsync<ResponseId>($"api/v1/folder/{folderId}/type/{dataTypeId}/field", content);
        }

        async public Task<ResponseId> UpdateFolderDataTypeField(string folderId, string dataTypeId, string fieldId, ResponseField content)
        {
            return await PutAsync<ResponseId>($"api/v1/folder/{folderId}/type/{dataTypeId}/field/{fieldId}", content);
        }

        async public Task DeleteFolderDataTypeField(string folderId, string dataTypeId, string fieldId)
        {
            await DeleteAsync($"api/v1/folder/{folderId}/type/{dataTypeId}/field/{fieldId}");
        }

        async public Task<ResponseId> CreateFolderInstance(string folderId, string dataTypeId, Dictionary<string,string> model)
        {
            ResponseInstance content = new ResponseInstance()
            {
                Data = model
            };
            return await PostAsync<ResponseId>($"api/v1/folder/{folderId}/type/{dataTypeId}/instance", content);
        }

        async public Task UpdateFolderInstance(string folderId, string dataTypeId, string instanceId, Dictionary<string, string> model)
        {
            ResponseInstance content = new ResponseInstance()
            {
                Id = instanceId,
                Data = model
            };
            await PutAsync($"api/v1/folder/{folderId}/type/{dataTypeId}/instance/{instanceId}", content);
        }

        async public Task<ResponseInstance> GetFolderInstance(string folderId, string dataTypeId, string instanceId)
        {
            return await GetAsync<ResponseInstance>($"api/v1/folder/{folderId}/type/{dataTypeId}/instance/{instanceId}");
        }

        async public Task DeleteFolderInstance(string folderId, string dataTypeId, string instanceId)
        {
            await DeleteAsync($"api/v1/folder/{folderId}/type/{dataTypeId}/instance/{instanceId}");
        }

        async public Task<InstanceSearchResponse> SearchFolderInstance(string folderId, string dataTypeId, Search content)
        {
            return await PostAsync<InstanceSearchResponse>($"api/v1/folder/{folderId}/type/{dataTypeId}/search", content);
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
