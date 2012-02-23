using System.Windows;
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
            MessageBox.Show("Hai");
        }

        public IRibbonUI RibbonUi { get; set; }
    }
}