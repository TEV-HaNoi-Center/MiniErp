using MiniErp.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniErp.DataAccess.Models
{
    public class ReceiveNoteMainModel
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string OrderCode { get; set; }
        public DateTime Date { get; set; }
        public string ProviderName { get; set; }
        public double Total {  get; set; }

        public static ReceiveNoteMainModel CreateModel(ReceiveNote entity)
        {
            return new ReceiveNoteMainModel()
            {
                Id = entity.Id,
                Code = entity.Code,
                OrderCode = entity.OrderCode,
                Date = entity.Date,
                ProviderName = entity.Provider.Name,
                Total = entity.Details.Sum(x=>x.Quantity * x.Price * x.Currency.ExchangeRate)
            };
        }
    }
}
