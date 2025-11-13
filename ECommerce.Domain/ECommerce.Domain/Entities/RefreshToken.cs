namespace ECommerce.Domain.Entities;

public class RefreshToken : BaseEntity
{
    public required string Token { get; set; }

    public DateTime ExpiresAt { get; set; }

    public DateTime? RevokedAt { get; set; }

    public string? ReplacedByToken { get; set; }

    public bool IsRevoked => RevokedAt.HasValue || IsExpired;

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;

    public Guid UserId { get; set; }

    public User? User { get; set; }
}

