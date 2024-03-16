using System;

namespace Evo.RAM.DataPermissions;

public class GetBrandsByDataPermissionsDto
{
    public Guid ParentDataPermissionId { get; set; }
    public Guid[] DataPermissionIds { get; set; }
}