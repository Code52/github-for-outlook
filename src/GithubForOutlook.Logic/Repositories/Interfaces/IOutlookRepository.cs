using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GithubForOutlook.Logic.Models;

namespace GithubForOutlook.Logic.Repositories.Interfaces
{
    public interface IOutlookRepository
    {
        IList<GithubIssue> GetIssues();
        IList<GithubIssue> GetModifiedIssues(DateTime since); 
        void SaveIssue(GithubIssue issue);
        void SaveIssues(IList<GithubIssue> issues);
    }
}
