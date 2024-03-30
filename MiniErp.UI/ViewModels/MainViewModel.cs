using MiniErp.UI.DependencyInjection;
using MiniErp.UI.Stores;
using MiniErp.UI.ViewModels.Abstract;
using MiniErp.UI.Views.Category;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Application = System.Windows.Application;

namespace MiniErp.UI.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly NavigationStore _navigationStore;
  
        public BaseViewModel CurrentViewModel => _navigationStore.CurrentViewModel;
        public Visibility LoadingVisibility => _navigationStore.LoadingVisibility;
        public MainViewModel(NavigationStore navigationStore, MainContentStore mainContentStore)
        {
            _navigationStore = navigationStore;
            _navigationStore.CurrentViewModelChanged += OnCurrenViewModelChanged;
            _navigationStore.LoadingVisibilityChanged += OnLoadingVisibilityChanged;
            _navigationStore.LoadingVisibility = Visibility.Hidden;
            if (_navigationStore.CurrentViewModel == null)
            {
                _navigationStore.CurrentViewModel = IoC.Resolve<LoginViewModel>();
            }
        }

        private void OnCurrenViewModelChanged()
        {
            OnPropertyChanged(nameof(CurrentViewModel));
        }

        private void OnLoadingVisibilityChanged()
        {
            OnPropertyChanged(nameof(LoadingVisibility));
        }
        ///
    }
}

