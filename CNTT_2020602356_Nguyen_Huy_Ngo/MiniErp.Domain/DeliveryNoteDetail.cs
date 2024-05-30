using MiniErp.Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniErp.Domain
{
    public class DeliveryNoteDetail : IEntity
    {
        #region Properties
        [Required]
        public Guid Id { get; set; }

        public Guid DeliveryNoteId { get; set; }

        public Guid ProductId { get; set; }

        public int Quantity { get; set; }

        public double Price { get; set; }

        public Guid CurrencyId { get; set; }

        public Guid UnitId { get; set; }

        [StringLength(50)]
        public string Note { get; set; }

        public virtual Currency Currency { get; set; }

        public virtual Product Product { get; set; }

        public virtual DeliveryNote DeliveryNote { get; set; }

        public virtual Unit Unit { get; set; }
        #endregion Properties
    }
}
