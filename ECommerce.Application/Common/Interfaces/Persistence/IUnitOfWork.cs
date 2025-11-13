using ECommerce.Domain.Entities;

namespace ECommerce.Application.Common.Interfaces.Persistence;

public interface IUnitOfWork : IAsyncDisposable
{
    IRepository<User> Users { get; }

    IRepository<Product> Products { get; }

    IRepository<Category> Categories { get; }

    IRepository<Order> Orders { get; }

    IRepository<OrderItem> OrderItems { get; }

    IRepository<RefreshToken> RefreshTokens { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

