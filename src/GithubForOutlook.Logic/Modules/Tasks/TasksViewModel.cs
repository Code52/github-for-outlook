using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using GithubForOutlook.Logic.Models;
using GithubForOutlook.Logic.Repositories.Interfaces;
using NGitHub.Models;
using VSTOContrib.Core.Wpf;
using Exception = System.Exception;
using User = NGitHub.Models.User;
using Repository = NGitHub.Models.Repository;

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

        public void Login()
        {
            var task = githubRepository.GetUser();
            task.ContinueWith<IEnumerable<Repository>>(GetProjectsForUser)
                .ContinueWith(AssignProjects);
        }

        private IEnumerable<Repository> GetProjectsForUser(Task<User> task)
        {
            if (task.Exception == null)
                User = task.Result;

            return githubRepository.GetProjects(User.Login).Result;
        }

        private void AssignProjects(Task<IEnumerable<Repository>> task)
        {
            Projects = task.Exception == null
                        ? new ObservableCollection<Repository>(task.Result.Where(p => p.HasIssues))
                        : new ObservableCollection<Repository>();
        }

        public User User { get; set; }

        private ObservableCollection<Repository> projects;
        public ObservableCollection<Repository> Projects
        {
            get { return projects; }
            set
            {
                projects = value;
                RaisePropertyChanged(() => Projects);
            }
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


        private ObservableCollection<User> users = new ObservableCollection<User>();
        public ObservableCollection<User> Users
        {
            get { return users; }
            set
            {
                users = value;
                RaisePropertyChanged(() => Users);
            }
        }

        private ObservableCollection<Milestone> milestones = new ObservableCollection<Milestone>();
        public ObservableCollection<Milestone> Milestones
        {
            get { return milestones; }
            set
            {
                milestones = value;
                RaisePropertyChanged(() => Milestones);
            }
        }

        public void GetOrganisationUsers(Repository repository)
        {
            Users.Clear();
            Users.Add(new User { Login = "No User", Name = "No User" });

            if (repository.Owner.IsOrganization)
            {
                GithubRepository
                    .GetOrganisationUsers(repository.Owner.Login)
                    .ContinueWith(t =>
                                      {
                                          if (t.Exception != null) return;
                                          foreach (var u in t.Result)
                                          {
                                              Users.Add(u);
                                          }
                                      });
            }
            else
                Users.Add(repository.Owner);
        }

        public void GetMilestonesFor(Repository repository)
        {
            Milestones.Clear();
            Milestones.Add(new Milestone { Title = "No Milestone" });

            if (User == null || repository == null) return;

            if (repository.HasIssues)
            {
                GithubRepository
                .GetMilestones(repository.Owner.Login, repository.Name)
                .ContinueWith(t =>
                {
                    if (t.Exception != null) return;
                    foreach (var milestone in t.Result)
                    {
                        Milestones.Add(milestone);
                    }
                });
            }
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

        private ObservableCollection<SelectionLabel> labels = new ObservableCollection<SelectionLabel>();
        public ObservableCollection<SelectionLabel> Labels
        {
            get { return labels; }
            set { labels = value; }
        }

        public void SetLabels()
        {
            GetLabels(SelectedProject);
        }

        public void GetLabels(Repository repository)
        {
            Labels.Clear();

            if (User == null || repository == null) return;

            if (repository.HasIssues)
            {
                GithubRepository.GetLabels(repository.Owner.Login, repository.Name)
                    .ContinueWith(t =>
                                    {
                                        foreach (var label in t.Result.Select(s => new SelectionLabel
                                                                {
                                                                    IsChecked = false,
                                                                    Color = s.Color,
                                                                    Name = s.Name,
                                                                    Url = s.Url
                                                                }))
                                        {
                                            Labels.Add(label);
                                        }
                                    });
            }
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