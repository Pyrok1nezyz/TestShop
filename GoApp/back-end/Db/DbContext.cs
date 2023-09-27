using GoApp.Entitys;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace GoApp.Db;

public class SqlDbContext : Microsoft.EntityFrameworkCore.DbContext
{
	public DbSet<Item> Items { get; set; } = null!;
	public DbSet<Category> Categories { get; set; } = null!;
	public SqlDbContext(DbContextOptions<SqlDbContext> options)
		: base(options)
	{
		Database.EnsureCreated();
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Item>().HasOne(e => e.Category)
			.WithMany(e => e.Items).HasForeignKey(e => e.CategoryId);
	}
}