using System.Windows.Input;
using GithubForOutlook.Logic.Modules.Settings;

namespace GithubForOutlook.Logic.Ribbons.Settings
{
    public partial class GithubSettingsWindow
    {
        public GithubSettingsWindow(SettingsViewModel settings)
        {
            InitializeComponent();

            Settings = settings;
            this.DataContext = Settings;
        }

        public SettingsViewModel Settings { get; set; }

        private void SaveBasicAuth_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var username = User.Text;
            var password = Password.Password;

            Settings.SaveBasicAuthSettings(username, password);

            Close();
        }

        private void OnDragMoveWindow(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.RightButton != MouseButtonState.Pressed && e.MiddleButton != MouseButtonState.Pressed && e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }
    }
}
