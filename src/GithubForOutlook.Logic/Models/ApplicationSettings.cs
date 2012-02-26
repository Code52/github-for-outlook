using System;
using VSTOContrib.Core.Wpf;

namespace GithubForOutlook.Logic.Models
{
    [Serializable]
    public class ApplicationSettings : OfficeViewModelBase
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
