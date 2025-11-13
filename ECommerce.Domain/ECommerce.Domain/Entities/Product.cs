namespace ECommerce.Domain.Entities;

public class Product : BaseEntity
{
    public required string Name { get; set; }

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public Guid CategoryId { get; set; }

    public Category? Category { get; set; }

    public int StockQuantity { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}

