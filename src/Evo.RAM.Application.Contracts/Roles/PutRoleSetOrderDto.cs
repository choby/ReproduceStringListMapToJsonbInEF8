using System;
using System.Collections.Generic;

namespace Evo.RAM.Roles;

public class PutRoleSetOrderDto
{
    public IEnumerable<RoleOrder> Roles { get; set; }
}

public class RoleOrder
{
    public Guid Id { get; set; }
    public int Sort { get; set; }
}