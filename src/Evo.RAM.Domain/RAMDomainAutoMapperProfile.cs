using AutoMapper;
using Evo.Distributed.Events;
using Evo.RAM.DataPermissions;

namespace Evo.RAM;

public class RAMDomainAutoMapperProfile : Profile
{
    public RAMDomainAutoMapperProfile()
    {
        CreateMap<DataPermission, DataPermissionEto>();
        CreateMap<DataPermissionUser, DataPermissionUserEto>();
        CreateMap<DataPermissionBrand, DataPermissionBrandEto>();
    }
}

