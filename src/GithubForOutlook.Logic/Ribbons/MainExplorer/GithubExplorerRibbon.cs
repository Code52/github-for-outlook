using System;
using System.Globalization;
using GithubForOutlook.Logic.Modules.Settings;
using GithubForOutlook.Logic.Modules.Tasks;
using GithubForOutlook.Logic.Ribbons.Settings;
using Microsoft.Office.Core;
using Microsoft.Office.Interop.Outlook;
using VSTOContrib.Core.RibbonFactory;
using VSTOContrib.Core.RibbonFactory.Interfaces;
using VSTOContrib.Core.Wpf;
using VSTOContrib.Outlook.RibbonFactory;
using VSTOContrib.Core.Extensions;

namespace GithubForOutlook.Logic.Ribbons.MainExplorer
{
    [RibbonViewModel(OutlookRibbonType.OutlookExplorer)]
    public class GithubExplorerRibbon : OfficeViewModelBase, IRibbonViewModel
    {
        readonly Func<SettingsViewModel> getSettingsViewModel;
        readonly TasksViewModel tasks;

        public GithubExplorerRibbon(
            Func<SettingsViewModel> getSettingsViewModel, TasksViewModel tasks)
        {
            this.getSettingsViewModel = getSettingsViewModel;
            this.tasks = tasks;
        }

        private Explorer explorer;

        public void Initialised(object context)
        {
        }

        private void CleanupFolder()
        {

        }

        public void CreateIssue(IRibbonControl ribbonControl)
        {
            //TODO create proper task window and show it here.. selectedMailItem will be populated properly

            if (selectedMailItem == null) return;

            var settingsViewModel = getSettingsViewModel();

            if (settingsViewModel.User.Name == null || tasks.User == null  || settingsViewModel.User.Name != tasks.User.Login)
                tasks.User = null;

            tasks.Login();

            tasks.Title = selectedMailItem.Subject;
            tasks.Sender = selectedMailItem.Sender.Name;
            tasks.ReceivedDate = selectedMailItem.ReceivedTime;
            tasks.Body = string.Format("Sender: {0} <{1}>\nReceived: {2}\n\n{3}",
                                        selectedMailItem.Sender.Name,
                                        selectedMailItem.Sender.Address,
                                        selectedMailItem.ReceivedTime.ToString(CultureInfo.CurrentCulture),
                                        selectedMailItem.Body);

            new GithubExplorerWindow(tasks).Show();
        }

        public void ShowSettings(IRibbonControl ribbonControl)
        {
            var viewModel = getSettingsViewModel();

            var window = new GithubSettingsWindow(viewModel);
            window.Show();
        }

        public void CurrentViewChanged(object currentView)
        {
            explorer = (Explorer)currentView;
            explorer.SelectionChange += ExplorerOnSelectionChange;
        }

        private void ExplorerOnSelectionChange()
        {
            using (var selection = explorer.Selection.WithComCleanup())
            {
                if (selection.Resource.Count == 1)
                {
                    object item = null;
                    MailItem mailItem = null;
                    try
                    {
                        item = selection.Resource[1];
                        mailItem = item as MailItem;
                        if (mailItem != null)
                        {
                            if (selectedMailItem != null)
                                selectedMailItem.ReleaseComObject();
                            selectedMailItem = mailItem;
                            MailItemSelected = true;
                        }
                        else
                        {
                            MailItemSelected = false;
                        }
                    }
                    finally
                    {
                        if (mailItem == null)
                            item.ReleaseComObject();
                    }
                }
                else
                {
                    MailItemSelected = false;
                }
            }
        }

        private bool mailItemSelected;
        private MailItem selectedMailItem;

        public bool MailItemSelected
        {
            get { return mailItemSelected; }
            set
            {
                mailItemSelected = value;
                RaisePropertyChanged(() => MailItemSelected);
            }
        }

        public void Cleanup()
        {
            CleanupFolder();
            explorer = null;
        }

        public IRibbonUI RibbonUi { get; set; }
    }
}