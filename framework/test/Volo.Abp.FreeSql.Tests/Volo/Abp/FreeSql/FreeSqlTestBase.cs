using Volo.Abp.Testing;

namespace Volo.Abp.FreeSql;

public abstract class FreeSqlTestBase : AbpIntegratedTest<AbpFreeSqlTestModule>
{
    protected override void SetAbpApplicationCreationOptions(AbpApplicationCreationOptions options)
    {
        options.UseAutofac();
    }
}
