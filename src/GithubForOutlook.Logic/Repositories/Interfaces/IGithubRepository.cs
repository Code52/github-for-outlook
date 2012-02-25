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
        Task<IEnumerable<Repository>> GetProjects(NGitHub.Models.User user, int page = 0);

        Task<Issue> CreateIssue(string username, string reponame, string title, string body, string assigneename,
                                       string milestone, string[] labels);

        Task<IEnumerable<Issue>> GetIssues(string username, string reponame, State state = State.Open, int page = 0);

        void Login(string username, string password);
    }
}
