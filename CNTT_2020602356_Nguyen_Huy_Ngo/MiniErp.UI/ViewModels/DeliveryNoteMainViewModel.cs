using Microsoft.EntityFrameworkCore;
using MiniErp.Core.Repositories;
using MiniErp.Core.UnitOfWorks;
using MiniErp.DataAccess.Models;
using MiniErp.Domain;
using MiniErp.UI.DependencyInjection;
using MiniErp.UI.Models;
using MiniErp.UI.Stores;
using MiniErp.UI.ViewModels.Abstract;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MiniErp.UI.ViewModels
{
    public class DeliveryNoteMainViewModel : BaseViewModel
    {
        private readonly MainContentStore _mainContentStore;
        private ObservableCollection<DeliveryNoteMainModel> _list = new ObservableCollection<DeliveryNoteMainModel>();
        public ObservableCollection<DeliveryNoteMainModel> List { get => _list; set { _list = value; OnPropertyChanged(); } }
        private ObservableCollection<Customer> _providerList = new ObservableCollection<Customer>();
        public ObservableCollection<Customer> CustomerList { get => _providerList; set { _providerList = value; OnPropertyChanged(); } }
        private DeliveryNoteMainModel _selectedItem;
        public DeliveryNoteMainModel SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                OnPropertyChanged();
            }
        }
        private Customer _customer;
        public Customer Customer { get => _customer; set { _customer = value; OnPropertyChanged(); } }
        private string _code;
        public string Code { get => _code; set { _code = value; OnPropertyChanged(); } }
        private DateTime _date = DateTime.Today;
        public DateTime Date { get => _date; set { _date = value; OnPropertyChanged(); } }
        private string _orderCode;
        public string OrderCode { get => _orderCode; set { _orderCode = value; OnPropertyChanged(); } }


        public ICommand AddCommand { get; set; }
        public ICommand UpdateCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand LoadCommand { get; set; }
        public ICommand PrintCommand { get; set; }
        private IRepository<DeliveryNote> _repository;
        private IRepository<Customer> _customerRepository;
        private IUnitOfWork _unitOfWork;

        public DeliveryNoteMainViewModel(IRepository<DeliveryNote> repository, 
            IRepository<Customer> customerRepository, 
            IUnitOfWork unitOfWork, 
            LoggedInUser loggedInUser,
            MainContentStore mainContentStore)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _customerRepository = customerRepository;
            _mainContentStore = mainContentStore;
            CurrentUser = loggedInUser;
            LoadSource();
            LoadData();

            LoadCommand = new RelayCommand<object>(p => true, p =>
            {
                LoadData();
            });

            AddCommand = new RelayCommand<object>(p => true, p =>
            {
                var vm = IoC.Resolve<DeliveryNoteViewModel>();
                _mainContentStore.CurrentViewModel = vm;
            });

            UpdateCommand = new RelayCommand<object>(p => SelectedItem is not null, async p =>
            {
                if (SelectedItem != null)
                {

                    var entity = await _repository.AsQueryable().Include(x => x.Customer)
                                                .Include(x => x.Details).ThenInclude(x => x.Product)
                                                .Include(x => x.Details).ThenInclude(x => x.Currency)
                                                .FirstOrDefaultAsync(x => x.Id == SelectedItem.Id);
                    if (entity == null)
                        return;
                    var vm = IoC.Resolve<DeliveryNoteViewModel>();
                    vm.Data = entity;
                    _mainContentStore.CurrentViewModel = vm;
                }
            });

            DeleteCommand = new RelayCommand<object>(p => SelectedItem is not null && !CurrentUser.IsUser, async p =>
            {
                if (SelectedItem != null && System.Windows.MessageBox.Show("Bạn có chắc chắn muốn xóa phiếu?", "Xác nhận xóa", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    var entity = await _repository.AsQueryable().FirstOrDefaultAsync(x => x.Id == SelectedItem.Id);
                    if (entity == null)
                    {
                        LoadData();
                        return;
                    }
                    await DeleteDeliveryNoteAsync(entity);
                    LoadData();
                }
            });

            PrintCommand = new RelayCommand<object>(p => SelectedItem != null, async p =>
            {
                var entity = await _repository.AsQueryable().Include(x => x.Details).ThenInclude(x => x.Currency)
                                                            .Include(x => x.Details).ThenInclude(x => x.Product)
                                                            .Include(x => x.Details).ThenInclude(x => x.Unit)
                                                            .FirstOrDefaultAsync(x => x.Id == SelectedItem.Id);
                if (entity == null)
                {
                    LoadData();
                    return;
                }
                var lst = entity.Details.Select(DeliveryNoteReportModel.CreateModel).ToList();
                int stt = 1;
                lst.ForEach(x => x.STT = stt++);
                var @params = new Dictionary<string, object>();
                @params.Add("NoteNumber", entity.Code);
                var vm = new ReportViewerViewModel();
                vm.ReportPath = @"Reports\PhieuXuatKho.rdlc";
                vm.Data = lst;
                vm.Params = @params;
                _mainContentStore.CurrentViewModel = vm;
            });
        }

        public void LoadData()
        {
            var query = _repository.AsQueryable();
            if (Customer != null)
                query = query.Where(x => x.CustomerId == Customer.Id);
            if (!string.IsNullOrWhiteSpace(Code))
                query = query.Where(x => x.Code.Contains(Code));
            if (!string.IsNullOrWhiteSpace(OrderCode))
                query = query.Where(x => x.OrderCode.Contains(OrderCode));

            List = new ObservableCollection<DeliveryNoteMainModel>(query
                .Include(x => x.Details).ThenInclude(x => x.Currency)
                .Include(x => x.Customer)
                .Select(DeliveryNoteMainModel.CreateModel).ToList());
        }

        private void LoadSource()
        {
            CustomerList = new ObservableCollection<Customer>(_customerRepository.AsQueryable().ToList());
        }

        public async Task DeleteDeliveryNoteAsync(DeliveryNote deliveryNote)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                await _repository.DeleteAsync(deliveryNote);
                await _unitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
            }
        }
    }
}
