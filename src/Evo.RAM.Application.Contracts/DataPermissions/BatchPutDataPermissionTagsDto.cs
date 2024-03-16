using System;
using System.Collections;
using System.Collections.Generic;

namespace Evo.RAM.DataPermissions;

public class BatchPutDataPermissionTagsDto
{
    public List<Guid> DataPermissionIds { get; set; }
    public List<string> Tags { get; set; }
}