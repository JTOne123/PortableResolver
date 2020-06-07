﻿using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mohmd.AspNetCore.PortableResolver
{
    public class ResolverEngine : IEngine, IDisposable
    {
        #region Fields

        private IServiceScope _serviceScope = null;
        private IServiceProvider _serviceProvider;

        #endregion

        #region Properties

        public virtual IServiceProvider ServiceProvider => _serviceProvider;

        #endregion

        #region Methods

        public void Configure(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        internal void Configure(IServiceScope serviceScope)
        {
            _serviceScope = serviceScope;
            _serviceProvider = serviceScope.ServiceProvider;
        }

        public T Resolve<T>() where T : class
        {
            return (T)Resolve(typeof(T));
        }

        public object Resolve(Type type)
        {
            return GetServiceProvider()?.GetService(type);
        }

        public virtual IEnumerable<T> ResolveAll<T>()
        {
            return (IEnumerable<T>)GetServiceProvider()?.GetServices(typeof(T));
        }

        public virtual object ResolveUnregistered(Type type)
        {
            Exception innerException = null;

            foreach (var constructor in type.GetConstructors())
            {
                try
                {
                    //try to resolve constructor parameters
                    var parameters = constructor.GetParameters().Select(parameter =>
                    {
                        var service = Resolve(parameter.ParameterType);
                        if (service == null)
                        {
                            throw new Exception($"Unknown dependency `{parameter.ParameterType.FullName}`");
                        }

                        return service;
                    });

                    //all is ok, so create instance
                    return Activator.CreateInstance(type, parameters.ToArray());
                }
                catch (Exception ex)
                {
                    innerException = ex;
                }
            }

            throw new Exception("No constructor was found that had all the dependencies satisfied.", innerException);
        }

        public virtual T ResolveUnregistered<T>()
        {
            return (T)ResolveUnregistered(typeof(T));
        }

        public void Dispose()
        {
            _serviceScope?.Dispose();
            _serviceScope = null;
        }

        #endregion

        #region Utilities

        private IServiceProvider GetServiceProvider()
        {
            if (ServiceProvider is null)
            {
                return null;
            }

            // if there is a scope, it means we should not use http-context RequestedServices
            if (_serviceScope != null)
            {
                return ServiceProvider;
            }

            var accessor = ServiceProvider?.GetService<IHttpContextAccessor>();
            var context = accessor?.HttpContext;
            return context?.RequestServices ?? ServiceProvider;
        }

        #endregion
    }
}
