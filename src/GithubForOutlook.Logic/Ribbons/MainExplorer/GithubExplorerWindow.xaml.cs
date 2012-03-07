using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GithubForOutlook.Logic.Modules.Tasks;
using NGitHub.Models;

namespace GithubForOutlook.Logic.Ribbons.MainExplorer
{
    public partial class GithubExplorerWindow
    {
        public GithubExplorerWindow(TasksViewModel tvm)
        {
            InitializeComponent();

            DataContext = tvm;
        }

        private void CreateIssueClick(object sender, RoutedEventArgs e)
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

        private void RepositoriesSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var dc = this.DataContext as TasksViewModel;
            if (dc == null) return;

            var selected = Repositories.SelectedItem as Repository;
            if(selected == null) return;

            dc.GetOrganisationUsers(selected);
            dc.GetMilestonesFor(selected);
            dc.SetLabels();
        }

        private void OnDragMoveWindow(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.RightButton != MouseButtonState.Pressed && e.MiddleButton != MouseButtonState.Pressed && e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }
    }
}
