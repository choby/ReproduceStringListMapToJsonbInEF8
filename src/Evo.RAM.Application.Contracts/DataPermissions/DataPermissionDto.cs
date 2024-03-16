using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace Evo.RAM.DataPermissions;

/// <summary>
/// 数据权限Dto
/// </summary>
public class DataPermissionDto : FullAuditedEntityDto<Guid>
{
    /// <summary>
    /// 岗位名称
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// 岗位编码
    /// </summary>
    public string Code { get; set; }
    /// <summary>
    /// 父级Id
    /// </summary>
    public Guid? ParentId { get; set; }
    /// <summary>
    /// 加载所有用户
    /// </summary>
    public bool? IncludeAllUsers { get; set; }
    /// <summary>
    /// 停用/启用
    /// </summary>
    public bool IsEnabled { get; set; }
    public IEnumerable<string> Tags { get; set; }
    /// <summary>
    /// 排序
    /// </summary>
    public int Sort { get; set; }

    /// <summary>
    /// 绑定的品牌，有序
    /// </summary>
    public IEnumerable<DataPermissionBindBrandDto> Brands { get; set; }

    /// <summary>
    /// 绑定的用户，有序
    /// </summary>
    public IEnumerable<DataPermissionBindUserDto> Users { get;  set; }

    public IEnumerable<DataPermissionDto> Children { get; set; }
}