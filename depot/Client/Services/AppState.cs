using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace depot.Client.Services
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

            DataTypes = new List<FolderTypeNav>();
            AllowedFolders = new List<AllowedFolder>();
            _authenticationStateProvider = authenticationStateProvider;
        }

        async public Task<bool> UpdateAppState(string selectedFolderId = null)
        {
            var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            if (authState.User.Identity.IsAuthenticated)
            {
                var userFolders = await _api.GetFoldersByAuthorizedUser();

                if (userFolders.Count > 0)
                {
                    AllowedFolders = userFolders.Select(o => new AllowedFolder() { Name = o.Name, Id = o.Id, IsAdministrator = o.IsAdministrator }).ToList();

                    if (selectedFolderId == null)
                    {
                        selectedFolderId = await _localStorage.GetItemAsync<string>("folderId");
                        if (selectedFolderId == null && AllowedFolders.Any())
                        {
                            selectedFolderId = AllowedFolders.FirstOrDefault().Id;
                        }
                    }

                    if (selectedFolderId != null)
                    {
                        var selectedFolder = userFolders.FirstOrDefault(g => g.Id == selectedFolderId);
                        if (selectedFolder != null)
                        {
                            CurrentFolderName = selectedFolder.Name;
                            CurrentFolderId = selectedFolder.Id;
                            CurrentFolderIsAdministrator = selectedFolder.IsAdministrator;

                            var folderTypes = await _api.GetFolderTypeAsMenuOptionList(selectedFolder.Id);
                            DataTypes = folderTypes.Select(o => new FolderTypeNav() { Text = o.Name, Data = o.Id }).ToList();

                            await _localStorage.SetItemAsync("folderId", selectedFolderId);
                        }
                    }
                }
                else
                {
                    AllowedFolders = new List<AllowedFolder>();
                    CurrentFolderId = default(string);
                    CurrentFolderName = default(string);
                    CurrentFolderIsAdministrator = false;
                    DataTypes = new List<FolderTypeNav>();
                }

                return true;
            }
            return false;
        }


        private void NotifyStateChanged() => OnChange?.Invoke();



        private string _currentFolderId;
        public string CurrentFolderId
        {
            get
            {
                return _currentFolderId;
            }
            set
            {
                _currentFolderId = value;
                NotifyStateChanged();
            }
        }

        private bool _currentFolderIsAdministrator;
        public bool CurrentFolderIsAdministrator
        {
            get
            {
                return _currentFolderIsAdministrator;
            }
            set
            {
                _currentFolderIsAdministrator = value;
                NotifyStateChanged();
            }
        }


        private string _currentFolderName;
        public string CurrentFolderName
        {
            get
            {
                return _currentFolderName;
            }
            set
            {
                _currentFolderName = value;
                NotifyStateChanged();
            }
        }


        private List<FolderTypeNav> _dataTypes;
        public List<FolderTypeNav> DataTypes
        {
            get
            {
                return _dataTypes;
            }
            set
            {
                _dataTypes = value;
                NotifyStateChanged();
            }
        }


        private List<AllowedFolder> _allowedFolders;
        public List<AllowedFolder> AllowedFolders
        {
            get
            {
                return _allowedFolders;
            }
            set
            {
                _allowedFolders = value;
                NotifyStateChanged();
            }
        }

        private bool _ready;
        public bool Ready
        {
            get
            {
                return _ready;
            }
            set
            {
                _ready = value;
                NotifyStateChanged();
            }
        }
    }

    public class FolderTypeNav
    {
        public string Data { get; set; }
        public string Text { get; set; }
    }

    public class AllowedFolder
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool IsAdministrator { get; set; } = false;
    }
}
