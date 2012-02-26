using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Threading;
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
        public TasksViewModel(IGithubRepository githubRepository)
        {
            GithubRepository = githubRepository;

            Users = new ObservableCollection<User>();
            Milestones = new ObservableCollection<Milestone>();
            Labels = new ObservableCollection<SelectionLabel>();
            Projects = new ObservableCollection<Repository>();
        }

        public User User { get; set; }

        private ObservableCollection<Repository> projects;
        public ObservableCollection<Repository> Projects
        {
            get { return projects; }
            set { projects = value; }
        }

        public ObservableCollection<User> Users { get; set; }

        public ObservableCollection<Milestone> Milestones { get; set; }

        public ObservableCollection<SelectionLabel> Labels { get; set; }

        public IGithubRepository GithubRepository { get; private set; }

        public void Login()
        {
            if (User == null)
            {
                GithubRepository.GetUser()
                    .ContinueWith<IEnumerable<Repository>>(GetProjectsForUser)
                    .ContinueWith(AssignProjects);
            }
            else
            {
                GithubRepository.GetProjects(User.Login)
                    .ContinueWith(AssignProjects);
            }
        }

        private IEnumerable<Repository> GetProjectsForUser(Task<User> task)
        {
            if (task.Exception == null)
                User = task.Result;

            if (User == null)
                return Enumerable.Empty<Repository>();

            return GithubRepository.GetProjects(User.Login).Result;
        }

        private void AssignProjects(Task<IEnumerable<Repository>> task)
        {
            if (task.Exception == null)
            {
                Dispatcher.CurrentDispatcher
                    .Invoke((Action)(() => PopulateCollections(task.Result)),
                                           DispatcherPriority.Background);
            }
        }

        private void PopulateCollections(IEnumerable<Repository> items)
        {
            Projects.Clear();
            foreach (var project in items) // TODO: .Where(p => p.HasIssues)
            {
                Projects.Add(project);
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
            get { return organisationUsers ?? (organisationUsers = GetOrganisationUsers()); }
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

            try
            {
                var result = GithubRepository.CreateIssue(User.Login, SelectedProject.Name, Title, Body, assigned, milestone, selectedLabels).Result;
                return ValidationResult<Issue>.Success.WithData(result);
            }
            catch (Exception ex)
            {
                return ValidationResult<Issue>.Failure("Error Uploading Issue: " + ex.Message);
            }
        }
    }
}