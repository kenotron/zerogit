using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeroGit.Models
{
    public class Repo : PropertyChangedBase
    {
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

        public short Port { get; set; }

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
    }
}
