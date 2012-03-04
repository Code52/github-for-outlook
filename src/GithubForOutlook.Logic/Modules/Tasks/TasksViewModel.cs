using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
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

        public ObservableCollection<Repository> Projects { get; set; }

        public ObservableCollection<User> Users { get; set; }

        public ObservableCollection<Milestone> Milestones { get; set; }

        public ObservableCollection<SelectionLabel> Labels { get; set; }

        public IGithubRepository GithubRepository { get; private set; }

        public void Login()
        {
            try
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
            catch
            {
                Projects = new ObservableCollection<Repository>();
            }
        }

        private IEnumerable<Repository> GetProjectsForUser(Task<User> task)
        {
            if (task.Exception == null)
                User = task.Result;

            if (User == null)
                return Enumerable.Empty<Repository>();

            var repos = new List<Repository>();

            var orgs = GithubRepository.GetOrganisations(User.Login).Result;
            var userRepos = GithubRepository.GetProjects(User.Login).Result;

            repos.AddRange(userRepos);

            foreach (var org in orgs)
            {
                var orgRepos = GithubRepository.GetProjects(org.Login).Result;
                repos.AddRange(orgRepos);
            }

            return repos;
        }

        private void AssignProjects(Task<IEnumerable<Repository>> task)
        {
            if (task.Exception == null)
            {
                // this is a bullshit fix
                ExecuteOnMainThread(() => PopulateCollections(task.Result));
            }
        }

        private void ExecuteOnMainThread(Action action)
        {
            Application.Current.Dispatcher.BeginInvoke(action, DispatcherPriority.Background);
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

            GithubRepository.GetCollaborators(repository.Owner.Login, repository.Name).ContinueWith(t =>
                                      {
                                          // this is a bullshit fix
                                          if (t.Exception != null) return;
                                          ExecuteOnMainThread(() => PopulateUsers(t.Result));
                                      });

            //if (repository.Owner.IsOrganization)
            //{
            //    GithubRepository
            //        .GetOrganisationUsers(repository.Owner.Login)
            //        .ContinueWith(t =>
            //                          {
            //                              // this is a bullshit fix
            //                              if (t.Exception != null) return;
            //                              ExecuteOnMainThread(() => PopulateUsers(t.Result));
            //                          });
            //}
            //else
            //{
            //    PopulateUsers(new[] { repository.Owner });
            //}
        }

        private void PopulateUsers(IEnumerable<User> result)
        {
            foreach (var u in result)
            {
                Users.Add(u);
            }
        }

        public void GetMilestonesFor(Repository repository)
        {
            if (User == null || repository == null) return;

            if (repository.HasIssues)
            {
                GithubRepository
                .GetMilestones(repository.Owner.Login, repository.Name)
                .ContinueWith(t =>
                {
                    if (t.Exception != null) return;
                    ExecuteOnMainThread(() => PopulateMilestones(t.Result));
                });
            }
        }

        private void PopulateMilestones(IEnumerable<Milestone> result)
        {
            Milestones.Clear();
            Milestones.Add(new Milestone { Title = "No Milestone" });

            foreach (var u in result)
            {
                Milestones.Add(u);
            }
        }
        private void PopulateLabels(IEnumerable<Label> result)
        {
            Labels.Clear();


            foreach (var label in result.Select(s => new SelectionLabel
            {
                IsChecked = false,
                Color = s.Color,
                Name = s.Name,
                Url = s.Url
            }))
            {
                Labels.Add(label);
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
                    .ContinueWith(t => ExecuteOnMainThread(() => PopulateLabels(t.Result)));
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