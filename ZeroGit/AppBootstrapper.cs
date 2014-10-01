using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using ZeroGit.ViewModels;

namespace ZeroGit
{
    class AppBootstrapper : BootstrapperBase
    {
        public AppBootstrapper()
	    {
            this.Initialize();
	    }

        protected override void OnStartup(object sender, System.Windows.StartupEventArgs e)
        {
 	         this.DisplayRootViewFor<MainWindowViewModel>();
        }
    }
}
