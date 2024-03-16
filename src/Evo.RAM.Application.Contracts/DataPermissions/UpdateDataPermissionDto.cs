using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace Evo.RAM.DataPermissions;

public class UpdateDataPermissionDto
{
    /// <summary>
    /// 数据权限名称
    /// </summary>
    [Required]
    [StringLength(DataPermissionConsts.MaxPositionName)]
    public string Name { get; set; }
    /// <summary>
    /// 数据权限编码
    /// </summary>
    [StringLength(DataPermissionConsts.MaxPositionCode)]
    public string Code { get; set; }
    /// <summary>
    /// 要绑定的品牌数组
    /// </summary>
    [CanBeNull]
    public IEnumerable<DataPermissionBindBrandDto> Brands { get; set; }

    public bool IsEnabled { get; set; }
    /// <summary>
    /// 包含所有用户
    /// </summary>
    public bool IncludeAllUsers { get; set; }
    /// <summary>
    /// 要绑定的用户
    /// </summary>
    
    [CanBeNull]
    public IEnumerable<DataPermissionBindUserDto> Users { get; set; }
}