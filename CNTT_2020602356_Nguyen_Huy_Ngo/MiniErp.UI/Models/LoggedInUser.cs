using Firebase.Auth;
using MiniErp.UI.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniErp.UI.Models
{
    public class LoggedInUser
    {
        public UserCredential UserCredential { get; set; }
        public Domain.User User { get; set; }

        public bool IsAdmin => User.RoleId == RoleConstants.AdminId;
        public bool IsManager => User.RoleId == RoleConstants.ManagerId;
        public bool IsUser => User.RoleId == RoleConstants.UserId;
    }
}
