using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using TechBlog.Domain.ValueObjects;

namespace TechBlog.Infrastructure.Identity.Configuration.Mappings
{
    public sealed class UserMapping : IEntityTypeConfiguration<EntityFrameworkIdentityUser>
    {
        public void Configure(EntityTypeBuilder<EntityFrameworkIdentityUser> builder)
        {
            builder.HasKey(b => b.Id);

            builder.Property(b => b.Id).IsRequired();

            builder.Property(b => b.BlogUserType)
                              .IsRequired()
                              .HasMaxLength(50)
                              .HasConversion(benum => Enum.GetName(benum),
                                             bname => Enum.Parse<BlogUserType>(bname));

            builder.Property(b => b.EmailConfirmed).HasDefaultValue(true);

            builder.Property(b => b.Name).HasMaxLength(300).IsRequired();

            builder.Property(b => b.Email).IsRequired();

            builder.Property(b => b.UserName).IsRequired();

            builder.Property(b => b.Enabled).HasDefaultValue(true).IsRequired();

            builder.Property(b => b.CreatedAt).HasDefaultValue(DateTime.Now).IsRequired();

            builder.Property(b => b.LastUpdateAt).HasDefaultValue(DateTime.Now).IsRequired();

            builder.ToTable("AspNetUsers");
        }
    }
}