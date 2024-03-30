using MiniErp.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniErp.DataAccess.Models
{
    public class DeliveryNoteMainModel
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string OrderCode { get; set; }
        public DateTime Date { get; set; }
        public string CustomerName { get; set; }
        public double Total { get; set; }

        public static DeliveryNoteMainModel CreateModel(DeliveryNote entity)
        {
            return new DeliveryNoteMainModel()
            {
                Id = entity.Id,
                Code = entity.Code,
                OrderCode = entity.OrderCode,
                Date = entity.Date,
                CustomerName = entity.Customer.Name,
                Total = entity.Details.Sum(x => x.Quantity * x.Price * x.Currency.ExchangeRate)
            };
        }
    }
}
