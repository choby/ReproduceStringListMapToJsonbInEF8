using System;

namespace Evo.RAM.DataPermissions;

public class DataPermissionUnbindUserDto
{
    /// <summary>
    /// 组 id
    /// </summary>
    public Guid DataPermissionId { get; set; }
    /// <summary>
    /// 用户 id
    /// </summary>
    public Guid UserId { get; set; }
}