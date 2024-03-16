using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using Evo.Infrastructure.Attributes;
using Evo.Infrastructure.Extensions;
using Evo.Infrastructure.Kendo;
using Evo.Infrastructure.Repositories;
using Kendo.Mvc;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.EventBus.Local;
using Volo.Abp.ObjectMapping;

namespace Evo.RAM.Roles;

/// <summary>
/// 角色管理
/// </summary>
public class RoleService : RAMAppService, IRoleService
{
    private readonly INoTrackingRepository<Role, Guid> _roleNoTrackingRepository;
    private readonly IRepository<Role, Guid> _roleRepository;
    private readonly RoleManager _roleManager;
    private readonly ILocalEventBus _localEventBus;
    private readonly IDistributedEventBus _distributedEventBus;
    public RoleService(INoTrackingRepository<Role, Guid> roleNoTrackingRepository,
        IRepository<Role, Guid> roleRepository, RoleManager roleManager,
        ILocalEventBus localEventBus, 
        IDistributedEventBus distributedEventBus)
    {
        _roleNoTrackingRepository = roleNoTrackingRepository;
        _roleRepository = roleRepository;
        _roleManager = roleManager;
        _localEventBus = localEventBus;
        _distributedEventBus = distributedEventBus;
    }

    /// <summary>
    /// 获取角色分页列表
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<KendoResult<RoleDto>> GetAsync([KendoDataSourceRequest] DataSourceRequest request, CancellationToken cancellationToken)
    {
        request.SetDefaultCreationTimeSort(ListSortDirection.Descending);
        return await (await _roleNoTrackingRepository.GetQueryableAsync())
            .ProjectTo<RoleDto>(ObjectMapper.GetMapper().ConfigurationProvider)
            .ToKendoResultAsync(request, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// 获取加工厂角色
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IEnumerable<RoleDto>> GetForFactoryAsync(CancellationToken cancellationToken)
    {
        return (await _roleNoTrackingRepository.GetQueryableAsync(x => x.Type == RoleType.Factory))
            .ProjectTo<RoleDto>(ObjectMapper.GetMapper().ConfigurationProvider);
    }

    /// <summary>
    /// 新增角色
    /// </summary>
    /// <param name="input"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<bool> PostAsync(CreateOrUpdateDto input, CancellationToken cancellationToken)
    {
        var role = await _roleManager.CreateAsync(input.Name, input.Code, input.Type);
        role.IsEnabled = input.IsEnabled;
        role.Sort = input.Sort;
        await _roleRepository.InsertAsync(role, cancellationToken: cancellationToken);
        return true;
    }

    /// <summary>
    /// 修改角色
    /// </summary>
    /// <param name="id"></param>
    /// <param name="input"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<bool> PutAsync(Guid id, CreateOrUpdateDto input, CancellationToken cancellationToken)
    {
        var role = await _roleRepository.GetAsync(id, cancellationToken: cancellationToken);
        await _roleManager.ChangeNameAsync(role, input.Name);
        await _roleManager.ChangeCodeAsync(role, input.Code);
        role.ChangeType(input.Type);
        role.Sort = input.Sort;
        role.IsEnabled = input.IsEnabled;
        role.Description = input.Description;
        await _roleRepository.UpdateAsync(role, cancellationToken: cancellationToken);
        return true;
    }

   

    /// <summary>
    /// 根据Id获取角色信息
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<RoleDto> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        var role = await _roleRepository.GetAsync(id, true, cancellationToken);
        return ObjectMapper.Map<Role, RoleDto>(role);
    }
    
    /// <summary>
    /// 获取角色标签
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IEnumerable<string>> GetTagsAsync(Guid id, CancellationToken cancellationToken)
    {
        var role = await _roleNoTrackingRepository.GetAsync(id,cancellationToken: cancellationToken);
        return role.Tags;
    }
    
    /// <summary>
    /// 获取角色标签
    /// </summary>
    /// <param name="id"></param>
    /// <param name="input"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<bool> PutTagsAsync(Guid id, PutRoleTagsDto input, CancellationToken cancellationToken)
    {
        var role = await _roleRepository.GetAsync(id, cancellationToken: cancellationToken);
        role.Tags = input.Tags;
        await _roleRepository.UpdateAsync(role, cancellationToken: cancellationToken);
        return true;
    }
    
    /// <summary>
    ///  批量设置角色标签
    /// </summary>
    /// <param name="input"></param> 
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<bool> PutTagsAsync(BatchPutRoleTagsDto input, CancellationToken cancellationToken)
    {
        var roles = await _roleRepository.GetListAsync(d=>input.RoleIds.Contains(d.Id), cancellationToken: cancellationToken);
        foreach (var role in roles)
        {
            role.Tags = input.Tags;

        }
        await _roleRepository.UpdateManyAsync(roles, cancellationToken: cancellationToken);
        return true;
    }
    
    /// <summary>
    /// 获取所有角色标签
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IEnumerable<string>> GetTagsAsync(CancellationToken cancellationToken)
    {
        var roles = await _roleNoTrackingRepository.GetListAsync(cancellationToken: cancellationToken);
        return roles.SelectMany(d=>d.Tags).Distinct();
    }

    /// <summary>
    /// 角色列表
    /// </summary>
    /// <param name="tag"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("api/app/role/list")]
    public async Task<IEnumerable<RoleDto>> GetListAsync(string tag, CancellationToken cancellationToken)
    {
        var roles = await _roleNoTrackingRepository.GetListAsync(x=>x.IsEnabled == true, cancellationToken: cancellationToken);
        if (!string.IsNullOrWhiteSpace(tag))
            roles = roles.Where(d => d.Tags.Contains(tag)).ToList();
        return ObjectMapper.Map<IEnumerable<Role>, IEnumerable<RoleDto>>(roles);
    }

    /// <summary>
    /// 排序
    /// </summary>
    /// <param name="input"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<bool> PutSetOrderAsync(PutRoleSetOrderDto input , CancellationToken cancellationToken)
    {
        var roles = await _roleRepository.GetListAsync(d => input.Roles.Select(x => x.Id).Contains(d.Id));
        roles.ForEach(d =>
        {
            d.Sort = input.Roles.SingleOrDefault(i => i.Id == d.Id)?.Sort;
        });
        await _roleRepository.UpdateManyAsync(roles);
        return true;
    }
}