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
    public class SelectReceiveProductViewModel : BaseViewModel
    {
        private readonly MainContentStore _mainContentStore;
        private ObservableCollection<Currency> _listCurrency = new ObservableCollection<Currency>();
        public ObservableCollection<Currency> ListCurrency { get => _listCurrency; set { _listCurrency = value; OnPropertyChanged(); } }
        private ObservableCollection<Unit> _listUnit = new ObservableCollection<Unit>();
        public ObservableCollection<Unit> ListUnit { get => _listUnit; set { _listUnit = value; OnPropertyChanged(); } }
        private ObservableCollection<Product> _listProduct = new ObservableCollection<Product>();
        public ObservableCollection<Product> ListProduct { get => _listProduct; set { _listProduct = value; OnPropertyChanged(); } }
        private Product _selectedProduct;
        public Product SelectedProduct
        {
            get => _selectedProduct;
            set
            {
                _selectedProduct = value;
                DisplayName = _selectedProduct?.Name;
                Code = _selectedProduct?.Code;
                if (value != null)
                    SelectedUnit = _selectedProduct.Unit;
                OnPropertyChanged();
            }
        }
        private Unit _selectedUnit;
        public Unit SelectedUnit { get => _selectedUnit; set { _selectedUnit = value; OnPropertyChanged(); } }
        private Currency _selectedCurrency;
        public Currency SelectedCurrency { get => _selectedCurrency; set { _selectedCurrency = value; ExchangeRate = value?.ExchangeRate.ToString(); OnPropertyChanged(); } }
        private int _quantity;
        public int Quantity { get => _quantity; set { _quantity = value; Total = CalculateTotal(); OnPropertyChanged(); } }

        private string _filter;
        public string Filter { get => _filter; set { _filter = value; LoadProduct(); OnPropertyChanged(); } }
        private string _displayName;
        public string DisplayName { get => _displayName; set { _displayName = value; OnPropertyChanged(); } }
        private string _code;
        public string Code { get => _code; set { _code = value; OnPropertyChanged(); } }
        private string _exchangeRate = "0";
        public string ExchangeRate { get => _exchangeRate; set { _exchangeRate = value; Total = CalculateTotal(); OnPropertyChanged(); } }
        private string _price;
        public string Price { get => _price; set { _price = value; Total = CalculateTotal(); OnPropertyChanged(); } }
        private double _total;
        public double Total { get => _total; set { _total = value; OnPropertyChanged(); } }
        private string _description;
        public string Description { get => _description; set { _description = value; OnPropertyChanged(); } }

        public ReceiveNoteDetail Detail { get; set; } = new ReceiveNoteDetail();
        public ICommand ConfirmCommand { get; set; }
        public ICommand CancelCommand { get; set; }
        private IRepository<Product> _productRepository;
        private IRepository<Unit> _unitRepository;
        private IRepository<Currency> _currencyRepository;
        private IEnumerable<Guid> _excludeIds;
        public IEnumerable<Guid> ExcludeIds { get => _excludeIds; set { _excludeIds = value; LoadProduct(); OnPropertyChanged(); } }

        public SelectReceiveProductViewModel(MainContentStore mainContentStore)
        {
            _mainContentStore = mainContentStore;
            _productRepository = IoC.ServiceProvider.GetRequiredService<IRepository<Product>>();
            _unitRepository = IoC.ServiceProvider.GetRequiredService<IRepository<Unit>>();
            _currencyRepository = IoC.ServiceProvider.GetRequiredService<IRepository<Currency>>();
            LoadData();
            LoadProduct();

            ConfirmCommand = new RelayCommand<object>(p => true, p =>
            {
                Detail.Note = Description;
                Detail.CurrencyId = SelectedCurrency.Id;
                Detail.Price = double.Parse(Price);
                Detail.Product = SelectedProduct;
                Detail.ProductId = SelectedProduct.Id;
                Detail.Quantity = Quantity;
                Detail.Currency = SelectedCurrency;
                Detail.UnitId = SelectedUnit.Id;
                var vm = IoC.Scope.ServiceProvider.GetRequiredService<ReceiveNoteViewModel>();
                vm.Data.Details.Add(Detail);
                vm.Details = new ObservableCollection<ReceiveNoteDetailModel>(vm.Data.Details.Select(ReceiveNoteDetailModel.CreateModel));
                _mainContentStore.CurrentViewModel = vm;
            });

            CancelCommand = new RelayCommand<object>(p => true, p =>
            {
                _mainContentStore.CurrentViewModel = IoC.ServiceProvider.GetRequiredService<ReceiveNoteViewModel>();
            });
        }

        private double CalculateTotal()
        {
            return double.Parse(string.IsNullOrEmpty(_price) ? "0" : _price) * double.Parse(string.IsNullOrEmpty(_exchangeRate) ? "0" : _exchangeRate) * _quantity;
        }
        private void LoadData()
        {
            ListCurrency = new ObservableCollection<Currency>(_currencyRepository.AsQueryable().ToList());
            ListUnit = new ObservableCollection<Unit>(_unitRepository.AsQueryable().ToList());
        }

        private void LoadProduct()
        {
            var query = _productRepository.AsQueryable();
            query = query.Include(x=>x.Unit).Where(x => !_excludeIds.Contains(x.Id));
            if (!string.IsNullOrWhiteSpace(Filter))
            {
                query = query.Where(x => x.Name.Contains(Filter) || x.Code.Contains(Filter));
            }
            ListProduct = new ObservableCollection<Product>(query.ToList());
        }
    }
}
