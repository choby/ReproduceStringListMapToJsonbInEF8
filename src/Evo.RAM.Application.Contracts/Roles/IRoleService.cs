using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Evo.RAM.Roles;

public interface IRoleService : IApplicationService
{
    Task<IEnumerable<RoleDto>> GetForFactoryAsync(CancellationToken cancellationToken);
}