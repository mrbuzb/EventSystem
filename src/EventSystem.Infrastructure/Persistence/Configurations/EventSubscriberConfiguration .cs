using EventSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventSystem.Infrastructure.Persistence.Configurations
{
    public class EventSubscriberConfiguration : IEntityTypeConfiguration<EventSubscriber>
    {
        public void Configure(EntityTypeBuilder<EventSubscriber> builder)
        {
            builder.ToTable("EventSubscribers");

            builder.HasKey(es => new { es.EventId, es.UserId });

            builder.HasOne(es => es.Event)
                   .WithMany(e => e.SubscribedUsers)
                   .HasForeignKey(es => es.EventId);

            builder.HasOne(es => es.User)
                   .WithMany(u => u.SubscribedEvents)
                   .HasForeignKey(es => es.UserId);

            builder.Property(es => es.SubscribedAt)
                   .HasDefaultValueSql("GETUTCDATE()")
                   .IsRequired();
        }
    }
}
