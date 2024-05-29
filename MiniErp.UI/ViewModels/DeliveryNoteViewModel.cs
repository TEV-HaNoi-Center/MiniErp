using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MiniErp.UI.ViewModels
{
    public class DeliveryNoteViewModel : BaseViewModel
    {
        private readonly MainContentStore _mainContentStore;
        private DeliveryNote _data;
        public DeliveryNote Data { get => _data; set { _data = value; LoadGrid(); OnPropertyChanged(); } }
        private Customer _customer;
        public Customer Customer
        {
            get => _customer;
            set
            {
                _customer = value;
                if (value != null)
                    Data.CustomerId = value.Id;
                OnPropertyChanged();
            }
        }
        private string _orderCode;
        public string OrderCode { get => _orderCode; set { _orderCode = value; Data.OrderCode = value; OnPropertyChanged(); } }
        private DateTime _date = DateTime.Today;
        public DateTime Date { get => _date; set { _date = value; Data.Date = value; OnPropertyChanged(); } }
        private string _code;
        public string Code { get => _code; set { _code = value; Data.Code = value; OnPropertyChanged(); } }
        private ObservableCollection<Customer> _listCustomer;
        public ObservableCollection<Customer> ListCustomer { get => _listCustomer; set { _listCustomer = value; OnPropertyChanged(); } }
        private ObservableCollection<DeliveryNoteDetailModel> _details;
        public ObservableCollection<DeliveryNoteDetailModel> Details { get => _details; set { _details = value; OnPropertyChanged(); } }
        private DeliveryNoteDetailModel _selectedItem;
        public DeliveryNoteDetailModel SelectedItem { get => _selectedItem; set { _selectedItem = value; OnPropertyChanged(); } }
        public ICommand AddCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand CancelCommand { get; set; }
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<DeliveryNote> _repository;
        private readonly IRepository<DeliveryNoteDetail> _detailRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeliveryNoteViewModel(MainContentStore mainContentStore, IRepository<Customer> customerRepository, IRepository<DeliveryNoteDetail> detailRepository, IRepository<DeliveryNote> repository, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _customerRepository = customerRepository;
            _detailRepository = detailRepository;
            _repository = repository;
            Data = new DeliveryNote();
            Date = DateTime.Now;
            _mainContentStore = mainContentStore;
            _customerRepository = IoC.ServiceProvider.GetRequiredService<IRepository<Customer>>();
            LoadDataSource();
            _unitOfWork.BeginTransactionAsync();
            Details = new ObservableCollection<DeliveryNoteDetailModel>(Data.Details.Select(DeliveryNoteDetailModel.CreateModel));
            AddCommand = new RelayCommand<object>(p => true, async p =>
            {
                var vm = IoC.Scope.ServiceProvider.GetRequiredService<SelectDeliveryProductViewModel>();
                vm.ExcludeIds = Details.Select(x => x.ProductId).ToList();
                _mainContentStore.CurrentViewModel = vm;
            });

            DeleteCommand = new RelayCommand<object>(p => SelectedItem is not null, async p =>
            {
                var entityDetail = Data.Details.FirstOrDefault(x => x.ProductId == SelectedItem.ProductId);
                var entity = await _detailRepository.AsQueryable().FirstOrDefaultAsync(x => x.Id == entityDetail.Id);
                if (entity != null)
                    Data.Details.Remove(entityDetail);
                var detail = Details.FirstOrDefault(x => x.ProductId == SelectedItem.ProductId);
                Details.Remove(detail);
            });

            SaveCommand = new RelayCommand<object>(p => true, async p =>
            {
                if (Data.Id == Guid.Empty)
                {
                    await AddDeliveryNoteAsync(Data);
                }
                else
                {
                    await UpdateDeliveryNoteAsync(Data);
                }

                var vm = IoC.Resolve<DeliveryNoteMainViewModel>();
                _mainContentStore.CurrentViewModel = vm;
            });

            CancelCommand = new RelayCommand<object>(p => true, p =>
            {
                _mainContentStore.CurrentViewModel = IoC.Resolve<DeliveryNoteMainViewModel>();
            });
        }

        private void LoadDataSource()
        {
            ListCustomer = new ObservableCollection<Customer>(_customerRepository.AsQueryable().ToList());
        }

        private void LoadGrid()
        {
            Customer = Data.Customer;
            OrderCode = Data.OrderCode;
            Date = Data.Date;
            Code = Data.Code;
            Details = new ObservableCollection<DeliveryNoteDetailModel>(Data.Details.Select(DeliveryNoteDetailModel.CreateModel));
        }

        public async Task AddDeliveryNoteAsync(DeliveryNote deliveryNote)
        {
            try
            {
                foreach (var detail in deliveryNote.Details)
                {
                    detail.Currency = null;
                    detail.Product = null;
                    detail.Unit = null;
                    detail.Note = detail.Note ?? "";
                }
                deliveryNote.Code = await GetNewCode();
                await _repository.AddAsync(deliveryNote);
                await _unitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
            }
        }

        public async Task UpdateDeliveryNoteAsync(DeliveryNote deliveryNote)
        {
            try
            {
                var entity = await _repository.AsQueryable().FirstOrDefaultAsync(x => x.Id == deliveryNote.Id);
                if (entity != null)
                {
                    entity.CustomerId = deliveryNote.CustomerId;
                    entity.OrderCode = deliveryNote.OrderCode;
                    entity.Date = deliveryNote.Date;
                    UpdateDetails(deliveryNote, entity);
                    foreach (var detail in entity.Details)
                    {
                        detail.Currency = null;
                        detail.Product = null;
                        detail.Unit = null;
                        detail.Note = detail.Note ?? "";
                    }
                    await _repository.UpdateAsync(entity);
                    await _unitOfWork.CommitAsync();
                }
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
            }
        }

        private void UpdateDetails(DeliveryNote newEntity, DeliveryNote oldEntity)
        {
            oldEntity.Details = oldEntity.Details.Where(x => newEntity.Details.Select(y => y.Id).Contains(x.Id)).ToList();
            foreach (var detail in oldEntity.Details)
            {
                var newDetail = newEntity.Details.FirstOrDefault(x => x.Id == detail.Id);
                if (newDetail == null)
                    continue;

                detail.CurrencyId = newDetail.CurrencyId;
                detail.Price = newDetail.Price;
                detail.Quantity = newDetail.Quantity;
                detail.ProductId = newDetail.ProductId;
                detail.Note = newDetail.Note ?? "";
                detail.DeliveryNoteId = newDetail.DeliveryNoteId;
                detail.UnitId = newDetail.UnitId;
            }
            foreach (var newDetail in newEntity.Details.Where(x => !oldEntity.Details.Select(y => y.Id).Contains(x.Id)))
            {
                newDetail.Note = newDetail.Note ?? "";
                oldEntity.Details.Add(newDetail);
            }
            foreach (var oldDetail in oldEntity.Details.Where(x => !newEntity.Details.Select(y => y.Id).Contains(x.Id)))
            {
                oldEntity.Details.Remove(oldDetail);
            }
        }

        private async Task<string> GetNewCode()
        {
            var count = await _repository.AsQueryable().CountAsync();
            return $"PX{count + 1}";
        }
    }
}
