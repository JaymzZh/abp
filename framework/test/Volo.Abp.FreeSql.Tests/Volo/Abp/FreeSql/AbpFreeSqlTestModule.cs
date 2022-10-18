using System;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Autofac;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Sqlite;
using Volo.Abp.Modularity;
using Volo.Abp.TestApp;

namespace Volo.Abp.FreeSql;

[DependsOn(typeof(AbpEntityFrameworkCoreSqliteModule))]
[DependsOn(typeof(TestAppModule))]
[DependsOn(typeof(AbpAutofacModule))]
[DependsOn(
    typeof(AbpFreeSqlModule),
    typeof(AbpAutofacModule)
    )]
public class AbpFreeSqlTestModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpDbConnectionOptions>(option =>
        {
            option.ConnectionStrings.Default = "Data Source=:memory:";
        });
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAbpDbContext<TestDbContext>(options =>
        {
            //取消EF通用仓储绑定到KmDbContext
            options.AddDefaultRepositories(true);
        });

        var sqliteConnection = CreateDatabaseAndGetConnection();

        //DbContext配置
        Configure<AbpDbContextOptions>(options =>
        {
            options.UseSqlite();

            options.Configure(abpDbContextConfigurationContext =>
            {
                abpDbContextConfigurationContext.DbContextOptions.UseSqlite(sqliteConnection);
            });
        });
        //Freesql配置
        Configure<AbpFreeSqlOption>(opt =>
        {
            //TestDbContext对应的Freesql初始化Action
            //注意Action会放到AbpFreeSqlOption列表上，因此多次 ConfigureFreeSql<TestDbContext>配置是追加Action，不是覆盖原有的Action
            opt.ConfigureFreeSql<TestDbContext>((freesql =>
            {
#if DEBUG
                freesql.Aop.CommandBefore += (_, e) => Console.WriteLine(e.Command.CommandText);
#endif
                freesql.GlobalFilter.Apply<ISoftDelete>(nameof(ISoftDelete), (x) => x.IsDeleted != true);
            }));
        });
    }

    private static SqliteConnection CreateDatabaseAndGetConnection()
    {
        var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();

        using (var context = new TestMigrationsDbContext(new DbContextOptionsBuilder<TestMigrationsDbContext>().UseSqlite(connection).Options))
        {
            context.GetService<IRelationalDatabaseCreator>().CreateTables();
            context.Database.ExecuteSqlRaw(
                @"CREATE VIEW View_PersonView AS 
                      SELECT Name, CreationTime, Birthday, LastActive FROM People");
        }

        return connection;
    }
}
