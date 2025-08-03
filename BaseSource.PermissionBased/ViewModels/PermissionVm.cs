using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseSource.PermissionBased.ViewModels
{
    public class RoleVm
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class PermissionVm
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public List<RoleClaimsResponse> RoleClaims { get; set; }
    }

    public class RoleClaimsResponse
    {
        public string Type { get; set; }
        public string Value { get; set; }
        public bool Selected { get; set; }
    }

    public class CreateRoleVm
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }
    }

    public class EditRoleVm : CreateRoleVm
    {
        [Required]
        public string Id { get; set; }
    }
}
