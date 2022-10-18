using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using FreeSql;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Volo.Abp.Domain.Repositories.FreeSql;

public class FreeSqlRepository<TDbContext, TEntity> : EfCoreRepository<TDbContext, TEntity>, IFreeSqlRepository<TEntity>
    where TDbContext : IEfCoreDbContext
    where TEntity : class, IEntity
{
    private readonly IDbContextProvider<TDbContext> _dbContextProvider;

    public FreeSqlRepository(IDbContextProvider<TDbContext> dbContextProvider) : base(dbContextProvider)
    {
        _dbContextProvider = dbContextProvider;
    }
    
    // [Obsolete("Use GetDbContextAsync() method.")]
    // protected virtual TDbContext DbContext => GetDbContext();
    //
    // [Obsolete("Use GetDbContextAsync() method.")]
    // private TDbContext GetDbContext()
    // {
    //     // Multi-tenancy unaware entities should always use the host connection string
    //     if (!EntityHelper.IsMultiTenant<TEntity>())
    //     {
    //         using (CurrentTenant.Change(null))
    //         {
    //             return _dbContextProvider.GetDbContext();
    //         }
    //     }
    //
    //     return _dbContextProvider.GetDbContext();
    // }
    //
    // protected virtual Task<TDbContext> GetDbContextAsync()
    // {
    //     // Multi-tenancy unaware entities should always use the host connection string
    //     if (!EntityHelper.IsMultiTenant<TEntity>())
    //     {
    //         using (CurrentTenant.Change(null))
    //         {
    //             return _dbContextProvider.GetDbContextAsync();
    //         }
    //     }
    //
    //     return _dbContextProvider.GetDbContextAsync();
    // }
    //
    // [Obsolete("Use GetDbSetAsync() method.")]
    // public virtual Microsoft.EntityFrameworkCore.DbSet<TEntity> DbSet => DbContext.Set<TEntity>();
    //
    // protected async Task<Microsoft.EntityFrameworkCore.DbSet<TEntity>> GetDbSetAsync()
    // {
    //     return (await GetDbContextAsync()).Set<TEntity>();
    // }

    [Obsolete("Use GetDbConnectionAsync method.")]
    public IDbConnection DbConnection => _dbContextProvider.GetDbContext().Database.GetDbConnection();

    public async Task<IDbConnection> GetDbConnectionAsync() =>
        (await _dbContextProvider.GetDbContextAsync()).Database.GetDbConnection();

    [Obsolete("Use GetDbTransactionAsync method.")]
    public IDbTransaction DbTransaction =>
        _dbContextProvider.GetDbContext().Database.CurrentTransaction?.GetDbTransaction();

    public async Task<IDbTransaction> GetDbTransactionAsync() => (await _dbContextProvider.GetDbContextAsync())
        .Database.CurrentTransaction?.GetDbTransaction();

    // public virtual async Task EnsureCollectionLoadedAsync<TProperty>(
    //     TEntity entity,
    //     Expression<Func<TEntity, IEnumerable<TProperty>>> propertyExpression,
    //     CancellationToken cancellationToken = default)
    //     where TProperty : class
    // {
    //     await (await GetDbContextAsync())
    //         .Entry(entity)
    //         .Collection(propertyExpression)
    //         .LoadAsync(GetCancellationToken(cancellationToken));
    // }
    //
    // public virtual async Task EnsurePropertyLoadedAsync<TProperty>(
    //     TEntity entity,
    //     Expression<Func<TEntity, TProperty>> propertyExpression,
    //     CancellationToken cancellationToken = default)
    //     where TProperty : class
    // {
    //     await (await GetDbContextAsync())
    //         .Entry(entity)
    //         .Reference(propertyExpression)
    //         .LoadAsync(GetCancellationToken(cancellationToken));
    // }

    // //https://github.com/dotnetcore/FreeSql/issues/267
    // /// <summary>
    // /// 不建议直接操作freesql实例
    // /// </summary>
    // protected IFreeSql FreeSql => DbConnection.GetIFreeSql();

    [Obsolete("use SelectAsync method.")]
    public ISelect<TEntity> Select(Func<Type, string, string> tableRule = null)
    {
        return _dbContextProvider.GetDbContext().Database.CurrentTransaction == null
            ? _dbContextProvider.GetDbContext().Database.GetDbConnection().Select<TEntity>().AsTable(tableRule)
            : _dbContextProvider.GetDbContext().Database.CurrentTransaction!.GetDbTransaction().Select<TEntity>().AsTable(tableRule);
    }

    public async Task<ISelect<TEntity>> SelectAsync(Func<Type, string, string> tableRule = null)
    {
        return (await _dbContextProvider.GetDbContextAsync()).Database.CurrentTransaction == null
            ? (await _dbContextProvider.GetDbContextAsync()).Database.GetDbConnection().Select<TEntity>().AsTable(tableRule)
            : (await _dbContextProvider.GetDbContextAsync()).Database.CurrentTransaction!.GetDbTransaction()
            .Select<TEntity>().AsTable(tableRule);
    }

    [Obsolete("use InsertAsync method.")]
    public IInsert<TEntity> Insert(Func<string, string> tableRule = null)
    {
        return _dbContextProvider.GetDbContext().Database.CurrentTransaction == null
            ? _dbContextProvider.GetDbContext().Database.GetDbConnection().Insert<TEntity>().AsTable(tableRule)
            : _dbContextProvider.GetDbContext().Database.CurrentTransaction!.GetDbTransaction().Insert<TEntity>().AsTable(tableRule);
    }

    public async Task<IInsert<TEntity>> InsertAsync(Func<string, string> tableRule = null)
    {
        return (await _dbContextProvider.GetDbContextAsync()).Database.CurrentTransaction == null
            ? (await _dbContextProvider.GetDbContextAsync()).Database.GetDbConnection().Insert<TEntity>().AsTable(tableRule)
            : (await _dbContextProvider.GetDbContextAsync()).Database.CurrentTransaction!.GetDbTransaction()
            .Insert<TEntity>().AsTable(tableRule);
    }

    [Obsolete("use UpdateAsync method.")]
    public IUpdate<TEntity> Update(Func<string, string> tableRule = null)
    {
        return _dbContextProvider.GetDbContext().Database.CurrentTransaction == null
            ? _dbContextProvider.GetDbContext().Database.GetDbConnection().Update<TEntity>().AsTable(tableRule)
            : _dbContextProvider.GetDbContext().Database.CurrentTransaction!.GetDbTransaction().Update<TEntity>().AsTable(tableRule);
    }

    public async Task<IUpdate<TEntity>> UpdateAsync(Func<string, string> tableRule = null)
    {
        return (await _dbContextProvider.GetDbContextAsync()).Database.CurrentTransaction == null
            ? (await _dbContextProvider.GetDbContextAsync()).Database.GetDbConnection().Update<TEntity>().AsTable(tableRule)
            : (await _dbContextProvider.GetDbContextAsync()).Database.CurrentTransaction!.GetDbTransaction()
            .Update<TEntity>().AsTable(tableRule);
    }

    // [Obsolete("use InsertOrUpdateAsync method.")]
    // public IInsertOrUpdate<TEntity> InsertOrUpdate<TEntity>() where TEntity : class
    // {
    //     return _dbContextProvider.GetDbContext().Database.CurrentTransaction == null
    //         ? _dbContextProvider.GetDbContext().Database.GetDbConnection().InsertOrUpdate<TEntity>()
    //         : _dbContextProvider.GetDbContext().Database.CurrentTransaction.GetDbTransaction()
    //             .InsertOrUpdate<TEntity>();
    // }
    //
    // public async Task<IInsertOrUpdate<TEntity>> InsertOrUpdateAsync<TEntity>() where TEntity : class
    // {
    //     return (await _dbContextProvider.GetDbContextAsync()).Database.CurrentTransaction == null
    //         ? (await _dbContextProvider.GetDbContextAsync()).Database.GetDbConnection().InsertOrUpdate<TEntity>()
    //         : (await _dbContextProvider.GetDbContextAsync()).Database.CurrentTransaction.GetDbTransaction()
    //         .InsertOrUpdate<TEntity>();
    // }

    [Obsolete("use DeleteAsync method.")]
    public IDelete<TEntity> Delete(Func<string, string> tableRule = null)
    {
        return _dbContextProvider.GetDbContext().Database.CurrentTransaction == null
            ? _dbContextProvider.GetDbContext().Database.GetDbConnection().Delete<TEntity>().AsTable(tableRule)
            : _dbContextProvider.GetDbContext().Database.CurrentTransaction!.GetDbTransaction().Delete<TEntity>().AsTable(tableRule);
    }

    public async Task<IDelete<TEntity>> DeleteAsync(Func<string, string> tableRule = null)
    {
        return (await _dbContextProvider.GetDbContextAsync()).Database.CurrentTransaction == null
            ? (await _dbContextProvider.GetDbContextAsync()).Database.GetDbConnection().Delete<TEntity>().AsTable(tableRule)
            : (await _dbContextProvider.GetDbContextAsync()).Database.CurrentTransaction!.GetDbTransaction()
            .Delete<TEntity>().AsTable(tableRule);
    }
    

    // public async Task<bool> SoftDeleteAsync<TEntity>(TEntity entity) where TEntity : class, ISoftDelete
    // {
    //     var row = await (await UpdateAsync<TEntity>()).Set(x => x.IsDeleted, true).SetSource(entity)
    //         .ExecuteAffrowsAsync();
    //     return row == 1;
    // }
    //
    // public async Task<bool> SoftDeleteAsync<TEntity>(IList<TEntity> entities) where TEntity : class, ISoftDelete
    // {
    //     var row = await (await UpdateAsync<TEntity>()).Set(x => x.IsDeleted, true).SetSource(entities)
    //         .ExecuteAffrowsAsync();
    //     return row == entities.Count();
    // }
    // public async override Task<TEntity> InsertAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default)
    // {
    //     throw new NotImplementedException();
    // }
    //
    // public async override Task<TEntity> UpdateAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default)
    // {
    //     throw new NotImplementedException();
    // }
    //
    // public async override Task DeleteAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default)
    // {
    //     throw new NotImplementedException();
    // }
    //
    // public async override Task<List<TEntity>> GetListAsync(bool includeDetails = false, CancellationToken cancellationToken = default)
    // {
    //     throw new NotImplementedException();
    // }
    //
    // public async override Task<long> GetCountAsync(CancellationToken cancellationToken = default)
    // {
    //     throw new NotImplementedException();
    // }
    //
    // public async override Task<List<TEntity>> GetPagedListAsync(int skipCount, int maxResultCount, string sorting, bool includeDetails = false,
    //     CancellationToken cancellationToken = default)
    // {
    //     throw new NotImplementedException();
    // }
    //
    // public async override Task<IQueryable<TEntity>> GetQueryableAsync()
    // {
    //     throw new NotImplementedException();
    // }
    //
    // public async override Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> predicate, bool includeDetails = true, CancellationToken cancellationToken = default)
    // {
    //     throw new NotImplementedException();
    // }
    //
    // public async override Task DeleteAsync(Expression<Func<TEntity, bool>> predicate, bool autoSave = false, CancellationToken cancellationToken = default)
    // {
    //     throw new NotImplementedException();
    // }
    //
    // public async override Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate, bool includeDetails = false, CancellationToken cancellationToken = default)
    // {
    //     throw new NotImplementedException();
    // }
    //
    // protected override IQueryable<TEntity> GetQueryable()
    // {
    //     throw new NotImplementedException();
    // }
}

