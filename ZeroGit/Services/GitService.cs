using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeroGit.Services
{
    public class GitService
    {
        public Process StartGitDaemon(ushort port, string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                var descriptionFile = path + @"\.git\description";

                if (File.Exists(descriptionFile))
                {
                    var args = string.Format(@"daemon --verbose --export-all --port={0} --base-path=""{1}"" --base-path-relaxed", port, path);

                    var psi = new ProcessStartInfo("git", args)
                    {
                        CreateNoWindow = true,
                        WindowStyle = ProcessWindowStyle.Hidden,
                        UseShellExecute = false,
                        RedirectStandardOutput = true
                    };

                    return Process.Start(psi);
                }
            }

            return null;
        }

        public Process Clone(string host, ushort port, string path)
        {
            if (!string.IsNullOrEmpty(host) && !string.IsNullOrEmpty(path))
            {
                var args = string.Format(@"clone git://{0}:{1}/ {2}/", host, port, Path.Combine(@"C:\temp\", path));

                var psi = new ProcessStartInfo("git", args)
                {
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    UseShellExecute = false,
                    RedirectStandardOutput = true
                };

                return Process.Start(psi);
            }

            return null;
        }
    }
}
