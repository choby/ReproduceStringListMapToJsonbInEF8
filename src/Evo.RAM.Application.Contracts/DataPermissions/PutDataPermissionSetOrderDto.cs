using System;
using System.Collections.Generic;

namespace Evo.RAM.DataPermissions;

public class PutDataPermissionSetOrderDto
{
    public IEnumerable<DataPermissionOrder> DataPermissions { get; set; }
}

public class DataPermissionOrder
{
    public Guid Id { get; set; }
    public int Sort { get; set; }
}