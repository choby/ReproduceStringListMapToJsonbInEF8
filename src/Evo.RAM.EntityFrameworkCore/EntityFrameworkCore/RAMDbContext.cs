using System.Net;
using System.Reflection;
using Evo.Infrastructure.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore.DistributedEvents;

namespace Evo.RAM.EntityFrameworkCore;

[ConnectionStringName("Default")]
public class RAMDbContext : ManagedDbContext<RAMDbContext>,IHasEventOutbox,IHasEventInbox
{
    public RAMDbContext(DbContextOptions<RAMDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly(),x=>x.Namespace.Contains("RAM") && !x.Namespace.Contains("Scm"));
        base.OnModelCreating(builder);

        /* Include modules to your migration db context */
        builder.ConfigureAuditLogging();
        builder.ConfigureEventInbox();
        builder.ConfigureEventOutbox();
    }

    
    protected override bool ShouldFilterEntity<TEntity>(IMutableEntityType entityType)
    {
        // if (!CurrentUser.IsInRole(Authorization.Roles.Admin) && this.Isolated<TEntity>())
        // {
        //     return true;
        // }
        return base.ShouldFilterEntity<TEntity>(entityType);
    }
    
    
#if DEBUG
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            //输出到控制台
            //.LogTo(System.Console.WriteLine, new[] { Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.CommandExecuting })
            //输出到vs输出窗口
            .EnableSensitiveDataLogging()
            .LogTo((msg) => System.Diagnostics.Trace.WriteLine(msg), new[] { Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.CommandExecuting })
            ;
        
    }
#endif
    public DbSet<OutgoingEventRecord> OutgoingEvents { get; set; }
    public DbSet<IncomingEventRecord> IncomingEvents { get; set; }
}
