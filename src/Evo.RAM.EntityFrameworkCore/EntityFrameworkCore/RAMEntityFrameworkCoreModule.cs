using System;
using Evo.Infrastructure.Repositories;
using Evo.RAM.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.PostgreSql;
using Volo.Abp.Guids;
using Volo.Abp.Modularity;

namespace Evo.RAM.EntityFrameworkCore;

[DependsOn(
    typeof(RAMDomainModule),
    typeof(AbpEntityFrameworkCorePostgreSqlModule),
    typeof(AbpAuditLoggingEntityFrameworkCoreModule)
    )]
public class RAMEntityFrameworkCoreModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        // https://www.npgsql.org/efcore/release-notes/6.0.html#opting-out-of-the-new-timestamp-mapping-logic
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        RAMEfCoreEntityExtensionMappings.Configure();
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAbpDbContext<RAMDbContext>(options =>
        {
                /* Remove "includeAllEntities: true" to create
                 * default repositories only for aggregate roots */
            options.AddDefaultRepositories(includeAllEntities: false);
            
        });

        Configure<AbpDbContextOptions>(options =>
        {
            options.PreConfigure<RAMDbContext>(opts =>
            {
                opts.DbContextOptions.UseLazyLoadingProxies();
            });
                /* The main point to change your DBMS.
                 * See also RAMMigrationsDbContextFactory for EF Core tooling. */
            options.UseNpgsql(o => o.MinBatchSize(1).MaxBatchSize(100));
        });
        
        //数据库提供程序在处理GUID时的行为有所不同,你应根据数据库提供程序进行设置. SequentialGuidType 有以下枚举成员:
        //SequentialAtEnd(default) 用于SQL Server.
        //SequentialAsString 用于MySQL和PostgreSQL.
        //SequentialAsBinary 用于Oracle.
        Configure<AbpSequentialGuidGeneratorOptions>(options =>
        {
            options.DefaultSequentialGuidType = SequentialGuidType.SequentialAsString;
        });
        //pgsql 8.0版本需要手动启动旧版本的 jsonb 映射方式
        // NpgsqlConnection.GlobalTypeMapper.EnableDynamicJson();
        NpgsqlConnection.GlobalTypeMapper.UseJsonNet();
        //使用自定义的Registrar注册仓储，这样dbcontext中的动态dbset才有效
        context.Services.AddDefaultRepositories();
        context.Services.AddTransient(typeof(INoTrackingRepository<,>), typeof(Repositories.NoTrackingRepository<,>));
        context.Services.AddTransient(typeof(INoTrackingRepository<>), typeof(NoTrackingRepository<>));
    }
}
