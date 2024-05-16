using System;
using System.Collections.Generic;
using System.Text;

namespace MiniErp.Domain
{
    public class Worksheet
    {
        public Guid Id { get; set; }
        public string Name { get; set; }    
        public int NumberWork {  get; set; }
        public double OvertimeHours { get; set; }
        public double LeaveEarlyHours { get; set; }
    }
}
