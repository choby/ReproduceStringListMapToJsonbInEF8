using System;

namespace Evo.RAM.DataPermissions;

public class DataPermissionUserDto
{
    // <summary>
    /// 所属组Id
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 所属数据权限
    /// </summary>
    public string ParentName { get; set; }
    /// <summary>
    /// 所属数据权限 id
    /// </summary>
    public Guid ParentId { get; set; }

    /// <summary>
    /// 用户Id
    /// </summary>
    public Guid UserId { get; set; }
    /// <summary>
    /// 姓名
    /// </summary>
    public string UserName { get; set; }
    /// <summary>
    /// 部门
    /// </summary>
    public string DepartmentName { get; set; }
    /// <summary>
    /// 职务
    /// </summary>
    public string JobTitle { get; set; }
    /// <summary>
    /// 用户状态（启用/禁用）
    /// </summary>
    public bool? IsEnabled { get; set; }
}