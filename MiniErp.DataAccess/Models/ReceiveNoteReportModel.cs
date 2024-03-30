using MiniErp.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniErp.DataAccess.Models
{
    public class ReceiveNoteReportModel
    {
        public int STT { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string UnitName { get; set; }
        public double Quantity { get; set; }
        public double Price { get; set; }
        public double Total {  get; set; }

        public static ReceiveNoteReportModel CreateModel(ReceiveNoteDetail entity)
        {
            return new ReceiveNoteReportModel
            {
                Name = entity.Product.Name,
                Code = entity.Product.Code,
                UnitName = entity.Unit.Name,
                Quantity = entity.Quantity,
                Price = entity.Price,
                Total = entity.Quantity * entity.Price * entity.Currency.ExchangeRate
            };
        }
    }
}
