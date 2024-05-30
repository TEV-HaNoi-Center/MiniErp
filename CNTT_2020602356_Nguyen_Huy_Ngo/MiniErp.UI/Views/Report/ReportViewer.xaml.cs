using Microsoft.Reporting.WinForms;
using MiniErp.DataAccess.Models;
using MiniErp.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MiniErp.UI.Views.Report
{
    /// <summary>
    /// Interaction logic for ReportViewer.xaml
    /// </summary>
    public partial class ReportViewer : System.Windows.Controls.UserControl
    {
        public ReportViewer()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as ReportViewerViewModel;
            if (vm is null)
                return;

            ReportDataSource reportDataSource = new ReportDataSource();
            reportDataSource.Name = "DataSet1"; // Tên của DataSet mà chúng ta vừa tạo ra ở trên kia
            reportDataSource.Value = vm.Data;
            List<ReportParameter> parameters = new List<ReportParameter>();
            foreach(var param in vm.Params)
            {
                parameters.Add(new ReportParameter(param.Key, param.Value.ToString()));
            }
            reportViewer.LocalReport.ReportPath = vm.ReportPath; // đường dẫn tới tệp .rdlc mà chúng ta vừa tạo ra trên kia
            reportViewer.LocalReport.DataSources.Add(reportDataSource);
            reportViewer.LocalReport.SetParameters(parameters);
            reportViewer.RefreshReport();
        }
    }
}
