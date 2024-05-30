using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniErp.Domain.Configurations
{
    public class DeliveryNoteConfiguration : IEntityTypeConfiguration<DeliveryNote>
    {
        public void Configure(EntityTypeBuilder<DeliveryNote> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasMany(x=>x.Details).WithOne(x=>x.DeliveryNote).HasForeignKey(x=>x.DeliveryNoteId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(x => x.Customer).WithMany().HasForeignKey(x => x.CustomerId).OnDelete(DeleteBehavior.NoAction);
        }
    }
}
