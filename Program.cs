using System.Security.Claims;
using IdentityService.Extensions;
using IdentityService.Data;
using IdentityService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using IdentityService.Services.Interfaces;
using SixLabors.ImageSharp.Web.DependencyInjection;
using IdentityService.Services;
using System.Security;
using IdentityService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using FmsProtos.Grpc;
using StackExchange.Redis;


var builder = WebApplication.CreateBuilder(args);

// Load configuration from appsettings.json
// builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Add logging to check loaded configuration
var logger = LoggerFactory.Create(config =>
{
  config.AddConsole();
}).CreateLogger("Program");

var jwtSection = builder.Configuration.GetSection("Jwt");
var jwtSettings = jwtSection.Get<JwtSettings>();
logger.LogInformation("JWT Configuration Loaded: {@JwtSettings}", jwtSettings);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
  options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure Redis
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
  var configuration = ConfigurationOptions.Parse(builder.Configuration.GetConnectionString("Redis"), true);
  configuration.AbortOnConnectFail = false;
  return ConnectionMultiplexer.Connect(configuration);
});

// Add Identity services and configure role management
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<ICustomUserService, CustomUserService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRoleService, RoleService>();
// builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IPermissionManager, PermissionManager>();
builder.Services.AddScoped<IPermissionStoreService, PermissionStoreService>();
builder.Services.AddScoped<IRedisService, RedisService>();
builder.Services.AddScoped<IOTPService, OTPService>();

// Register the SmsPanel
builder.Services.AddScoped<SmsPanel>();
builder.Services.AddScoped<MailPanel>();

// Register OtpSms for dependency injection
builder.Services.AddScoped<OtpSms>();

MassTransitConfig.ConfigureMassTransit(builder.Services, builder.Configuration);




builder.Services.AddGrpcClient<OrganizationService.OrganizationServiceClient>(options =>
{
  options.Address = new Uri(builder.Configuration["GrpcSettings:AssetManagerUrl"]);
}).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
  ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
});

builder.Services.AddScoped<OrganizationGrpcClient>();




builder.Services.AddCors(options =>
{
  options.AddPolicy("MyCorsPolicy", builder =>
  {
    builder.WithOrigins(["http://localhost:3000", "https://localhost:3000", "https://whatsapp.bugtech.ir"])
           .AllowAnyMethod()
           .AllowAnyHeader()
           .AllowCredentials(); // Adjust the policy according to your needs
  });
});
// Bind JwtSettings
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));

// Configure Authentication
builder.Services.AddAuthentication(options =>
{
  options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
  options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
  options.TokenValidationParameters = new TokenValidationParameters
  {
    ValidateIssuer = true,
    ValidateAudience = true,
    ValidateLifetime = true,
    ValidateIssuerSigningKey = true,
    ValidIssuer = jwtSettings.Issuer,
    ValidAudience = jwtSettings.Audience,
    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key))
  };
});

builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddImageSharp();



var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
  var services = scope.ServiceProvider;
  try
  {
    var context = services.GetRequiredService<ApplicationDbContext>();
    context.Database.Migrate();
    PermissionSeeder.SeedPermissionsAsync(services).Wait();
  }
  catch (Exception ex)
  {
    var logger2 = services.GetRequiredService<ILogger<Program>>();
    logger2.LogError(ex, "An error occurred seeding the DB.");
  }
}


app.Use(async (context, next) =>
{
  var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
  if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
  {
    var token = authHeader.Substring("Bearer ".Length).Trim();
    context.Request.Headers["Cookie"] = $"jwt={token}";
  }

  await next();
});


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
}
app.UseCors("MyCorsPolicy");
app.UseDeveloperExceptionPage();
app.UseSwagger();
app.UseSwaggerUI();

// app.UseHttpsRedirection(); //it is not a good option to use in production
app.UseStaticFiles(); // Serve static files for images



app.UseRouting();
app.UseMiddleware<JwtMiddleware>(); // Add JWT middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();



// using (var scope = app.Services.CreateScope())
// {
//   var services = scope.ServiceProvider;
//   try
//   {
//     var context = services.GetRequiredService<ApplicationDbContext>();
//     context.Database.Migrate();
//     PermissionSeeder.SeedPermissionsAsync(services).Wait();
//     UserSeeder.SeedAdminUserAsync(services).Wait();
//   }
//   catch (Exception ex)
//   {
//     var logger2 = services.GetRequiredService<ILogger<Program>>();
//     logger2.LogError(ex, "An error occurred seeding the DB.");
//   }
// }
