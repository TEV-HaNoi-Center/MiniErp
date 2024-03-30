using MiniErp.UI.ViewModels.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniErp.UI.ViewModels
{
    public class ReportViewerViewModel : BaseViewModel
    {
        public object Data { get; set; }
        public string ReportPath { get; set; }
        public IDictionary<string, object> Params { get; set; } = new Dictionary<string, object>();
    }
}
