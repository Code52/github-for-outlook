using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using GithubForOutlook.Logic.Modules.Settings;
using MahApps.Metro.Controls;

namespace GithubForOutlook.Logic.Ribbons.About
{
    public partial class AboutWindow
    {
        public AboutWindow()
        {
            InitializeComponent();

            Authors.Text = string.Join(", ", authors);
            Components.Text = string.Join(", ", components);
        }

        private readonly List<string> authors = new List<string>
                                                    {
                                                         "Jake Ginnivan",
                                                         "David Christiansen",
                                                         "Andrew Tobin",
                                                         "Brendan Forster",
                                                         "Paul Jenkins",
                                                     };
    
        private readonly List<string> components = new List<string>
                                                       {
                                                         "Analects Libraries",
                                                         "Autofac",
                                                         "JSON.NET",
                                                         "MahApps.Metro",
                                                         "NGitHub",
                                                         "Notify Property Weaver",
                                                         "RestSharp",
                                                         "VSTO Contrib",
                                                         
                                                     };

        private void OnDragMoveWindow(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.RightButton != MouseButtonState.Pressed && e.MiddleButton != MouseButtonState.Pressed && e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void Visit_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("http://code52.org/github-for-outlook/");
            }
            catch //I forget what exceptions can be raised if the browser is crashed?
            {

            }

        }

        private void TryClose_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Close();
        }
    }
}
