using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using ZeroGit.ViewModels;
using ZeroGit.Services;

namespace ZeroGit
{
    class AppBootstrapper : BootstrapperBase
    {
        private SimpleContainer container;

        public AppBootstrapper()
	    {
            this.Initialize();
	    }

        protected override void Configure()
        {
            base.Configure();

            this.container = new SimpleContainer();
            this.container.Singleton<IWindowManager, WindowManager>();
            this.container.Singleton<IEventAggregator, EventAggregator>();
            this.container.Singleton<IFolderDialogService, FolderDialogService>();

            this.container.Singleton<GitService>();
            this.container.Singleton<BroadcastService>();

            this.container.PerRequest<MainWindowViewModel>();
            this.container.PerRequest<RepoViewModel>();
        }

        protected override void BuildUp(object instance)
        {
            base.BuildUp(instance);
            this.container.BuildUp(instance);
        }

        //
        // Summary:
        //     Override this to provide an IoC specific implementation
        //
        // Parameters:
        //   service:
        //     The service to locate.
        //
        // Returns:
        //     The located services.
        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return this.container.GetAllInstances(service);
        }

        //
        // Summary:
        //     Override this to provide an IoC specific implementation.
        //
        // Parameters:
        //   service:
        //     The service to locate.
        //
        //   key:
        //     The key to locate.
        //
        // Returns:
        //     The located service.
        protected override object GetInstance(Type service, string key)
        {
            var instance = this.container.GetInstance(service, key);
            if (instance != null)
                return instance;

            throw new Exception("Could not locate any instances.");
        }


        protected override void OnStartup(object sender, System.Windows.StartupEventArgs e)
        {
 	         this.DisplayRootViewFor<MainWindowViewModel>();
        }
    }
}
