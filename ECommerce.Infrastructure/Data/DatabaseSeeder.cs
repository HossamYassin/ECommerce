using ECommerce.Domain.Entities;
using ECommerce.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ECommerce.Infrastructure.Data;

public class DatabaseSeeder
{
    private readonly ECommerceDbContext _context;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly ILogger<DatabaseSeeder> _logger;

    public DatabaseSeeder(
        ECommerceDbContext context,
        IPasswordHasher<User> passwordHasher,
        ILogger<DatabaseSeeder> logger)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            // Ensure database is created
            await _context.Database.MigrateAsync(cancellationToken);

            // Seed Users
            if (!await _context.Users.AnyAsync(cancellationToken))
            {
                await SeedUsersAsync(cancellationToken);
                _logger.LogInformation("Seeded Users");
            }

            // Seed Categories
            if (!await _context.Categories.AnyAsync(cancellationToken))
            {
                await SeedCategoriesAsync(cancellationToken);
                _logger.LogInformation("Seeded Categories");
            }

            // Seed Products
            if (!await _context.Products.AnyAsync(cancellationToken))
            {
                await SeedProductsAsync(cancellationToken);
                _logger.LogInformation("Seeded Products");
            }

            // Save changes before seeding orders (orders depend on users and products)
            await _context.SaveChangesAsync(cancellationToken);

