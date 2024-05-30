using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace MiniErp.Domain.Configurations
{
    public class TimeKeepingConfiguration : IEntityTypeConfiguration<TimeKeeping>
    {
        public void Configure(EntityTypeBuilder<TimeKeeping> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasOne(x=>x.User).WithMany().HasForeignKey(x=>x.UserId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
