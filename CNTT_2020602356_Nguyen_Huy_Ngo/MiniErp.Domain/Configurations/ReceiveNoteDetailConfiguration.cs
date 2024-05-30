using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniErp.Domain.Configurations
{
    public class ReceiveNoteDetailConfiguration : IEntityTypeConfiguration<ReceiveNoteDetail>
    {
        public void Configure(EntityTypeBuilder<ReceiveNoteDetail> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasOne(x=>x.Currency).WithMany().HasForeignKey(x=>x.CurrencyId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(x=>x.Product).WithMany().HasForeignKey(x=>x.ProductId).OnDelete(DeleteBehavior.NoAction);
        }
    }
}
