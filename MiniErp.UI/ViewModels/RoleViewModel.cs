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
    public class RoleViewModel : BaseViewModel
    {
        private ObservableCollection<Role> _list = new ObservableCollection<Role>();
        public ObservableCollection<Role> List { get => _list; set { _list = value; OnPropertyChanged(); } }
        private Role _selectedItem;
        public Role SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                if (_selectedItem != null)
                {
                    DisplayName = _selectedItem.Name;
                }
                OnPropertyChanged();
            }
        }
        private string _displayName;
        public string DisplayName { get => _displayName; set { _displayName = value; OnPropertyChanged(); } }


        public ICommand AddCommand { get; set; }
        public ICommand UpdateCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        private IRepository<Role> _repository;
        private IUnitOfWork _unitOfWork;

        public RoleViewModel(IRepository<Role> repository, IUnitOfWork unitOfWork)
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
                var role = new Role() { Name = DisplayName, Permission = "" };

                await _unitOfWork.BeginTransactionAsync();
                try
                {
                    role = await _repository.AddAsync(role);
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
                    var role = await _repository.AsQueryable().FirstOrDefaultAsync(x => x.Id == SelectedItem.Id);
                    role.Name = DisplayName;
                    await _repository.UpdateAsync(role);
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
                    var role = await _repository.AsQueryable().FirstOrDefaultAsync(x => x.Id == SelectedItem.Id);
                    await _repository.DeleteAsync(role);
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
            List = new ObservableCollection<Role>(_repository.AsQueryable().ToList());
        }
    }
}
