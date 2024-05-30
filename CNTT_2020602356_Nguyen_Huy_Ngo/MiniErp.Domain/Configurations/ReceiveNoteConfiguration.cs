using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniErp.Domain.Configurations
{
    public class ReceiveNoteConfiguration : IEntityTypeConfiguration<ReceiveNote>
    {
        public void Configure(EntityTypeBuilder<ReceiveNote> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasMany(x => x.Details).WithOne(x => x.ReceiveNote).HasForeignKey(x => x.ReceiveNoteId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(x => x.Provider).WithMany().HasForeignKey(x => x.ProviderId).OnDelete(DeleteBehavior.NoAction);
        }
    }
}
