using System;

namespace GithubForOutlook.Logic.Models
{
    [Serializable]
    public class ApplicationSettings
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string AccessToken { get; set; }
    }
}
