using Mono.Zeroconf;
using Mono.Zeroconf.Providers.Bonjour;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeroGit.Services
{
    public class BroadcastService
    {
        private ushort port = 9419;
        private Mono.Zeroconf.Providers.Bonjour.RegisterService service;

        public event EventHandler<ServiceResolvedEventArgs> ServiceRemoved;
        public event EventHandler<ServiceResolvedEventArgs> ServiceAdded;

        public void Browse()
        {
            var browser = new Mono.Zeroconf.ServiceBrowser();

            browser.ServiceAdded += browser_ServiceAdded;
            browser.ServiceRemoved += browser_ServiceRemoved;

            browser.Browse("_git._tcp", "local");
        }

        void browser_ServiceRemoved(object o, Mono.Zeroconf.ServiceBrowseEventArgs args)
        {
            if (args.Service.RegType == "_git._tcp.")
            {
                args.Service.Resolved += (s, e) =>
                {
                    if (this.ServiceRemoved != null)
                    {
                        this.ServiceRemoved(s, e);
                    }
                };

                args.Service.Resolve();
            }
        }

        void browser_ServiceAdded(object o, Mono.Zeroconf.ServiceBrowseEventArgs args)
        {
            if (args.Service.RegType == "_git._tcp.")
            {
                args.Service.Resolved += (s, e) =>
                {
                    if (this.ServiceAdded != null)
                    {
                        this.ServiceAdded(s, e);
                    }
                };

                args.Service.Resolve();
            }
        }
    }
}
