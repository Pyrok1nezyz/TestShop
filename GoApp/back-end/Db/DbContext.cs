using Microsoft.EntityFrameworkCore;

namespace TestShop.Entitys;

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
