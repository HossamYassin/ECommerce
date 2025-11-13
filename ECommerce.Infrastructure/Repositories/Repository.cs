using System.Linq.Expressions;
using ECommerce.Application.Common.Interfaces.Persistence;
using ECommerce.Domain.Entities;
using ECommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Repositories;

public class Repository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly ECommerceDbContext Context;
    protected readonly DbSet<T> DbSet;

    public Repository(ECommerceDbContext context)
    {
        Context = context;
        DbSet = context.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet.FindAsync([id], cancellationToken);
    }

    public virtual async Task<IReadOnlyList<T>> ListAsync(
        Expression<Func<T, bool>>? predicate = null,
        CancellationToken cancellationToken = default,
        params Expression<Func<T, object>>[] includes)
    {
        var query = BuildQueryable(predicate, includes);
        return await query.ToListAsync(cancellationToken);
    }

    public virtual async Task<T?> FirstOrDefaultAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default,
        params Expression<Func<T, object>>[] includes)
    {
        var query = BuildQueryable(predicate, includes);
        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    public virtual async Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        entity.CreatedDate = DateTime.UtcNow;
        await DbSet.AddAsync(entity, cancellationToken);
    }

    public virtual async Task<(IReadOnlyList<T> Items, int TotalCount)> GetPagedAsync(
        Expression<Func<T, bool>>? predicate,
        int pageNumber,
        int pageSize,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        CancellationToken cancellationToken = default,
        params Expression<Func<T, object>>[] includes)
    {
        pageNumber = Math.Max(1, pageNumber);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var query = BuildQueryable(predicate, includes);
        var totalCount = await query.CountAsync(cancellationToken);

        if (orderBy is not null)
        {
            query = orderBy(query);
        }

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public virtual async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        foreach (var entity in entities)
        {
            entity.CreatedDate = DateTime.UtcNow;
        }

        await DbSet.AddRangeAsync(entities, cancellationToken);
    }

    public virtual Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        entity.UpdatedDate = DateTime.UtcNow;
        DbSet.Update(entity);
        return Task.CompletedTask;
    }

    public virtual Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        entity.IsDeleted = true;
        entity.DeletedDate = DateTime.UtcNow;
        DbSet.Update(entity);
        return Task.CompletedTask;
    }

    public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await DbSet.AnyAsync(predicate, cancellationToken);
    }

    protected IQueryable<T> BuildQueryable(
        Expression<Func<T, bool>>? predicate,
        params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = DbSet.AsQueryable();

        if (predicate is not null)
        {
            query = query.Where(predicate);
        }

        if (includes is not null)
        {
            query = includes.Aggregate(query, (current, include) => current.Include(include));
        }

        return query;
    }
}

