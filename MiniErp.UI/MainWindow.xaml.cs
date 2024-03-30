using MiniErp.UI.DependencyInjection;
using MiniErp.UI.ViewModels;
using MiniErp.UI.ViewModels.Abstract;
using MiniErp.UI.Views;
using MiniErp.UI.Views.Category;
using MiniErp.UI.Views.WarehouseManagement;
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

namespace MiniErp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
        public MainWindow(MainViewModel vm)
        {
            InitializeComponent();
            //vm.UnitCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
            //{
            //    UnitView view = IoC.Resolve<UnitView>();
            //    MainContent.Children.Clear();
            //    MainContent.Children.Add(view);
            //});
            //vm.CurrencyCommand = new RelayCommand<object>(p => { return true; }, p =>
            //{
            //    CurrencyView view = IoC.Resolve<CurrencyView>();
            //    MainContent.Children.Clear();
            //    MainContent.Children.Add(view);
            //});
            //vm.CustomerCommand = new RelayCommand<object>(p => true, p =>
            //{
            //    CustomerView view = IoC.Resolve<CustomerView>();
            //    MainContent.Children.Clear();
            //    MainContent.Children.Add(view);
            //});
            //vm.ProviderCommand = new RelayCommand<object>(p => true, p =>
            //{
            //    ProviderView view = IoC.Resolve<ProviderView>();
            //    MainContent.Children.Clear();
            //    MainContent.Children.Add(view);
            //});
            //vm.RoleCommand = new RelayCommand<object>(p => true, p =>
            //{
            //    RoleView view = IoC.Resolve<RoleView>();
            //    MainContent.Children.Clear();
            //    MainContent.Children.Add(view);
            //});
            //vm.ProductCommand = new RelayCommand<object>(p => true, p =>
            //{
            //    ProductView view = IoC.Resolve<ProductView>();
            //    MainContent.Children.Clear();
            //    MainContent.Children.Add(view);
            //});
            //vm.ReceiveNoteCommand = new RelayCommand<object>(p => true, p =>
            //{
            //    ReceiveNoteView view = IoC.Resolve<ReceiveNoteView>();
            //    MainContent.Children.Clear();
            //    MainContent.Children.Add(view);
            //});
            //vm.DeliveryNoteCommand = new RelayCommand<object>(p => true, p =>
            //{
            //    DeliveryNoteMainView view = IoC.Resolve<DeliveryNoteMainView>();
            //    MainContent.Children.Clear();
            //    MainContent.Children.Add(view);
            //});
            DataContext = vm;
        }
    }
}