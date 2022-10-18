using System;
using System.Collections.Generic;
using FreeSql;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Volo.Abp.EntityFrameworkCore;

namespace Volo.Abp.FreeSql;

/// <summary>
/// FreeSql的EF上下文<para></para>
/// (主要是根据AbpFreeSqlOption和TDbContext进行freesql初始化)<para></para>
/// 不建议直接操作freesql实例
/// </summary>
/// <typeparam name="TDbContext"></typeparam>
public abstract class FreeSqlEfDbContext<TDbContext> : AbpDbContext<TDbContext>
    where TDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    private readonly static Dictionary<Type, bool> SetDic = new Dictionary<Type, bool>();

    private readonly static object LockObj = new object();

    protected FreeSqlEfDbContext(DbContextOptions<TDbContext> options) : base(options)
    {
    }

    public override void Initialize(AbpEfCoreDbContextInitializationContext initializationContext)
    {
        lock (LockObj)
        {
            var isSet = SetDic.GetOrAdd(this.GetType(), () => false);
            if (!isSet)
            {
                base.Initialize(initializationContext);
                    
                var opt = initializationContext.UnitOfWork.ServiceProvider.GetService<IOptions<AbpFreeSqlOption>>();
                var actions = opt.Value.GetFreeAction(this.GetType());
                var free = this.Database.GetDbConnection().GetIFreeSql();

                foreach (var action in actions)
                {
                    action(free);
                }

                SetDic[this.GetType()] = true;
            }
        }
    }
}