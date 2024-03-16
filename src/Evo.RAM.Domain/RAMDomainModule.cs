using Evo.Distributed.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Evo.RAM.DataPermissions;
using Volo.Abp.Auditing;
using Volo.Abp.AuditLogging;
using Volo.Abp.AutoMapper;
using Volo.Abp.Domain.Entities.Events.Distributed;
using Volo.Abp.Emailing;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;

namespace Evo.RAM;

[DependsOn(
    typeof(RAMDomainSharedModule),
    typeof(AbpAuditLoggingDomainModule)
)]
public class RAMDomainModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpDistributedEntityEventOptions>(options =>
        {
            
            options.AutoEventSelectors.Add<DataPermission>();
            
            options.EtoMappings.Add<DataPermission, DataPermissionEto>();
        });
        
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<RAMDomainModule>();
        });

        Configure<AbpMultiTenancyOptions>(options =>
        {
            options.IsEnabled = false;//MultiTenancyConsts.IsEnabled;
        });

#if DEBUG
        context.Services.Replace(ServiceDescriptor.Singleton<IEmailSender, NullEmailSender>());
#endif
    }

    public override void PostConfigureServices(ServiceConfigurationContext context)
    {
        base.PostConfigureServices(context);
        context.Services.Replace(ServiceDescriptor.Transient<IAuditingStore, Evo.Infrastructure.Auditing.AuditingStore>());
        context.Services.Replace(ServiceDescriptor.Transient<IAuditPropertySetter, Evo.Infrastructure.Auditing.AuditPropertySetter>());
    }
}
