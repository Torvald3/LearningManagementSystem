using LMS.Users.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Users.Infrastructure.EntityTypeConfigurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users", "users"); //TODO: create migrations
        
        builder.HasKey(x => x.Id);
        
        builder.HasIndex(x => x.Username)
               .IsUnique();

        builder.Property(x => x.Username)
               .HasMaxLength(128)
               .IsRequired();
        
        builder.Property(x => x.Bio)
               .HasMaxLength(1024)
               .IsRequired(false);
        
        builder.Property(x => x.AvatarUrl)
               .HasMaxLength(512)
               .IsRequired(false);
        
        builder.HasOne(x => x.Contacts)
               .WithOne()
               .HasForeignKey<User>(x => x.ContactsId)
               .IsRequired();
    }
}