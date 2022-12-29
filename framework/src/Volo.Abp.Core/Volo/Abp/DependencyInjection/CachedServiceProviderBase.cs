using System;
using System.Collections.Concurrent;

namespace Volo.Abp.DependencyInjection;

public abstract class CachedServiceProviderBase : IDisposable
{
    private bool _isDisposed;
    protected IServiceProvider ServiceProvider { get; }
    protected ConcurrentDictionary<Type, Lazy<object>> CachedServices { get; }

    protected CachedServiceProviderBase(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
        CachedServices = new ConcurrentDictionary<Type, Lazy<object>>();
        CachedServices.TryAdd(typeof(IServiceProvider), new Lazy<object>(() => ServiceProvider));
    }

    public virtual object GetService(Type serviceType)
    {
        return CachedServices.GetOrAdd(
            serviceType,
            _ => new Lazy<object>(() => ServiceProvider.GetService(serviceType))
        ).Value;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_isDisposed)
        {
            return;
        }

        if (disposing)
        {
            CachedServices.Clear();
        }

        _isDisposed = true;
    }
}
