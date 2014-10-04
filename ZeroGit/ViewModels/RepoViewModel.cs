using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeroGit.Services;

namespace ZeroGit.ViewModels

{
    public class RepoViewModel : PropertyChangedBase
    {
        private GitService gitService;

        private string host;
        private string name;

        public string Host 
        { 
            get
            {
                return this.host;
            }

            set
            {
                this.host = value;
                this.NotifyOfPropertyChange(() => this.Host);
            }
        }

        public ushort Port { get; set; }

        public string Name
        {
            get
            {
                return this.name;
            }

            set
            {
                this.name = value;
                this.NotifyOfPropertyChange(() => this.Name);
            }
        }
        
        public string Description { get; set; }

        public RepoViewModel(GitService gitService)
        {
            this.gitService = gitService;
        }

        public void Clone()
        {
            this.gitService.Clone(this.Host, this.Port, this.Name);
        }
    }
}
