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
using System.Windows;
using System.Windows.Input;

namespace MiniErp.UI.ViewModels
{
    public class UserMainViewModel : BaseViewModel
    {
        private readonly MainContentStore _mainContentStore;
        private IRepository<User> _repository;
        private IRepository<Role> _roleRepository;
        private IUnitOfWork _unitOfWork;
        private User _selectedItem;
        public User SelectedItem { get => _selectedItem; set { _selectedItem = value; OnPropertyChanged(); } }
        private IEnumerable<Role> _roleList = new List<Role>();
        public IEnumerable<Role> RoleList { get => _roleList; set { _roleList = value; OnPropertyChanged(); } }
        private Role _role;
        public Role Role { get => _role; set { _role = value; OnPropertyChanged(); } }
        private string _name;
        public string Name { get => _name; set { _name = value; OnPropertyChanged(); } }
        private string _email;
        public string Email { get => _email; set { _email = value; OnPropertyChanged(); } }
        private IEnumerable<User> _list;
        public IEnumerable<User> List { get => _list; set { _list = value; OnPropertyChanged(); } }
        public ICommand LoadCommand { get; set; }
        public ICommand UpdateCommand { get; set; }
        public ICommand DeleteCommand { get; set; }

        public UserMainViewModel(MainContentStore mainContentStore, IRepository<User> repository, IRepository<Role> roleRepository, IUnitOfWork unitOfWork)
        {
            _mainContentStore = mainContentStore;
            _repository = repository;
            _roleRepository = roleRepository;
            _unitOfWork = unitOfWork;

            LoadSource();
            LoadCommand = new RelayCommand<object>(p => true, async p =>
            {
                await LoadData();
            });

            UpdateCommand = new RelayCommand<object>(p => SelectedItem is not null, async p =>
            {
                var user = await _repository.AsQueryable().FirstOrDefaultAsync(x => x.Id == SelectedItem.Id);
                if (user == null)
                    return;

                var vm = IoC.Resolve<UserViewModel>();
                vm.Data = user;
                _mainContentStore.CurrentViewModel = vm;
            });

            DeleteCommand = new RelayCommand<object>(p => SelectedItem is not null, async p =>
            {
                if (SelectedItem != null && System.Windows.MessageBox.Show("Bạn có chắc chắn muốn xóa người dùng?", "Xác nhận xóa", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    var entity = await _repository.AsQueryable().FirstOrDefaultAsync(x => x.Id == SelectedItem.Id);
                    if (entity == null)
                    {
                        await LoadData();
                        return;
                    }
                    await _unitOfWork.BeginTransactionAsync();
                    try
                    {
                        await _repository.DeleteAsync(entity);
                        await _unitOfWork.CommitAsync();
                    }
                    catch (Exception ex)
                    {
                        await _unitOfWork.RollbackAsync();
                    }
                    await LoadData();
                }
            });
        }

        private void LoadSource()
        {
            var dbRoles = _roleRepository.AsQueryable().ToList();
            RoleList = new ObservableCollection<Role>(new List<Role> { new Role { Name = "Tất cả" } }.Concat(dbRoles));
        }

        private async Task LoadData()
        {
            var query = _repository.AsQueryable();
            if (Role is not null)
                query = query.Where(x => x.RoleId == Role.Id || Role.Id == Guid.Empty);
            if (!string.IsNullOrWhiteSpace(Name))
                query = query.Where(x => x.Name.Contains(Name));
            if (!string.IsNullOrWhiteSpace(Email))
                query = query.Where(x => x.Email.Contains(Email));

            List = new ObservableCollection<User>(await query.Include(x => x.Role).ToListAsync());
        }
    }
}
