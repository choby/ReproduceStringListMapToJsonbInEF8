using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Evo.Infrastructure;
using Evo.Infrastructure.Repositories;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Users;

namespace Evo.RAM.DataPermissions
{
    public class DataPermissionManager : DomainService
    {
        private IRepository<DataPermission, Guid> repository;
        private INoTrackingRepository<DataPermission, Guid> noTrackingRepository;
        
        //private readonly ICurrentUserBrandsAccessor currentUserBrandsAccessor;//不要注意这个类,. 会循环依赖报错
        private ICurrentUser _currentUser;
        
        public DataPermissionManager(IRepository<DataPermission, Guid> repository, INoTrackingRepository<DataPermission, Guid> noTrackingRepository,
           
           
            ICurrentUser currentUser)
        {
            this.repository = repository;
            this.noTrackingRepository = noTrackingRepository;
            
            _currentUser = currentUser;
        }

        //使用信号量代替 lock
        private static readonly SemaphoreSlim SemaphoreSlim = new SemaphoreSlim(1,1);
        private async Task<string> GetSequenceAsync()
        {
            await SemaphoreSlim.WaitAsync();
            try
            {
                var latest = (await noTrackingRepository.GetQueryableAsync(p => p.Code.StartsWith("SJQX"))).OrderByDescending(d => d.CreationTime).FirstOrDefault();
                if (latest is null)
                    return "SJQX00001";
                else
                {
                    int index = int.Parse(latest.Code.Replace("SJQX", ""));
                    return $"SJQX{(index + 1).ToString().PadLeft(6, '0')}";
                }
            }
            finally
            {
                SemaphoreSlim.Release();
            }
        }

        public async Task<DataPermission> CreateAsync(string name, string code, Guid? parentId, bool canLookupAllUser = true, bool isEnabled = true)
        {
            if (await noTrackingRepository.AnyAsync(p => p.Name == name))
                throw new BusinessException(ExceptionCodes.请求数据校验失败, $"名称{name}已存在");

            if (!string.IsNullOrEmpty(code) && await noTrackingRepository.AnyAsync(p => p.Code == code))
                throw new BusinessException(ExceptionCodes.请求数据校验失败, $"编码{code}已存在");
            if (string.IsNullOrEmpty(code))
            {
                code = await this.GetSequenceAsync();
            }
            
            if (parentId.HasValue && parentId.Value != default(Guid))
                if (!await noTrackingRepository.AnyAsync(p => p.Id == parentId.Value))
                    throw new BusinessException(ExceptionCodes.请求数据校验失败, $"父级Id{parentId.Value}不存在");

            return new DataPermission(name, code, parentId, canLookupAllUser, isEnabled);
        }

        // /// <summary>
        // /// 设置组和品牌的关系
        // /// </summary>
        // /// <param name="id">组Id</param>
        // /// <param name="brandIds">要绑定的品牌Id集合</param>
        // /// <returns></returns>
        // /// <exception cref="BusinessException"></exception>
        // public async Task SetPositionBrandAsync([NotNull] Guid id, [NotNull] IEnumerable<string> brandIds)
        // {
        //     Check.NotNull(id, nameof(id));
        //     Check.NotNull(id, nameof(brandIds));
        //
        //     try
        //     {
        //         var currentPostionUsers = await positionBrandRepository.GetListAsync(b => b.PositionId == id);
        //         var currentBrandIds = currentPostionUsers.Select(b => b.BrandId.ToString());
        //         var deleteBrandIds = currentBrandIds.Except(brandIds);
        //         var deletePositionBrands = currentPostionUsers.Where(b => deleteBrandIds.Contains(b.BrandId.ToString()));
        //         await Task.WhenAll(deletePositionBrands.Select(item =>
        //         {
        //             return positionBrandRepository.DeleteAsync(item);
        //         }));
        //
        //         var addPositionBrands = brandIds.Except(currentBrandIds)
        //             .Select(brandId => new PositionBrand(id, Guid.Parse(brandId)));
        //         await Task.WhenAll(addPositionBrands.Select(item =>
        //         {
        //             return positionBrandRepository.InsertAsync(item);
        //         }));
        //
        //     }
        //     catch (Exception ex)
        //     {
        //         throw new BusinessException(ExceptionCodes.请求失败, ex.Message);
        //     }
        // }

