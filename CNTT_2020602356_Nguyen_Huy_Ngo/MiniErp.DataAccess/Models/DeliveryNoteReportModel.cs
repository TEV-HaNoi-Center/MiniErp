using MiniErp.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace MiniErp.DataAccess.Models
{
    public class DeliveryNoteReportModel
    {
        public int STT { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string UnitName { get; set; }
        public double Quantity { get; set; }
        public double Price { get; set; }
        public double Total { get; set; }

        public static DeliveryNoteReportModel CreateModel(DeliveryNoteDetail entity)
        {
            return new DeliveryNoteReportModel
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
