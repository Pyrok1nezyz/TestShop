namespace TestShop.back_end.Data.middlewares;

public class CookieCheckMiddleware
{
	private readonly RequestDelegate next;

	public CookieCheckMiddleware(RequestDelegate next)
	{
		this.next = next;
	}

	public async Task InvokeAsync(HttpContext context)
	{
		var coockieCheck = "";
		var isParsed = false;
		try
		{
			isParsed = context.Request.Cookies.TryGetValue("guid", out coockieCheck);
		}
		finally
		{
			if (!isParsed)
			{
				var cookieOptions = new CookieOptions
				{
					Expires = DateTime.Now.AddHours(48)
				};
				context.Response.Cookies.Append("guid", Guid.NewGuid().ToString(), cookieOptions);
			}
		}

		await next.Invoke(context);
	}
}