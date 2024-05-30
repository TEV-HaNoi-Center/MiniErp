using Firebase.Auth;
using Firebase.Auth.Providers;
using MiniErp.UI.DependencyInjection;
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
    public class ResetPasswordViewModel : BaseViewModel
    {
        private readonly FirebaseAuthClient _client;
        private readonly NavigationStore _navigationStore;
        private string _email;
        public string Email { get => _email; set { _email = value; OnPropertyChanged(); } }
        public ICommand SendPasswordResetEmailCommand { get; set; }
        public ICommand NavigateLoginCommand { get; set; }

        public ResetPasswordViewModel(FirebaseAuthClient firebaseAuthClient, NavigationStore navigationStore)
        {
            _client = firebaseAuthClient;
            _navigationStore = navigationStore;

            SendPasswordResetEmailCommand = new RelayCommand<object>(p => true, async p =>
            {
                _navigationStore.LoadingVisibility = Visibility.Visible;
                try
                {
                    await _client.ResetEmailPasswordAsync(Email);

                    System.Windows.MessageBox.Show("Đã gửi email đặt lại mật khẩu thành công. Vui lòng kiểm tra email của bạn.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

                    _navigationStore.CurrentViewModel = IoC.Resolve<LoginViewModel>();
                }
                catch (Exception)
                {
                    _navigationStore.LoadingVisibility = Visibility.Hidden;
                    System.Windows.MessageBox.Show("Gửi email đặt lại mật khẩu thất bại. Vui lòng thử lại.", "Thất bại", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                _navigationStore.LoadingVisibility = Visibility.Hidden;
            });

            NavigateLoginCommand = new RelayCommand<object>(p => true, p =>
            {
                _navigationStore.CurrentViewModel = IoC.Resolve<LoginViewModel>();
            });
        }
    }
}