        /// <summary>
        /// 设置组和品牌的关系
        /// </summary>
        /// <param name="jobPosition">岗位</param>
        /// <param name="brands">要绑定的品牌Id集合</param>
        /// <returns></returns>
        /// <exception cref="BusinessException"></exception>
        public void BindBrands([NotNull] DataPermission jobPosition, IEnumerable<DataPermissionBrand> brands)
        {
            if (brands is not null)
                jobPosition.Brands = brands.ToList();
            else
                jobPosition.Brands = null;
        }

        /// <summary>
        /// 设置组和用户的关系
        /// </summary>
        /// <param name="jobPosition">岗位</param>
        /// <param name="users">用户集合</param>
        /// <returns></returns>
        /// <exception cref="BusinessException"></exception>
        public void BindUsers([NotNull] DataPermission jobPosition,  IEnumerable<DataPermissionUser> users)
        {
            if (users is not null)
                jobPosition.Users = users.ToList();
            else
                jobPosition.Users = null;
        }

        /// <summary>
        /// 解绑用户
        /// </summary>
        /// <param name="jobPosition"></param>
        /// <param name="userIds"></param>
        public void UnbindUsers(DataPermission jobPosition, IEnumerable<Guid> userIds)
        {
            jobPosition.Users = jobPosition.Users.Where(u => !userIds.Contains(u.Id)).ToList();
        }

        /// <summary>
        /// 更改岗位名称
        /// </summary>
        /// <param name="dataPermission">岗位实体对象</param>
        /// <param name="name">要修改的名称</param>
        /// <returns></returns>
        /// <exception cref="BusinessException"></exception>
        public async Task ChangeNameAsync(DataPermission dataPermission, string name)
        {
            if (await noTrackingRepository.AnyAsync(p => p.Name == name && p.Id != dataPermission.Id))
                throw new BusinessException(ExceptionCodes.请求数据校验失败, $"组名称{name}已存在");

            dataPermission.SetName(name);
        }

        /// <summary>
        /// 更改岗位编码
        /// </summary>
        /// <param name="dataPermission">岗位实体对象</param>
        /// <param name="code">要修改的编码</param>
        /// <returns></returns>
        /// <exception cref="BusinessException"></exception>
        public async Task ChangeCodeAsync(DataPermission dataPermission, string code)
        {
            if (await noTrackingRepository.AnyAsync(p => p.Code == code && p.Id != dataPermission.Id ))
                throw new BusinessException(ExceptionCodes.请求数据校验失败, $"组名称{code}已存在");

            dataPermission.SetCode(code);
        }

        /// <summary>
        /// 修改用户排序
        /// </summary>
        /// <param name="dataPermission"></param>
        /// <param name="users"></param>
        public void ChangeUserOrder(DataPermission dataPermission,IEnumerable<DataPermissionUser> users)
        {
            dataPermission.Users = dataPermission.Users.Select(d =>
            {
                var sort = users.FirstOrDefault(i => i.Id == d.Id)?.Sort;

                return new DataPermissionUser()
                {
                    Id = d.Id,
                    Name = d.Name,
                    Sort = sort.HasValue ? sort : d.Sort
                };
            }).ToList();
        }

        /// <summary>
        /// 根据用户Id获取用户和组的关系
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <returns></returns>
        public async Task<List<DataPermission>> GetDataPermissionsUserAsync(Guid userId)
        {
            var list = await noTrackingRepository.GetListAsync();
            list =  list.Where(d => d.Users.Any(u => u.Id == userId)).ToList();
            return list;
        }

        public Task ChangeIsEnabledAsync(DataPermission dataPermission)
        {
            dataPermission.SetIsEnabled();
            return Task.CompletedTask;
        }

        public void ChangeIncludeAllUsers(DataPermission dataPermission, bool canLookupAllUser)
        {
            dataPermission.SetIncludeAllUsers(canLookupAllUser);
        }

        // /// <summary>
        // /// 检查组和用户是否已存在关系
        // /// </summary>
        // /// <param name="positionUsers"></param>
        // /// <returns></returns>
        // public async Task CheckPositionUserExistAsync(IEnumerable<PositionUser> positionUsers)
        // {
        //     var groupIds = positionUsers.Select(p => p.PositionId).ToArray();
        //     var userIds = positionUsers.Select(p => p.UserId).ToArray();
        //     var queryable = (await positionUserRepository.GetQueryableAsync())
        //         .Where(p => groupIds.Contains(p.PositionId) && userIds.Contains(p.UserId));
        //     var count = await AsyncExecuter.CountAsync(queryable);
        //     if (count > 0)
        //         throw new BusinessException(ExceptionCodes.请求数据校验失败, "已存在组和用户的关系");
        // }

