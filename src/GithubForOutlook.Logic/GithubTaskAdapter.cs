using Microsoft.Office.Interop.Outlook;
using VSTOContrib.Outlook;

namespace GithubForOutlook.Logic
{
    public class GithubTaskAdapter
    {
        private const string Githubissueid = "GithubIssueId";
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
            set
            {
                isGithubTask = value;
                outlookTask.SetPropertyValue(Githubissueid, OlUserPropertyType.olInteger, value, true);
            }
        }

        private bool CheckIfGithubTask()
        {
            var issueId = outlookTask.GetPropertyValue(Githubissueid, OlUserPropertyType.olInteger, false, o=>(int?)o, null);

            return issueId != null;
        }
    }
}