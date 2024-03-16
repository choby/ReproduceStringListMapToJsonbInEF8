using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace Evo.RAM.Roles
{
    public class RoleDto: FullAuditedEntityDto<Guid>
    {
        public virtual string Name { get; set; }
        public virtual string Code { get; set; }
        public virtual RoleType Type { get; set; }
        public virtual bool? IsEnabled { get; set; }
        public IEnumerable<string> Tags { get; set; }
        public virtual double? Sort { get; set; }
        public virtual Guid? TenantId { get; set; }
        public string Description { get; set; }
    }
}
