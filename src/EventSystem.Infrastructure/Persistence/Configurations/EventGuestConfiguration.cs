using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventSystem.Infrastructure.Persistence.Configurations;

public class EventGuestConfiguration : IEntityTypeConfiguration<EventGuest>
{
    public void Configure(EntityTypeBuilder<EventGuest> builder)
    {
        builder.ToTable("EventGuests");

        builder.HasKey(eg => new { eg.EventId, eg.UserId });

        builder.HasOne(eg => eg.Event)
               .WithMany(e => e.Guests)
               .HasForeignKey(eg => eg.EventId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(eg => eg.User)
               .WithMany(u => u.GuestEvents)
               .HasForeignKey(eg => eg.UserId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}