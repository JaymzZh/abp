using Microsoft.EntityFrameworkCore;

namespace Volo.Abp.FreeSql;
public class TestDbContext : FreeSqlEfDbContext<TestDbContext>
{
    public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
    {

    }
}
