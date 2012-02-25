using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GithubForOutlook.Logic.Repositories.Interfaces;
using NGitHub;
using NGitHub.Models;
using NGitHub.Services;
using NGitHub.Web;
using RestSharp;

namespace GithubForOutlook.Logic.Repositories
{
    public class GithubRepository : IGithubRepository
    {
        private GitHubClient client;
        private readonly Action<GitHubException> _exceptionAction = ex => Console.WriteLine("Error: " + Enum.GetName(typeof(ErrorType), ex.ErrorType), "");

        private string _password;
        private string _username;
        public void Login(string username, string password)
        {
            _username = username;
            _password = password;
            client = new GitHubClient
            {
                Authenticator = new HttpBasicAuthenticator(_username, _password)
            };
        }

        public Task<User> GetUser()
        {
            return NGitHub<User>(client.Users.GetAuthenticatedUserAsync);
        }

        public Task<IEnumerable<User>> GetOrganisations(string username, int page = 0)
        {
            return NGitHub<IEnumerable<User>, string, int>(client.Organizations.GetOrganizationsAsync, username, page);
        }

        public Task<IEnumerable<Repository>> GetProjects(NGitHub.Models.User user, int page = 0)
        {
            //if (user.IsOrganisation)
            //    return NGitHub<IEnumerable<Repository>, string, int, RepositoryTypes>(_client.Organizations., user.Username, page, RepositoryTypes.Member);
            //else
            return NGitHub<IEnumerable<Repository>, string, int, RepositoryTypes>(client.Repositories.GetRepositoriesAsync, user.Login, page, RepositoryTypes.All);
        }

        public Task<Issue> CreateIssue(string username, string reponame, string title, string body, string assigneename,
                                string milestone, string[] labels)
        {
            return NGitHub<Issue, string, string, string, string, string, string, string[]>(client.Issues.CreateIssueAsync, username, reponame, title, body, assigneename, milestone, labels);
        }

        public Task<IEnumerable<Issue>> GetIssues(string username, string reponame, State state = State.Open,
                                                  int page = 0)
        {
            return NGitHub<IEnumerable<Issue>, string, string, State, int>(client.Issues.GetIssuesAsync, username,
                                                                           reponame, state, page);
        }

        public Task<IEnumerable<User>> GetOrganisationUsers(NGitHub.Models.User user, int page = 0)
        {
            return NGitHub<IEnumerable<User>, string, int>(client.Organizations.GetMembersAsync, user.Login, page);
        }
        
        private Task<T> NGitHub<T>(Func<Action<T>, Action<Exception>, GitHubRequestAsyncHandle> call)
        {
            var completionSource = new TaskCompletionSource<T>();
            call(completionSource.SetResult, completionSource.SetException);
            return completionSource.Task;
        }

        private Task<T> NGitHub<T, TArg>(Func<TArg, Action<T>, Action<Exception>, GitHubRequestAsyncHandle> call,
                                         TArg t1)
        {
            var completionSource = new TaskCompletionSource<T>();
            call(t1, completionSource.SetResult, completionSource.SetException);
            return completionSource.Task;
        }

        private Task<T> NGitHub<T, TArg, TArg2>(
            Func<TArg, TArg2, Action<T>, Action<Exception>, GitHubRequestAsyncHandle> call, TArg t1, TArg2 t2)
        {
            var completionSource = new TaskCompletionSource<T>();
            call(t1, t2, completionSource.SetResult, completionSource.SetException);
            return completionSource.Task;
        }

        private Task<T> NGitHub<T, TArg, TArg2, TArg3>(
            Func<TArg, TArg2, TArg3, Action<T>, Action<Exception>, GitHubRequestAsyncHandle> call, TArg t1, TArg2 t2,
            TArg3 t3)
        {
            var completionSource = new TaskCompletionSource<T>();
            call(t1, t2, t3, completionSource.SetResult, completionSource.SetException);
            return completionSource.Task;
        }

        private Task<T> NGitHub<T, TArg, TArg2, TArg3, TArg4>(
            Func<TArg, TArg2, TArg3, TArg4, Action<T>, Action<Exception>, GitHubRequestAsyncHandle> call, TArg t1,
            TArg2 t2, TArg3 t3, TArg4 t4)
        {
            var completionSource = new TaskCompletionSource<T>();
            call(t1, t2, t3, t4, completionSource.SetResult, completionSource.SetException);
            return completionSource.Task;
        }

        private Task<T> NGitHub<T, TArg, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7>(
    Func<TArg, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, Action<T>, Action<Exception>, GitHubRequestAsyncHandle> call, TArg t1,
    TArg2 t2, TArg3 t3, TArg4 t4, TArg5 t5, TArg6 t6, TArg7 t7)
        {
            var completionSource = new TaskCompletionSource<T>();
            call(t1, t2, t3, t4, t5, t6, t7, completionSource.SetResult, completionSource.SetException);
            return completionSource.Task;
        }
    }
}