using GoApp.back_end.Data;
using GoApp.back_end.Entitys;
using GoApp.Entitys;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace GoApp.Db;

public class SqlDbContext : Microsoft.EntityFrameworkCore.DbContext
{
	public DbSet<Item> Items { get; set; }
	public DbSet<Category> Categories { get; set; } 
    public DbSet<CustomerShoppingCart> CustomersShoppingCart { get; set; }
	public SqlDbContext(DbContextOptions<SqlDbContext> options)
		: base(options)
	{
		Database.EnsureCreated();
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Item>().HasOne(e => e.Category)
			.WithMany().HasForeignKey(e => e.CategoryId);
	}
}