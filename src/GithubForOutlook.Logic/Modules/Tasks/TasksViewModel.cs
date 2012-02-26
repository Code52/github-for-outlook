using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GithubForOutlook.Logic.Models;
using GithubForOutlook.Logic.Repositories.Interfaces;
using NGitHub.Models;
using VSTOContrib.Core.Wpf;
using Exception = System.Exception;
using User = NGitHub.Models.User;

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

        public User User { get; set; }

        public IEnumerable<Repository> GetProjects()
        {
            if (User == null) return new List<Repository>();

            return githubRepository.GetProjects(User.Login).Result.Where(p => p.HasIssues);
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
                var result = GithubRepository.GetOrganisationUsers(repository.Owner.Login).Result;

                list.AddRange(result);
            }
            else list.Add(repository.Owner);

            return list;
        }

        public IEnumerable<Label> GetLabels(Repository repository)
        {
            var list = new List<Label>();

            if (User == null || repository == null) return list;

            if (repository.HasIssues)
            {
                var result = GithubRepository.GetLabels(repository.Owner.Login, repository.Name).Result;

                list.AddRange(result);
            }

            return list;
        }

        public IEnumerable<Milestone> GetMilestones(Repository repository)
        {
            var list = new List<Milestone> { new Milestone { Title = "No Milestone" }};

            if (User == null || repository == null) return list;

            if (repository.HasIssues)
            {
                var result = GithubRepository.GetMilestones(repository.Owner.Login, repository.Name).Result;

                list.AddRange(result);
            }

            return list;
        }
       
        public Dictionary<User, IEnumerable<User>> GetOrganisationUsers()
        {
            var results = new Dictionary<User, IEnumerable<User>>();

            if (User == null) return results;

            var organisations = GithubRepository.GetOrganisations(User.Login).Result.ToList();
            organisations.Add(User);

            foreach (var repo in organisations)
            {
                var result = GithubRepository.GetOrganisationUsers(repo.Login).Result;
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

        public ObservableCollection<SelectionLabel> Labels { get; set; }

        public void SetLabels()
        {
            Labels = new ObservableCollection<SelectionLabel>(GetLabels(SelectedProject).Select(s => new SelectionLabel
                                                                {
                                                                  IsChecked = false,
                                                                  Color = s.Color,
                                                                  Name = s.Name,
                                                                  Url = s.Url
                                                                }));
        }

        public Repository SelectedProject { get; set; }

        public string Title { get; set; }

        public string Body { get; set; }

        public string Sender { get; set; }

        public DateTime ReceivedDate { get; set; }

        public User AssignedUser { get; set; }

        public Milestone SelectedMilestone { get; set; }

        public ValidationResult<Issue> CreateIssue()
        {
            if (User == null || SelectedProject == null || string.IsNullOrWhiteSpace(Title) || string.IsNullOrWhiteSpace(Body))
                return ValidationResult<Issue>.Failure("Please enter all details");

            string assigned;

            if (AssignedUser == null || AssignedUser.Login == "No User") assigned = null;
            else assigned = AssignedUser.Login;

            int? milestone;

            if (SelectedMilestone == null || SelectedMilestone.Title == "No Milestone") milestone = null;
            else milestone = SelectedMilestone.Number;

            string[] selectedLabels = Labels.Where(l => l.IsChecked).Select(l => l.Name).ToArray();

            var result = new Issue();

            try
            {
                result = githubRepository.CreateIssue(User.Login, SelectedProject.Name, Title, Body, assigned, milestone, selectedLabels).Result;
            }
            catch (Exception ex)
            {
                return ValidationResult<Issue>.Failure("Error Uploading Issue: " + ex.Message);
            }

            return ValidationResult<Issue>.Success.WithData(result);
        }
    }
}