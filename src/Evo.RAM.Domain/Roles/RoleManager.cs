using System;
using System.Linq;
using System.Threading.Tasks;
using Evo.Infrastructure;
using Evo.Infrastructure.Repositories;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace Evo.RAM.Roles
{
    /// <summary>
    /// 领域服务，关于领域的核心业务放在这一层
    /// </summary>
    public class RoleManager : DomainService
    {
        INoTrackingRepository<Role, Guid> _noTrackingRepository;
        public RoleManager(INoTrackingRepository<Role, Guid> noTrackingRepository)
        {
            this._noTrackingRepository = noTrackingRepository;
        }

        public async Task<Role> CreateAsync(string name, string code,RoleType type)
        {
            Check.NotNullOrEmpty(name, nameof(name));

            var existing = (await _noTrackingRepository.GetQueryableAsync()).Any(x => x.Name == name);
            if (existing)
                throw new BusinessException(ExceptionCodes.请求失败, $"名称{name}已存在");
            if (!string.IsNullOrEmpty(code))
            {
                existing = (await _noTrackingRepository.GetQueryableAsync()).Any(x => x.Code == code);
                if (existing)
                    throw new BusinessException(ExceptionCodes.请求失败, $"编码{code}已存在");
            }

            return new Role(name, code, type);
        }

        public async Task ChangeNameAsync(Role role, string name)
        {
            Check.NotNullOrEmpty(name, nameof(name));
            var existing = (await _noTrackingRepository.GetQueryableAsync()).Any(x => x.Name == name && role.Id != x.Id);
            if (existing)
                throw new BusinessException(ExceptionCodes.请求失败, $"名称{name}已存在");
            role.ChangeName(name);
        }

        public async Task ChangeCodeAsync(Role role, string code)
        {
            if (!string.IsNullOrEmpty(code))
            {
                var existing = (await _noTrackingRepository.GetQueryableAsync()).Any(x => x.Code == code && role.Id != x.Id);
                if (existing)
                    throw new BusinessException(ExceptionCodes.请求失败, $"编码{code}已存在");
            }
            role.ChangeCode(code);
        }

        

    }
}
