using Microsoft.EntityFrameworkCore;
using MiniErp.Core.Repositories;
using MiniErp.Domain;
using MiniErp.UI.ViewModels;
using MiniErp.UI.ViewModels.Abstract;
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

namespace MiniErp.UI.Views.WarehouseManagement
{
    /// <summary>
    /// Interaction logic for DeliveryNoteMainView.xaml
    /// </summary>
    public partial class DeliveryNoteMainView : System.Windows.Controls.UserControl
    {
        public DeliveryNoteMainView()
        {
            InitializeComponent();
            //_repository = repository;
            //vm.AddCommand = new RelayCommand<object>(p => true, async p =>
            //{
            //    DeliveryNoteWindow wd = new DeliveryNoteWindow();
            //    var result = wd.ShowDialog();
            //    if (result == false)
            //        return;

            //    var vm = (DeliveryNoteMainViewModel)DataContext;
            //    await vm.AddDeliveryNoteAsync(wd.DeliveryNote);
            //    vm.LoadData();
            //});
            //vm.UpdateCommand = new RelayCommand<object>(p => true, async p =>
            //{
            //    if (vm.SelectedItem != null)
            //    {
            //        var vm = (DeliveryNoteMainViewModel)DataContext;
            //        var entity = await _repository.AsQueryable().FirstOrDefaultAsync(x => x.Id == vm.SelectedItem.Id);
            //        if (entity == null)
            //            return;
            //        DeliveryNoteWindow wd = new DeliveryNoteWindow(entity);
            //        var result = wd.ShowDialog();
            //        if (result != false)
            //        {
            //            await vm.UpdateDeliveryNoteAsync(wd.DeliveryNote);
            //            vm.LoadData();
            //        }
            //    }
            //});
            //vm.DeleteCommand = new RelayCommand<object>(p => true, async p =>
            //{
            //    if (vm.SelectedItem != null)
            //    {
            //        var vm = (DeliveryNoteMainViewModel)DataContext;
            //        var entity = await _repository.AsQueryable().FirstOrDefaultAsync(x => x.Id == vm.SelectedItem.Id);
            //        if (entity == null)
            //            return;

            //        await vm.DeleteDeliveryNoteAsync(entity);
            //        vm.LoadData();
            //    }
            //});
            //vm.LoadCommand = new RelayCommand<object>(p => true, async p =>
            //{
            //    vm.LoadData();
            //});
            //DataContext = vm;
        }
    }
}
