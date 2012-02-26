using VSTOContrib.Core.Wpf;

namespace GithubForOutlook.Logic.Models
{
    public class SelectionLabel: OfficeViewModelBase
    {
        public bool IsChecked { get; set; }
        public string Color { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
    }
}