        // /// <summary>
        // /// 根据岗位Code获取岗位Id
        // /// </summary>
        // /// <param name="positionCode">岗位Code</param>
        // /// <returns></returns>
        // public async Task<Guid> GetPositionIdByCode(string positionCode)
        // {
        //     return (await noTrackingRepository.GetQueryableAsync())
        //         .Where(p => p.Code == positionCode && p.ParentId == default(Guid))
        //         .Select(p => p.Id).First();
        // }

        /// <summary>
        /// 根据岗位Id获取组Id集合
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Guid[]> GetGroupIdsByPositionId(Guid id)
        {
            return await AsyncExecuter.ToArrayAsync((await noTrackingRepository.GetQueryableAsync()).Where(g => g.ParentId == id).Select(g => g.Id));
        }

        // /// <summary>
        // /// 根据岗位Id和品牌Id获取绑定了该品牌的组Id和没有绑定品牌的组Id
        // /// </summary>
        // /// <param name="id">岗位Id</param>
        // /// <param name="brandId">品牌Id</param>
        // /// <returns></returns>
        // public async Task<Guid[]> GetGroupIdsByPositionIdAndBrandId(Guid id, Guid brandId)
        // {
        //     var groupQueryable = (await noTrackingRepository.GetQueryableAsync(g => g.PositionBrands)).Where(g => g.ParentId == id);
        //     var positionBrandQueryable = await positionBrandRepository.GetQueryableAsync();
        //
        //     // 获取岗位下所有的组Id集合
        //     var groups = await AsyncExecuter.ToListAsync(groupQueryable);
        //     var groupIds = groups.Select(g => g.Id);
        //
        //     // 根据组Id集合获取绑定的品牌
        //     var positionBrands = groups.SelectMany(g => g.PositionBrands);
        //
        //     // 获取绑定了指定品牌的组Id集合
        //     var hasBrandIds = positionBrands.Where(b => b.BrandId == brandId).Distinct().Select(b => b.PositionId);
        //     // 获取没有绑定品牌的组Id集合
        //     var noBrandIds = (groupIds.Except(hasBrandIds)).Where(x => !positionBrands.Select(b => b.PositionId).Contains(x));
        //
        //     return (hasBrandIds.Union(noBrandIds)).ToArray();
        // }

        // /// <summary>
        // /// 根据岗位Id和品牌Id获取绑定了该品牌的组Id和没有绑定品牌的组Id
        // /// </summary>
        // /// <param name="id">岗位Id</param>
        // /// <param name="brandIds">品牌Id集合</param>
        // /// <returns></returns>
        // public async Task<Guid[]> GetGroupIdsByPositionIdAndBrandId(Guid id, Guid[] brandIds)
        // {
        //     var groupQueryable = (await noTrackingRepository.GetQueryableAsync(g => g.PositionBrands)).Where(g => g.ParentId == id);
        //     var positionBrandQueryable = await positionBrandRepository.GetQueryableAsync();
        //
        //     // 获取岗位下所有的组Id集合
        //     var groups = await AsyncExecuter.ToListAsync(groupQueryable);
        //     var groupIds = groups.Select(g => g.Id);
        //
        //     // 根据组Id集合获取绑定的品牌
        //     var positionBrands = groups.SelectMany(g => g.PositionBrands);
        //
        //     // 获取绑定了指定品牌的组Id集合
        //     var hasBrandIds = positionBrands.Where(b => brandIds.Contains(b.BrandId)).Distinct().Select(b => b.PositionId);
        //     // 获取没有绑定品牌的组Id集合
        //     var noBrandIds = (groupIds.Except(hasBrandIds)).Where(x => !positionBrands.Select(b => b.PositionId).Contains(x));
        //
        //     return (hasBrandIds.Union(noBrandIds)).ToArray();
        // }

