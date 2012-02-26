using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Threading;
using GithubForOutlook.Logic.Models;
using GithubForOutlook.Logic.Repositories.Interfaces;
using Microsoft.Office.Interop.Outlook;
using VSTOContrib.Core.Extensions;
using VSTOContrib.Outlook.Extensions.Proxies;
using VSTOContrib.Outlook.Extensions.Proxies.Interfaces;


namespace GithubForOutlook.Logic.Repositories
{
    public class OutlookDispatchingRepository : IOutlookRepository
    {
        private readonly Dispatcher _outlookStaDispatcher;
        private readonly NameSpace _session;
        private readonly ApplicationSettings _settings;

        public OutlookDispatchingRepository(NameSpace session, ApplicationSettings settings)
        {
            _session = session;
            _settings = settings;
            if(System.Windows.Application.Current != null)
                _outlookStaDispatcher = System.Windows.Application.Current.Dispatcher;

            //Verify user properties are set
            using (var tasks = _session.GetDefaultFolder(OlDefaultFolders.olFolderTasks).WithComCleanupProxy())
            using (var userProperties = tasks.UserDefinedProperties.WithComCleanupProxy())
            using (var githubTaskIdProperty = userProperties.Find(GithubTaskAdapter.Githubissueid).WithComCleanupProxy())
            {
                if (githubTaskIdProperty == null)
                    userProperties.Add(GithubTaskAdapter.Githubissueid, OlUserPropertyType.olText).ReleaseComObject();
            }
        }


        public IList<GithubIssue> GetIssues()
        {
            return _outlookStaDispatcher.Invoke(new Func<IList<GithubIssue>>(() =>
            {
                var tasks = new List<GithubIssue>();
                using (var otasks = _session.GetDefaultFolder(OlDefaultFolders.olFolderTasks).WithComCleanupProxy())
                using (var items = otasks.Items.WithComCleanupProxy())
                {
                    tasks.AddRange(items.ComLinq<TaskItem>()
                                    .Select(task => new GithubTaskAdapter(task))
                                    .Select(t => new GithubIssue
                                    {
                                        TaskId = t.TaskId,
                                        Title = t.Title,
                                        Body = t.Body,
                                        LastModified = t.LastModified
                                    }));
                }

                return tasks;
            })) as IList<GithubIssue>;
        }

        public IList<Models.GithubIssue> GetModifiedIssues(DateTime since)
        {
            return _outlookStaDispatcher.Invoke(new Func<IList<GithubIssue>>(() =>
            {
                var tasks = new List<GithubIssue>();
                using (var otasks = _session.GetDefaultFolder(OlDefaultFolders.olFolderTasks).WithComCleanupProxy())
                using (var items = otasks.Items.WithComCleanupProxy())
                {
                    var lastModStr = since.ToString("d/MM/yyy h:mmtt");

                    var restrictedItems = items.Restrict("[LastModificationTime] > '" + lastModStr + "'");

                    using (var modifiedItems = restrictedItems.WithComCleanupProxy())
                    {

                        tasks.AddRange(modifiedItems.ComLinq<TaskItem>()
                                           .Select(task => new GithubTaskAdapter(task))
                                           .Select(t => new GithubIssue
                                           {
                                               TaskId = t.TaskId,
                                               Title = t.Title,
                                               Body = t.Body,
                                               LastModified = t.LastModified
                                           }));
                    }
                }

                return tasks;
            })) as IList<GithubIssue>;
        }

        public void SaveIssue(Models.GithubIssue issue)
        {
            _outlookStaDispatcher.Invoke(new System.Action(() =>
                    {
                        using (var tasks = _session.GetDefaultFolder(OlDefaultFolders.olFolderTasks).WithComCleanupProxy())
                        using (var items = tasks.Items.WithComCleanupProxy())
                        {
                            var taskItem = _session.GetItemFromID(issue.TaskId, tasks.StoreID) as TaskItem;
                            using (var outlookTaskItem = taskItem.WithComCleanupProxy())
                            {
                                CreateOrUpdateTask(issue, outlookTaskItem, items);
                            }
                        }
                    }));
        }

        private void CreateOrUpdateTask(GithubIssue issue, TaskItem outlookTaskItem, IItems items)
        {
            if (outlookTaskItem != null)
            {
                UpdateAdapter(issue, outlookTaskItem);
            }
            else using (var newItem = ((TaskItem)items.Add(OlItemType.olTaskItem)).WithComCleanupProxy())
            {
                UpdateAdapter(issue, newItem);
            }
        }

        private void UpdateAdapter(GithubIssue issue, TaskItem outlookTaskItem)
        {
            var itemAdapter = new GithubTaskAdapter(outlookTaskItem);
            itemAdapter.Title = issue.Title;
            itemAdapter.Body = issue.Body;

            if (!outlookTaskItem.Saved)
                outlookTaskItem.Save();
        }

        public void SaveIssues(IList<Models.GithubIssue> issues)
        {
            throw new NotImplementedException();
        }
    }
}