public class FreeSqlRepository<TDbContext, TEntity, TKey> : FreeSqlRepository<TDbContext, TEntity>,
    IFreeSqlRepository<TEntity, TKey>,
    ISupportsExplicitLoading<TEntity, TKey>

    where TDbContext : IEfCoreDbContext
    where TEntity : class, IEntity<TKey>
{
    public FreeSqlRepository(IDbContextProvider<TDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }
    
    public virtual async Task<TEntity> GetAsync(TKey id, bool includeDetails = true, CancellationToken cancellationToken = default)
    {
        var entity = await FindAsync(id, includeDetails, GetCancellationToken(cancellationToken));

        if (entity == null)
        {
            throw new EntityNotFoundException(typeof(TEntity), id);
        }

        return entity;
    }

    public virtual async Task<TEntity> FindAsync(TKey id, bool includeDetails = true, CancellationToken cancellationToken = default)
    {
        return includeDetails
            ? await (await WithDetailsAsync()).OrderBy(e => e.Id).FirstOrDefaultAsync(e => e.Id.Equals(id), GetCancellationToken(cancellationToken))
            : await (await GetDbSetAsync()).FindAsync(new object[] { id }, GetCancellationToken(cancellationToken));
    }

    public virtual async Task DeleteAsync(TKey id, bool autoSave = false, CancellationToken cancellationToken = default)
    {
        var entity = await FindAsync(id, cancellationToken: cancellationToken);
        if (entity == null)
        {
            return;
        }

        await DeleteAsync(entity, autoSave, cancellationToken);
    }

    public virtual async Task DeleteManyAsync(IEnumerable<TKey> ids, bool autoSave = false, CancellationToken cancellationToken = default)
    {
        cancellationToken = GetCancellationToken(cancellationToken);

        var entities = await (await GetDbSetAsync()).Where(x => ids.Contains(x.Id)).ToListAsync(cancellationToken);

        await DeleteManyAsync(entities, autoSave, cancellationToken);
    }
}