using Microsoft.Reporting.WinForms;
using System.Data;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MiniErp.ReportDesigner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("Name", typeof(string)));
            dt.Columns.Add(new DataColumn("Amount", typeof(int)));
            DataRow dr = dt.NewRow();
            dr["Name"] = "CK Nitin";
            dr["Amount"] = 2000;
            dt.Rows.Add(dr);

            ReportDataSource reportDataSource = new ReportDataSource();
            reportDataSource.Name = "DataSet1"; // Tên của DataSet mà chúng ta vừa tạo ra ở trên kia
            reportDataSource.Value = dt;
            reportViewer.LocalReport.ReportPath = "Report1.rdlc"; // đường dẫn tới tệp .rdlc mà chúng ta vừa tạo ra trên kia
            reportViewer.LocalReport.DataSources.Add(reportDataSource);
            reportViewer.RefreshReport();
        }
    }
}