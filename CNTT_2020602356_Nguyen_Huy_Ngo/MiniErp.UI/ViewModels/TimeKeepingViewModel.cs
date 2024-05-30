using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Win32;
using MiniErp.Core.Repositories;
using MiniErp.DataAccess.Models;
using MiniErp.Domain;
using MiniErp.UI.DependencyInjection;
using MiniErp.UI.Stores;
using MiniErp.UI.ViewModels.Abstract;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using BorderStyle = NPOI.SS.UserModel.BorderStyle;

namespace MiniErp.UI.ViewModels
{
    public class TimeKeepingViewModel : BaseViewModel
    {
        private readonly IRepository<TimeKeeping> _timeKeepingRepository;
        private readonly IRepository<Role> _roleRepository;
        private readonly MainContentStore _mainContentStore;
        private TimeKeepingModel _selectedItem;
        public TimeKeepingModel SelectedItem { get => _selectedItem; set { _selectedItem = value; OnPropertyChanged(); } }
        private ObservableCollection<TimeKeepingModel> _list;
        public ObservableCollection<TimeKeepingModel> List { get => _list; set { _list = value; OnPropertyChanged(); } }
        private ObservableCollection<Role> _roleList = new ObservableCollection<Role>();
        public ObservableCollection<Role> RoleList { get => _roleList; set { _roleList = value; OnPropertyChanged(); } }
        private Role _role;
        public Role Role { get => _role; set { _role = value; OnPropertyChanged(); } }
        private string _name;
        public string Name { get => _name; set { _name = value; OnPropertyChanged(); } }
        private string _email;
        public string Email { get => _email; set { _email = value; OnPropertyChanged(); } }
        private DateTime _date = DateTime.Today;
        public DateTime Date { get => _date; set { _date = value; OnPropertyChanged(); } }
        public ICommand LoadCommand { get; set; }
        public ICommand RegisterCommand { get; set; }
        public ICommand ExportCommand { get; set; }
        public ICommand CalculationCommand { get; set; }

        public TimeKeepingViewModel(MainContentStore mainContentStore, IRepository<TimeKeeping> timeKeepingRepository, IRepository<Role> roleRepository)
        {
            _mainContentStore = mainContentStore;
            _timeKeepingRepository = timeKeepingRepository;
            _roleRepository = roleRepository;

            LoadSource();
            RegisterCommand = new RelayCommand<object>(p => true, async p =>
            {
                _mainContentStore.CurrentViewModel = IoC.Resolve<RegisterFingersprintViewModel>();
            });
            CalculationCommand = new RelayCommand<object>(p => true, async p =>
            {
                _mainContentStore.CurrentViewModel = IoC.Resolve<PersonnelCalculationViewModel>();
            });
            LoadCommand = new RelayCommand<object>(p => true, async p =>
            {
                await LoadData();
            });

            ExportCommand = new RelayCommand<object>(p => List != null && List.Any(), p =>
            {
                XSSFWorkbook workbook = new XSSFWorkbook();

                ISheet sheet = workbook.CreateSheet("Report");

                IRow headerRow = sheet.CreateRow(0);
                CreateCell(headerRow, 0, "STT", true);
                CreateCell(headerRow, 1, "Email", true);
                CreateCell(headerRow, 2, "Tên người dùng", true);
                CreateCell(headerRow, 3, "Vai trò", true);
                CreateCell(headerRow, 4, "Ngày", true);
                CreateCell(headerRow, 5, "Giờ vào", true);
                CreateCell(headerRow, 6, "Giờ ra", true);

                int rowIndex = 1;

                foreach (TimeKeepingModel item in List)
                {
                    IRow currentRow = sheet.CreateRow(rowIndex);
                    CreateCell(currentRow, 0, rowIndex.ToString());
                    CreateCell(currentRow, 1, item.Email);
                    CreateCell(currentRow, 2, item.Name);
                    CreateCell(currentRow, 3, item.RoleName);
                    CreateCell(currentRow, 4, item.Date);
                    CreateCell(currentRow, 5, item.CheckInTime);
                    CreateCell(currentRow, 6, item.CheckOutTime);

                    rowIndex++;
                }
                // Auto sized all the affected columns
                int lastColumNum = sheet.GetRow(0).LastCellNum;
                for (int i = 0; i <= lastColumNum; i++)
                {
                    sheet.AutoSizeColumn(i);
                }
                // Write Excel to disk 
                Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
                saveFileDialog.Filter = "Excel 2003 file (*.xls)| *.xls|Excel 2007 file (*.xlsx)|*.xlsx";
                string path = "";
                if (saveFileDialog.ShowDialog() != true)
                    return;
                path = saveFileDialog.FileName;
                using (var fileData = new FileStream(path, FileMode.Create))
                {
                    workbook.Write(fileData);
                }
            });
        }

        private async Task LoadData()
        {
            var query = _timeKeepingRepository.AsQueryable();
            query = query.Where(x => x.Date == Date.Date);
            if (Role is not null)
                query = query.Where(x => x.User.RoleId == Role.Id || Role.Id == Guid.Empty);
            if (!string.IsNullOrWhiteSpace(Name))
                query = query.Where(x => x.User.Name.Contains(Name));
            if (!string.IsNullOrWhiteSpace(Email))
                query = query.Where(x => x.User.Email.Contains(Email));

            var result = await query.Include(x => x.User).ThenInclude(x => x.Role).GroupBy(x => new { Date = x.Date, UserId = x.UserId })
                        .Select(x => new TimeKeepingModel()
                        {
                            Id = x.First().Id,
                            CheckDate = x.Key.Date,
                            Email = x.First().User.Email,
                            Name = x.First().User.Name,
                            RoleName = x.First().User.Role.Name,
                            CheckIn = x.Min(y => y.CheckTime),
                            CheckOut = x.Max(y => y.CheckTime)
                        }).ToListAsync();

            List = new ObservableCollection<TimeKeepingModel>(result);
        }

        private void LoadSource()
        {
            var dbRoles = _roleRepository.AsQueryable().ToList();
            RoleList = new ObservableCollection<Role>(new List<Role> { new Role { Name = "Tất cả" } }.Concat(dbRoles));
        }

        private void CreateCell(IRow CurrentRow, int CellIndex, string Value, bool textCenter = false)
        {
            XSSFCellStyle borderedCellStyle = (XSSFCellStyle)CurrentRow.Sheet.Workbook.CreateCellStyle();
            borderedCellStyle.BorderLeft = BorderStyle.Medium;
            borderedCellStyle.BorderTop = BorderStyle.Medium;
            borderedCellStyle.BorderRight = BorderStyle.Medium;
            borderedCellStyle.BorderBottom = BorderStyle.Medium;
            if (textCenter)
                borderedCellStyle.VerticalAlignment = VerticalAlignment.Center;
            ICell Cell = CurrentRow.CreateCell(CellIndex);
            Cell.SetCellValue(Value);
            Cell.CellStyle = borderedCellStyle;
        }
    }
}
