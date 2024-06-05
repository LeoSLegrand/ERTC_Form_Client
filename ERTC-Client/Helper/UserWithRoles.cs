using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERTC_Client.Helper
{
    public class UserWithRoles
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; }

        public string RoleDisplay => Roles.Count == 0 ? "None" : string.Join(", ", Roles);
    }


}