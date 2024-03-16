using System;
using System.Collections.Generic;
using Evo.Infrastructure.Domain;
using Volo.Abp;
using Volo.Abp.MultiTenancy;

namespace Evo.RAM.Roles;

public class Role : FullAuditedAggregateRootWithUserName<Guid>
{
    /// <summary>
    /// 迁移专用构造函数， 迁移完成改为protected
    /// </summary>
    public Role(Guid id) : this()
    {
        this.Id = id;
    }

    protected Role()
    {
        this.Tags = new List<string>();
    }

    internal Role(string name, string code, RoleType type) : this()
    {
        Check.NotNullOrEmpty(name, nameof(name));
        //Check.NotNullOrEmpty(code, nameof(code));
        this.Name = name;
        this.Code = code;
        this.Type = type;
        // this.Permissions = new Collection<RolePermission>();
    }

    public string Name { get; set; } = null!;

    public string? Code { get; set; }

    public RoleType Type { get; set; }

    public bool? IsEnabled { get; set; }

    public double? Sort { get; set; }
    /// <summary>
    /// here, if use IList<string> or List<string> , it dont work
    /// </summary>
    public List<string> Tags { get; set; }

    public string ExtraProperties { get; set; }

    public string Description { get; set; }


    public Guid? TenantId { get; set; }


    internal void ChangeName(string name)
    {
        Check.NotNullOrEmpty(name, nameof(name));
        this.Name = name;
    }

    internal void ChangeCode(string code)
    {
        this.Code = code;
    }

    public void ChangeType(RoleType type)
    {
        this.Type = type;
    }
}