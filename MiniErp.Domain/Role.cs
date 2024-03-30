﻿using MiniErp.Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniErp.Domain
{
    public class Role : IEntity
    {
        #region Properties
        [Required]
        public Guid Id { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(50)]
        public string Permission { get; set; }
        #endregion Properties
    }
}
