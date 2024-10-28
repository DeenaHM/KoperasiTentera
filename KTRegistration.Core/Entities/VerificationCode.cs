using System.ComponentModel.DataAnnotations.Schema;

namespace KTRegistration.Core.Entities;
public class VerificationCode
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser? User { get; set; }
    public int Code { get; set; } = default;
    public DateTime ExpiryDate { get; set; }
    public VerificationPurpose Purpose { get; set; }
    public bool IsUsed { get; set; }

}