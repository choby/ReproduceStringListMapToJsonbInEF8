using System;
using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace Evo.RAM.DataPermissions;

public class CreateDataPermissionDto
{
    /// <summary>
    /// 数据权限名称
    /// </summary>
    [Required]
  
    public string Name { get; set; }
    /// <summary>
    /// 数据权限编码
    /// </summary> 
    
    public string Code { get; set; }
    /// <summary>
    /// 父级Id
    /// </summary>
    public Guid? ParentId { get; set; }
    /// <summary>
    /// 加载所有用户
    /// </summary>
    public bool IncludeAllUsers { get; set; }
    /// <summary>
    /// 停用/启用
    /// </summary>
    public bool IsEnabled { get; set; }
    /// <summary>
    /// 要绑定的品牌
    /// </summary>
    [CanBeNull]
    public DataPermissionBindBrandDto[] Brands { get; set; }

    /// <summary>
    /// 要绑定的用户
    /// </summary>
    [CanBeNull]
    public DataPermissionBindUserDto[] Users { get; set; }
}