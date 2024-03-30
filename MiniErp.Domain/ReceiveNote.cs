using MiniErp.Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniErp.Domain
{
    public class ReceiveNote : IEntity
    {
        #region Properties
        [Required]
        public Guid Id { get; set; }

        [StringLength(50)]
        public string Code { get; set; }

        public Guid ProviderId { get; set; }

        [StringLength(50)]
        public string OrderCode { get; set; }

        public DateTime Date { get;set; }

        public virtual Provider Provider { get; set; }

        public virtual ICollection<ReceiveNoteDetail> Details { get; set; } = new List<ReceiveNoteDetail>();
        #endregion Properties

        #region Constructors
        public ReceiveNote()
        {
            Date = DateTime.Now;
        }
        #endregion Constructors
    }
}
