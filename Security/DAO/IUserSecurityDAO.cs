using Security.Models.Database;
using System;
using System.Collections.Generic;
using System.Text;

namespace Security.DAO
{
    public interface IUserSecurityDAO
    {
        #region UserItem

        int AddUserItem(UserItem item);
        bool UpdateUserItem(UserItem item);
        void DeleteUserItem(int userId);
        UserItem GetUserItem(int userId);
        UserItem GetUserItem(string username);
        List<UserItem> GetUserItems();

        #endregion

        #region RoleItem

        int AddRoleItem(RoleItem item);
        List<RoleItem> GetRoleItems();

        #endregion        
    }
}
