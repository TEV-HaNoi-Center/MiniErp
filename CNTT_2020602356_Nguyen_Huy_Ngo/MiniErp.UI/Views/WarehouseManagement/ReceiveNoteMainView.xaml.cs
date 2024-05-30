using Microsoft.EntityFrameworkCore;
using MiniErp.Core.Repositories;
using MiniErp.Domain;
using MiniErp.UI.Models;
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
    /// Interaction logic for ReceiveNoteView.xaml
    /// </summary>
    public partial class ReceiveNoteMainView : System.Windows.Controls.UserControl
    {
        public ReceiveNoteMainView()
        {
            InitializeComponent();
            //_repository = repository;
            //DataContext = vm;
            //vm.AddCommand = new RelayCommand<object>(p => true, async p =>
            //{
            //    AddReceiveNoteWindow wd = new AddReceiveNoteWindow();
            //    var result = wd.ShowDialog();
            //    if (result == false)
            //        return;

            //    var vm = (ReceiveNoteMainViewModel)DataContext;
            //    await vm.AddReceiveNoteAsync(wd.ReceiveNote);
            //    vm.LoadData();
            //});
            //vm.UpdateCommand = new RelayCommand<object>(p => true, async p =>
            //{
            //    if (vm.SelectedItem != null)
            //    {
            //        var vm = (ReceiveNoteMainViewModel)DataContext;
            //        var entity = await _repository.AsQueryable().FirstOrDefaultAsync(x => x.Id == vm.SelectedItem.Id);
            //        if (entity == null)
            //            return;
            //        AddReceiveNoteWindow wd = new AddReceiveNoteWindow(entity);
            //        var result = wd.ShowDialog();
            //        if (result != false)
            //        {
            //            await vm.UpdateReceiveNoteAsync(wd.ReceiveNote);
            //            vm.LoadData();
            //        }
            //    }
            //});
            //vm.DeleteCommand = new RelayCommand<object>(p => true, async p =>
            //{
            //    if (vm.SelectedItem != null)
            //    {
            //        var vm = (ReceiveNoteMainViewModel)DataContext;
            //        var entity = await _repository.AsQueryable().FirstOrDefaultAsync(x => x.Id == vm.SelectedItem.Id);
            //        if (entity == null)
            //            return;

            //        await vm.DeleteReceiveNoteAsync(entity);
            //        vm.LoadData();
            //    }
            //});
            //vm.LoadCommand = new RelayCommand<object>(p => true, async p =>
            //{
            //    vm.LoadData();
            //});
        }
    }
}
