using System;
using System.ComponentModel.DataAnnotations;

namespace Evo.RAM.DataPermissions;

public class DataPermissionBindUserDto
{
    /// <summary>
    /// 用户名
    /// </summary>
    [Required]
    public string Name { get; set; }
    /// <summary>
    /// 用户Id
    /// </summary>
    [Required]
    public Guid Id { get; set; }

    public int? Sort { get; set; }
}