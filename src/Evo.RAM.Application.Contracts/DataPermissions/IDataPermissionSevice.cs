using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Evo.RAM.DataPermissions;

public interface IDataPermissionSevice : IApplicationService
{
    /// <summary>
    /// 全部数据权限
    /// </summary>
    /// <param name="code">数据权限编码，一般用来区分业务系统</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IEnumerable<DataPermissionDto>> GetTenantDataPermissionsAsync(string code, CancellationToken cancellationToken);
}