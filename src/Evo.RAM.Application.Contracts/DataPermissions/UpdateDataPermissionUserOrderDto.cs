using System;

namespace Evo.RAM.DataPermissions;

public class UpdateDataPermissionUserOrderDto
{
    /// <summary>
    /// 用户Id
    /// </summary>
    public Guid UserId { get; set; }
    /// <summary>
    /// 序号
    /// </summary>
    public int Sort { get; set; }
}