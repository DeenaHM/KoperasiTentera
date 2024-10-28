namespace KTRegistration.Infrastructure.Persistence.EntitiesConfigurations;
public class VerificationCodeConfiguration : IEntityTypeConfiguration<VerificationCode>
{
    public void Configure(EntityTypeBuilder<VerificationCode> builder)
    {
        builder.HasKey(vc => vc.Id);

        builder.Property(vc => vc.UserId)
            .IsRequired()
            .HasMaxLength(450);

        builder.Property(vc => vc.Code)
            .IsRequired();

        builder.Property(vc => vc.ExpiryDate)
            .IsRequired();

        builder.Property(vc => vc.Purpose)
            .IsRequired();

        builder.Property(vc => vc.IsUsed)
            .IsRequired();

        // Relationships
        builder.HasOne(vc => vc.User)
            .WithMany(au => au.VerificationCodes)
            .HasForeignKey(vc => vc.UserId)
            .OnDelete(DeleteBehavior.Cascade);// we can Restrict if we need soft delete for users and their data

        builder.HasIndex(vc => vc.Code);
        builder.HasIndex(vc => vc.ExpiryDate);
    }
}
