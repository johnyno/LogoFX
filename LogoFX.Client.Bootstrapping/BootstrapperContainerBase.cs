﻿using System.Reflection;
using System.Windows;
using Caliburn.Micro;
using LogoFX.Client.Bootstrapping.Adapters.Contracts;
using Solid.Practices.IoC;

namespace LogoFX.Client.Bootstrapping
{    
    /// <summary>
    /// Base class for application and test boostrappers.
    /// Used when no navigation or special IoC container adapter-dependent logic is needed.
    /// </summary>
    /// <typeparam name="TRootViewModel">Type of Root ViewModel</typeparam>
    /// <typeparam name="TIocContainerAdapter">Type of IoC container adapter</typeparam>
    public partial class BootstrapperContainerBase<TRootViewModel, TIocContainerAdapter> :
#if !WinRT
        BootstrapperBase
#else
        CaliburnApplication
#endif               
        where TRootViewModel : class
        where TIocContainerAdapter : class, IIocContainer, IBootstrapperAdapter, new()
    {        
        private readonly TIocContainerAdapter _iocContainerAdapter;

        /// <summary>
        /// Initializes a new instance of the <see cref="BootstrapperContainerBase{TRootViewModel, TIocContainerAdapter}"/> class.
        /// </summary>
        /// <param name="iocContainerAdapter">The ioc container adapter.</param>        
        public BootstrapperContainerBase(
            TIocContainerAdapter iocContainerAdapter)
            :this(iocContainerAdapter, new BootstrapperCreationOptions())            
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BootstrapperContainerBase{TRootViewModel, TIocContainerAdapter}"/> class.
        /// </summary>
        /// <param name="iocContainerAdapter">The ioc container adapter.</param>
        /// <param name="creationOptions">The creation options.</param>
        public BootstrapperContainerBase(
            TIocContainerAdapter iocContainerAdapter,
            BootstrapperCreationOptions creationOptions)
            :base(creationOptions.UseApplication)
        {
            _iocContainerAdapter = iocContainerAdapter;
            _reuseCompositionInformation = creationOptions.ReuseCompositionInformation;
            if (creationOptions.DiscoverCompositionModules)
            {
                InitializeCompositionModules();
            }
            if (creationOptions.InspectAssemblies)
            {
                InitializeInspectedAssemblies();
            }
            Initialize();
        }

        private void DisplayRootView()
        {
            DisplayRootViewFor(typeof(TRootViewModel));
        }

        /// <summary>
        /// Override this to add custom behavior to execute after the application starts.
        /// </summary>
        /// <param name="sender">The sender.</param><param name="e">The args.</param>
        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            base.OnStartup(sender, e);
            DisplayRootView();
        }

        /// <summary>
        /// Configures the framework and executes boiler-plate registrations.
        /// </summary>
        protected sealed override void Configure()
        {
            base.Configure();            
            BootstrapperHelper<TRootViewModel, TIocContainerAdapter>.RegisterCore(_iocContainerAdapter);            
            BootstrapperHelper<TRootViewModel, TIocContainerAdapter>.RegisterViewsAndViewModels(_iocContainerAdapter,
                Assemblies);
            BootstrapperHelper<TRootViewModel, TIocContainerAdapter>.RegisterCompositionModules(_iocContainerAdapter,
                Modules);
            InitializeViewLocator();
            InitializeAdapter();      
            BootstrapperHelper<TRootViewModel, TIocContainerAdapter>.InitializeDispatcher();
            RegisterPlatformSpecificServices(_iocContainerAdapter);                        
            OnConfigure(_iocContainerAdapter);
        }
        
        private static void RegisterPlatformSpecificServices(TIocContainerAdapter iocContainerAdapter)
        {
            iocContainerAdapter.RegisterSingleton<IWindowManager, WindowManager>();
        }

        /// <summary>
        /// Override this method to inject custom logic during bootstrapper configuration.
        /// </summary>
        /// <param name="iocContainerAdapter">IoC container adapter</param>
        protected virtual void OnConfigure(TIocContainerAdapter iocContainerAdapter)
        {
        }

        private readonly object _defaultLifetimeScope = new object();

        /// <summary>
        /// Gets the current lifetime scope.
        /// </summary>
        /// <value>
        /// The current lifetime scope.
        /// </value>
        public virtual object CurrentLifetimeScope
        {
            get { return _defaultLifetimeScope; }
        }
        
        /// <summary>
        /// Gets the assemblies that will be inspected for the application components.
        /// </summary>
        /// <value>
        /// The assemblies.
        /// </value>
        protected Assembly[] Assemblies { get; private set; }        
    }    
}
