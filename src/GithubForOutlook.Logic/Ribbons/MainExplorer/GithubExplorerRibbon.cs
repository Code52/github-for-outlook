using System.Linq;
using System.Windows;
using System.Windows.Controls;
using GithubForOutlook.Logic.Models;
using GithubForOutlook.Logic.Modules.Settings;
using GithubForOutlook.Logic.Modules.Tasks;
using GithubForOutlook.Logic.Repositories.Interfaces;
using Microsoft.Office.Core;
using Microsoft.Office.Interop.Outlook;
using VSTOContrib.Core.RibbonFactory;
using VSTOContrib.Core.RibbonFactory.Interfaces;
using VSTOContrib.Core.Wpf;
using VSTOContrib.Outlook.RibbonFactory;
using VSTOContrib.Core.Extensions;
using MessageBox = System.Windows.Forms.MessageBox;

namespace GithubForOutlook.Logic.Ribbons.MainExplorer
{
    [RibbonViewModel(OutlookRibbonType.OutlookExplorer)]
    public class GithubExplorerRibbon : OfficeViewModelBase, IRibbonViewModel
    {
        public readonly SettingsViewModel Settings;
        public readonly TasksViewModel Tasks;
        public readonly IOutlookRepository OutlookRepository;

        public GithubExplorerRibbon(SettingsViewModel settings, TasksViewModel tasks, IOutlookRepository outlookRepository)
        {
            Settings = settings;
            Tasks = tasks;
            OutlookRepository = outlookRepository;
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
            
            if(Tasks.User == null)
                Tasks.Login(Settings.UserName, Settings.Password);

            var tasks = Tasks.GetProjects();

            Tasks.MailItem = selectedMailItem;

            new GithubExplorerWindow(Tasks).Show();

            //new Window
            //    {
            //        Content = new TextBlock { Text = string.Format("Create issue here for {0}", selectedMailItem.Subject)}
            //    }.Show();
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
                RaisePropertyChanged(()=>MailItemSelected);
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