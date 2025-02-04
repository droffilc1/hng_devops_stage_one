using System.Net;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://0.0.0.0:5000");

builder.Services.AddCors(options =>
{
  options.AddPolicy("AllowAll", policy =>
  {
    policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
  });
});

builder.Services.Configure<JsonOptions>(options =>
{
  options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

// Configure forwarded headers
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
  options.KnownProxies.Add(IPAddress.Parse("44.203.123.37"));
});


var app = builder.Build();

// Use forwarded headers middleware
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
  ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});


app.UseCors("AllowAll");

bool IsPrime(int n)
{
  if (n <= 1)
    return false;
  if (n <= 3)
    return true;
  if (n % 2 == 0 || n % 3 == 0)
    return false;
  for (int i = 5; i * i <= n; i += 6)
  {
    if (n % i == 0 || n % (i + 2) == 0)
      return false;
  }
  return true;
}

bool IsPerfect(int n)
{
  if (n < 2)
    return false;
  int sum = 1;
  for (int i = 2; i <= Math.Sqrt(n); i++)
  {
    if (n % i == 0)
    {
      sum += i;
      if (i != n / i)
        sum += n / i;
    }
  }
  return sum == n;
}

bool IsArmstrong(int n)
{
  var digits = n.ToString().ToCharArray();
  int power = digits.Length;
  int sum = 0;
  foreach (char c in digits)
  {
    int d = int.Parse(c.ToString());
    sum += (int)Math.Pow(d, power);
  }
  return sum == n;
}

int DigitSum(int n)
{
  int sum = 0;
  foreach (char c in n.ToString())
  {
    sum += int.Parse(c.ToString());
  }
  return sum;
}

string[] GetProperties(int n)
{
  bool armstrong = IsArmstrong(n);
  bool odd = n % 2 != 0;

  if (armstrong)
  {
    return odd ? ["armstrong", "odd"] : ["armstrong", "even"];
  }
  else
  {
    return odd ? ["odd"] : ["even"];
  }
}

var httpClient = new HttpClient();

// GET /api/classify-number?number=371
app.MapGet("/api/classify-number", async (HttpContext context) =>
{
  var query = context.Request.Query;
  if (!query.ContainsKey("number") || string.IsNullOrWhiteSpace(query["number"]))
  {
    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
    return Results.Json(new { number = "", error = true });
  }

  // Try to parse the input number
  if (!int.TryParse(query["number"], out int number))
  {
    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
    return Results.Json(new { number = query["number"].ToString(), error = true });
  }

  // Retrieve fun fact from Numbers API
  string funFact;
  try
  {
    funFact = await httpClient.GetStringAsync($"http://numbersapi.com/{number}/math");
  }
  catch
  {
    funFact = "No fun fact available.";
  }

  var response = new
  {
    number,
    is_prime = IsPrime(number),
    is_perfect = IsPerfect(number),
    properties = GetProperties(number),
    digit_sum = DigitSum(number),
    fun_fact = funFact
  };

  return Results.Json(response);
});

// Fallback endpoint for invalid paths
app.MapFallback(() => Results.NotFound("Endpoint not found"));

app.Run();
