using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GithubForOutlook.Logic.Repositories.Interfaces;
using NGitHub;
using NGitHub.Models;

namespace GithubForOutlook.Logic.Repositories
{
    public class GithubRepository : IGithubRepository
    {
        private readonly IGitHubClient client;

        public GithubRepository(IGitHubClient client)
        {
            this.client = client;
        }

        public Task<User> GetUser()
        {
            return NGitHub<User>(client.Users.GetAuthenticatedUserAsync);
        }

        public Task<IEnumerable<User>> GetOrganisations(string username, int page = 0)
        {
            return NGitHub<IEnumerable<User>, string, int>(client.Organizations.GetOrganizationsAsync, username, page);
        }

        public Task<IEnumerable<Repository>> GetProjects(string username, int page = 0)
        {
            //if (user.IsOrganisation)
            //    return NGitHub<IEnumerable<Repository>, string, int, RepositoryTypes>(_client.Organizations., user.Username, page, RepositoryTypes.Member);
            //else
            return NGitHub<IEnumerable<Repository>, string, int, RepositoryTypes>(client.Repositories.GetRepositoriesAsync, username, page, RepositoryTypes.All);
        }

        public Task<Issue> CreateIssue(string username, string reponame, string title, string body, string assigneename,
                                int? milestone, string[] labels)
        {
            return NGitHub<Issue, string, string, string, string, string, int?, string[]>(client.Issues.CreateIssueAsync, username, reponame, title, body, assigneename, milestone, labels);
        }

        public Task<IEnumerable<Issue>> GetIssues(string username, string reponame, State state = State.Open,
                                                  int page = 0)
        {
            return NGitHub<IEnumerable<Issue>, string, string, State, int>(client.Issues.GetIssuesAsync, username,
                                                                           reponame, state, page);
        }

        public Task<IEnumerable<User>> GetOrganisationUsers(string username, int page = 0)
        {
            return NGitHub<IEnumerable<User>, string, int>(client.Organizations.GetMembersAsync, username, page);
        }

        public Task<IEnumerable<Label>> GetLabels(string username, string reponame)
        {
            return NGitHub<IEnumerable<Label>, string, string>(client.Issues.GetLabelsAsync, username,
                                                                           reponame);
        }

        public Task<IEnumerable<Milestone>> GetMilestones(string username, string reponame)
        {
            return NGitHub<IEnumerable<Milestone>, string, string>(client.Issues.GetMilestonesAsync, username,
                                                                           reponame);
        }

        public Task<IEnumerable<User>> GetCollaborators(string username, string reponame)
        {
            return NGitHub<IEnumerable<User>, string, string>(client.Repositories.GetRepositoryCollaboratorsAsync, username,
                                                                           reponame);
        }


        private Task<T> NGitHub<T>(Func<Action<T>, Action<Exception>, GitHubRequestAsyncHandle> call)
        {
            var completionSource = new TaskCompletionSource<T>();
            call(completionSource.SetResult, completionSource.SetException);
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