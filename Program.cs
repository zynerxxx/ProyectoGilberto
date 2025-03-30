using Microsoft.EntityFrameworkCore;
using BibliotecaMVC;
using System.Globalization;
using Microsoft.Extensions.Localization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configurar la cadena de conexión
var connectionString = builder.Configuration.GetConnectionString("BibliotecaDB");
builder.Services.AddDbContext<BibliotecaDbContext>(options =>
    options.UseSqlServer(connectionString));

// Configurar idioma predeterminado
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("es");
    options.SupportedCultures = new[] { new CultureInfo("es") };
    options.SupportedUICultures = new[] { new CultureInfo("es") };
});

// Configurar autenticación
builder.Services.AddAuthentication("CookieAuth")
    .AddCookie("CookieAuth", options =>
    {
        options.LoginPath = "/Account/Login"; // Ruta para la página de login
        options.AccessDeniedPath = "/"; // Redirigir a la página de inicio en caso de acceso denegado
    });

var app = builder.Build();

// Aplicar configuración de localización
app.UseRequestLocalization();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage(); // Habilitar página de errores detallados en desarrollo
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication(); // Agregar autenticación
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
