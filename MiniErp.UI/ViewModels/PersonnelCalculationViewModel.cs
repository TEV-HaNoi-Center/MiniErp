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
    public class PersonnelCalculationViewModel : BaseViewModel
    {
        private readonly IRepository<TimeKeeping> _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly MainContentStore _mainContentStore;
        private ObservableCollection<Worksheet> _list = new ObservableCollection<Worksheet>()
        {
            new Worksheet(){Id = Guid.NewGuid(),Name = "Admin",NumberWork = 26, LeaveEarlyHours = 10.5, OvertimeHours = 12.5},
        };
        public ObservableCollection<Worksheet> List
        {
            get
            {
                return _list;
            }
            set
            {
                _list = value;
                OnPropertyChanged(nameof(List));
            }
        }

        private ObservableCollection<int> _listMonth = new ObservableCollection<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
        public ObservableCollection<int> ListMonth
        {
            get => _listMonth;
            set
            {
                _listMonth = value;
                OnPropertyChanged(nameof(ListMonth));
            }
        }
        private int _selectMonth = DateTime.Now.Month;

        public int SelectMonth
        {
            get
            {
                return _selectMonth;
            }
            set
            {
                _selectMonth = value;
                OnPropertyChanged();
            }
        }
        private static ObservableCollection<int> ListYearToNow()
        {
            ObservableCollection<int> ListYear = new ObservableCollection<int>();
            for (int i = 2000; i <= DateTime.Now.Year; ++i) ListYear.Add(i);
            return ListYear;
        }
        private ObservableCollection<int> _listYear = ListYearToNow();
        public ObservableCollection<int> ListYear
        {
            get => _listYear;
            set
            {
                _listYear = value;
                OnPropertyChanged(nameof(ListYear));
            }
        }
        private int _selectYear = DateTime.Now.Year;

        public int SelectYear
        {
            get
            {
                return _selectYear;
            }
            set
            {
                _selectMonth = value;
                OnPropertyChanged(nameof(SelectYear));
            }
        }
        public ICommand CancelCommand { get; set; }
        public ICommand DisplayWorksheet { get; set; }
        // 2. Làm tròn giờ
        DateTime RoundToNearestHalfHour(DateTime dt)
        {
            int hour = dt.Hour;
            int minutes = dt.Minute;
            if (hour == 23 && minutes >= 45)
                return new DateTime(dt.Year, dt.Month, dt.Day + 1, 0, 0, 0);
            if (minutes < 15)
                return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, 0, 0);
            else if (minutes >= 15 && minutes < 45)
                return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, 30, 0);
            else
                return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour + 1, 0, 0);
        }
        // 3. Tính số giờ đi muộn
        double CalculateHoursFromEight(DateTime dt)
        {
            DateTime roundedTime = RoundToNearestHalfHour(dt);
            if (roundedTime.Hour >= 8 && roundedTime.Hour < 12)
            {
                double hoursFromEight = roundedTime.Hour - 8;
                if (roundedTime.Minute == 30)
                    return hoursFromEight + 0.5;
                else
                    return hoursFromEight;
            }
            else
                return 0;
        }
        // 4. Tính số giờ về sớm
        double CalculateHoursFromFivePM(DateTime dt)
        {
            DateTime roundedTime = RoundToNearestHalfHour(dt);

            if (roundedTime.Hour >= 13 && roundedTime.Hour < 17)
            {
                double hoursFromFivePM = 17 - roundedTime.Hour;
                if (roundedTime.Minute == 30)
                    hoursFromFivePM -= 0.5;
                return hoursFromFivePM;
            }
            else
            {
                return 0;
            }
        }
        double CalculateHoursFromNineteenToMidnightToday(DateTime dt)
        {
            DateTime roundedTime = RoundToNearestHalfHour(dt);

            if (roundedTime.Hour >= 17 && roundedTime.Hour < 24)
            {
                double hoursFromNineteen = roundedTime.Hour - 17;
                if (roundedTime.Minute == 30)
                    hoursFromNineteen -= 0.5;

                return hoursFromNineteen;
            }
            else
            {
                return 0;
            }
        }
        public PersonnelCalculationViewModel(IUnitOfWork unitOfWork, IRepository<TimeKeeping> userRepository, MainContentStore mainContentStore, MainContentStore _mainContentStore)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _mainContentStore = mainContentStore;
            CancelCommand = new RelayCommand<object>(p => true, p =>
            {
                _mainContentStore.CurrentViewModel = IoC.Resolve<TimeKeepingViewModel>();
            });
            DisplayWorksheet = new RelayCommand<object>(p => true, p =>
            {
                ObservableCollection<TimeKeeping> timeKeepings = new ObservableCollection<TimeKeeping>();
                timeKeepings = new ObservableCollection<TimeKeeping>(_userRepository.AsQueryable().ToList());

                HashSet<Guid> UserCodes = new HashSet<Guid>();
                foreach (var item in timeKeepings)
                {
                    UserCodes.Add(item.UserId);
                }
                int Month = SelectMonth, Year = SelectYear;
                foreach (var item in UserCodes)
                {
                    Guid _id = item;
                    string _name = timeKeepings.FirstOrDefault(x => x.UserId == _id).User.Name;
                    int _daywork = 0;
                    double _overtimeHours = 0.0, _leaveEarlyHours = 0.0;
                    foreach (var item2 in timeKeepings)
                    {
                        if (item2.Date.Year == Year && item2.Date.Month == Month)
                        {
                            ++_daywork;

                        }
                    }
                }
            });
        }
    }
}
