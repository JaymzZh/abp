using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using FreeSql;
using Volo.Abp.Domain.Entities;

namespace Volo.Abp.Domain.Repositories.FreeSql;

public interface IFreeSqlRepository<TEntity>
    where TEntity : class, IEntity
{
    [Obsolete("Use GetDbConnectionAsync method.")]
    IDbConnection DbConnection { get; }

    Task<IDbConnection> GetDbConnectionAsync();

    [Obsolete("Use GetDbTransactionAsync method.")]
    IDbTransaction DbTransaction { get; }

    Task<IDbTransaction> GetDbTransactionAsync();

    ISelect<TEntity> Select(Func<Type, string, string> tableRule = null);

    Task<ISelect<TEntity>> SelectAsync(Func<Type, string, string> tableRule = null);

    IInsert<TEntity> Insert(Func<string, string> tableRule = null);

    Task<IInsert<TEntity>> InsertAsync(Func<string, string> tableRule = null);

    IUpdate<TEntity> Update(Func<string, string> tableRule = null);

    Task<IUpdate<TEntity>> UpdateAsync(Func<string, string> tableRule = null);

    // IInsertOrUpdate<TEntity> InsertOrUpdate();
    //
    // Task<IInsertOrUpdate<TEntity>> InsertOrUpdateAsync();

    IDelete<TEntity> Delete(Func<string, string> tableRule = null);

    Task<IDelete<TEntity>> DeleteAsync(Func<string, string> tableRule = null);

    // Task<bool> SoftDeleteAsync<TEntity>(TEntity entity), ISoftDelete;
    //
    // Task<bool> SoftDeleteAsync<TEntity>(IList<TEntity> entity), ISoftDelete;

    Task<long> InsertAsync(TEntity entity, CancellationToken cancellationToken = default);

    Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

    Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);

    Task<List<TEntity>> GetListAsync(CancellationToken cancellationToken = default);

    Task<long> GetCountAsync(CancellationToken cancellationToken = default);

    Task<List<TEntity>> GetPagedListAsync(int skipCount, int maxResultCount, string sorting,
        CancellationToken cancellationToken = default);

    Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    Task DeleteAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default);
}

public interface IFreeSqlRepository<TEntity, TKey> : IFreeSqlRepository<TEntity>
    where TEntity : class, IEntity<TKey>
{

}