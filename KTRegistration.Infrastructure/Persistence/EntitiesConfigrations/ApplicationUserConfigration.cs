namespace KTRegistration.Infrastructure.Persistence.EntitiesConfigurations;
public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(u => u.FullName)
               .HasMaxLength(250)
               .IsRequired();

        builder.Property(u => u.DisplayPhoneNumber)
               .HasMaxLength(20)
               .IsRequired();

        builder.Property(u => u.HasAgreedToTerms)
               .IsRequired();

        builder.Property(u => u.IsBiometricEnabled)
               .IsRequired();

        builder.Property(u => u.IsMigrated)
               .IsRequired();
    }
}
