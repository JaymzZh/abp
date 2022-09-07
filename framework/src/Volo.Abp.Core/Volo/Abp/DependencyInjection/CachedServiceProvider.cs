using System;
using System.Collections.Generic;

namespace Volo.Abp.DependencyInjection
{
    [ExposeServices(typeof(ICachedServiceProvider))]
    public class CachedServiceProvider : ICachedServiceProvider, IScopedDependency
    {
        private bool _isDisposed;

        protected IServiceProvider ServiceProvider { get; }
        
        protected IDictionary<Type, object> CachedServices { get; set; }

        public CachedServiceProvider(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
            
            CachedServices = new Dictionary<Type, object>
            {
                {typeof(IServiceProvider), serviceProvider}
            };
        }
        
        public object GetService(Type serviceType)
        {
            return CachedServices.GetOrAdd(
                serviceType,
                () => ServiceProvider.GetService(serviceType)
            );
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
                CachedServices = null;
            }

            _isDisposed = true;
        }
    }
}