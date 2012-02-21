using GithubForOutlook.Logic.Models;
using GithubForOutlook.Logic.Modules.Settings;
using Xunit;

namespace GithubForOutlook.Tests.Modules.Settings
{
    public class SettingsViewModelTests
    {
        [Fact]
        public void SignIn_WithSomeBackendService_SetsTheUser()
        {
            var viewModel = new SettingsViewModel();
            viewModel.SignIn();
            Assert.NotNull(viewModel.User);
        }

        [Fact]
        public void Clear_WithUserPopulated_ClearsValue()
        {
            var viewModel = new SettingsViewModel();
            viewModel.User = new User();
            viewModel.Clear();
            Assert.Null(viewModel.User);
        }
    }
}
