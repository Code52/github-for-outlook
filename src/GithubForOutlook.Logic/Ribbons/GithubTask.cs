using System;
using GithubForOutlook.Logic.Modules.Notifications;
using GithubForOutlook.Logic.Modules.Settings;
using GithubForOutlook.Logic.Modules.Tasks;
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
        private bool isGithubTask;

        public GithubTask(TasksViewModel tasks, NotificationsViewModel notifications, SettingsViewModel settings)
        {
            Tasks = tasks;
            Notifications = notifications;
            Settings = settings;
        }

        public void Initialised(object context)
        {
            //Stash this then start using custom properties to extend.
            var task = (TaskItem)context;
        }

        public void CurrentViewChanged(object currentView)
        {
        }

        public TasksViewModel Tasks { get; private set; }
        public NotificationsViewModel Notifications { get; private set; }
        public SettingsViewModel Settings { get; private set; }

        public bool IsGithubTask
        {
            get { return isGithubTask; }
            private set
            {
                isGithubTask = value;
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

        public void RegisterTaskPanes(Register register)
        {
            githubTaskPane = register(() => new WpfPanelHost
            {
                Child = new GithubTaskPanel
                {
                    DataContext = this
                }
            }, "Github");
            githubTaskPane.Visible = true;
            PanelShown = true;
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