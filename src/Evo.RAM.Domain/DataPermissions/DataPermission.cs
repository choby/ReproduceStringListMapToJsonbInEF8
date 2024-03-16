using System;
using System.Collections.Generic;
using Evo.Infrastructure.Domain;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Data;

namespace Evo.RAM.DataPermissions;

/// <summary>
/// 数据权限
/// </summary>
public class DataPermission : FullAuditedAggregateRootWithUserName<Guid>, IHasExtraProperties
{
    /// <summary>
    /// 迁移专用
    /// </summary>
    /// <param name="id"></param>
    public DataPermission(Guid id) : this()
    {
        this.Id = id;
    }

    protected DataPermission()
    {
        this.Children = new List<DataPermission>();
        this.Brands = new List<DataPermissionBrand>();
        this.Users = new List<DataPermissionUser>();
        this.Tags = new List<string>();
    }
    
    public DataPermission(string name, string code, Guid? parentId, bool canLookupAllUser = true, bool isEnabled = true, int? sort = 0) : this()
    {
        SetName(name);
        SetCode(code);
        this.ParentId = parentId;
        this.IncludeAllUsers = canLookupAllUser;
        this.IsEnabled = isEnabled;
        this.Sort = sort;
    }
    
    public DataPermission(Guid id,string name, string code, Guid? parentId, bool? canLookupAllUser, bool? isEnabled , int? sort ,List<DataPermissionBrand> brands,List<DataPermissionUser> users, List<DataPermission> children) : this()
    {
        this.Id = id;
        SetName(name);
        SetCode(code);
        this.ParentId = parentId;
        this.IncludeAllUsers = canLookupAllUser;
        this.IsEnabled = isEnabled;
        this.Sort = sort;
        this.Children = children;
        this.Brands = brands;
        this.Users = users;
    }
    public string Name { get; set; } = null!;

    public string Code { get; set; } = null!;

    public Guid? ParentId { get; set; }

    /// <summary>
    /// 加载所有用户
    /// </summary>
    public bool? IncludeAllUsers { get; set; }

    /// <summary>
    /// 停用启用
    /// </summary>
    public bool? IsEnabled { get; set; }

    public List<string> Tags { get; set; }
    
    public int? Sort { get; set; }

    //迁移回复 internal
    /// <summary>
    /// 绑定的品牌，有序
    /// </summary>
    public List<DataPermissionBrand> Brands { get;  set; }
//迁移回复 internal
    /// <summary>
    /// 绑定的用户，有序
    /// </summary>
    public List<DataPermissionUser> Users { get;  set; }

    public ExtraPropertyDictionary ExtraProperties { get; set; }

    public Guid? TenantId { get; set; }

    
    //迁移 回复private
    public virtual List<DataPermission> Children { get;  set; }


    internal virtual void SetName([NotNull] string name)
    {
        this.Name = Check.NotNullOrEmpty(name, nameof(name));
    }

    internal virtual void SetCode([NotNull] string code)
    {
        this.Code = Check.NotNullOrEmpty(code, nameof(code));
    }

    public virtual void SetIsEnabled(bool isEnabled)
    {
        this.IsEnabled = isEnabled;
    }
    
    
    internal virtual void SetIsEnabled()
    {
        this.IsEnabled = !this.IsEnabled;
    }
    
    
    internal virtual void SetIncludeAllUsers([NotNull] bool includeAllUsers)
    {
        this.IncludeAllUsers = Check.NotNull(includeAllUsers, nameof(includeAllUsers));
    }

    /// <summary>
    /// 更换排序
    /// </summary>
    /// <param name="sort"></param>
    public virtual void ChangeOrder(int sort)
    {
        this.Sort = Check.NotNull(sort, nameof(sort));
    }
}


public record DataPermissionBrand
{
    public string Name { get; set; }
    public Guid Id { get; set; }
    public int? Sort { get; set; }
}

public class DataPermissionUser
{
    public string Name { get; set; }
    public Guid Id { get; set; }
    public int? Sort { get; set; }
}