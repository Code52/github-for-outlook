using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NGitHub.Models;

namespace GithubForOutlook.Logic.Repositories.Interfaces
{
    public interface IGithubRepository
    {
        Task<User> GetUser();
        Task<IEnumerable<User>> GetOrganisations(string username, int page = 0);
        Task<IEnumerable<Repository>> GetProjects(string username, int page = 0);

        Task<Issue> CreateIssue(string username, string reponame, string title, string body, string assigneename,
                                       int? milestone, string[] labels);

        Task<IEnumerable<Issue>> GetIssues(string username, string reponame, State state = State.Open, int page = 0);

        Task<IEnumerable<User>> GetOrganisationUsers(string username, int page = 0);

        Task<IEnumerable<Label>> GetLabels(string username, string reponame);

        Task<IEnumerable<Milestone>> GetMilestones(string username, string reponame);

        void Login(string username, string password);
    }
}
