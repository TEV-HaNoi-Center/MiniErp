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
    public class CurrencyViewModel : BaseViewModel
    {
        private ObservableCollection<Currency> _list = new ObservableCollection<Currency>();
        public ObservableCollection<Currency> List { get => _list; set { _list = value; OnPropertyChanged(); } }
        private Currency _selectedItem;
        public Currency SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                if (_selectedItem != null)
                {
                    DisplayName = _selectedItem.Name;
                    Code = _selectedItem.Code;
                    ExchangeRate = _selectedItem.ExchangeRate.ToString();
                }
                OnPropertyChanged();
            }
        }
        private string _displayName;
        public string DisplayName { get => _displayName; set { _displayName = value; OnPropertyChanged(); } }
        private string _code;
        public string Code { get => _code; set { _code = value; OnPropertyChanged(); } }
        private string _exchangeRate = "0";
        public string ExchangeRate { get => _exchangeRate; set { _exchangeRate = value; OnPropertyChanged(); } }


        public ICommand AddCommand { get; set; }
        public ICommand UpdateCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        private IRepository<Currency> _repository;
        private IUnitOfWork _unitOfWork;

        public CurrencyViewModel(IRepository<Currency> repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;


            LoadData();
            AddCommand = new RelayCommand<object>((p) =>
            {
                if (string.IsNullOrEmpty(DisplayName))
                    return false;

                if (_repository.AsQueryable().Any(x => x.Name == DisplayName))
                    return false;

                return true;

            }, async (p) =>
            {
                var currency = new Currency() { Name = DisplayName, ExchangeRate = double.Parse(ExchangeRate), Code = Code };

                await _unitOfWork.BeginTransactionAsync();
                try
                {
                    currency = await _repository.AddAsync(currency);
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
                if (SelectedItem == null)
                    return false;

                if (!_repository.AsQueryable().Any(x => x.Id == SelectedItem.Id))
                    return false;

                return true;

            }, async (p) =>
            {
                await _unitOfWork.BeginTransactionAsync();
                try
                {
                    var currency = await _repository.AsQueryable().FirstOrDefaultAsync(x => x.Id == SelectedItem.Id);
                    currency.Name = DisplayName;
                    currency.ExchangeRate = double.Parse(ExchangeRate);
                    currency.Code = Code;
                    await _repository.UpdateAsync(currency);
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
                    var currency = await _repository.AsQueryable().FirstOrDefaultAsync(x => x.Id == SelectedItem.Id);
                    currency.IsDelete = true;
                    await _repository.UpdateAsync(currency);
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
            List = new ObservableCollection<Currency>(_repository.AsQueryable().Where(x => !x.IsDelete).ToList());
        }
    }
}
