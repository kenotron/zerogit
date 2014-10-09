using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ZeroGit.Services
{
    public class FolderDialogService : IFolderDialogService
    {
        public string GetFolder()
        {
            var dialog = new FolderBrowserDialog();
            var result = dialog.ShowDialog();

            // Process open file dialog box results 
            if (result != null)
            {
                // Open document 
                return dialog.SelectedPath;
            }

            return null;
        }
    }
}
