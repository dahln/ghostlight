using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ghostlight.Client.Services
{
    //https://chrissainty.com/3-ways-to-communicate-between-components-in-blazor/
    public class AppState
    {
        public event Action OnChange;

        private API _api;
        private ILocalStorageService _localStorage;
        private AuthenticationStateProvider _authenticationStateProvider;

        public AppState(API api, ILocalStorageService localStorage, AuthenticationStateProvider authenticationStateProvider)
        {
            _api = api;
            _localStorage = localStorage;

            _authenticationStateProvider = authenticationStateProvider;
        }

        async public Task<bool> UpdateAppState(string selectedFolderId = null)
        {
            var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            if (authState.User.Identity.IsAuthenticated)
            {
                return true;
            }
            return false;
        }


        private void NotifyStateChanged() => OnChange?.Invoke();

    }
}
