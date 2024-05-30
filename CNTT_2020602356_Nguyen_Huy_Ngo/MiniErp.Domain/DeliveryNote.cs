using MiniErp.Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniErp.Domain
{
    public class DeliveryNote : IEntity
    {
        #region Properties
        [Required]
        public Guid Id { get; set; }

        [StringLength(50)]
        public string Code { get; set; }

        public Guid CustomerId { get; set; }

        [StringLength(50)]
        public string OrderCode { get; set; }

        public DateTime Date { get; set; }

        public virtual Customer Customer { get; set; }

        public virtual ICollection<DeliveryNoteDetail> Details { get; set; } = new List<DeliveryNoteDetail>();
        #endregion Properties
    }
}
