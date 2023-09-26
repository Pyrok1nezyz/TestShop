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
		Database.EnsureDeleted();
		Database.EnsureCreated();
		// создаем базу данных при первом обращении
	}

	//protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	///	=> optionsBuilder.UseSqlServer("Server=localhost;Database=TestShop;Trusted_Connection=true;TrustServerCertificate=True;");

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		var category1 = new Category() { Id = 1, Name = "Телевизоры" };
		var category2 = new Category() { Id = 2, Name = "Телефоны" };
		var category3 = new Category() { Id = 3, Name = "Бытовая техника" };

		//modelBuilder.Entity<Category>().HasData(category1, category2, category3);

		modelBuilder.Entity<Item>(b =>
		{
			b.HasData(new Item()
			{
				Id = 1,
				Customer = "Aihaoh41",
				CategoryId = 1,
				Name = "SAMSUNG N200",
				//Category = category1,
				Description = "Discription №1",
				ImageString = "d:\\image.png",
				Price = 3500f
			});
			//new Item()
			//{
			//	Id = 2,
			//	Customer = "Pyrokxnezxz",
			//	CategoryId = 2,
			//	Name = "IPhone 15",
			//	Category = category2,
			//	Description = "Discription №2",
			//	ImageString = "d:\\image2.png",
			//	Price = 80000.24f
			//},
			//new Item()
			//{
			//	Id = 3,
			//	Customer = "BlackPowder",
			//	CategoryId = 3,
			//	Category = category3,
			//	Name = "Плита",
			//	Description = "Discription №3",
			//	ImageString = "d:\\image3.png",
			//	Price = 24522f
			//});
			b.OwnsOne(e => e.Category).HasData(category1);
		});

		//modelBuilder.Entity<Category>().OwnsMany(e => e.Items);

		//modelBuilder.Entity<Category>()  .HasMany(e => e.Items).WithOne(e => e.Category).HasForeignKey(e => e.CategoryId).IsRequired();
		//modelBuilder.Entity<Item>().HasOne(e => e.Category).WithMany(e => e.Items).HasForeignKey(e => e.CategoryId).IsRequired();

		//modelBuilder.Entity<Item>().UseTpcMappingStrategy();
	}
}