        /// <summary>
        /// 复制用户数据权限
        /// </summary>
        /// <param name="fromUserId"></param>
        /// <param name="toUserId"></param>
        /// <param name="toUserName"></param>
        /// <param name="isincrement">是否增量</param>
        /// <returns></returns>
        public async Task<bool> CopyUserToUserAsync([NotNull] Guid fromUserId, [NotNull] Guid toUserId, string toUserName, bool isincrement)
        {
            Check.NotNull(fromUserId, nameof(fromUserId));
            Check.NotNull(toUserId, nameof(toUserId));
        
            Check.NotNull(fromUserId, nameof(fromUserId));
            Check.NotNull(toUserId, nameof(toUserId));
        
            var dataPermissions = (await repository.GetListAsync()).Where(x => x.Users.Any(d => d.Id == fromUserId || d.Id == toUserId));
            //只包含 from user 的数据权限
            var fromUserDataPermissions = dataPermissions.Where(x => x.Users.Any(d => d.Id == fromUserId && d.Id != toUserId)).ToList();
        
            if (!isincrement)
            {
                //只包含 to user 的数据权限
                var toUserDataPermissions = dataPermissions.Where(x => x.Users.Any(d => d.Id == toUserId && d.Id != fromUserId)).ToList();
                //从分组里面移除用户
                toUserDataPermissions.ForEach(p => p.Users = p.Users.Where(d => d.Id != toUserId).ToList());
            }

            // 新增不存在的组
            fromUserDataPermissions.ForEach(p =>
            {
                var users = p.Users.ToList();
                users.Add(new DataPermissionUser()
                {
                    Id = toUserId,
                    Name = toUserName
                });
                p.Users = users;
            });
            return true;
        }

        // /// <summary>
        // /// 复制用户岗位信息,增量
        // /// </summary>
        // /// <param name="fromUserId"></param>
        // /// <param name="toUserId"></param>
        // /// <returns></returns>
        // public async Task<bool> CopyPositionWithIncrementAsync([NotNull] Guid fromUserId, [NotNull] Guid toUserId)
        // {
        //     Check.NotNull(fromUserId, nameof(fromUserId));
        //     Check.NotNull(toUserId, nameof(toUserId));
        //
        //     var positionUsers = await positionUserRepository.GetListAsync(x => x.UserId == fromUserId || x.UserId == toUserId);
        //     var fromUserGroups = positionUsers.Where(x => x.UserId == fromUserId).ToList();
        //     var toUserGroups = positionUsers.Where(x => x.UserId == toUserId).ToList();
        //     
        //     // 新增不存在的组
        //     var addPositionGroups = fromUserGroups.Except(toUserGroups).Select(x => new PositionUser(GuidGenerator.Create(),x.PositionId, toUserId));
        //     await positionUserRepository.InsertManyAsync(addPositionGroups);
        //
        //     return true;
        // }

        // /// <summary>
        // /// 根据组Id集合获取组和品牌的关系
        // /// </summary>
        // /// <param name="groupIds"></param>
        // /// <returns></returns>
        // public Task<List<PositionBrand>> GetPositionBrandsAsync(Guid[] groupIds)
        // {
        //     return positionBrandRepository.GetListAsync(g => groupIds.Contains(g.PositionId));
        // }

        // /// <summary>
        // /// 根据用户Id获取数据组的品牌Id数组
        // /// </summary>
        // /// <param name="userId"></param>
        // /// <returns></returns>
        // public async Task<Guid[]> GetBrandIdsFromDataGroup(Guid userId)
        // {
        //     var dataGroupQueryable = (await noTrackingRepository.GetQueryableAsync()).Where(new PositionDataGroupSpecification());
        //     var groupQueryable = (await noTrackingRepository.GetQueryableAsync()).Where(new PositionGroupSpecification());
        //     var positionUserQueryable = (await positionUserRepository.GetQueryableAsync()).Where(u => u.UserId == userId);
        //     var positionBrandQueryable = await positionBrandRepository.GetQueryableAsync();
        //
        //     var groupIdsQueryable = from g in groupQueryable
        //                             where g.ParentId == dataGroupQueryable.Select(p => p.Id).FirstOrDefault()
        //                             select g.Id;
        //
        //     var brandIdsQueryable = from positionUser in positionUserQueryable
        //                             join positionBrand in positionBrandQueryable on positionUser.PositionId equals positionBrand.PositionId
        //                             where groupIdsQueryable.Contains(positionUser.PositionId)
        //                             select positionBrand.BrandId;
        //
        //     return await AsyncExecuter.ToArrayAsync(brandIdsQueryable.Distinct());
        // }

