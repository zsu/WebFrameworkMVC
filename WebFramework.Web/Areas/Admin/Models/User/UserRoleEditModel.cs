using BrockAllen.MembershipReboot.Nh;
using System.Collections.Generic;

namespace Web.Areas.Admin.Models
{
    public class UserRoleEditModel
    {
        IList<UserRoleModel> _roles = new List<UserRoleModel>();
        public NhUserAccount User { get; set; }
        public IList<UserRoleModel> Roles { get { return _roles; } set { _roles = value; } }
    }
}