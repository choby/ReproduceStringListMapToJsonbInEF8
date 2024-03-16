using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AutoMapper;
using Volo.Abp.Json.SystemTextJson;
using Volo.Abp.Modularity;

namespace Evo.RAM;

[DependsOn(
    typeof(RAMDomainModule),
    typeof(AbpAutoMapperModule),
    typeof(RAMApplicationContractsModule)
    //,
    // typeof(AbpIdentityApplicationModule),
    // typeof(AbpPermissionManagementApplicationModule),
    // typeof(AbpTenantManagementApplicationModule),
    // typeof(AbpFeatureManagementApplicationModule),
    // typeof(AbpSettingManagementApplicationModule)
    )]
public class RAMApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<RAMApplicationModule>();
        });
     
        context.Services.Configure<AbpSystemTextJsonSerializerOptions>(options =>
        {
            options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        });
    }
}
