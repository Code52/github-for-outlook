using VSTOContrib.Core.Wpf;

namespace GithubForOutlook.Logic.Models
{
    public class User : OfficeViewModelBase
    {
        private string name;
        private string icon;

        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                RaisePropertyChanged(() => Name);
            }
        }

        public string Icon
        {
            get { return icon; }
            set
            {
                icon = value;
                RaisePropertyChanged(() => Icon);
            }
        }
    }
}
