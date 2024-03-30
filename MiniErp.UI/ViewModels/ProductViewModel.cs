using Microsoft.EntityFrameworkCore;
using MiniErp.Core.Repositories;
using MiniErp.Core.UnitOfWorks;
using MiniErp.Domain;
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
    public class ProductViewModel : BaseViewModel
    {
        private ObservableCollection<Product> _list = new ObservableCollection<Product>();
        public ObservableCollection<Product> List { get => _list; set { _list = value; OnPropertyChanged(); } }
        private ObservableCollection<Unit> _listUnit = new ObservableCollection<Unit>();
        public ObservableCollection<Unit> ListUnit { get => _listUnit; set { _listUnit = value; OnPropertyChanged(); } }
        private Product _SelectedItem;
        public Product SelectedItem
        {
            get => _SelectedItem;
            set
            {
                _SelectedItem = value;
                if (_SelectedItem != null)
                {
                    DisplayName = _SelectedItem.Name;
                    Description = _SelectedItem.Description;
                    Code = _SelectedItem.Code;
                    Unit = ListUnit.FirstOrDefault(x => x.Id == _SelectedItem.UnitId);
                }
                OnPropertyChanged();
            }
        }
        private string _displayName;
        public string DisplayName { get => _displayName; set { _displayName = value; OnPropertyChanged(); } }
        private string _description;
        public string Description { get => _description; set { _description = value; OnPropertyChanged(); } }
        private string _code;
        public string Code { get => _code; set { _code = value; OnPropertyChanged(); } }
        private Unit _unit;
        public Unit Unit { get => _unit; set { _unit = value; OnPropertyChanged(); } }

        public ICommand AddCommand { get; set; }
        public ICommand UpdateCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        private IRepository<Product> _repository;
        private IRepository<Unit> _unitRepository;
        private IUnitOfWork _unitOfWork;

        public ProductViewModel(IRepository<Product> repository, IRepository<Unit> unitRepository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _unitRepository = unitRepository;


            LoadData();
            LoadDataSource();
            AddCommand = new RelayCommand<object>((p) =>
            {
                if (string.IsNullOrEmpty(DisplayName) || string.IsNullOrEmpty(Code))
                    return false;

                if (_repository.AsQueryable().Any(x => x.Code == Code))
                    return false;

                return true;

            }, async (p) =>
            {
                var product = new Product() { Name = DisplayName, Code = Code, Description = Description, UnitId = Unit.Id };

                await _unitOfWork.BeginTransactionAsync();
                try
                {
                    product = await _repository.AddAsync(product);
                    await _unitOfWork.CommitAsync();
                    LoadData();
                }
                catch (Exception ex)
                {
                    await _unitOfWork.RollbackAsync();
                }
            });

            UpdateCommand = new RelayCommand<object>((p) =>
            {
                if (SelectedItem == null || string.IsNullOrEmpty(DisplayName) || string.IsNullOrEmpty(Code))
                    return false;

                if (!_repository.AsQueryable().Any(x => x.Id == SelectedItem.Id))
                    return false;

                return true;

            }, async (p) =>
            {
                await _unitOfWork.BeginTransactionAsync();
                try
                {
                    var product = await _repository.AsQueryable().FirstOrDefaultAsync(x => x.Id == SelectedItem.Id);
                    product.Name = DisplayName;
                    product.Code = Code;
                    product.Description = Description;
                    product.UnitId = Unit.Id;

                    await _repository.UpdateAsync(product);
                    await _unitOfWork.CommitAsync();
                    LoadData();
                }
                catch (Exception ex)
                {
                    await _unitOfWork.RollbackAsync();
                }
            });

            DeleteCommand = new RelayCommand<object>((p) =>
            {
                if (SelectedItem == null)
                    return false;

                return true;

            }, async (p) =>
            {
                if (!_repository.AsQueryable().Any(x => x.Id == SelectedItem.Id))
                    return;

                await _unitOfWork.BeginTransactionAsync();
                try
                {
                    var product = await _repository.AsQueryable().FirstOrDefaultAsync(x => x.Id == SelectedItem.Id);
                    product.IsDeleted = true;
                    await _repository.UpdateAsync(product);
                    await _unitOfWork.CommitAsync();
                    LoadData();
                }
                catch (Exception ex)
                {
                    await _unitOfWork.RollbackAsync();
                }
            });
        }

        private void LoadData()
        {
            List = new ObservableCollection<Product>(_repository.AsQueryable().Include(x => x.Unit).ToList());
        }

        private void LoadDataSource()
        {
            ListUnit = new ObservableCollection<Unit>(_unitRepository.AsQueryable().ToList());
        }
    }
}
