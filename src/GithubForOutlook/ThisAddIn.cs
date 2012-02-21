using System;
using System.Windows;
using GithubForOutlook.Logic;
using Microsoft.Office.Core;
using VSTOContrib.Core.RibbonFactory;
using VSTOContrib.Core.RibbonFactory.Interfaces;
using VSTOContrib.Outlook.RibbonFactory;

namespace GithubForOutlook
{
    public partial class ThisAddIn
    {
        private AddinBootstrapper core;

        private void ThisAddInStartup(object sender, EventArgs e)
        {
            if (System.Windows.Application.Current == null)
                new Application { ShutdownMode = ShutdownMode.OnExplicitShutdown };
        }

        private void ThisAddInShutdown(object sender, EventArgs e)
        {
            core.Dispose();
            System.Windows.Application.Current.Shutdown();
        }

        private void InternalStartup()
        {
            core = new AddinBootstrapper();
            OutlookRibbonFactory.SetApplication(Application);
            RibbonFactory.Current.InitialiseFactory(
                t => (IRibbonViewModel)core.Resolve(t),
                CustomTaskPanes);

            Startup += ThisAddInStartup;
            Shutdown += ThisAddInShutdown;
        }

        protected override IRibbonExtensibility CreateRibbonExtensibilityObject()
        {
            return new OutlookRibbonFactory(typeof(AddinBootstrapper).Assembly);
        }
    }
}
