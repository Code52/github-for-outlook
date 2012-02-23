using System;
using System.Windows;
using Microsoft.Office.Core;
using Microsoft.Office.Interop.Outlook;
using VSTOContrib.Core.RibbonFactory;
using VSTOContrib.Core.RibbonFactory.Interfaces;
using VSTOContrib.Core.RibbonFactory.Internal;
using VSTOContrib.Core.Wpf;
using VSTOContrib.Outlook.RibbonFactory;

namespace GithubForOutlook.Logic.Ribbons
{
    [RibbonViewModel(OutlookRibbonType.OutlookTask)]
    public class GithubTask : OfficeViewModelBase, IRibbonViewModel, IRegisterCustomTaskPane
    {
        private bool panelShown;
        private ICustomTaskPaneWrapper githubTaskPane;
        private GithubTaskAdapter githubIssue;

        public void Initialised(object context)
        {
            var task = (TaskItem)context;
            githubIssue = new GithubTaskAdapter(task);
        }

        public void CurrentViewChanged(object currentView)
        {
        }

        public bool IsGithubTask
        {
            get { return githubIssue.IsGithubTask; }
            private set
            {
                githubIssue.IsGithubTask = value;
                RaisePropertyChanged(() => IsGithubTask);
            }
        }

        public bool PanelShown
        {
            get { return panelShown; }
            set
            {
                if (panelShown == value) return;
                panelShown = value;
                githubTaskPane.Visible = value;
                RaisePropertyChanged("PanelShown");
            }
        }

        public void CreateIssue(IRibbonControl control)
        {
            MessageBox.Show("Hai");
        }

        public void RegisterTaskPanes(Register register)
        {
            githubTaskPane = register(() => new WpfPanelHost
            {
                Child = new GithubTaskPanel
                {
                    DataContext = this
                }
            }, "Github");
            githubTaskPane.Visible = IsGithubTask;
            PanelShown = IsGithubTask;
            githubTaskPane.VisibleChanged += GithubTaskPaneVisibleChanged;
            GithubTaskPaneVisibleChanged(this, EventArgs.Empty);
        }

        public void Cleanup()
        {
            githubTaskPane.VisibleChanged -= GithubTaskPaneVisibleChanged;
        }

        public IRibbonUI RibbonUi { get; set; }

        private void GithubTaskPaneVisibleChanged(object sender, EventArgs e)
        {
            panelShown = githubTaskPane.Visible;
            RaisePropertyChanged("PanelShown");
        }
    }
}