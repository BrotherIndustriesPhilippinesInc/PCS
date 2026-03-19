using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using PartsControlSystem.Data;
using PartsControlSystem.DTO;
using PartsControlSystem.Services;

ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

var builder = WebApplication.CreateBuilder(args);


// Add controllers with views   
builder.Services.AddControllersWithViews();

// SQL Server
builder.Services.AddDbContext<SqlServerAppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServerConnection")));

// SQL Server
builder.Services.AddDbContext<SqlServerAppDbContextCas>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServerConnection_CAS")));

// PostgresSQL
builder.Services.AddDbContext<PostgreAppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IUpdateActivityMapperService, UpdateActivityMapperService>();
//builder.Services.AddScoped<MP1SaveService>();
//builder.Services.AddScoped<MP2SaveService>();

builder.Services.AddHttpContextAccessor();

builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddTransient<MailService>();

builder.Services.AddScoped<ExcelApprovalService>();

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 50 * 1024 * 1024; // 50 MB
});

// ✅ Add authentication with cookies
builder.Services.AddAuthentication("MyCookieAuth")
    .AddCookie("MyCookieAuth", options =>
    {
        options.LoginPath = "/Home/IportalConfirmationForm"; // redirect if not logged in
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });

var app = builder.Build();

// Configure pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// ✅ Enable routing
app.UseRouting();

// ✅ Enable authentication & authorization
app.UseAuthentication();
app.UseAuthorization();

//app.MapControllers();

// Default route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Login}/{id?}");

app.Run();
