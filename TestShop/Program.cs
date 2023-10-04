using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Razor.Language.Intermediate;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using TestShop.back_end.Data.middlewares;
using TestShop.Entitys;

namespace TestShop
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			builder.Services.AddRazorPages(options => options.RootDirectory = "/front-end/Pages");
			builder.Services.AddServerSideBlazor();
			builder.Services.AddHttpContextAccessor();
			builder.Services.AddScoped<CustomerShoppingCartService>();
			builder.Services.AddControllers();

			var connection = builder.Configuration.GetConnectionString("DefaultConnection");
			builder.Services.AddDbContextFactory<SqlDbContext>(options =>
			{
				options.UseSqlServer(connection);
				options.EnableSensitiveDataLogging();
			});

			builder.Services.AddHttpClient();

			var app = builder.Build();

			if (!app.Environment.IsDevelopment())
			{
				app.UseExceptionHandler("/Error");
				app.UseHsts();
			}

			app.UseHttpsRedirection();

			app.UseStaticFiles();

			app.UseRouting();

			app.UseMiddleware<CookieCheckMiddleware>();

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
					var db = service!.CreateDbContext();

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

				context.Response.StatusCode = StatusCodes.Status404NotFound;
				await context.Response.StartAsync();
			});

			app.MapGet("/searchitems", async (context) =>
			{
				if (!string.IsNullOrEmpty(context.Request.Query["q"]))
				{
					var service = context.RequestServices.GetService<IDbContextFactory<SqlDbContext>>();
					var db = service!.CreateDbContext();

					var query = context.Request.Query["q"];
					var strictMode = false;
					if (!bool.TryParse(context.Request.Query["sm"], out strictMode) || !StringValues.IsNullOrEmpty(query)) return;

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
					var db = service!.CreateDbContext();

					var query = context.Request.Query["id"];
					int queryItemId = -1;
					int.TryParse(query, out queryItemId);
					var founded = db.Items.Any(e => e.Id == queryItemId);
					if (founded)
					{
						var errorCheck = new EntitysChecks();
						context.Response.StatusCode = StatusCodes.Status200OK;
						var item = db.Items.Include(e => e.Category).First(e => e.Id == queryItemId);
						var checkedItem = errorCheck.CheckErrorImage(Environment.CurrentDirectory, item);
						await context.Response.WriteAsJsonAsync(checkedItem);
						return;
					}
				}

				context.Response.StatusCode = StatusCodes.Status404NotFound;
				await context.Response.StartAsync();
			});

			///

			app.MapGet("/cart", async (context) =>
			{
				if (!string.IsNullOrEmpty(context.Request.Query["id"]))
				{
					var service = context.RequestServices.GetService<IDbContextFactory<SqlDbContext>>();
					var db = service!.CreateDbContext();

					var query = context.Request.Query["id"];
					Guid queryCartId;
					Guid.TryParse(query, out queryCartId);
					var founded = db.CustomersShoppingCart.Any(e => e.Id == queryCartId);
					if (founded)
					{
						context.Response.StatusCode = StatusCodes.Status200OK;
						var item = db.CustomersShoppingCart.Include(e => e.ListItems).ThenInclude(e => e.Item).ThenInclude(e => e.Category).First(e => e.Id == queryCartId);
						await context.Response.WriteAsJsonAsync(item);
						return;
					}
				}

				context.Response.StatusCode = StatusCodes.Status404NotFound;
			});

			app.MapPost("/cart", async (context) =>
			{
				var service = context.RequestServices.GetService<IDbContextFactory<SqlDbContext>>();
				var db = service!.CreateDbContext();

				var cart = await context.Request.ReadFromJsonAsync<CustomerShoppingCart>();
				var isFounded = db.CustomersShoppingCart.Any(e => cart != null && e.Id == cart.Id);
				if (!isFounded && cart is not null)
				{
					db.CustomersShoppingCart.Add(cart);
					await db.SaveChangesAsync();
				}
			});

			app.MapPut("/cart", async (context) =>
			{
				var service = context.RequestServices.GetService<IDbContextFactory<SqlDbContext>>();
				var db = service!.CreateDbContext();

				var cart = await context.Request.ReadFromJsonAsync<CustomerShoppingCart>();
				var isFounded = db.CustomersShoppingCart.Any(e => cart != null && e.Id == cart.Id);
				if (isFounded && cart is not null)
				{
					var existingCart = db.CustomersShoppingCart.Include(e => e.ListItems).ThenInclude(e => e.Item)
						.FirstOrDefault(e => e.Id == cart.Id);

					if (existingCart != null)
					{
						// Обновление скалярных свойств
						db.Entry(existingCart).CurrentValues.SetValues(cart);

						// Обновление ListItems
						foreach (var newItemCounter in cart.ListItems)
						{
							var existingItemCounter = existingCart.ListItems.FirstOrDefault(e => e.Id == newItemCounter.Id);

							if (existingItemCounter != null)
							{
								// Обновление скалярных свойств ItemCounter
								db.Entry(existingItemCounter).CurrentValues.SetValues(newItemCounter);
							}
							else
							{
								// Добавление нового ItemCounter
								existingCart.ListItems.Add(newItemCounter);
							}
						}

						// Удаление удаленных ListItems
						foreach (var existingItemCounter in existingCart.ListItems.ToList())
						{
							if (!cart.ListItems.Any(e => e.Id == existingItemCounter.Id))
							{
								existingCart.ListItems.Remove(existingItemCounter);
								db.Entry(existingItemCounter).State = EntityState.Deleted;
							}
						}

						await db.SaveChangesAsync();
					}
				}
			});

			app.MapGet("/newitemtocustomercard", async (context) =>
			{
				var service = context.RequestServices.GetService<IDbContextFactory<SqlDbContext>>();
				var db = service!.CreateDbContext();
				StringValues queryItemId;
				int itemId;
				

				if (!context.Request.Query.TryGetValue("id", out queryItemId)) return;
				if (!int.TryParse(queryItemId, out itemId)) return;
				var item = db.Items.FirstOrDefault(e => e.Id == itemId);
				if(item is null) return;

				string? cookie;
				Guid guidItem = Guid.NewGuid();
				if (context.Request.Cookies.TryGetValue("guid", out cookie))
				{
					if (Guid.TryParse(cookie, out guidItem))
					{
						if (db.CustomersShoppingCart.Any(e => e.Id == guidItem))
						{
							var cart = db.CustomersShoppingCart.Include(e => e.ListItems).ThenInclude(e => e.Item).First(e => e.Id == guidItem);
							if (cart.ListItems != null && !cart.ListItems.Exists(e => e.Item.Id == itemId))
							{
								cart.LastCheck = DateTime.Now;
								cart.ListItems.Add(new CustomerShoppingCart.ItemCounter()
								{
									Item = item,
									Count = 1
								});
								db.CustomersShoppingCart.Update(cart);
								await db.SaveChangesAsync();

								context.Response.Redirect("/shoppingcart");
								return;
							}
							else
							{
								context.Response.Redirect("/shoppingcart");
								return;
							}
						}
					}
				}

				var itemCounter = new CustomerShoppingCart.ItemCounter()
				{
					Item = item,
					Count = 1
				};
				var newCart = new CustomerShoppingCart()
				{
					Id = guidItem,
					ListItems = new List<CustomerShoppingCart.ItemCounter>() { itemCounter }
				};
				db.CustomersShoppingCart.Add(newCart);
				await db.SaveChangesAsync();

				context.Response.Cookies.Append("guid", newCart.Id.ToString());
				context.Response.Redirect("/shoppingcart");
			});

			app.MapBlazorHub();
			app.MapFallbackToPage("/_Host");

			app.Run();
		}
	}
}