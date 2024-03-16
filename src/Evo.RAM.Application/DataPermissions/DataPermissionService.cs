using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Evo.Infrastructure;
using Evo.Infrastructure.Repositories;
using Masuit.Tools;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;

namespace Evo.RAM.DataPermissions;

/// <summary>
/// 数据权限
/// </summary>
public class DataPermissionService : RAMAppService, IDataPermissionSevice
{
    private readonly IRepository<DataPermission, Guid> repository;
    private readonly INoTrackingRepository<DataPermission, Guid> noTrackingRepository;
    private readonly DataPermissionManager dataPermissionManager;

    public DataPermissionService(IRepository<DataPermission, Guid> repository,
        INoTrackingRepository<DataPermission, Guid> noTrackingRepository,
        DataPermissionManager dataPermissionManager)
    {
        this.repository = repository;
        this.noTrackingRepository = noTrackingRepository;
        this.dataPermissionManager = dataPermissionManager;
    }

    /// <summary>
    /// 创建数据权限/组
    /// </summary>
    /// <param name="input"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<bool> CreateAsync(CreateDataPermissionDto input, CancellationToken cancellationToken)
    {
        var dataPermission = await dataPermissionManager.CreateAsync(input.Name, input.Code, input.ParentId, input.IncludeAllUsers, input.IsEnabled);

        if (input.Brands is not null && input.Brands.Length > 0)
        {
            dataPermissionManager.BindBrands(dataPermission, input.Brands.Select(d => new DataPermissionBrand()
            {
                Id = d.Id,
                Name = d.Name,
                Sort = d.Sort
            }));
        }

        if (input.Users is not null&& input.Users.Length > 0)
            dataPermissionManager.BindUsers(dataPermission, input.Users.Select(d => new DataPermissionUser()
            {
                Id = d.Id,
                Name = d.Name
            }));


        await repository.InsertAsync(dataPermission, true, cancellationToken);
        return true;
    }