        // /// <summary>
        // /// 获取用户可见品牌
        // /// </summary>
        // /// <param name="userId">用户id</param>
        // /// <param name="isFiltration">是否创建可见</param>
        // /// <param name="brandName">品牌名称</param>
        // /// <returns></returns>
        // public async Task<IEnumerable<Brand>> GetVisibleBrandsOfUserAsync(Guid userId, bool? isFiltration = null, string brandName = null)
        // {
        //     IEnumerable<Brand> brands;
        //     //数据组
        //     var dataGroupQueryable = (await noTrackingRepository.GetQueryableAsync()).Where(new PositionDataGroupSpecification());
        //     var user = await _userCacheManager.GetAsync(userId);
        //
        //     Expression<Func<Brand, bool>> brandExpression = x => true;
        //     if (!string.IsNullOrEmpty(brandName))
        //     {
        //         brandExpression = brandExpression.And(x => x.Name.Contains(brandName));
        //     }
        //     var brandQueryable = await brandRepository.GetQueryableAsync(new BrandSpecification(isFiltration).ToExpression().And(brandExpression));
        //
        //     if ( user.AllBrands == true || dataGroupQueryable.FirstOrDefault()?.CanLookupAllUser == true)
        //     {
        //         brands = brandQueryable.OrderBy(t => t.Sort == 0).ThenBy(t => t.Sort).ToList();
        //     }
        //     else
        //     {
        //         //查询数据组子项
        //         var groupQueryable = from dataGroup in (await noTrackingRepository.GetQueryableAsync()).Where(new PositionGroupSpecification())
        //                              join g in dataGroupQueryable on dataGroup.ParentId equals g.Id
        //                              select dataGroup;
        //     
        //         var positionUserQueryable = await positionUserRepository.GetQueryableAsync();
        //         var positionBrandQueryable = await positionBrandRepository.GetQueryableAsync();
        //         brands = (from dataGroup in groupQueryable
        //                         join positionUser in positionUserQueryable
        //                         on new { PositionId = dataGroup.Id, UserId = userId } equals new { positionUser.PositionId, positionUser.UserId }
        //                         join positionBrand in positionBrandQueryable
        //                         on  dataGroup.Id equals  positionBrand.PositionId
        //                         join brand in brandQueryable
        //                         on positionBrand.BrandId equals brand.Id                               
        //                         select brand).Distinct().OrderBy(t => t.Sort == 0).ThenBy(t => t.Sort).ToList();
        //        
        //     }
        //     return brands;
        // }


        // /// <summary>
        // /// 获取用户可见岗位员工
        // /// </summary>
        // /// <param name="userId"></param>
        // /// <param name="positionCode"></param>
        // /// <param name="allBrandsPermission">用户是否有所有品牌的权限, 需要从user系统中获取</param>
        // /// <param name="assignBrandIds">是否查询指定品牌下的某个岗位员工</param>
        // /// <returns></returns>
        // public async Task<(bool, IEnumerable<Guid>)> GetVisiblePositionStaffIdsOfUserAsync(Guid userId, string positionCode, bool allBrandsPermission, 
        //     Guid[]? assignBrandIds)
        // {
        //     //岗位
        //     var positionGroupQueryable = (await noTrackingRepository.GetQueryableAsync()).Where(x => x.Code == positionCode);
        //
        //     //查询数据组子项
        //     var positionQueryable = from position in (await noTrackingRepository.GetQueryableAsync())
        //             .Where(new PositionGroupSpecification().ToExpression().And(new PositionEnabledSpecification()))
        //                             join g in positionGroupQueryable on position.ParentId equals g.Id
        //                             select position;
        //
        //     Expression<Func<PositionBrand, bool>> expression = t => true;
        //     var brandIds = new List<Guid>();
        //     if (assignBrandIds is { Length: > 0 })
        //     {
        //         brandIds.AddRange(assignBrandIds);
        //     }
        //     else
        //     {
        //         brandIds = (await this.GetVisibleBrandsOfUserAsync(userId)).Select(d => d.Id).ToList();
        //     }
        //     
        //     var positionUserQueryable = await positionUserRepository.GetQueryableAsync();
        //     var positionBrandQueryable = (await positionBrandRepository.GetQueryableAsync()).Where(expression);
        //     IEnumerable<Guid> userIds = (from position in positionQueryable
        //                                  join positionUser in positionUserQueryable
        //                                  on position.Id equals positionUser.PositionId
        //                                  join positionBrand in positionBrandQueryable
        //                                  on position.Id equals positionBrand.PositionId
        //                                  where brandIds.Contains(positionBrand.BrandId)
        //                                  select positionUser.UserId).Distinct().ToList();
        //     
        //     return (positionGroupQueryable.FirstOrDefault()?.CanLookupAllUser == true, userIds);
        // }

