using Evo.Infrastructure.Repositories;
using Evo.RAM.EntityFrameworkCore;
using Volo.Abp.Domain.Entities;
using Volo.Abp.EntityFrameworkCore;

namespace Evo.RAM.Repositories;

public class NoTrackingRepository<TEntity, TKey>
   : Evo.Infrastructure.Repositories.NoTrackingRepository<RAMDbContext, TEntity, TKey>, INoTrackingRepository<TEntity, TKey>
     where TEntity : class, IEntity<TKey>
{
    public NoTrackingRepository(IDbContextProvider<RAMDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
        
    }

    

    

    
}



public class NoTrackingRepository<TEntity>
   : Evo.Infrastructure.Repositories.NoTrackingRepository<RAMDbContext, TEntity>, INoTrackingRepository<TEntity>
     where TEntity : class, IEntity
{
    public NoTrackingRepository(IDbContextProvider<RAMDbContext> dbContextProvider)
        : base(dbContextProvider)
    {

    }
}
