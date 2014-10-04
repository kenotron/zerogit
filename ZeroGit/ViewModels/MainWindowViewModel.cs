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
using ZeroGit.Services;
using System.Net;

namespace ZeroGit.ViewModels
{
    class MainWindowViewModel : Screen
    {
        private Process process;
        
        private ushort port = 9419;
        private RegisterService service;

        private GitService gitService;
        private BroadcastService broadcastService;

        private bool isReadyToDrop = false;

        public bool IsReadyToDrop
        {
            get { return this.isReadyToDrop; }
            set { this.isReadyToDrop = value; this.NotifyOfPropertyChange(() => this.IsReadyToDrop); }
        }
        
        private BindableCollection<RepoViewModel> repos;
 
        public BindableCollection<RepoViewModel> Repos
        { 
            get
            {
                return this.repos;
            }
            
            private set
            {
                this.repos = value;
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

        public MainWindowViewModel(GitService gitService, BroadcastService broadcastService)
        {
            this.gitService = gitService;
            this.broadcastService = broadcastService;

            this.Repos = new BindableCollection<RepoViewModel>();

            this.DisplayName = "ZeroGit";
            this.StatusText = "Drop Project Folder Here to Share Repository";
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            this.broadcastService.ServiceAdded += broadcastService_ServiceAdded;
            this.broadcastService.ServiceRemoved += broadcastService_ServiceRemoved;
            this.broadcastService.Browse();
        }

        void broadcastService_ServiceRemoved(object sender, ServiceResolvedEventArgs e)
        {
            var repos = from r in this.Repos where r.Name == e.Service.Name && r.Host == e.Service.HostEntry.HostName select r;

            if (repos.Any())
            {
                this.Repos.Remove(repos.First());
            }
        }

        void broadcastService_ServiceAdded(object sender, ServiceResolvedEventArgs e)
        {
            if (this.isPublished)
            {
                return;
            }

            var repo = IoC.Get<RepoViewModel>();

            repo.Name = e.Service.Name;
            repo.Port = (ushort)IPAddress.NetworkToHostOrder((short)e.Service.UPort);
            repo.Host = e.Service.HostEntry.HostName;

            if (!this.Repos.Where(r => r.Name == repo.Name).Any())
            {
                this.Repos.Add(repo);
            }

            this.FlyoutOpened = true;
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);

            if (this.process != null && !this.process.HasExited && this.process.Responding)
            {
                this.process.Kill();
            }
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

        public void DragOver(DragEventArgs e)
        {
            this.IsReadyToDrop = true;
        }

        public void DragLeave(DragEventArgs e)
        {
            this.IsReadyToDrop = true;
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
                this.process = this.gitService.StartGitDaemon(port, path);
                this.PublishService(Path.GetFileName(path), this.port);
                this.StatusText = "Publishing";
            }
        }

        private void PublishService(string name, ushort port)
        {
            this.service = new RegisterService();
            this.service.Name = name;
            this.service.RegType = "_git._tcp";
            this.service.ReplyDomain = "local.";
            this.service.UPort = port;
            

            /*// TxtRecords are optional
            var txtRecord = new TxtRecord();
            txtRecord.Add("description", description);

            this.service.TxtRecord = txtRecord;*/

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
