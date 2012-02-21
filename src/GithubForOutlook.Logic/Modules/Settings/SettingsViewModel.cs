using System.Windows.Input;
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
        private readonly IGitHubOAuthAuthorizer authorizer;
        private readonly IGitHubClient client;

        public SettingsViewModel(IGitHubOAuthAuthorizer authorizer, IGitHubClient client)
        {
            this.authorizer = authorizer;
            this.client = client;
        }

        private bool trackIssues;
        public bool TrackIssues
        {
            get { return trackIssues; }
            set
            {
                trackIssues = value;
                RaisePropertyChanged(() => TrackIssues);
            }
        }

        private bool trackPullRequests;
        public bool TrackPullRequests
        {
            get { return trackPullRequests; }
            set
            {
                trackPullRequests = value;
                RaisePropertyChanged(() => TrackPullRequests);
            }
        }

        private User user;
        public User User
        {
            get { return user; }
            set
            {
                user = value;
                RaisePropertyChanged(() => User);
            }
        }

        public ICommand SignInCommand { get { return new DelegateCommand(SignIn); } }

        public void SignIn()
        {
            // TODO: settings provider
            authorizer.GetAccessTokenAsync("clientId", "clientSecret", "", OnCompleted, OnError);
        }

        private void OnError(GitHubException obj)
        {

        }

        private void OnCompleted(string obj)
        {
            // TODO: save token to settings
            client.Authenticator = new OAuth2UriQueryParameterAuthenticator(obj);
            client.Users.GetAuthenticatedUserAsync(MapUser, LogError);
        }

        private void LogError(GitHubException obj)
        {

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
