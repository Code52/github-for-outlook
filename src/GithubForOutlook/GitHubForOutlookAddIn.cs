using GithubForOutlook.Core;
using Microsoft.Office.Core;
using VSTOContrib.Core.RibbonFactory;
using VSTOContrib.Core.RibbonFactory.Interfaces;
using VSTOContrib.Outlook.RibbonFactory;

namespace GithubForOutlook {
    public partial class GitHubForOutlookAddIn {
        private AddinBootstrapper _core;
        private void AddIn_Startup(object sender, System.EventArgs e) {
        }

        private void AddIn_Shutdown(object sender, System.EventArgs e) {
            _core.Dispose();
            System.Windows.Application.Current.Shutdown();
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup() {
            _core = new AddinBootstrapper();
            OutlookRibbonFactory.SetApplication(Application);
            RibbonFactory.Current.InitialiseFactory(
                t => (IRibbonViewModel)_core.Resolve(t),
                CustomTaskPanes);

            Startup += AddIn_Startup;
            Shutdown += AddIn_Shutdown;
        }

        protected override IRibbonExtensibility CreateRibbonExtensibilityObject() {
            return new OutlookRibbonFactory(typeof(AddinBootstrapper).Assembly);
        }
    }
}
