using LMS.Users.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Users.Infrastructure.EntityTypeConfigurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users", "users");
        
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Username)
               .HasMaxLength(128)
               .IsRequired();
        
        builder.Property(x => x.Bio)
               .HasMaxLength(1024)
               .IsRequired(false);
        
        builder.Property(x => x.AvatarUrl)
               .HasMaxLength(512)
               .IsRequired(false);
        
        builder.HasOne<Contacts>()
               .WithOne()
               .HasForeignKey<User>(x => x.ContactsId);
    }
}