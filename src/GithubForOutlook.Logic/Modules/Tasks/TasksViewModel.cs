
using System;
using System.Collections.Generic;
using System.Linq;
using GithubForOutlook.Logic.Repositories.Interfaces;
using Microsoft.Office.Interop.Outlook;
using NGitHub.Models;
using VSTOContrib.Core.Wpf;
using Exception = System.Exception;

namespace GithubForOutlook.Logic.Modules.Tasks
{
    public class TasksViewModel : OfficeViewModelBase
    {
        private readonly IGithubRepository githubRepository;

        public TasksViewModel(IGithubRepository githubRepository)
        {
            this.githubRepository = githubRepository;
        }

        public IGithubRepository GithubRepository
        {
            get { return githubRepository; }
        }

        public void Login(string username, string password)
        {
            githubRepository.Login(username, password);
            User = githubRepository.GetUser().Result;
        }

        public NGitHub.Models.User User { get; set; }

        public IEnumerable<Repository> GetProjects()
        {
            if (User == null) return new List<Repository>();

            return githubRepository.GetProjects(User).Result.Where(p => p.HasIssues);
        }

        private IEnumerable<Repository> projects;
        public IEnumerable<Repository> Projects
        {
            get
            {
                if (projects == null || !projects.Any())
                    projects = GetProjects();

                return projects;
            }
        }

        public IEnumerable<User> GetOrganisationUsers(Repository repository)
        {
            var list = new List<User> { new User { Login = "No User", Name = "No User" } };

            if (repository.Owner.IsOrganization)
            {
                var result = GithubRepository.GetOrganisationUsers(repository.Owner).Result;

                list.AddRange(result);
            }
            else list.Add(repository.Owner);

            return list;
        }

        public IEnumerable<Label> GetLabels(Repository repository)
        {
            var list = new List<Label>();

            if (repository.HasIssues)
            {
                var result = GithubRepository.GetLabels(User.Login, repository.Name).Result;

                list.AddRange(result);
            }

            return list;
        }

        public IEnumerable<Milestone> GetMilestones(Repository repository)
        {
            var list = new List<Milestone>();

            if (repository.HasIssues)
            {
                var result = GithubRepository.GetMilestones(User.Login, repository.Name).Result;

                list.AddRange(result);
            }

            return list;
        }
       
        public Dictionary<User, IEnumerable<User>> GetOrganisationUsers()
        {
            if (User == null) return new Dictionary<User, IEnumerable<User>>();

            var results = new Dictionary<User, IEnumerable<User>>();

            var organisations = GithubRepository.GetOrganisations(User.Login).Result.ToList();
            organisations.Add(User);

            foreach (var repo in organisations)
            {
                var result = GithubRepository.GetOrganisationUsers(repo).Result;
                results.Add(repo, result);
            }

            return null;
        }

        private Dictionary<User, IEnumerable<User>> organisationUsers;
        public Dictionary<User, IEnumerable<User>> OrganisationUsers
        {
            get
            {
                if (organisationUsers == null)
                    organisationUsers = GetOrganisationUsers();

                return organisationUsers;
            }
        }

        public Repository SelectedProject { get; set; }

        public string Title { get; set; }

        public string Body { get; set; }

        public string Sender { get; set; }

        public DateTime ReceivedDate { get; set; }

        public User AssignedUser { get; set; }

        public ValidationResult<Issue> CreateIssue()
        {
            if (User == null || SelectedProject == null || string.IsNullOrWhiteSpace(Title) || string.IsNullOrWhiteSpace(Body))
                return ValidationResult<Issue>.Failure("Please enter all details");

            string assigned;

            if (AssignedUser == null || AssignedUser.Login == "No User") assigned = null;
            else assigned = AssignedUser.Login;

            var result = new Issue();

            try
            {
                result = githubRepository.CreateIssue(User.Login, SelectedProject.Name, Title, Body, assigned, null, null).Result;
            }
            catch (Exception ex)
            {
                return ValidationResult<Issue>.Failure("Error Uploading Issue: " + ex.Message);
            }

            return ValidationResult<Issue>.Success.WithData(result);
        }
    }
}