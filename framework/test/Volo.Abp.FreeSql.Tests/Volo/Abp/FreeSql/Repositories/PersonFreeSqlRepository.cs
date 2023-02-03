using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FreeSql;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories.FreeSql;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.TestApp.Domain;

namespace Volo.Abp.FreeSql.Repositories;

public class PersonFreeSqlRepository : FreeSqlRepository<TestDbContext, Person>, ITransientDependency
{
    public PersonFreeSqlRepository(IDbContextProvider<TestDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }

    public virtual async Task<List<string>> GetAllPersonNames()
    {
        return (await (await GetDbConnectionAsync())
                .Select<string>(
                    "select Name from People"
                ).ToListAsync()
            ).ToList();
    }

    public virtual async Task<int> UpdatePersonNames(string name)
    {
        return await (await GetDbConnectionAsync())
            .Update<object>()
            .SetRaw("Name = @Name", new { Name = name })
            .Where("Id = @id", 1)
            .ExecuteAffrowsAsync();
    }
}
