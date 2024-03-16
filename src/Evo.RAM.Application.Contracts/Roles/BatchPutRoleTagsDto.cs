using System;
using System.Collections.Generic;

namespace Evo.RAM.Roles;

public class BatchPutRoleTagsDto
{
    public List<Guid> RoleIds { get; set; }
    public List<string> Tags { get; set; }
}