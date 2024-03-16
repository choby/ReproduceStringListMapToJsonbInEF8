using System;
using System.ComponentModel.DataAnnotations;

namespace Evo.RAM.DataPermissions;


/// <summary>
/// 数据权限组排序Dto
/// </summary>
public class ChangeDataPermissionChildrenOrderDto
{
    /// <summary>
    /// 岗位组Id
    /// </summary>
    [Required]
    public Guid Id { get; set; }
    /// <summary>
    /// 序号
    /// </summary>
    [Required]
    public int Sort { get; set; }
}
