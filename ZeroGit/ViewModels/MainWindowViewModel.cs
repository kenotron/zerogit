using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

using System.Diagnostics;

using System.Windows;
using System.IO;
using Mono.Zeroconf;
using ZeroGit.Models;

namespace ZeroGit.ViewModels
{
    class MainWindowViewModel : Screen
    {
        private Process process;
        private ushort port = 9419;
        private RegisterService service;

        private BindableCollection<Repo> repos;
 
        public BindableCollection<Repo> Repos
        { 
            get
            {
                return this.repos;
            }
            
            set
            {
                this.repos = value;
                this.NotifyOfPropertyChange(() => this.Repos);
            }
        }

        private bool isPublished;

        public bool IsPublished
        {
            get
            {
                return this.isPublished;
            }

            set
            {
                this.isPublished = value;
                this.NotifyOfPropertyChange(() => this.IsPublished);
            }
        }

        private string statusText;

        public string StatusText { 
            get
            {
                return this.statusText;
            }
            
            set
            {
                this.statusText = value;
                this.NotifyOfPropertyChange(() => this.StatusText);
            }
        }

        private bool flyoutOpened;

        public bool FlyoutOpened
        {
            get
            {
                return this.flyoutOpened;
            }

            set
            {
                this.flyoutOpened = value;
                this.NotifyOfPropertyChange(() => this.FlyoutOpened);
            }
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            this.Repos = new BindableCollection<Repo>();

            var browser = new ServiceBrowser();

            this.StatusText = "Drop Project Folder Here to Share Repository";

            browser.ServiceAdded += browser_ServiceAdded;
            browser.ServiceRemoved += browser_ServiceRemoved;
            browser.Browse("_git._tcp", "local");
            
            this.DisplayName = "ZeroGit";
        }

        void browser_ServiceRemoved(object o, ServiceBrowseEventArgs args)
        {
            var repos = from r in this.Repos where r.Name == args.Service.Name && r.Host == args.Service.HostEntry.HostName select r;

            if (repos.Any())
            {
                this.Repos.Remove(repos.First());
            }
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);

            if (this.process != null && !this.process.HasExited && this.process.Responding)
            {
                this.process.Kill();
            }
        }

        void browser_ServiceAdded(object o, ServiceBrowseEventArgs args)
        {
            if (args.Service.RegType == "_git._tcp." && !this.isPublished)
            {
                args.Service.Resolved += Service_Resolved;
                args.Service.Resolve();
            }
        }

        void Service_Resolved(object o, ServiceResolvedEventArgs args)
        {
            if (this.isPublished)
            {
                return;
            }

            var repo = new Repo
            {
                Description = args.Service.TxtRecord["description"].ValueString,
                Name = args.Service.Name,
                Port = args.Service.UPort,
                Host = args.Service.HostEntry.HostName
            };

            if (!this.Repos.Where(r => r.Name == repo.Name).Any())
            {
                this.Repos.Add(repo);
            }

            this.FlyoutOpened = true;
        }

        public void DropFile(DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // Note that you can have more than one file.
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                if (files.Any())
                {
                    var path = files[0];
                    this.StartGitDaemon(path);
                }
            }
        }

        public void StopPublishing()
        {
            this.service.Dispose();
            if (!this.process.HasExited && this.process.Responding)
            {
                this.process.Kill();
            }

            this.StatusText = "Drop Project Folder Here to git daemon Effortlessly";
            this.IsPublished = false;
        }

        public void StartGitDaemon(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                var descriptionFile = path + @"\.git\description";
                
                if (File.Exists(descriptionFile)) 
                {
                    var args = string.Format(@"daemon --verbose --export-all --port={0} --base-path=""{1}"" --base-path-relaxed", this.port, path);

                    var psi = new ProcessStartInfo("git", args)
                    {
                        CreateNoWindow = true,
                        WindowStyle = ProcessWindowStyle.Hidden,
                        UseShellExecute = false,
                        RedirectStandardOutput = true
                    };

                    this.process = Process.Start(psi);

                    this.PublishService(Path.GetFileName(path) , this.port, File.ReadAllText(descriptionFile));
                    
                    this.StatusText = "Publishing";
                }                
            }
        }

        private void PublishService(string name, ushort port, string description)
        {
            this.service = new RegisterService();
            this.service.Name = name;
            this.service.RegType = "_git._tcp";
            this.service.ReplyDomain = "local.";
            this.service.UPort = port;
            

            // TxtRecords are optional
            var txtRecord = new TxtRecord();
            txtRecord.Add("description", description);

            this.service.TxtRecord = txtRecord;

            this.service.Response += service_Response;

            this.service.Register();
        }

        void service_Response(object o, RegisterServiceEventArgs args)
        {
            if (args.IsRegistered)
            {
                this.StatusText = "Published";
                this.IsPublished = true;
            }
            else
            {
                this.StatusText = "Cannot Publish";
                this.IsPublished = false;
            }
        }
    }
}
