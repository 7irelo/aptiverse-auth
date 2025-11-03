using Aptiverse.Domain.Models;
using Aptiverse.Domain.Models.Users;
using Aptiverse.Infrastructure.Data;
using BenchmarkDotNet.Attributes;
using Microsoft.EntityFrameworkCore;

[MemoryDiagnoser]
public class UserRepositoryBenchmarks
{
    private ApplicationDbContext _context;

    [GlobalSetup]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql("Host=aptiverse-dev.cfki4qim2woi.eu-north-1.rds.amazonaws.com;Port=5432;Database=aptiverse-dev;Username=postgres;Password=09Y4VMdH97zOjIt3nHQv;")
            .Options;
        _context = new ApplicationDbContext(options);

        _context.Database.EnsureCreated();
        //_context.Users.Add(new ApplicationUser
        //{
        //    Id = 1,
        //    FirstName = "Test",
        //    LastName = "User",
        //    TypeId = 1,
        //    CategoryId = 1
        //});
        //_context.SaveChanges();
    }

    [Benchmark]
    public async Task AddUser_LINQ()
    {
        //_context.Set<ApplicationUser>().Add(new ApplicationUser
        //{
        //    FirstName = "John",
        //    LastName = "Doe",
        //    TypeId = 1,
        //    CategoryId = 1,
        //});
        await _context.SaveChangesAsync();
    }

    [Benchmark]
    public async Task AddUser_RawSQL()
    {
        //var user = new ApplicationUser
        //{
        //    FirstName = "John",
        //    LastName = "Cena",
        //    TypeId = 1,
        //    CategoryId = 1,
        //    Type = new UserType { Id = 1, DisplayName = "Superuser" },
        //    Category = new UserCategory { Id = 1, DisplayName = "User" }
        //};

        using var transaction = await _context.Database.BeginTransactionAsync();

        //try
        //{
        //    // Insert Type if not exists (PostgreSQL syntax)
        //    await _context.Database.ExecuteSqlRawAsync(
        //        @"INSERT INTO UserTypes (Id, DisplayName)
        //      VALUES ({0}, {1})
        //      ON CONFLICT (Id) DO NOTHING",
        //        user.Type.Id, user.Type.DisplayName);

        //    // Insert Category if not exists
        //    await _context.Database.ExecuteSqlRawAsync(
        //        @"INSERT INTO UserCategories (Id, DisplayName)
        //      VALUES ({0}, {1})
        //      ON CONFLICT (Id) DO NOTHING",
        //        user.Category.Id, user.Category.DisplayName);

        //    // Insert User
        //    await _context.Database.ExecuteSqlRawAsync(
        //        @"INSERT INTO Users (FirstName, LastName, TypeId, CategoryId)
        //      VALUES ({0}, {1}, {2}, {3})",
        //        user.FirstName, user.LastName, user.Type.Id, user.Category.Id);

        //    await transaction.CommitAsync();
        //}
        //catch
        //{
        //    await transaction.RollbackAsync();
        //    throw;
        //}
    }

    [Benchmark]
    public async Task<ApplicationUser?> GetUser_LINQ()
    {
        return await _context.Set<ApplicationUser>().FindAsync(1);
    }

    [Benchmark]
    public async Task<ApplicationUser?> GetUser_RawSQL()
    {
        return await _context.Users
            .FromSqlRaw("SELECT * FROM Users WHERE Id = 1")
            .AsNoTracking()
            .FirstOrDefaultAsync();
    }

    [GlobalCleanup]
    public void Cleanup() => _context.Dispose();
}