﻿using System;
using System.Collections.Generic;
using LogoFX.Practices.IoC;
using LogoFX.UI.Bootstraping.Contracts;
using Solid.Practices.IoC;

namespace LogoFX.UI.Bootstrapping.SimpleContainer
{
    public class ExtendedSimpleIocContainer : IIocContainer, IBootstrapperAdapter
    {
        private readonly ExtendedSimpleContainer _container = new ExtendedSimpleContainer();

        public void RegisterTransient<TService, TImplementation>() where TImplementation : class, TService
        {
            _container.RegisterPerRequest(typeof(TService), null, typeof(TImplementation));
        }

        public void RegisterTransient<TService>() where TService : class
        {
            RegisterTransient<TService, TService>();
        }

        public void RegisterTransient(Type serviceType, Type implementationType)
        {
            _container.RegisterPerRequest(serviceType, null, implementationType);
        }

        public void RegisterSingleton<TService, TImplementation>() where TImplementation : class, TService
        {
            _container.RegisterSingleton(typeof(TService), null, typeof(TImplementation));
        }

        public void RegisterSingleton<TService, TImplementation>(string key) where TImplementation : class, TService
        {
            _container.RegisterSingleton(typeof(TService), key, typeof(TImplementation));
        }

        public void RegisterInstance<TService>(TService instance) where TService : class
        {
            _container.RegisterInstance(typeof(TService), null, instance);
        }

        public bool HasHandler(Type service, string key)
        {
            return _container.HasHandler(service, key);
        }

        public void RegisterHandler(Type service, string key, Func<Practices.IoC.SimpleContainer, object> handler)
        {
            _container.RegisterHandler(service,key,handler);
        }

        public void RegisterPerLifetime<TService, TImplementation>(Func<object> lifetimeScopeAccess)
        {
            _container.RegisterPerLifetime(lifetimeScopeAccess,typeof(TService), null, typeof(TImplementation));
        }

        public TService GetInstance<TService>(Type serviceType) where TService : class
        {
            return (TService)_container.GetInstance(serviceType, null);
        }

        public TService GetInstance<TService>() where TService : class
        {
            return GetInstance<TService>(typeof(TService));
        }

        public object GetInstance(Type serviceType)
        {
            return _container.GetInstance(serviceType, null);
        }

        public IEnumerable<object> GetAllInstances(Type serviceType)
        {
            return _container.GetAllInstances(serviceType);
        }

        public void BuildUp(object instance)
        {
            _container.BuildUp(instance);
        }

        public TService Resolve<TService>() where TService : class
        {
            return GetInstance<TService>();
        }
    }
}