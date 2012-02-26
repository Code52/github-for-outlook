using System;

namespace GithubForOutlook.Logic.Models
{
    public class GithubIssue
    {
        public string TaskId { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public DateTime LastModified { get; set; }
    }
}
