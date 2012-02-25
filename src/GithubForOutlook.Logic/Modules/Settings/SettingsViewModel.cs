using System.Windows.Input;
using Analects.SettingsService;
using GithubForOutlook.Logic.Models;
using NGitHub;
using NGitHub.Authentication;
using RestSharp;
using VSTOContrib.Core.Wpf;

namespace GithubForOutlook.Logic.Modules.Settings
{
    // TODO: is there a "Loaded" event in VSTOContrib to mimic the Activate/Deactivate hooks inside CM?

    public class SettingsViewModel : OfficeViewModelBase
    {
        private readonly ApplicationSettings settings;
        private readonly ISettingsService settingsService;

        public SettingsViewModel(ApplicationSettings settings, ISettingsService settingsService)
        {
            this.settings = settings;
            this.settingsService = settingsService;
        }

        public bool TrackIssues { get; set; }

        public bool TrackPullRequests { get; set; }

        public User User { get; set; }

        public string UserName
        {
            get { return settings.UserName; }
            set
            {
                settings.UserName = value;
                RaisePropertyChanged(() => "UserName");
            }
        }

        public string Password
        {
            get { return settings.Password; }
            set
            {
                settings.Password = value;
                RaisePropertyChanged(() => "Password");
            }
        }

        public void SaveSettings()
        {
            settingsService.Set("Settings", settings);
            settingsService.Save();
        }

        private void MapUser(NGitHub.Models.User obj)
        {
            User = new User
            {
                Name = obj.Name,
                Icon = obj.AvatarUrl
            };
        }

        public ICommand ClearCommand { get { return new DelegateCommand(Clear); } }

        public void Clear()
        {
            User = null;
        }
    }
}
