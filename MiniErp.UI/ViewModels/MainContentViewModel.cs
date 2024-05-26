using Firebase.Auth;
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
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MiniErp.UI.ViewModels
{
    public class MainContentViewModel : BaseViewModel
    {
        private readonly NavigationStore _navigationStore;
        private readonly MainContentStore _mainContentStore;
        private readonly FirebaseAuthClient _authClient;
        public ICommand UnitCommand { get; set; }
        public ICommand CheckOutCommand { get; set; }
        public ICommand CurrencyCommand { get; set; }
        public ICommand ProviderCommand { get; set; }
        public ICommand CustomerCommand { get; set; }
        public ICommand ProductCommand { get; set; }
        public ICommand RoleCommand { get; set; }
        public ICommand ReceiveNoteCommand { get; set; }
        public ICommand DeliveryNoteCommand { get; set; }
        public ICommand LogoutCommand { get; set; }
        public ICommand InventoryCommand { get; set; }
        public ICommand UserCommand { get; set; }
        public ICommand TimeKeepingCommand { get; set; }
        public ICommand LoadCommand { get; set; }
        public ICommand HomeViewCommand { get; set; }
        public BaseViewModel CurrentViewModel => _mainContentStore.CurrentViewModel;
        private LanguageModel _language;
        public LanguageModel Language { get => _language; set { _language = value; SwitchLanguage(Language.LanguageCode); OnPropertyChanged(); } }
        private List<LanguageModel> _languages;
        public List<LanguageModel> Languages { get => _languages; set { _languages = value; OnPropertyChanged(); } }

        public MainContentViewModel(NavigationStore navigationStore, MainContentStore mainContentStore, LoggedInUser user, FirebaseAuthClient authClient)
        {
            _navigationStore = navigationStore;
            _mainContentStore = mainContentStore;
            _mainContentStore.CurrentViewModel = new HomeViewModel();
            CurrentUser = user;
            _mainContentStore.CurrentViewModelChanged += OnCurrenViewModelChanged;
            _authClient = authClient;
            Languages = new List<LanguageModel>
            {
                new LanguageModel
                {
                    Language = "Vietnamese",
                    LanguageCode = "vi",
                    LanguageUri = new Uri("..\\Resources\\StringResources.vi.xaml", UriKind.Relative),
                    Icon = "..\\Resources\\vn-flag (1) (1).png"
                },
                new LanguageModel
                {
                    Language = "English",
                    LanguageCode = "en",
                    LanguageUri = new Uri("..\\Resources\\StringResources.en.xaml", UriKind.Relative),
                    Icon = "..\\Resources\\en-flag (1).png"
                },
                new LanguageModel
                {
                    Language = "Korean",
                    LanguageCode = "ko",
                    LanguageUri = new Uri("..\\Resources\\StringResources.ko.xaml", UriKind.Relative),
                    Icon = "..\\Resources\\flag-south-korea.png"
                }
            };

            UnitCommand = new RelayCommand<object>(p => CurrentUser.IsAdmin, p =>
            {
                _mainContentStore.CurrentViewModel = IoC.Resolve<UnitViewModel>();
            });

            CurrencyCommand = new RelayCommand<object>(p => CurrentUser.IsAdmin, p =>
            {
                _mainContentStore.CurrentViewModel = IoC.Resolve<CurrencyViewModel>();
            });

            ProviderCommand = new RelayCommand<object>(p => CurrentUser.IsAdmin, p =>
            {
                _mainContentStore.CurrentViewModel = IoC.Resolve<ProviderViewModel>();
            });

            CustomerCommand = new RelayCommand<object>(p => CurrentUser.IsAdmin, p =>
            {
                _mainContentStore.CurrentViewModel = IoC.Resolve<CustomerViewModel>();
            });

            ProductCommand = new RelayCommand<object>(p => CurrentUser.IsAdmin, p =>
            {
                _mainContentStore.CurrentViewModel = IoC.Resolve<ProductViewModel>();
            });

            RoleCommand = new RelayCommand<object>(p => CurrentUser.IsAdmin, p =>
            {
                _mainContentStore.CurrentViewModel = IoC.Resolve<RoleViewModel>();
            });

            ReceiveNoteCommand = new RelayCommand<object>(p => true, p =>
            {
                _mainContentStore.CurrentViewModel = IoC.Resolve<ReceiveNoteMainViewModel>();
            });

            DeliveryNoteCommand = new RelayCommand<object>(p => true, p =>
            {
                _mainContentStore.CurrentViewModel = IoC.Resolve<DeliveryNoteMainViewModel>();
            });

            LogoutCommand = new RelayCommand<object>(p => true, p =>
            {
                _authClient.SignOut();
                CurrentUser.User = null;
                CurrentUser.UserCredential = null;
                _navigationStore.CurrentViewModel = IoC.Resolve<LoginViewModel>();
                _mainContentStore.CurrentViewModel = null;
            });
            CheckOutCommand = new RelayCommand<object>(p => true, p =>
            {
                _mainContentStore.CurrentViewModel = IoC.Resolve<CheckOutViewModel>();
            });
            InventoryCommand = new RelayCommand<object>(p => true, p =>
            {
                _mainContentStore.CurrentViewModel = IoC.Resolve<InventoryViewModel>();
            });

            UserCommand = new RelayCommand<object>(p => CurrentUser.IsAdmin || CurrentUser.IsManager, p =>
            {
                _mainContentStore.CurrentViewModel = IoC.Resolve<UserMainViewModel>();
            });

            TimeKeepingCommand = new RelayCommand<object>(p => CurrentUser.IsAdmin || CurrentUser.IsManager, p =>
            {
                _mainContentStore.CurrentViewModel = IoC.Resolve<TimeKeepingViewModel>();
            });
            HomeViewCommand = new RelayCommand<object>(p => CurrentUser.IsAdmin || CurrentUser.IsManager, p =>
            {
                _mainContentStore.CurrentViewModel = IoC.Resolve<HomeViewModel>();
            });
            LoadCommand = new RelayCommand<object>(p => true, p =>
            {
                Language = Languages[0];
            });
        }

        private void OnCurrenViewModelChanged()
        {
            OnPropertyChanged(nameof(CurrentViewModel));
        }

        private void SwitchLanguage(string language)
        {
            ResourceDictionary resourceDictionary = new ResourceDictionary();
            switch (language)
            {
                case "vi":
                    resourceDictionary.Source = new Uri("..\\Resources\\StringResources.vi.xaml", UriKind.Relative);
                    break;
                case "en":
                    resourceDictionary.Source = new Uri("..\\Resources\\StringResources.en.xaml", UriKind.Relative);
                    break;
                case "ko":
                    resourceDictionary.Source = new Uri("..\\Resources\\StringResources.ko.xaml", UriKind.Relative);
                    break;
                default:
                    resourceDictionary.Source = new Uri("..\\Resources\\StringResources.vi.xaml", UriKind.Relative);
                    break;
            }
            System.Windows.Application.Current.Resources.MergedDictionaries.Add(resourceDictionary);
        }
    }
}
