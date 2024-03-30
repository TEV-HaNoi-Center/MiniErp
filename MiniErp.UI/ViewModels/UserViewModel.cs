using Microsoft.EntityFrameworkCore;
using MiniErp.Core.Repositories;
using MiniErp.Core.UnitOfWorks;
using MiniErp.Domain;
using MiniErp.UI.DependencyInjection;
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
    public class UserViewModel : BaseViewModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<User> _repository;
        private readonly IRepository<Role> _roleRepository;
        private readonly MainContentStore _mainContentStore;
        private User _data;
        public User Data { get => _data; set { _data = value; OnPropertyChanged(); Name = value?.Name; Role = value?.Role; } }
        private string _name;
        public string Name { get; set; }
        private Role _role;
        public Role Role { get => _role; set { _role = value; OnPropertyChanged(); } }
        private IEnumerable<Role> _roleList = new List<Role>();
        public IEnumerable<Role> RoleList { get => _roleList; set { _roleList = value; OnPropertyChanged(); } }
        public ICommand ConfirmCommand { get; set; }
        public ICommand CancelCommand { get; set; }

        public UserViewModel(IUnitOfWork unitOfWork, IRepository<User> repository, IRepository<Role> roleRepository, MainContentStore mainContentStore)
        {
            _unitOfWork = unitOfWork;
            _repository = repository;
            _roleRepository = roleRepository;
            _mainContentStore = mainContentStore;

            LoadSource();

            ConfirmCommand = new RelayCommand<object>(p => !string.IsNullOrEmpty(Name) && Role != null, async p =>
            {
                var entity = await _repository.AsQueryable().FirstOrDefaultAsync(x=>x.Email == Data.Email);
                if (entity == null)
                    return;

                entity.Name = Name;
                entity.RoleId = Role.Id;
                await _unitOfWork.BeginTransactionAsync();
                try
                {
                    await _repository.UpdateAsync(entity);
                    await _unitOfWork.CommitAsync();
                    _mainContentStore.CurrentViewModel = IoC.Resolve<UserMainViewModel>();
                }
                catch (Exception ex)
                {
                    await _unitOfWork.RollbackAsync();
                }
            });

            CancelCommand = new RelayCommand<object>(p => true, p =>
            {
                _mainContentStore.CurrentViewModel = IoC.Resolve<UserMainViewModel>();
            });
        }

        private void LoadSource()
        {
            var dbRoles = _roleRepository.AsQueryable().ToList();
            RoleList = new ObservableCollection<Role>(dbRoles);
        }
    }
}
