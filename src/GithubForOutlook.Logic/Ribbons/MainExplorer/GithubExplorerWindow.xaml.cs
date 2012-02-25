using GithubForOutlook.Logic.Modules.Tasks;
using Microsoft.Office.Interop.Outlook;
using NGitHub.Models;

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
            ResultText.Text = "";

            var dc = this.DataContext as TasksViewModel;
            if (dc == null) return;

            var issue = dc.CreateIssue();

            if (issue.IsValid == false)
            {
                ResultText.Text = issue.Message;
                ResultText.Foreground = System.Windows.Media.Brushes.Red;
            }
            else
            {
                ResultText.Text = string.Format("Issue created: {0}", issue.Data.Number);
                ResultText.Foreground = System.Windows.Media.Brushes.Green;
            }
        }

        private void Repositories_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var dc = this.DataContext as TasksViewModel;
            if (dc == null) return;

            var selected = Repositories.SelectedItem as Repository;
            if(selected == null) return;

            UsersCombo.Items.Clear();
            UsersCombo.ItemsSource = dc.GetOrganisationUsers(selected);
        }
    }
}
