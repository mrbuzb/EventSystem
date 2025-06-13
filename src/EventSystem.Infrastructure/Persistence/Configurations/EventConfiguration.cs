using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace EventSystem.Infrastructure.Persistence.Configurations
{
    public class EventConfiguration : IEntityTypeConfiguration<Event>
    {
        public void Configure(EntityTypeBuilder<Event> builder)
        {
            builder.ToTable("Events");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Title).HasMaxLength(100).IsRequired();
            builder.Property(e => e.Description).HasMaxLength(1000);
            builder.Property(e => e.Location).HasMaxLength(255);
            builder.Property(e => e.Capasity).IsRequired();

            builder.HasOne(e => e.Creator)
                   .WithMany(u => u.CreatedEvents)
                   .HasForeignKey(e => e.CreatorId)
                   .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
