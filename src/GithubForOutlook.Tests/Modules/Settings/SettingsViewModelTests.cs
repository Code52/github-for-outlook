using GithubForOutlook.Logic.Models;
using GithubForOutlook.Logic.Modules.Settings;
using NGitHub.Authentication;
using NSubstitute;
using Xunit;

namespace GithubForOutlook.Tests.Modules.Settings
{
    public class SettingsViewModelTests
    {
        SettingsViewModel viewModel;
        IGitHubOAuthAuthorizer authorizer = Substitute.For<IGitHubOAuthAuthorizer>();

        public SettingsViewModelTests()
        {
            viewModel = new SettingsViewModel(authorizer);
        }

        [Fact]
        public void SignIn_WithSomeBackendService_SetsTheUser()
        {
            // act 
            viewModel.SignIn();

            Assert.NotNull(viewModel.User);
        }

        [Fact]
        public void SignIn_Always_CallsTheAuthorizer()
        {
            // act
            viewModel.SignIn();
            authorizer.ReceivedWithAnyArgs().GetAccessTokenAsync(null, null, null, null, null);
        }

        [Fact]
        public void Clear_WithUserPopulated_ClearsValue()
        {
            // arrange 
            viewModel.User = new User();

            // act
            viewModel.Clear();

            Assert.Null(viewModel.User);
        }
    }
}
