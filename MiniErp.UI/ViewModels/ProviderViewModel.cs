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
    public class ProviderViewModel : BaseViewModel
    {
        private ObservableCollection<Provider> _list = new ObservableCollection<Provider>();
        public ObservableCollection<Provider> List { get => _list; set { _list = value; OnPropertyChanged(); } }
        private Customer _SelectedItem;
        public Customer SelectedItem
        {
            get => _SelectedItem;
            set
            {
                _SelectedItem = value;
                if (_SelectedItem != null)
                {
                    DisplayName = _SelectedItem.Name;
                    Address = _SelectedItem.Address;
                    Code = _SelectedItem.Code;
                    PhoneNumber = _SelectedItem.PhoneNumber;
                    Email = _SelectedItem.Email;
                }
                OnPropertyChanged();
            }
        }
        private string _displayName;
        public string DisplayName { get => _displayName; set { _displayName = value; OnPropertyChanged(); } }
        private string _address;
        public string Address { get => _address; set { _address = value; OnPropertyChanged(); } }
        private string _code;
        public string Code { get => _code; set { _code = value; OnPropertyChanged(); } }
        private string _email;
        public string Email { get => _email; set { _email = value; OnPropertyChanged(); } }
        private string _phoneNumber;
        public string PhoneNumber { get => _phoneNumber; set { _phoneNumber = value; OnPropertyChanged(); } }



        public ICommand AddCommand { get; set; }
        public ICommand UpdateCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        private IRepository<Provider> _repository;
        private IUnitOfWork _unitOfWork;

        public ProviderViewModel(IRepository<Provider> repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;


            LoadData();
            AddCommand = new RelayCommand<object>((p) =>
            {
                if (string.IsNullOrEmpty(DisplayName) || string.IsNullOrEmpty(Code))
                    return false;

                if (_repository.AsQueryable().Any(x => x.Code == Code))
                    return false;

                return true;

            }, async (p) =>
            {
                var provider = new Provider() { Name = DisplayName, Address = Address, Code = Code, Email = Email, PhoneNumber = PhoneNumber };

                await _unitOfWork.BeginTransactionAsync();
                try
                {
                    provider = await _repository.AddAsync(provider);
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
                    var provider = await _repository.AsQueryable().FirstOrDefaultAsync(x => x.Id == SelectedItem.Id);
                    provider.Name = DisplayName;
                    provider.Code = Code;
                    provider.Address = Address;
                    provider.Email = Email;
                    provider.PhoneNumber = PhoneNumber;

                    await _repository.UpdateAsync(provider);
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
                    var provider = await _repository.AsQueryable().FirstOrDefaultAsync(x => x.Id == SelectedItem.Id);
                    await _repository.DeleteAsync(provider);
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
            List = new ObservableCollection<Provider>(_repository.AsQueryable().ToList());
        }
    }
}
