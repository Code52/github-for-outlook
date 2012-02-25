using GithubForOutlook.Logic.Modules.Tasks;
using Microsoft.Office.Interop.Outlook;

namespace GithubForOutlook.Logic.Ribbons.MainExplorer
{
    public partial class GithubExplorerWindow
    {
        public GithubExplorerWindow(TasksViewModel tvm)
        {
            InitializeComponent();

            this.DataContext = tvm;
        }
    }
}
