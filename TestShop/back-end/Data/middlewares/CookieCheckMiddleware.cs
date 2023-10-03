namespace TestShop.back_end.Data.middlewares;

public class CookieCheckMiddleware
{
	private readonly RequestDelegate next;
	private int DaysOfLiveCookies = 2;

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
					Expires = DateTime.Now.AddDays(DaysOfLiveCookies)
				};
				context.Response.Cookies.Append("guid", Guid.NewGuid().ToString(), cookieOptions);
			}
		}

		await next.Invoke(context);
	}
}