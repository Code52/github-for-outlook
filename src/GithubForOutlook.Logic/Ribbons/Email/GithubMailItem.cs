using System.Globalization;
using System.Windows;
using GithubForOutlook.Logic.Modules.Settings;
using GithubForOutlook.Logic.Modules.Tasks;
using GithubForOutlook.Logic.Repositories.Interfaces;
using GithubForOutlook.Logic.Ribbons.MainExplorer;
using Microsoft.Office.Core;
using Microsoft.Office.Interop.Outlook;
using VSTOContrib.Core.RibbonFactory;
using VSTOContrib.Core.RibbonFactory.Interfaces;
using VSTOContrib.Core.Wpf;
using VSTOContrib.Outlook.RibbonFactory;

namespace GithubForOutlook.Logic.Ribbons.Email
{
    [RibbonViewModel(OutlookRibbonType.OutlookMailRead)]
    public class GithubMailItem : OfficeViewModelBase, IRibbonViewModel
    {
        private readonly SettingsViewModel settings;
        private readonly TasksViewModel tasks;

        public GithubMailItem(SettingsViewModel settings, TasksViewModel tasks)
        {
            this.settings = settings;
            this.tasks = tasks;
        }

        private MailItem mailItem;

        public void Initialised(object context)
        {
            mailItem = (MailItem)context;
        }

        public void CurrentViewChanged(object currentView)
        {
        }

        public void Cleanup()
        {
            
        }

        public void CreateIssue(IRibbonControl control)
        {
            if (mailItem == null) return;

            if (tasks.User == null)
                tasks.Login(settings.UserName, settings.Password);

            tasks.Title = mailItem.Subject;
            tasks.Sender = mailItem.Sender.Name;
            tasks.ReceivedDate = mailItem.ReceivedTime;
            tasks.Body = string.Format("Sender: {0} <{1}>\nReceived: {2}\n\n{3}",
                                        mailItem.Sender.Name,
                                        mailItem.Sender.Address,
                                        mailItem.ReceivedTime.ToString(CultureInfo.CurrentCulture),
                                        mailItem.Body);

            new GithubExplorerWindow(tasks).Show();
        }

        public IRibbonUI RibbonUi { get; set; }
    }
}