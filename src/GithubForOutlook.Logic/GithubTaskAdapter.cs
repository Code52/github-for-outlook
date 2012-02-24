using System;
using Microsoft.Office.Interop.Outlook;
using VSTOContrib.Outlook;

namespace GithubForOutlook.Logic
{
    public class GithubTaskAdapter
    {
        public const string Githubissueid = "GithubIssueId";
        private readonly TaskItem outlookTask;
        private bool? isGithubTask;

        public GithubTaskAdapter(TaskItem outlookTask)
        {
            this.outlookTask = outlookTask;
        }

        public bool IsGithubTask
        {
            get
            {
                if (isGithubTask == null)
                {
                    isGithubTask = CheckIfGithubTask();
                }
                return isGithubTask.Value;
            }
        }

        private bool CheckIfGithubTask()
        {
            var issueId = outlookTask.GetPropertyValue(Githubissueid, OlUserPropertyType.olText, false, o=>string.IsNullOrWhiteSpace(o.ToString()), false);

            return issueId;
        }

        public string TaskId
        {
            get
            {
                return outlookTask.GetPropertyValue(Githubissueid, OlUserPropertyType.olText, false, (o) => o.ToString(), null);
            }
            set
            {
                outlookTask.SetPropertyValue(Githubissueid, OlUserPropertyType.olText, value.ToString(), true);
            }
        }

        public string Title
        {
            get { return outlookTask.Subject; }
            set { outlookTask.Subject = value; }
        }

        public string Body
        {
            get { return outlookTask.Body; }
            set { outlookTask.Body = value; }
        }

        public OlTaskStatus Status
        {
            get { return outlookTask.Status; }
            set { outlookTask.Status = value; }
        }

        public DateTime LastModified
        {
            get { return outlookTask.LastModificationTime; }
        }
    }
}