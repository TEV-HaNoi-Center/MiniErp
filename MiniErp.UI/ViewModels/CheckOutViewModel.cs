using MiniErp.UI.ViewModels.Abstract;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MiniErp.UI.ViewModels
{
    public static class Define
    {
        public static string DateFormat = "dd/MM/yyyy";
    }
    public class CCheckOutInfomation : BaseViewModel
    {
        public static string DateFormat = Define.DateFormat;

        private string _Name = "";
        private string _IdNumber = "TEV";
        private string _Team = "Vĩnh Phúc";
        private string _Date = DateTime.Now.ToString(DateFormat, new CultureInfo("vi-VN"));
        private string _Time = "08:00 ~ 17:00";
        private string _Description = "Nội dung công việc";
        private string _BackupFolder = @"D:\Checkout";

        public string Name
        {
            get { return _Name; }
            set
            {
                _Name = value;
                OnPropertyChanged();
            }
        }

        public string IdNumber
        {
            get { return _IdNumber; }
            set
            {
                _IdNumber = value;
                OnPropertyChanged();
            }
        }

        public string Team
        {
            get { return _Team; }
            set
            {
                _Team = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Project> ListProject { get; set; } = new ObservableCollection<Project>() { new Project() { CodeProject = "loc1", NameProject = "1" }, new Project() { CodeProject = "loc2", NameProject = "2" } };

        [System.Text.Json.Serialization.JsonIgnore]
        public string Date
        {
            get { return _Date; }
            set
            {
                _Date = value;
                OnPropertyChanged();
            }
        }

        public string Time
        {
            get { return _Time; }
            set
            {
                _Time = value;
                OnPropertyChanged();
            }
        }

        public string Description
        {
            get { return _Description; }
            set { _Description = value; }
        }

        public string BackupFolder
        {
            get { return _BackupFolder; }
            set { _BackupFolder = value; }
        }


        [System.Text.Json.Serialization.JsonIgnore]
        public bool IsValid
        {
            get
            {
                return !string.IsNullOrEmpty(Name) &
                       !string.IsNullOrEmpty(IdNumber) &
                       !string.IsNullOrEmpty(Team) &&
                       !string.IsNullOrEmpty(Date) &
                       !string.IsNullOrEmpty(Time) &
                       !string.IsNullOrEmpty(Description);
            }
        }
    }
    public class Project
    {
        public string CodeProject { get; set; }
        public string NameProject { get; set; }
    }
    public static class DateTimeExtensions
    {
        public static DateTime RoundToTicks(this DateTime target, long ticks) => new DateTime((target.Ticks + ticks / 2) / ticks * ticks, target.Kind);
        public static DateTime RoundUpToTicks(this DateTime target, long ticks) => new DateTime((target.Ticks + ticks - 1) / ticks * ticks, target.Kind);
        public static DateTime RoundDownToTicks(this DateTime target, long ticks) => new DateTime(target.Ticks / ticks * ticks, target.Kind);

        public static DateTime Round(this DateTime target, TimeSpan round) => RoundToTicks(target, round.Ticks);
        public static DateTime RoundUp(this DateTime target, TimeSpan round) => RoundUpToTicks(target, round.Ticks);
        public static DateTime RoundDown(this DateTime target, TimeSpan round) => RoundDownToTicks(target, round.Ticks);

        public static DateTime RoundToMinutes(this DateTime target, int minutes = 1) => RoundToTicks(target, minutes * TimeSpan.TicksPerMinute);
        public static DateTime RoundUpToMinutes(this DateTime target, int minutes = 1) => RoundUpToTicks(target, minutes * TimeSpan.TicksPerMinute);
        public static DateTime RoundDownToMinutes(this DateTime target, int minutes = 1) => RoundDownToTicks(target, minutes * TimeSpan.TicksPerMinute);

        public static DateTime RoundToHours(this DateTime target, int hours = 1) => RoundToTicks(target, hours * TimeSpan.TicksPerHour);
        public static DateTime RoundUpToHours(this DateTime target, int hours = 1) => RoundUpToTicks(target, hours * TimeSpan.TicksPerHour);
        public static DateTime RoundDownToHours(this DateTime target, int hours = 1) => RoundDownToTicks(target, hours * TimeSpan.TicksPerHour);

        public static DateTime RoundToDays(this DateTime target, int days = 1) => RoundToTicks(target, days * TimeSpan.TicksPerDay);
        public static DateTime RoundUpToDays(this DateTime target, int days = 1) => RoundUpToTicks(target, days * TimeSpan.TicksPerDay);
        public static DateTime RoundDownToDays(this DateTime target, int days = 1) => RoundDownToTicks(target, days * TimeSpan.TicksPerDay);
    }
    public class RelayCommand : ICommand
    {
        private Action<object> _execute;

        private Func<object, bool> _canExecute;

        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }
    }
    public class CheckOutViewModel : BaseViewModel
    {

        private CCheckOutInfomation _CheckOutInformation;

        public CCheckOutInfomation CheckOutInfomation
        {
            get { return _CheckOutInformation; }
            set
            {
                _CheckOutInformation = value;
                OnPropertyChanged();
            }
        }

        public CheckOutViewModel()
        {
            try
            {
                CheckOutInfomation = JsonConvert.DeserializeObject<CCheckOutInfomation>(File.ReadAllText("Backup.json"));
            }
            catch
            {
                CheckOutInfomation = new CCheckOutInfomation();
            }

            CheckOutInfomation.Date = DateTime.Now.ToString(Define.DateFormat);
            CheckOutInfomation.Time = "08:00 ~ " + DateTime.Now.RoundToMinutes(30).ToString("HH:mm");
        }
        public RelayCommand SelectBackupFolderCommand
        {
            get
            {
                return new RelayCommand((o) =>
                {
                    var dialog = new System.Windows.Forms.FolderBrowserDialog();
                    System.Windows.Forms.DialogResult result = dialog.ShowDialog();

                    if (result == System.Windows.Forms.DialogResult.OK)
                    {
                        CheckOutInfomation.BackupFolder = dialog.SelectedPath;
                    }
                });
            }
        }
        public RelayCommand SubmitCommand
        {
            get
            {
                return new RelayCommand((o) =>
                {
                    if (CheckOutInfomation.IsValid == false)
                    {
                        System.Windows.MessageBox.Show("Nhập đủ thông tin và thử lại");
                        return;
                    }

                    //Verify whether date entered in dd/MM/yyyy format.
                    DateTime InputDateTime;
                    bool isValid = DateTime.TryParseExact(CheckOutInfomation.Date.Trim(),
                                                          Define.DateFormat,
                                                          CultureInfo.InvariantCulture,
                                                          DateTimeStyles.None, out InputDateTime);
                    if (isValid == false)
                    {
                        System.Windows.MessageBox.Show($"Vui lòng nhập lại thông tin ngày làm việc theo định dạng \"{Define.DateFormat}\"");
                        return;
                    }

                    string defaulLink = @"https://docs.google.com/forms/d/e/1FAIpQLSdF_GIEu8WQf4B6kyaBvbj0zbysECGbRAwqET3A6BGM_LanKg/viewform";
                    string url = string.Format(defaulLink + "?"
                        + $"entry.1919919867={CheckOutInfomation.Team}&"
                        + $"entry.1108472107={CheckOutInfomation.IdNumber}&"
                        + $"entry.861737742={CheckOutInfomation.Name}&"
                        + $"entry.1033621518={CheckOutInfomation.Time}&"
                        + $"entry.2021547845={CheckOutInfomation.Description}&"
                        + $"entry.1958890831_year={InputDateTime:yyyy}&"
                        + $"entry.1958890831_month={InputDateTime:MM}&"
                        + $"entry.1958890831_day={InputDateTime:dd}&");

                    // Write backup information
                    File.WriteAllText("Backup.json", JsonConvert.SerializeObject(CheckOutInfomation));

                    string fileName = $"{InputDateTime:yyMM}_{CheckOutInfomation.IdNumber}.csv";
                    string filePath = Path.Combine(CheckOutInfomation.BackupFolder, fileName);
                    bool fileExists = File.Exists(filePath);

                    using (var sw = new StreamWriter(filePath, true, Encoding.UTF8))
                    {
                        if (fileExists == false)
                        {
                            sw.WriteLine("Team,Mã nhân viên,Họ tên,Ngày làm việc,Thời gian làm việc,Nội dung công việc");
                        }
                        sw.WriteLine($"{CheckOutInfomation.Team},{CheckOutInfomation.IdNumber},{CheckOutInfomation.Name},{CheckOutInfomation.Date},{CheckOutInfomation.Time},{CheckOutInfomation.Description.Replace(',', '/')}");
                    }
                    var psi = new ProcessStartInfo(url)
                    {
                        UseShellExecute = true
                    };
                    Process.Start(psi);
                    //System.Diagnostics.Process.Start(url);
                });
            }

        }
    }
}
