using MiniErp.UI.ViewModels;
using MiniErp.UI.ViewModels.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MiniErp.UI.Stores
{
    public class MainContentStore
    {
        private BaseViewModel _currentViewModel = new HomeViewModel();
        public BaseViewModel CurrentViewModel
        {
            get => _currentViewModel;
            set
            {
                _currentViewModel = value;
                OnCurrentViewModelChanged();
            }
        }
        private Visibility _isLoading;
        public Visibility IsLoading { get => _isLoading; set { _isLoading = value; } }
        public event Action CurrentViewModelChanged;

        private void OnCurrentViewModelChanged()
        {
            CurrentViewModelChanged?.Invoke();
        }
    }
}
