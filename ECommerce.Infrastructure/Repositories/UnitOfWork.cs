using ECommerce.Application.Common.Interfaces.Persistence;
using ECommerce.Domain.Entities;
using ECommerce.Infrastructure.Data;

namespace ECommerce.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ECommerceDbContext _context;

    private Repository<User>? _users;
    private Repository<Product>? _products;
    private Repository<Category>? _categories;
    private Repository<Order>? _orders;
    private Repository<OrderItem>? _orderItems;
    private Repository<RefreshToken>? _refreshTokens;

    public UnitOfWork(ECommerceDbContext context)
    {
        _context = context;
    }

    public IRepository<User> Users => _users ??= new Repository<User>(_context);

    public IRepository<Product> Products => _products ??= new Repository<Product>(_context);

    public IRepository<Category> Categories => _categories ??= new Repository<Category>(_context);

    public IRepository<Order> Orders => _orders ??= new Repository<Order>(_context);

    public IRepository<OrderItem> OrderItems => _orderItems ??= new Repository<OrderItem>(_context);

    public IRepository<RefreshToken> RefreshTokens => _refreshTokens ??= new Repository<RefreshToken>(_context);

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        await _context.DisposeAsync();
    }
}

