using LMS.Users.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Users.Infrastructure.EntityTypeConfigurations;

public class ContactsConfiguration : IEntityTypeConfiguration<Contacts>
{
    public void Configure(EntityTypeBuilder<Contacts> builder)
    {
        builder.ToTable("contacts", "users");
        
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Email)
               .HasMaxLength(255)
               .IsRequired();
        
        builder.Property(x => x.Phone)
               .HasMaxLength(32)
               .IsRequired(false);
    }
}