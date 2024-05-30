using Firebase.Auth;
using Microsoft.EntityFrameworkCore;
using MiniErp.Core.Repositories;
using MiniErp.UI.DependencyInjection;
using MiniErp.UI.Models;
using MiniErp.UI.Stores;
using MiniErp.UI.ViewModels.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MiniErp.UI.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private string _email = "huyngu20@gmail.com";
        public string Email { get => _email; set { _email = value; OnPropertyChanged(); } }
        private string _password = "111111";
        public string Password { get => _password; set { _password = value; OnPropertyChanged(); } }
        public ICommand SubmitCommand { get; set; }
        public ICommand PasswordResetCommand { get; set; }
        public ICommand RegisterCommand { get; set; }
        private readonly FirebaseAuthClient _authClient;
        private LoggedInUser User { get; set; }
        private string _errorMessage;
        public string ErrorMessage { get => _errorMessage; set { _errorMessage = value; OnPropertyChanged(); } }
        private readonly NavigationStore _navigationStore;
        private readonly IRepository<Domain.User> _repository;
        public bool UserShouldEditValueNow
        {
            get
            {
                return true;
            }
        }
        public LoginViewModel(FirebaseAuthClient authClient, LoggedInUser user, NavigationStore navigationStore, IRepository<Domain.User> repository)
        {
            _authClient = authClient;
            _navigationStore = navigationStore;
            _repository = repository;
            User = user;
            SubmitCommand = new RelayCommand<object>(p => !string.IsNullOrEmpty(Email.ToString()) && !string.IsNullOrEmpty(Password), async p =>
            {
                _navigationStore.LoadingVisibility = Visibility.Visible;
                try
                {
                    User.UserCredential = await _authClient.SignInWithEmailAndPasswordAsync(Email.Trim(), Password);
                    var entity = await _repository.AsQueryable().FirstOrDefaultAsync(x => x.Email == Email.Trim());
                    User.User = entity;
                    _navigationStore.CurrentViewModel = IoC.Resolve<MainContentViewModel>();
                }
                catch (Exception ex)
                {
                    _navigationStore.LoadingVisibility = Visibility.Hidden;
                    ErrorMessage = "Mật khẩu hoặc tài khoản ko tồn tại. Vui lòng thử lại.";
                }
                _navigationStore.LoadingVisibility = Visibility.Hidden;
            })
            {

            };
            RegisterCommand = new RelayCommand<object>(p => true, p =>
            {
                _navigationStore.CurrentViewModel = IoC.Resolve<RegisterViewModel>();
            });

            PasswordResetCommand = new RelayCommand<object>(p => true, p =>
            {
                _navigationStore.CurrentViewModel = IoC.Resolve<ResetPasswordViewModel>();
            });
        }
    }
}