    /// <summary>
    /// 删除数据权限/组
    /// </summary>
    /// <param name="id">数据权限/组Id</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<Guid> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var dataPermission = await repository.GetAsync(id, default, cancellationToken);
        if (dataPermission.ParentId == default(Guid) || dataPermission.ParentId == null)
            throw new BusinessException(ExceptionCodes.请求数据校验失败, $"不能删除数据权限");
        await repository.DeleteAsync(dataPermission, cancellationToken: cancellationToken);
        return id;
    }


    /// <summary>
    /// 批量删除数据权限/组
    /// </summary>
    /// <param name="ids"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<bool> DeleteAsync([FromBody] Guid[] ids, CancellationToken cancellationToken)
    {
        var dataPermissions = await repository.GetListAsync(d => ids.Contains(d.Id), cancellationToken: cancellationToken);


        await repository.DeleteManyAsync(dataPermissions, true, cancellationToken);
        return true;
    }

    /// <summary>
    /// 根据Id获取数据权限/组信息
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<DataPermissionDto> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        var dataPermission = await repository.GetAsync(id, true, cancellationToken);
        return ObjectMapper.Map<DataPermission, DataPermissionDto>(dataPermission);
    }

    /// <summary>
    /// 获取数据权限列表
    /// </summary>
    /// <param name="tag">标签，可空</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IEnumerable<DataPermissionDto>> GetListAsync(string tag, CancellationToken cancellationToken)
    {
        var list = await repository.GetListAsync(x => x.ParentId == null, true, cancellationToken);
        if (!string.IsNullOrWhiteSpace(tag))
            list = list.Where(d => d.Tags.Contains(tag)).ToList();
        var dtos = ObjectMapper.Map<IEnumerable<DataPermission>, IEnumerable<DataPermissionDto>>(list);
        dtos.ForEach(d =>
        {
            if (d.Children is not null)
            {
                d.Children = d.Children.Where(c => c.IsEnabled == true).ToList();
            }
        });
        return dtos;
    }

    /// <summary>
    /// 全部数据权限
    /// </summary>
    /// <param name="code">数据权限编码，一般用来区分业务系统</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IEnumerable<DataPermissionDto>> GetTenantDataPermissionsAsync(string code, CancellationToken cancellationToken)
    {
        var list = await repository.GetListAsync(x => x.Code == code && x.IsEnabled == true, true, cancellationToken);
        var dtos = ObjectMapper.Map<IEnumerable<DataPermission>, IEnumerable<DataPermissionDto>>(list);
        dtos.ForEach(d =>
        {
            if (d.Children is not null)
            {
                d.Children = d.Children.Where(c => c.IsEnabled == true).ToList();
            }
        });
        return dtos;
    }

    /// <summary>
    /// /禁用/启用数据权限
    /// </summary>
    /// <param name="ids"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<bool> PatchDisableOrEnableAsync(Guid[] ids, CancellationToken cancellationToken)
    {
        var groups = await repository.GetListAsync(g => ids.Contains(g.Id), cancellationToken: cancellationToken);

        await Task.WhenAll(groups.Select(async dataPermission =>
        {
            await dataPermissionManager.ChangeIsEnabledAsync(dataPermission);

            await repository.UpdateAsync(dataPermission, cancellationToken: cancellationToken);
        }));

        return true;
    }

    /// <summary>
    /// 修改组
    /// </summary>
    /// <param name="id"></param>
    /// <param name="input"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<bool> UpdateAsync(Guid id, UpdateDataPermissionDto input, CancellationToken cancellationToken)
    {
        var dataPermission = await repository.GetAsync(id, true, cancellationToken);
        if (!string.IsNullOrEmpty(input.Code) && dataPermission.Code != input.Code)
            await dataPermissionManager.ChangeCodeAsync(dataPermission, input.Code);
        if (dataPermission.Name != input.Name)
            await dataPermissionManager.ChangeNameAsync(dataPermission, input.Name);

        dataPermission.SetIsEnabled(input.IsEnabled);
        dataPermission.IncludeAllUsers = input.IncludeAllUsers;

        if (input.Brands is not null)
            dataPermissionManager.BindBrands(dataPermission, input.Brands.Select(d => new DataPermissionBrand()
            {
                Id = d.Id,
                Name = d.Name,
                Sort = d.Sort
            }));
        else
        {
            dataPermission.Brands = new List<DataPermissionBrand>();
        }

        if (input.Users is not null)
            dataPermissionManager.BindUsers(dataPermission, input.Users.Select(d => new DataPermissionUser()
            {
                Id = d.Id,
                Name = d.Name
            }));
        else
        {
            dataPermission.Users = new List<DataPermissionUser>();
        }

        await repository.UpdateAsync(dataPermission, true, cancellationToken);
        return true;
    }

    /// <summary>
    /// 设置数据权限是否加载所有用户
    /// </summary>
    /// <param name="id"></param>
    /// <param name="includeAllUsers"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<bool> PatchIncludeAllUsersAsync(Guid id, [Required] bool includeAllUsers, CancellationToken cancellationToken)
    {
        var dataPermission = await repository.GetAsync(id, false, cancellationToken);
        dataPermissionManager.ChangeIncludeAllUsers(dataPermission, includeAllUsers!);
        await repository.UpdateAsync(dataPermission, cancellationToken: cancellationToken);
        return true;
    }

    /// <summary>
    /// 数据权限组更换排序
    /// </summary>
    /// <param name="id"></param>
    /// <param name="input"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<bool> PutChangeChildrenOrderAsync(Guid id, ICollection<ChangeDataPermissionChildrenOrderDto> input, CancellationToken cancellationToken)
    {
        var dataPermission = await repository.GetAsync(id, true, cancellationToken);

        dataPermission.Children.ForEach(p =>
        {
            var sort = input.FirstOrDefault(x => x.Id == p.Id)?.Sort;
            if (sort.HasValue)
                p.ChangeOrder(sort.Value);
        });

        await repository.UpdateAsync(dataPermission, cancellationToken: cancellationToken);
        return true;
    }

   
    

   

    /// <summary>
    /// 修改数据权限组人员序号
    /// </summary>
    /// <param name="id"></param>
    /// <param name="input"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<bool> PutChangeUserOrderAsync(Guid id, IEnumerable<UpdateDataPermissionUserOrderDto> input, CancellationToken cancellationToken)
    {
        var dataPermission = await repository.GetAsync(id, true, cancellationToken);
        dataPermissionManager.ChangeUserOrder(dataPermission, input.Select(d => new DataPermissionUser()
        {
            Id = d.UserId,
            Sort = d.Sort
        }));
        await repository.UpdateAsync(dataPermission);
        return true;
    }


    /// <summary>
    /// 获取数据权限标签
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IEnumerable<string>> GetTagsAsync(Guid id, CancellationToken cancellationToken)
    {
        var role = await noTrackingRepository.GetAsync(id, cancellationToken: cancellationToken);
        return role.Tags;
    }

    /// <summary>
    /// 设置数据权限标签
    /// </summary>
    /// <param name="id"></param>
    /// <param name="input"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<bool> PutTagsAsync(Guid id, PutDataPermissionTagsDto input, CancellationToken cancellationToken)
    {
        var dataPermission = await repository.GetAsync(id, cancellationToken: cancellationToken);
        dataPermission.Tags = input.Tags;
        await repository.UpdateAsync(dataPermission, cancellationToken: cancellationToken);
        return true;
    }
    
    /// <summary>
    ///  批量设置数据权限标签
    /// </summary>
    /// <param name="input"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<bool> PutTagsAsync(BatchPutDataPermissionTagsDto input, CancellationToken cancellationToken)
    {
        var dataPermissions = await repository.GetListAsync(d=>input.DataPermissionIds.Contains(d.Id), cancellationToken: cancellationToken);
        foreach (var dataPermission in dataPermissions)
        {
            dataPermission.Tags = input.Tags;

        }
        await repository.UpdateManyAsync(dataPermissions, cancellationToken: cancellationToken);
        return true;
    }

    /// <summary>
    /// 获取数据权限标签
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IEnumerable<string>> GetTagsAsync(CancellationToken cancellationToken)
    {
        var dataPermissions = await noTrackingRepository.GetListAsync(cancellationToken: cancellationToken);
        return dataPermissions.SelectMany(d => d.Tags).Distinct();
    }
}