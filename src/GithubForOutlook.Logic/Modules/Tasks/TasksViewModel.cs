
using System.Collections.Generic;
using System.Linq;
using GithubForOutlook.Logic.Repositories.Interfaces;
using Microsoft.Office.Interop.Outlook;
using NGitHub.Models;
using VSTOContrib.Core.Wpf;

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

            return githubRepository.GetProjects(User).Result;
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

        public Repository SelectedProject { get; set; }

        public MailItem MailItem { get; set; }

        public void CreateIssue(string title, string body)
        {
            if (User == null || SelectedProject == null) return;

            var result = githubRepository.CreateIssue(User.Login, SelectedProject.Name, title, body, User.Login, null, null).Result;
        }
    }
}