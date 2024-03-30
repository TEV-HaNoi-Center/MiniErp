using MiniErp.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace MiniErp.DataAccess.Models
{
    public class TimeKeepingModel
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string RoleName { get; set; }
        public DateTime CheckDate { get; set; }
        public string Date { get => CheckDate.ToString("dd-MM-yyyy"); }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public string CheckInTime { get => CheckIn.ToString("HH:mm"); }
        public string CheckOutTime { get => CheckOut.ToString("HH:mm"); }
    }
}