        // /// <summary>
        // /// 新增组和人员的关系
        // /// </summary>
        // /// <param name="userId">用户Id</param>
        // /// <param name="groupPositionIds">数据岗位组Id</param>
        // /// <returns></returns>
        // public async Task CreatePositionUserAsync(Guid userId, IEnumerable<Guid> groupPositionIds)
        // {
        //     var existPositionUsers = await positionUserRepository.GetListAsync(x => x.UserId == userId && groupPositionIds.Contains(x.PositionId));
        //     List<PositionUser> addPositionUsers = new List<PositionUser>();
        //     foreach (var groupId in groupPositionIds)
        //     {
        //         if (existPositionUsers.Any(x => x.PositionId == groupId))
        //             continue;
        //
        //         var positionUser = new PositionUser(GuidGenerator.Create(),groupId, userId);
        //         addPositionUsers.Add(positionUser);
        //     }
        //     if (addPositionUsers.Any())
        //         await positionUserRepository.InsertManyAsync(addPositionUsers);
        // }
        
        // /// <summary>
        // /// 根据岗位编码获取旗下的组信息
        // /// </summary>
        // /// <param name="code"></param>
        // /// <returns></returns>
        // public async Task<IEnumerable<Position>> GetGroupByPositionCode(string code)
        // {
        //     Guid positionId = await GetPositionIdByCode(code);
        //     var queryable = (await noTrackingRepository.GetQueryableAsync()).Where(p => p.ParentId == positionId)
        //         .OrderBy(p => p.Name)
        //         .Select(p => p);
        //     var positions = await AsyncExecuter.ToListAsync(queryable);
        //     return positions;
        // }
        
        /// <summary>
        /// 根据用户Id获取已绑定的岗位和组信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<DataPermission>> GetDataPermissionsOfUserAsync(Guid userId)
        {
            var dataPermissions = (await noTrackingRepository.GetListAsync(x => x.ParentId == null, default, x => x.Children))
                .Select(d => new DataPermission(d.Id, d.Name, d.Code, d.ParentId, d.IncludeAllUsers, d.IsEnabled, d.Sort, d.Brands,d.Users,
                    d.Children.Select(x => new DataPermission(x.Id, x.Name, x.Code, x.ParentId, x.IncludeAllUsers, x.IsEnabled, x.Sort, x.Brands,x.Users, default))
                        .ToList())
                );
            var list = dataPermissions.Where(d => d.IncludeAllUsers == true || d.Children.Any(x => x.Users != null && x.Users.Any(u => u.Id == userId))).ToList();
            list.Where(d=>d.IncludeAllUsers != true).ToList().ForEach(d =>
            {
                d.Children = d.Children.Where(x => x.Users.Any(u => u.Id == userId)).ToList();
            });
            return list;
        }
        //
        // /// <summary>
        // /// 判断用户是否属于某岗位
        // /// </summary>
        // /// <param name="userId"></param>
        // /// <param name="positionCode"></param>
        // /// <param name="cancellationToken"></param>
        // /// <returns></returns>
        // public async Task<bool> CheckUserIsPositionAsync(Guid userId, string positionCode, CancellationToken cancellationToken = default)
        // {
        //     return await (
        //         from position in await noTrackingRepository.GetQueryableAsync(x => x.Code == positionCode)
        //         join positionGroup in await noTrackingRepository.GetQueryableAsync(new PositionGroupSpecification()) on
        //             position.Id equals positionGroup.ParentId
        //         select positionGroup.PositionUsers.Select(x => x.UserId)
        //     ).AnyAsync(x => x.Contains(userId), cancellationToken: cancellationToken);
        // }
    }
}
