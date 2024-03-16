using AutoMapper;
using Evo.RAM.DataPermissions;
using Evo.RAM.Roles;

namespace Evo.RAM;

public class RAMApplicationAutoMapperProfile : Profile
{
    public RAMApplicationAutoMapperProfile()
    {
        
        CreateMap<Role, RoleDto>();

        CreateMap<DataPermission, DataPermissionDto>()
            .ForMember(o => o.Children, d => d.MapFrom(s => s.Children != null ? s.Children : default));
        CreateMap<DataPermissionUser, DataPermissionBindUserDto>();
        CreateMap<DataPermissionBrand, DataPermissionBindBrandDto>();
    }
}