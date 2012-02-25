using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using VSTOContrib.Core.Wpf;

namespace GithubForOutlook.Logic.Models
{
    [Serializable]
    public class ApplicationSettings : OfficeViewModelBase
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
