using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MiniErp.Core.Repositories;
using MiniErp.Core.UnitOfWorks;
using MiniErp.Domain;
using MiniErp.UI.DependencyInjection;
using MiniErp.UI.FingerScanner;
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
    public class RegisterFingersprintViewModel : BaseViewModel
    {
        private readonly IRepository<User> _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly MainContentStore _mainContentStore;
        private User _user;
        public User User { get => _user; set {  _user = value; OnPropertyChanged(); } }
        private ObservableCollection<User> _listUser;
        public ObservableCollection<User> ListUser { get => _listUser; set { _listUser = value; OnPropertyChanged(); } }
        public ICommand RegisterCommand { get; set; }
        public ICommand CancelCommand { get; set; }

        public RegisterFingersprintViewModel(IUnitOfWork unitOfWork, IRepository<User> userRepository, MainContentStore mainContentStore)
        {
            _unitOfWork = unitOfWork;
            _userRepository = userRepository;
            _mainContentStore = mainContentStore;

            LoadSource();

            CancelCommand = new RelayCommand<object>(p => true, p =>
            {
                _mainContentStore.CurrentViewModel = IoC.Resolve<TimeKeepingViewModel>();
            });

            RegisterCommand = new RelayCommand<object>(p => User is not null, async p =>
            {
                var fingerSensor = IoC.ServiceProvider.GetRequiredService<FingerSensor>();
                var index = await _userRepository.AsQueryable().CountAsync(x => !string.IsNullOrEmpty(x.FingerprintCode));
                var user = await _userRepository.AsQueryable().FirstOrDefaultAsync(x => x.Email == User.Email);
                if (string.IsNullOrEmpty(user.FingerprintCode))
                {
                    index++;
                }
                else
                {
                    index = int.Parse(user.FingerprintCode);
                }

                fingerSensor.Index = index;
                fingerSensor.IsAdding = true;
                while(fingerSensor.IsAdding)
                {
                    //wait for adding process
                }

                if (fingerSensor.AddResult)
                {
                    await _unitOfWork.BeginTransactionAsync();
                    try
                    {
                        user.FingerprintCode = (index).ToString();
                        await _userRepository.UpdateAsync(user);
                        await _unitOfWork.CommitAsync();
                    }
                    catch (Exception ex)
                    {
                        await _unitOfWork.RollbackAsync();
                    }
                    fingerSensor.AddResult = false;
                    MessageBox.Show("Thêm vân tay thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    _mainContentStore.CurrentViewModel = IoC.Resolve<TimeKeepingViewModel>();
                }
                else
                    MessageBox.Show("Thêm vân tay thất bại. Vui lòng thử lại", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            });
        }

        private void LoadSource()
        {
            var users = _userRepository.AsQueryable().ToList();
            ListUser = new ObservableCollection<User>(users);
        }
    }
}
