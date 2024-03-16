using System;

namespace Evo.RAM.DataPermissions;

public class DataPermissionBindBrandDto
{
    
    /// <summary>
    /// 品牌名
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// 品牌 id
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// 排序
    /// </summary>
    public int? Sort { get; set; }
}