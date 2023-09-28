using System.Text.Json;
using System.Text.Json.Serialization;
using GoApp.Data;
using GoApp.Db;
using GoApp.Db.Checks;
using GoApp.Entitys;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace GoApp
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.
			builder.Services.AddRazorPages(options => options.RootDirectory = "/front-end/Pages");
			builder.Services.AddServerSideBlazor();
			builder.Services.AddSingleton<WeatherForecastService>();

			var connection = builder.Configuration.GetConnectionString("DefaultConnection");
			builder.Services.AddDbContextFactory<SqlDbContext>(options =>
			{
				options.UseSqlServer(connection);
				options.EnableSensitiveDataLogging();
			});

			builder.Services.AddHttpClient();

			var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (!app.Environment.IsDevelopment())
			{
				app.UseExceptionHandler("/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseHttpsRedirection();

			app.UseStaticFiles();

			app.UseRouting();

			app.MapGet("/items", (SqlDbContext db) =>
			{
				var items = db.Items.AsNoTracking().ToList();
				var check = new EntitysChecks();
				var checkedItems = check.CheckErrorImagesInItemList(Environment.CurrentDirectory, items);
				return checkedItems;
			});

			app.MapGet("/categories", (SqlDbContext db) =>
			{
				var categories = db.Categories.AsNoTracking().ToList();
				return categories;
			});

			app.MapGet("/categoryitems", async (context) =>
			{
				if (!string.IsNullOrEmpty(context.Request.Query["Id"]))
				{
					var service = app.Services.GetService<IDbContextFactory<SqlDbContext>>();
					var db = service.CreateDbContext();

					var categoryIdQuery = context.Request.Query["Id"];
					int categoryId = -1;
					int.TryParse(categoryIdQuery, out categoryId);

					var isFounded = db.Categories.Any(e => e.Id == categoryId);
					if (isFounded)
					{
						context.Response.StatusCode = StatusCodes.Status200OK;
						var list = db.Items.Where(e => e.CategoryId == categoryId).Include(e => e.Category).ToList();
						await context.Response.WriteAsJsonAsync(list, new JsonSerializerOptions()
						{
							ReferenceHandler = ReferenceHandler.IgnoreCycles,
							WriteIndented = true
						});
						return;
					} 
					else if (categoryId == 0)
					{
						context.Response.StatusCode = StatusCodes.Status200OK;
						var list = db.Items.Include(e => e.Category).ToList();
						await context.Response.WriteAsJsonAsync(list, new JsonSerializerOptions()
						{
							ReferenceHandler = ReferenceHandler.IgnoreCycles,
							WriteIndented = true
						});
						return;
					}
				}

				context.Response.Redirect("/");
			});

			//app.MapGet("/category", async context =>
			//{
			//	context.Response.Redirect("");
			//})
		
			app.MapBlazorHub();
			app.MapFallbackToPage("/_Host");

			app.Run();
		}
	}
}