using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace depot.Client
{
    //https://chrissainty.com/3-ways-to-communicate-between-components-in-blazor/
    public class AppState
    {
        public GroupState GroupState { get; private set; } = new GroupState();

        public event Action OnChange;

        public void SetGroupState(List<GroupTypeNav> types, List<AllowedGroups> allowedGroups, string GroupId, string GroupName)
        {
            GroupState.GroupId = GroupId;
            GroupState.GroupName = GroupName;
            GroupState.GroupTypeNavs = types;
            GroupState.AllowedGroups = allowedGroups;
            
            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }

    public class GroupState
    {
        public string GroupId { get; set; }
        public string GroupName { get; set; }
        public List<GroupTypeNav> GroupTypeNavs { get; set; } = new List<GroupTypeNav>();
        public List<AllowedGroups> AllowedGroups { get; set; } = new List<AllowedGroups>();
    }

    public class GroupTypeNav
    {
        public string Data { get; set; }
        public string Text { get; set; }
    }

    public class AllowedGroups
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