            // Seed Orders
            if (!await _context.Orders.AnyAsync(cancellationToken))
            {
                await SeedOrdersAsync(cancellationToken);
                _logger.LogInformation("Seeded Orders");
            }

            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Database seeding completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while seeding database");
            throw;
        }
    }

    private async Task SeedUsersAsync(CancellationToken cancellationToken)
    {
        var users = new List<User>
        {
            new User
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Name = "Admin User",
                Email = "admin@ecommerce.com",
                Role = UserRole.Admin,
                PasswordHash = string.Empty
            },
            new User
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Name = "John Doe",
                Email = "john.doe@example.com",
                Role = UserRole.Customer,
                PasswordHash = string.Empty
            },
            new User
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                Name = "Jane Smith",
                Email = "jane.smith@example.com",
                Role = UserRole.Customer,
                PasswordHash = string.Empty
            },
            new User
            {
                Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                Name = "Bob Johnson",
                Email = "bob.johnson@example.com",
                Role = UserRole.Customer,
                PasswordHash = string.Empty
            }
        };

        // Hash passwords
        foreach (var user in users)
        {
            user.PasswordHash = _passwordHasher.HashPassword(user, "Password123!");
        }

        await _context.Users.AddRangeAsync(users, cancellationToken);
    }

    private async Task SeedCategoriesAsync(CancellationToken cancellationToken)
    {
        var categories = new List<Category>
        {
            new Category
            {
                Id = Guid.Parse("AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA"),
                Name = "Electronics",
                Description = "Electronic devices and accessories"
            },
            new Category
            {
                Id = Guid.Parse("BBBBBBBB-BBBB-BBBB-BBBB-BBBBBBBBBBBB"),
                Name = "Clothing",
                Description = "Men's and women's clothing"
            },
            new Category
            {
                Id = Guid.Parse("CCCCCCCC-CCCC-CCCC-CCCC-CCCCCCCCCCCC"),
                Name = "Books",
                Description = "Books and reading materials"
            },
            new Category
            {
                Id = Guid.Parse("DDDDDDDD-DDDD-DDDD-DDDD-DDDDDDDDDDDD"),
                Name = "Home & Garden",
                Description = "Home improvement and garden supplies"
            },
            new Category
            {
                Id = Guid.Parse("EEEEEEEE-EEEE-EEEE-EEEE-EEEEEEEEEEEE"),
                Name = "Sports & Outdoors",
                Description = "Sports equipment and outdoor gear"
            }
        };

        await _context.Categories.AddRangeAsync(categories, cancellationToken);
    }

    private async Task SeedProductsAsync(CancellationToken cancellationToken)
    {
        var electronicsCategoryId = Guid.Parse("AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA");
        var clothingCategoryId = Guid.Parse("BBBBBBBB-BBBB-BBBB-BBBB-BBBBBBBBBBBB");
        var booksCategoryId = Guid.Parse("CCCCCCCC-CCCC-CCCC-CCCC-CCCCCCCCCCCC");
        var homeGardenCategoryId = Guid.Parse("DDDDDDDD-DDDD-DDDD-DDDD-DDDDDDDDDDDD");
        var sportsCategoryId = Guid.Parse("EEEEEEEE-EEEE-EEEE-EEEE-EEEEEEEEEEEE");

        var products = new List<Product>
        {
            // Electronics
            new Product
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000001"),
                Name = "Smartphone Pro Max",
                Description = "Latest generation smartphone with advanced features",
                Price = 999.99m,
                CategoryId = electronicsCategoryId,
                StockQuantity = 50,
                IsActive = true
            },
            new Product
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000002"),
                Name = "Wireless Headphones",
                Description = "Premium noise-cancelling wireless headphones",
                Price = 249.99m,
                CategoryId = electronicsCategoryId,
                StockQuantity = 100,
                IsActive = true
            },
            new Product
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000003"),
                Name = "Laptop Ultrabook",
                Description = "High-performance laptop for professionals",
                Price = 1299.99m,
                CategoryId = electronicsCategoryId,
                StockQuantity = 30,
                IsActive = true
            },
            new Product
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000004"),
                Name = "Smart Watch",
                Description = "Fitness tracking smartwatch with health monitoring",
                Price = 299.99m,
                CategoryId = electronicsCategoryId,
                StockQuantity = 75,
                IsActive = true
            },

            // Clothing
            new Product
            {
                Id = Guid.Parse("20000000-0000-0000-0000-000000000001"),
                Name = "Classic T-Shirt",
                Description = "Comfortable cotton t-shirt in multiple colors",
                Price = 19.99m,
                CategoryId = clothingCategoryId,
                StockQuantity = 200,
                IsActive = true
            },
            new Product
            {
                Id = Guid.Parse("20000000-0000-0000-0000-000000000002"),
                Name = "Denim Jeans",
                Description = "Premium quality denim jeans",
                Price = 79.99m,
                CategoryId = clothingCategoryId,
                StockQuantity = 150,
                IsActive = true
            },
            new Product
            {
                Id = Guid.Parse("20000000-0000-0000-0000-000000000003"),
                Name = "Winter Jacket",
                Description = "Warm and stylish winter jacket",
                Price = 149.99m,
                CategoryId = clothingCategoryId,
                StockQuantity = 80,
                IsActive = true
            },

            // Books
            new Product
            {
                Id = Guid.Parse("30000000-0000-0000-0000-000000000001"),
                Name = "Programming Guide",
                Description = "Comprehensive guide to modern programming",
                Price = 39.99m,
                CategoryId = booksCategoryId,
                StockQuantity = 120,
                IsActive = true
            },
            new Product
            {
                Id = Guid.Parse("30000000-0000-0000-0000-000000000002"),
                Name = "Mystery Novel",
                Description = "Bestselling mystery thriller",
                Price = 14.99m,
                CategoryId = booksCategoryId,
                StockQuantity = 200,
                IsActive = true
            },
            new Product
            {
                Id = Guid.Parse("30000000-0000-0000-0000-000000000003"),
                Name = "Cookbook Collection",
                Description = "Delicious recipes from around the world",
                Price = 29.99m,
                CategoryId = booksCategoryId,
                StockQuantity = 90,
                IsActive = true
            },

            // Home & Garden
            new Product
            {
                Id = Guid.Parse("40000000-0000-0000-0000-000000000001"),
                Name = "Garden Tool Set",
                Description = "Complete set of gardening tools",
                Price = 49.99m,
                CategoryId = homeGardenCategoryId,
                StockQuantity = 60,
                IsActive = true
            },
            new Product
            {
                Id = Guid.Parse("40000000-0000-0000-0000-000000000002"),
                Name = "Coffee Maker",
                Description = "Programmable coffee maker with timer",
                Price = 89.99m,
                CategoryId = homeGardenCategoryId,
                StockQuantity = 40,
                IsActive = true
            },

            // Sports & Outdoors
            new Product
            {
                Id = Guid.Parse("50000000-0000-0000-0000-000000000001"),
                Name = "Running Shoes",
                Description = "Professional running shoes for athletes",
                Price = 119.99m,
                CategoryId = sportsCategoryId,
                StockQuantity = 100,
                IsActive = true
            },
            new Product
            {
                Id = Guid.Parse("50000000-0000-0000-0000-000000000002"),
                Name = "Yoga Mat",
                Description = "Non-slip premium yoga mat",
                Price = 34.99m,
                CategoryId = sportsCategoryId,
                StockQuantity = 150,
                IsActive = true
            },
            new Product
            {
                Id = Guid.Parse("50000000-0000-0000-0000-000000000003"),
                Name = "Dumbbell Set",
                Description = "Adjustable dumbbell set for home workouts",
                Price = 159.99m,
                CategoryId = sportsCategoryId,
                StockQuantity = 25,
                IsActive = true
            }
        };

        await _context.Products.AddRangeAsync(products, cancellationToken);
    }

    private async Task SeedOrdersAsync(CancellationToken cancellationToken)
    {
        // Customer IDs
        var johnDoeId = Guid.Parse("22222222-2222-2222-2222-222222222222");
        var janeSmithId = Guid.Parse("33333333-3333-3333-3333-333333333333");
        var bobJohnsonId = Guid.Parse("44444444-4444-4444-4444-444444444444");

        // Product IDs
        var smartphoneId = Guid.Parse("10000000-0000-0000-0000-000000000001");
        var headphonesId = Guid.Parse("10000000-0000-0000-0000-000000000002");
        var laptopId = Guid.Parse("10000000-0000-0000-0000-000000000003");
        var smartwatchId = Guid.Parse("10000000-0000-0000-0000-000000000004");
        var tshirtId = Guid.Parse("20000000-0000-0000-0000-000000000001");
        var jeansId = Guid.Parse("20000000-0000-0000-0000-000000000002");
        var jacketId = Guid.Parse("20000000-0000-0000-0000-000000000003");
        var programmingBookId = Guid.Parse("30000000-0000-0000-0000-000000000001");
        var mysteryNovelId = Guid.Parse("30000000-0000-0000-0000-000000000002");
        var cookbookId = Guid.Parse("30000000-0000-0000-0000-000000000003");
        var runningShoesId = Guid.Parse("50000000-0000-0000-0000-000000000001");
        var yogaMatId = Guid.Parse("50000000-0000-0000-0000-000000000002");

        // Get products for prices
        var products = await _context.Products.ToListAsync(cancellationToken);
        var productLookup = products.ToDictionary(p => p.Id);

        var orders = new List<Order>
        {
            // Order 1: John Doe - Completed order (Electronics)
            new Order
            {
                Id = Guid.Parse("A1111111-1111-1111-1111-111111111111"),
                CustomerId = johnDoeId,
                OrderDate = DateTime.UtcNow.AddDays(-10),
                Status = OrderStatus.Completed,
                TotalAmount = 0, // Will be calculated
                Items = new List<OrderItem>
                {
                    new OrderItem
                    {
                        Id = Guid.Parse("B1111111-1111-1111-1111-111111111111"),
                        ProductId = smartphoneId,
                        Quantity = 1,
                        PriceAtOrder = productLookup[smartphoneId].Price
                    },
                    new OrderItem
                    {
                        Id = Guid.Parse("B1111111-1111-1111-1111-111111111112"),
                        ProductId = headphonesId,
                        Quantity = 1,
                        PriceAtOrder = productLookup[headphonesId].Price
                    }
                }
            },

            // Order 2: Jane Smith - Pending order (Clothing & Books)
            new Order
            {
                Id = Guid.Parse("A2222222-2222-2222-2222-222222222222"),
                CustomerId = janeSmithId,
                OrderDate = DateTime.UtcNow.AddDays(-3),
                Status = OrderStatus.Pending,
                TotalAmount = 0,
                Items = new List<OrderItem>
                {
                    new OrderItem
                    {
                        Id = Guid.Parse("B2222222-2222-2222-2222-222222222221"),
                        ProductId = jeansId,
                        Quantity = 2,
                        PriceAtOrder = productLookup[jeansId].Price
                    },
                    new OrderItem
                    {
                        Id = Guid.Parse("B2222222-2222-2222-2222-222222222222"),
                        ProductId = mysteryNovelId,
                        Quantity = 3,
                        PriceAtOrder = productLookup[mysteryNovelId].Price
                    },
                    new OrderItem
                    {
                        Id = Guid.Parse("B2222222-2222-2222-2222-222222222223"),
                        ProductId = cookbookId,
                        Quantity = 1,
                        PriceAtOrder = productLookup[cookbookId].Price
                    }
                }
            },

            // Order 3: Bob Johnson - Cancelled order (Electronics)
            new Order
            {
                Id = Guid.Parse("A3333333-3333-3333-3333-333333333333"),
                CustomerId = bobJohnsonId,
                OrderDate = DateTime.UtcNow.AddDays(-7),
                Status = OrderStatus.Cancelled,
                TotalAmount = 0,
                Items = new List<OrderItem>
                {
                    new OrderItem
                    {
                        Id = Guid.Parse("B3333333-3333-3333-3333-333333333331"),
                        ProductId = laptopId,
                        Quantity = 1,
                        PriceAtOrder = productLookup[laptopId].Price
                    }
                }
            },

            // Order 4: John Doe - Pending order (Sports & Clothing)
            new Order
            {
                Id = Guid.Parse("A4444444-4444-4444-4444-444444444444"),
                CustomerId = johnDoeId,
                OrderDate = DateTime.UtcNow.AddDays(-1),
                Status = OrderStatus.Pending,
                TotalAmount = 0,
                Items = new List<OrderItem>
                {
                    new OrderItem
                    {
                        Id = Guid.Parse("B4444444-4444-4444-4444-444444444441"),
                        ProductId = runningShoesId,
                        Quantity = 1,
                        PriceAtOrder = productLookup[runningShoesId].Price
                    },
                    new OrderItem
                    {
                        Id = Guid.Parse("B4444444-4444-4444-4444-444444444442"),
                        ProductId = tshirtId,
                        Quantity = 3,
                        PriceAtOrder = productLookup[tshirtId].Price
                    }
                }
            },

            // Order 5: Jane Smith - Completed order (Electronics & Sports)
            new Order
            {
                Id = Guid.Parse("A5555555-5555-5555-5555-555555555555"),
                CustomerId = janeSmithId,
                OrderDate = DateTime.UtcNow.AddDays(-15),
                Status = OrderStatus.Completed,
                TotalAmount = 0,
                Items = new List<OrderItem>
                {
                    new OrderItem
                    {
                        Id = Guid.Parse("B5555555-5555-5555-5555-555555555551"),
                        ProductId = smartwatchId,
                        Quantity = 1,
                        PriceAtOrder = productLookup[smartwatchId].Price
                    },
                    new OrderItem
                    {
                        Id = Guid.Parse("B5555555-5555-5555-5555-555555555552"),
                        ProductId = yogaMatId,
                        Quantity = 2,
                        PriceAtOrder = productLookup[yogaMatId].Price
                    }
                }
            },

            // Order 6: Bob Johnson - Pending order (Clothing)
            new Order
            {
                Id = Guid.Parse("A6666666-6666-6666-6666-666666666666"),
                CustomerId = bobJohnsonId,
                OrderDate = DateTime.UtcNow.AddHours(-5),
                Status = OrderStatus.Pending,
                TotalAmount = 0,
                Items = new List<OrderItem>
                {
                    new OrderItem
                    {
                        Id = Guid.Parse("B6666666-6666-6666-6666-666666666661"),
                        ProductId = jacketId,
                        Quantity = 1,
                        PriceAtOrder = productLookup[jacketId].Price
                    },
                    new OrderItem
                    {
                        Id = Guid.Parse("B6666666-6666-6666-6666-666666666662"),
                        ProductId = tshirtId,
                        Quantity = 2,
                        PriceAtOrder = productLookup[tshirtId].Price
                    }
                }
            },

            // Order 7: John Doe - Completed order (Books)
            new Order
            {
                Id = Guid.Parse("A7777777-7777-7777-7777-777777777777"),
                CustomerId = johnDoeId,
                OrderDate = DateTime.UtcNow.AddDays(-20),
                Status = OrderStatus.Completed,
                TotalAmount = 0,
                Items = new List<OrderItem>
                {
                    new OrderItem
                    {
                        Id = Guid.Parse("B7777777-7777-7777-7777-777777777771"),
                        ProductId = programmingBookId,
                        Quantity = 2,
                        PriceAtOrder = productLookup[programmingBookId].Price
                    },
                    new OrderItem
                    {
                        Id = Guid.Parse("B7777777-7777-7777-7777-777777777772"),
                        ProductId = mysteryNovelId,
                        Quantity = 1,
                        PriceAtOrder = productLookup[mysteryNovelId].Price
                    }
                }
            },

            // Order 8: Jane Smith - Cancelled order (Electronics)
            new Order
            {
                Id = Guid.Parse("A8888888-8888-8888-8888-888888888888"),
                CustomerId = janeSmithId,
                OrderDate = DateTime.UtcNow.AddDays(-5),
                Status = OrderStatus.Cancelled,
                TotalAmount = 0,
                Items = new List<OrderItem>
                {
                    new OrderItem
                    {
                        Id = Guid.Parse("B8888888-8888-8888-8888-888888888881"),
                        ProductId = headphonesId,
                        Quantity = 2,
                        PriceAtOrder = productLookup[headphonesId].Price
                    }
                }
            }
        };

        // Calculate total amounts for each order and set UpdatedDate
        foreach (var order in orders)
        {
            order.TotalAmount = order.Items.Sum(item => item.PriceAtOrder * item.Quantity);
            
            // Set UpdatedDate: for cancelled orders, set it after OrderDate
            // For others, set it to OrderDate
            if (order.Status == OrderStatus.Cancelled)
            {
                order.UpdatedDate = order.OrderDate.AddDays(1);
            }
            else
            {
                order.UpdatedDate = order.OrderDate;
            }
        }

        await _context.Orders.AddRangeAsync(orders, cancellationToken);

        // Update product stock quantities to reflect seed orders
        // For completed and pending orders: reduce stock (reserved/consumed)
        // For cancelled orders: stock was already returned, so don't reduce
        foreach (var order in orders.Where(o => o.Status == OrderStatus.Completed || o.Status == OrderStatus.Pending))
        {
            foreach (var item in order.Items)
            {
                if (productLookup.TryGetValue(item.ProductId, out var product))
                {
                    // Reduce stock for completed and pending orders
                    // For seed data, we assume the stock was already at the initial value
                    product.StockQuantity = Math.Max(0, product.StockQuantity - item.Quantity);
                }
            }
        }
    }
}

