using MiniErp.Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MiniErp.Domain
{
    public class TimeKeeping : IEntity
    {
        #region Properties
        [Required]
        public Guid Id { get; set; }

        [Required]
        public Guid UserId { get; set; }

        public DateTime CheckTime { get; set; }

        public DateTime Date { get; set; }

        public virtual User User { get; set; }
        #endregion Properties
    }
}
