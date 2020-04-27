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

namespace depot.Client
{
    public class API
    {
        private HttpClient _client { get; set; }
        private IAccessTokenProvider _authenticationService { get; set; }
        private NavigationManager _navigationManger { get; set; }
        private IToastService _toastService { get; set; }
        public API(IAccessTokenProvider authenticationService, NavigationManager navigationManager, IToastService toastService)
        {
            _authenticationService = authenticationService;
            _navigationManger = navigationManager;
            _toastService = toastService;

            _client = new HttpClient();
            _client.BaseAddress = new Uri(_navigationManger.BaseUri);
        }



        async public Task GroupCreate(GroupCreateEditRequestModel content)
        {
            await PostAsync<ResponseId>("api/v1/Group", content);
        }

        async public Task GroupEditName(string GroupId, GroupCreateEditRequestModel content)
        {
            await PutAsync($"api/v1/Group/{GroupId}", content);
        }

        async public Task<ResponseGroup> GroupGetById(string GroupId)
        {
            return await GetAsync<ResponseGroup>($"api/v1/Group/{GroupId}");
        }

        async public Task<List<ResponseGroupShort>> GetGroupsByAuthorizedUser()
        {
            return await GetAsync<List<ResponseGroupShort>>("api/v1/Group/user/authorized");
        }

        async public Task UpdateGroupSetUserAuthorized(string GroupId, GroupAddAuthorizedEmailModel model)
        {
            await PutAsync($"api/v1/Group/{GroupId}/user/authorized", model);
        }

        async public Task UpdateGroupToggleUserAdministrator(string GroupId, string applicationUserId, GroupToggleAuthorizedModel model)
        {
            await PutAsync($"api/v1/Group/{GroupId}/user/{applicationUserId}/authorized/toggle", model);
        }

        async public Task UpdateGroupRemoveUserAuthorized(string GroupId, string applicationUserId)
        {
            await DeleteAsync($"api/v1/Group/{GroupId}/user/{applicationUserId}/authorized");
        }

        async public Task DeleteGroup(string GroupId)
        {
            await DeleteAsync($"api/v1/Group/{GroupId}");
        }

        async public Task<ResponseId> CreateGroupType(string GroupId, ResponseInstanceType content )
        {
            return await PostAsync<ResponseId>($"api/v1/Group/{GroupId}/type", content);
        }

        async public Task<ResponseInstanceType> GetGroupTypeById(string GroupId, string instanceTypeId)
        {
            return await GetAsync<ResponseInstanceType>($"api/v1/Group/{GroupId}/type/{instanceTypeId}");
        }

        async public Task<List<ResponseInstanceType>> GetGroupTypeAsList(string GroupId)
        {
            return await GetAsync<List<ResponseInstanceType>>($"api/v1/Group/{GroupId}/type");
        }

        async public Task<List<ResponseInstanceType>> GetGroupTypeAsMenuOptionList(string GroupId)
        {
            return await GetAsync<List<ResponseInstanceType>>($"api/v1/Group/{GroupId}/type/menu");
        }

        async public Task<ResponseId> CreateGroupInstanceTypeField(string GroupId, string instanceTypeId, ResponseField content)
        {
            return await PostAsync<ResponseId>($"api/v1/Group/{GroupId}/type/{instanceTypeId}/field", content);
        }

        async public Task<ResponseId> UpdateGroupInstanceTypeField(string GroupId, string instanceTypeId, string fieldId, ResponseField content)
        {
            return await PutAsync<ResponseId>($"api/v1/Group/{GroupId}/type/{instanceTypeId}/field/{fieldId}", content);
        }

        async public Task DeleteGroupInstanceTypeField(string GroupId, string instanceTypeId, string fieldId)
        {
            await DeleteAsync($"api/v1/Group/{GroupId}/type/{instanceTypeId}/field/{fieldId}");
        }

        async public Task CreateGroupInstance(string GroupId, string instanceTypeId, Dictionary<string,string> model)
        {
            ResponseInstance content = new ResponseInstance()
            {
                InstanceData = model
            };
            await PostAsync($"api/v1/Group/{GroupId}/type/{instanceTypeId}/instance", content);
        }

        async public Task UpdateGroupInstance(string GroupId, string instanceTypeId, string instanceId, Dictionary<string, string> model)
        {
            ResponseInstance content = new ResponseInstance()
            {
                Id = instanceId,
                InstanceData = model
            };
            await PutAsync($"api/v1/Group/{GroupId}/type/{instanceTypeId}/instance/{instanceId}", content);
        }

        async public Task<ResponseInstance> GetGroupInstance(string GroupId, string instanceTypeId, string instanceId)
        {
            return await GetAsync<ResponseInstance>($"api/v1/Group/{GroupId}/type/{instanceTypeId}/instance/{instanceId}");
        }

        async public Task DeleteGroupInstance(string GroupId, string instanceTypeId, string instanceId)
        {
            await DeleteAsync($"api/v1/Group/{GroupId}/type/{instanceTypeId}/instance/{instanceId}");
        }

        async public Task<InstanceSearchResponse> SearchGroupInstance(string GroupId, string instanceTypeId, Search content)
        {
            return await PostAsync<InstanceSearchResponse>($"api/v1/Group/{GroupId}/type/{instanceTypeId}/search", content);
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
