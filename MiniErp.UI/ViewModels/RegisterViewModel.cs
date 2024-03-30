using Firebase.Auth;
using MiniErp.Core.Repositories;
using MiniErp.Core.UnitOfWorks;
using MiniErp.Domain;
using MiniErp.UI.Constants;
using MiniErp.UI.DependencyInjection;
using MiniErp.UI.Stores;
using MiniErp.UI.Utils;
using MiniErp.UI.ViewModels.Abstract;
using System.Windows;
using System.Windows.Input;

namespace MiniErp.UI.ViewModels
{
    public class RegisterViewModel : BaseViewModel
    {
        private readonly FirebaseAuthClient _client;
        private readonly IRepository<Domain.User> _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly NavigationStore _navigationStore;
        private string _email;
        public string Email { get => _email; set { _email = value; OnPropertyChanged(); } }
        private string _userName;
        public string UserName { get => _userName; set { _userName = value; OnPropertyChanged(); } }
        private string _password;
        public string Password { get => _password; set { _password = value; OnPropertyChanged(); } }
        private string _confirmPassword;
        public string ConfirmPassword { get => _confirmPassword; set { _confirmPassword = value; OnPropertyChanged(); } }
        public ICommand SubmitCommand { get; set; }
        public ICommand LoginCommand { get; set; }

        public RegisterViewModel(FirebaseAuthClient authClient, IRepository<Domain.User> repository, NavigationStore navigationStore, IUnitOfWork unitOfWork)
        {
            _client = authClient;
            _repository = repository;
            _unitOfWork = unitOfWork;
            _navigationStore = navigationStore;
            LoginCommand = new RelayCommand<object>(p => true, p =>
            {
                _navigationStore.CurrentViewModel = IoC.Resolve<LoginViewModel>();
            });

            SubmitCommand = new RelayCommand<object>(p => true, async p =>
            {
                _navigationStore.LoadingVisibility = Visibility.Visible;
                try
                {
                    if (Password.Length < 6)
                    {
                        _navigationStore.LoadingVisibility = Visibility.Hidden;
                        System.Windows.MessageBox.Show("Mật khẩu phải dài ít nhất 6 ký tự!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    var x = await _client.CreateUserWithEmailAndPasswordAsync(Email, Password, UserName);
                    var user = new Domain.User()
                    {
                        Email = Email,
                        Name = UserName,
                        Password = CryptographyUtil.SHA256Hash(Password),
                        FingerprintCode = "",
                        RoleId = RoleConstants.UserId
                    };

                    await _unitOfWork.BeginTransactionAsync();

                    await _repository.AddAsync(user);
                    await _unitOfWork.CommitAsync();
                    System.Windows.MessageBox.Show("Đăng ký thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    _navigationStore.LoadingVisibility = Visibility.Hidden;
                    await _unitOfWork.RollbackAsync();
                    System.Windows.MessageBox.Show("Đăng ký thất bại!", "Thất bại", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                _navigationStore.LoadingVisibility = Visibility.Hidden;
            });
        }
    }
}
