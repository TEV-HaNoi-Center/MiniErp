using MiniErp.UI.ViewModels.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MiniErp.UI.Stores
{
    public class NavigationStore
    {
        private BaseViewModel _currentViewModel;
        public BaseViewModel CurrentViewModel
        {
            get => _currentViewModel;
            set
            {
                _currentViewModel = value;
                OnCurrentViewModelChanged();
            }
        }
        private Visibility _loadingVisibility;
        public Visibility LoadingVisibility { get => _loadingVisibility; set { _loadingVisibility = value; OnLoadingVisibilityChanged(); } }
        public event Action CurrentViewModelChanged;
        public event Action LoadingVisibilityChanged;

        private void OnCurrentViewModelChanged()
        {
            CurrentViewModelChanged?.Invoke();  
        }

        private void OnLoadingVisibilityChanged()
        {
            LoadingVisibilityChanged?.Invoke();
        }
    }
}
