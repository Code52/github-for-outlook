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

        private void CreateIssue_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var dc = this.DataContext as TasksViewModel;
            if (dc == null) return;

            dc.CreateIssue(dc.MailItem.Subject, dc.MailItem.Body);
        }
    }
}
