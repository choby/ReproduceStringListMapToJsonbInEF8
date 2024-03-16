using System;
using System.Collections.Generic;

namespace Evo.RAM.DataPermissions;

public class ChangeDataPermissionBindUserDto
{
    /// <summary>
    /// 原组 id
    /// </summary>
    public Guid OriginalDataPermissionId { get; set; }
    /// <summary>
    /// 原用户 id
    /// </summary>
    public Guid OriginalUserId { get; set; }
    /// <summary>
    /// 新组 id
    /// </summary>
    public Guid NewJobPositionId { get; set; }
    /// <summary>
    /// 用户集合
    /// </summary>
    public IEnumerable<DataPermissionBindUserDto> Users { get; set; }
}