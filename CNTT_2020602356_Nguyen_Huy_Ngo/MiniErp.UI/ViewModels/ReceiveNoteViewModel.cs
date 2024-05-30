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
using MiniErp.UI.Views.WarehouseManagement;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MiniErp.UI.ViewModels
{
    public class ReceiveNoteViewModel : BaseViewModel
    {
        private readonly MainContentStore _mainContentStore;
        private ReceiveNote _data;
        public ReceiveNote Data { get => _data; set { _data = value; LoadGrid(); OnPropertyChanged(); } }
        private Provider _provider;
        public Provider Provider
        {
            get => _provider;
            set
            {
                _provider = value;
                if (value != null)
                    Data.ProviderId = value.Id;
                OnPropertyChanged();
            }
        }
        private string _orderCode;
        public string OrderCode { get => _orderCode; set { _orderCode = value; Data.OrderCode = value; OnPropertyChanged(); } }
        private DateTime _date = DateTime.Today;
        public DateTime Date { get => _date; set { _date = value; Data.Date = value; OnPropertyChanged(); } }
        private string _code;
        public string Code { get => _code; set { _code = value; Data.Code = value; OnPropertyChanged(); } }
        private ObservableCollection<Provider> _listProvider;
        public ObservableCollection<Provider> ListProvider { get => _listProvider; set { _listProvider = value; OnPropertyChanged(); } }
        private ObservableCollection<ReceiveNoteDetailModel> _details;
        public ObservableCollection<ReceiveNoteDetailModel> Details { get => _details; set { _details = value; OnPropertyChanged(); } }
        private ReceiveNoteDetailModel _selectedItem;
        public ReceiveNoteDetailModel SelectedItem { get => _selectedItem; set { _selectedItem = value; OnPropertyChanged(); } }
        public ICommand AddCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand CancelCommand { get; set; }
        private readonly IRepository<Provider> _providerRepository;
        private readonly IRepository<ReceiveNote> _repository;
        private readonly IRepository<ReceiveNoteDetail> _detailrepository;
        private readonly IUnitOfWork _unitOfWork;

        public ReceiveNoteViewModel(MainContentStore mainContentStore, IRepository<ReceiveNote> repository, IRepository<ReceiveNoteDetail> detailRepository, IUnitOfWork unitOfWork)
        {
            _mainContentStore = mainContentStore;
            _detailrepository = detailRepository;
            _unitOfWork = unitOfWork;
            _repository = repository;
            Data = new ReceiveNote();
            _providerRepository = IoC.ServiceProvider.GetRequiredService<IRepository<Provider>>();
            LoadDataSource();
            _unitOfWork.BeginTransactionAsync();
            Details = new ObservableCollection<ReceiveNoteDetailModel>(Data.Details.Select(ReceiveNoteDetailModel.CreateModel));
            AddCommand = new RelayCommand<object>(p => true, async p =>
            {
                var vm = IoC.Scope.ServiceProvider.GetRequiredService<SelectReceiveProductViewModel>();
                vm.ExcludeIds = Details.Select(x => x.ProductId).ToList();
                _mainContentStore.CurrentViewModel = vm;
            });

            DeleteCommand = new RelayCommand<object>(p => SelectedItem is not null, async p =>
            {
                var entityDetail = Data.Details.FirstOrDefault(x => x.ProductId == SelectedItem.ProductId);
                var entity = await _detailrepository.AsQueryable().FirstOrDefaultAsync(x => x.Id == entityDetail.Id);
                if (entity != null)
                    await _detailrepository.DeleteAsync(entity);
                Data.Details.Remove(entityDetail);
                var detail = Details.FirstOrDefault(x => x.ProductId == SelectedItem.ProductId);
                Details.Remove(detail);
            });

            SaveCommand = new RelayCommand<object>(p => true, async p =>
            {
                if (Data.Id == Guid.Empty)
                {
                    await AddReceiveNoteAsync(Data);
                }
                else
                {
                    await UpdateReceiveNoteAsync(Data);
                }

                var vm = IoC.Resolve<ReceiveNoteMainViewModel>();
                _mainContentStore.CurrentViewModel = vm;
            });

            CancelCommand = new RelayCommand<object>(p => true, p =>
            {
                _mainContentStore.CurrentViewModel = IoC.Resolve<ReceiveNoteMainViewModel>();
            });
        }

        private void LoadDataSource()
        {
            ListProvider = new ObservableCollection<Provider>(_providerRepository.AsQueryable().ToList());
        }

        private void LoadGrid()
        {
            Provider = Data.Provider;
            OrderCode = Data.OrderCode;
            Date = Data.Date;
            Code = Data.Code;
            Details = new ObservableCollection<ReceiveNoteDetailModel>(Data.Details.Select(ReceiveNoteDetailModel.CreateModel));
        }

        private async Task<string> GetNewCode()
        {
            var count = await _repository.AsQueryable().CountAsync();
            return $"PN{count + 1}";
        }

        private void UpdateDetails(ReceiveNote newEntity, ReceiveNote oldEntity)
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
                detail.ReceiveNoteId = newDetail.ReceiveNoteId;
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

        private async Task AddReceiveNoteAsync(ReceiveNote receiveNote)
        {
            try
            {
                foreach (var detail in receiveNote.Details)
                {
                    detail.Currency = null;
                    detail.Product = null;
                    detail.Unit = null;
                    detail.Note = detail.Note ?? "";
                }
                receiveNote.Code = await GetNewCode();
                await _repository.AddAsync(receiveNote);
                await _unitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
            }
        }

        private async Task UpdateReceiveNoteAsync(ReceiveNote receiveNote)
        {
            try
            {
                var entity = await _repository.AsQueryable().FirstOrDefaultAsync(x => x.Id == receiveNote.Id);
                if (entity != null)
                {
                    entity.ProviderId = receiveNote.ProviderId;
                    entity.OrderCode = receiveNote.OrderCode;
                    entity.Date = receiveNote.Date;
                    UpdateDetails(receiveNote, entity);
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
    }
}
