using MiniErp.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniErp.DataAccess.Models
{
    public class DeliveryNoteDetailModel
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Note { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public double Total { get; set; }
        public string CurrencyCode { get; set; }
        public double ExchangeRate { get; set; }

        public static DeliveryNoteDetailModel CreateModel(DeliveryNoteDetail entity)
        {
            return new DeliveryNoteDetailModel()
            {
                Id = entity.Id,
                Code = entity.Product.Code,
                ProductId = entity.ProductId,
                Name = entity.Product.Name,
                CurrencyCode = entity.Currency.Code,
                ExchangeRate = entity.Currency.ExchangeRate,
                Note = entity.Note,
                Quantity = entity.Quantity,
                Price = entity.Price,
                Total = entity.Price * entity.Quantity * entity.Currency.ExchangeRate
            };
        }
    }
}
