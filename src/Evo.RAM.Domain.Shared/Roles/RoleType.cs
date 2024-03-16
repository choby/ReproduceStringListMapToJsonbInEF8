using System.ComponentModel;

namespace Evo.RAM.Roles;

public enum RoleType
{
    /// <summary>
    /// 内部角色
    /// </summary>
    [Description("内部角色")]
    Internal = 0,
    /// <summary>
    /// 供应商角色
    /// </summary>
    [Description("供应商角色")]
    Supplier = 1,
    /// <summary>
    /// 加工厂角色
    /// </summary>
    [Description("加工厂角色")]
    Factory =2
}