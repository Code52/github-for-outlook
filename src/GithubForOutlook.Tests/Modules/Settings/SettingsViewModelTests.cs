using System;
using GithubForOutlook.Logic.Models;
using GithubForOutlook.Logic.Modules.Settings;
using NGitHub;
using NGitHub.Authentication;
using NGitHub.Services;
using NSubstitute;
using NSubstitute.Core;
using Xunit;

namespace GithubForOutlook.Tests.Modules.Settings
{
    public class SettingsViewModelTests
    {
        readonly SettingsViewModel viewModel;
        readonly IGitHubOAuthAuthorizer authorizer = Substitute.For<IGitHubOAuthAuthorizer>();
        readonly IGitHubClient client = Substitute.For<IGitHubClient>();

        public SettingsViewModelTests()
        {
            client.Users.Returns(Substitute.For<IUserService>());
            viewModel = new SettingsViewModel(authorizer, client);
        }

        [Fact]
        public void SignIn_Always_CallsTheAuthorizer()
        {
            // act
            viewModel.SignIn();
            authorizer.ReceivedWithAnyArgs().GetAccessTokenAsync(null, null, null, null, null);
        }

        [Fact]
        public void SignIn_WhenCallbackInvoked_SetsTheUser()
        {
            const string someValue = "1234";
            SetAuthResponse(c => ExecuteSuccessCallback(c[3], someValue));
            SetClientResponse(c => ReturnUser(c[0], "shiftkey", "foo"));

            // act
            viewModel.SignIn();
            Assert.NotNull(viewModel.User);
        }

        [Fact]
        public void SignIn_WhenErrorInvoked_DoesNotSetsTheUser()
        {
            SetAuthResponse(c => ExecuteErrorCallback(c[4]));

            // act
            viewModel.SignIn();
            Assert.Null(viewModel.User);
        }

        [Fact]
        public void SignIn_WhenNoCallbackInvoked_DoesNotSetsTheUser()
        {
            // act
            viewModel.SignIn();
            Assert.Null(viewModel.User);
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

        private static void ReturnUser(object parameter, string name, string avatarUrl)
        {
            var callback = parameter as Action<NGitHub.Models.User>;
            if (callback != null) callback(new NGitHub.Models.User { AvatarUrl = avatarUrl, Name = name });
        }

        private static void ExecuteErrorCallback(object parameter)
        {
            var callback = parameter as Action<GitHubException>;
            if (callback != null) callback(null);
        }

        private static void ExecuteSuccessCallback(object parameter, string someValue)
        {
            var callback = parameter as Action<string>;
            if (callback != null) callback(someValue);
        }

        private void SetAuthResponse(Action<CallInfo> callback)
        {
            authorizer.When(
                a =>
                a.GetAccessTokenAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<Action<string>>(),
                                      Arg.Any<Action<GitHubException>>()))
                .Do(callback);
        }

        private void SetClientResponse(Action<CallInfo> callback)
        {
            client.Users.When(
                a =>
                a.GetAuthenticatedUserAsync(Arg.Any<Action<NGitHub.Models.User>>(), Arg.Any<Action<GitHubException>>()))
                .Do(callback);
        }
    }
}
