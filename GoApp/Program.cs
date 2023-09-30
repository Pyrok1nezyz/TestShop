using System.Text.Json;
using System.Text.Json.Serialization;
using GoApp.back_end.Entitys;
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
			builder.Services.AddSingleton<CustomerShoppingCartService>();
            builder.Services.AddHttpContextAccessor();

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
					var service = context.RequestServices.GetService<IDbContextFactory<SqlDbContext>>();
					var db = service.CreateDbContext();

					var categoryIdQuery = context.Request.Query["Id"];
					int categoryId = -1;
					int.TryParse(categoryIdQuery, out categoryId);

					var isFounded = db.Categories.Any(e => e.Id == categoryId);
					if (isFounded)
					{
						context.Response.StatusCode = StatusCodes.Status200OK;
						var list = db.Items.Where(e => e.CategoryId == categoryId).Include(e => e.Category).ToList();
						var check = new EntitysChecks();
						var checkedList = check.CheckErrorImagesInItemList(Environment.CurrentDirectory, list);
						await context.Response.WriteAsJsonAsync(checkedList, new JsonSerializerOptions()
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
						var check = new EntitysChecks();
						var checkedList = check.CheckErrorImagesInItemList(Environment.CurrentDirectory, list);
						await context.Response.WriteAsJsonAsync(checkedList, new JsonSerializerOptions()
						{
							ReferenceHandler = ReferenceHandler.IgnoreCycles,
							WriteIndented = true
						});
						return;
					}
				}

				//context.Response.Redirect("/");
			});

			app.MapGet("/searchitems", async (context) =>
			{
				if (!string.IsNullOrEmpty(context.Request.Query["q"]))
				{
					var service = context.RequestServices.GetService<IDbContextFactory<SqlDbContext>>();
					var db = service.CreateDbContext();

					var query = context.Request.Query["q"];
					var strictMode = false;
					bool.TryParse(context.Request.Query["sm"], out strictMode);

					var isFounded = db.Items.Where(e => e.Name.Contains(query)).Count();
					if (isFounded > 0 && strictMode)
					{
						context.Response.StatusCode = StatusCodes.Status200OK;
						var list = db.Items.Where(e => e.Name.ToLower().StartsWith(query.ToString().ToLower())).Include(e => e.Category).ToList();
						var check = new EntitysChecks();
						var checkedList = check.CheckErrorImagesInItemList(Environment.CurrentDirectory, list);
						await context.Response.WriteAsJsonAsync(checkedList, new JsonSerializerOptions()
						{
							ReferenceHandler = ReferenceHandler.IgnoreCycles,
							WriteIndented = true
						});
						return;
					}
					else if (isFounded > 0)
					{
						context.Response.StatusCode = StatusCodes.Status200OK;
						var list = db.Items.Where(e => e.Name.ToLower().Contains(query.ToString().ToLower())).Include(e => e.Category).ToList();
						var check = new EntitysChecks();
						var checkedList = check.CheckErrorImagesInItemList(Environment.CurrentDirectory, list);
						await context.Response.WriteAsJsonAsync(checkedList, new JsonSerializerOptions()
						{
							ReferenceHandler = ReferenceHandler.IgnoreCycles,
							WriteIndented = true
						});
						return;
					}
					else
					{
						context.Response.StatusCode = StatusCodes.Status404NotFound;
						await context.Response.StartAsync();
					}
				}
			});

			app.MapGet("/getitemview", async (context) =>
			{
				if (!string.IsNullOrEmpty(context.Request.Query["id"]))
				{
					var service = context.RequestServices.GetService<IDbContextFactory<SqlDbContext>>();
					var db = service.CreateDbContext();

					var query = context.Request.Query["id"];
					int queryItemId = -1;
					int.TryParse(query, out queryItemId);
					var founded = db.Items.Any(e => e.Id == queryItemId);
					if (founded)
					{
						context.Response.StatusCode = StatusCodes.Status200OK;
						var item = db.Items.Find(queryItemId);
						await context.Response.WriteAsJsonAsync(item);
						return;
					}
				}

				context.Response.StatusCode = StatusCodes.Status404NotFound;
				context.Response.StartAsync();
			});
		
			app.MapBlazorHub();
			app.MapFallbackToPage("/_Host");

			app.Run();
		}
	}
}