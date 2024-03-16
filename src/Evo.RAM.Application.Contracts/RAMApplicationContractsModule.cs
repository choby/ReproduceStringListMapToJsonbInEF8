using Volo.Abp.Modularity;
using Volo.Abp.ObjectExtending;

namespace Evo.RAM;

[DependsOn(
    typeof(RAMDomainSharedModule),
    // typeof(AbpAccountApplicationContractsModule),
    // typeof(AbpFeatureManagementApplicationContractsModule),
    // typeof(AbpIdentityApplicationContractsModule),
    // typeof(AbpPermissionManagementApplicationContractsModule),
    // typeof(AbpSettingManagementApplicationContractsModule),
    // typeof(AbpTenantManagementApplicationContractsModule),
    typeof(AbpObjectExtendingModule)
)]
public class RAMApplicationContractsModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        RAMDtoExtensions.Configure();
    }
}
