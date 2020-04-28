using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace depot.Client
{
    //https://chrissainty.com/3-ways-to-communicate-between-components-in-blazor/
    public class AppState
    {
        private string _currentGroupId;
        public string CurrentGroupId
        {
            get
            {
                return _currentGroupId;
            }
            set
            {
                _currentGroupId = value;
                NotifyStateChanged();
            }
        }


        private string _currentGroupName;
        public string CurrentGroupName
        {
            get
            {
                return _currentGroupName;
            }
            set
            {
                _currentGroupName = value;
                NotifyStateChanged();
            }
        }


        private List<GroupTypeNav> _dataTypes;
        public List<GroupTypeNav> DataTypes
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


        private List<AllowedGroup> _allowedGroups;
        public List<AllowedGroup> AllowedGroups
        {
            get
            {
                return _allowedGroups;
            }
            set
            {
                _allowedGroups = value;
                NotifyStateChanged();
            }
        }

        public event Action OnChange;

        private API _api;
        public AppState(API api)
        {
            _api = api;

            DataTypes = new List<GroupTypeNav>();
            AllowedGroups = new List<AllowedGroup>();
        }

        async public Task UpdateAppState(string selectedGroupId = null)
        {
            var userOrganizations = await _api.GetGroupsByAuthorizedUser();

            if (userOrganizations != null)
            {
                AllowedGroups = userOrganizations.Select(o => new AllowedGroup() { Name = o.Name, Id = o.Id }).ToList();

                if (selectedGroupId != null)
                {
                    var selectedOrganization = userOrganizations.FirstOrDefault(g => g.Id == selectedGroupId);
                    if (selectedOrganization != null)
                    {
                        CurrentGroupName = selectedOrganization.Name;
                        CurrentGroupId = selectedOrganization.Id;

                        var orgTypes = await _api.GetGroupTypeAsMenuOptionList(selectedOrganization.Id);
                        DataTypes = orgTypes.Select(o => new GroupTypeNav() { Text = o.Name, Data = o.Id }).ToList();
                    }
                }
            }
        }


        private void NotifyStateChanged() => OnChange?.Invoke();
    }

    public class GroupTypeNav
    {
        public string Data { get; set; }
        public string Text { get; set; }
    }

    public class AllowedGroup
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
