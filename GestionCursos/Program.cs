using GestionCursos.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

// Configuración de MySQL (Pomelo); la contraseña no va en el código fuente.
var conexion = builder.Configuration.GetConnectionString("cn")
    ?? throw new InvalidOperationException("Configura ConnectionStrings:cn en appsettings.json.");
builder.Services.AddDbContext<GestionCursosDbContext>(opciones =>
    opciones.UseMySql(conexion, ServerVersion.AutoDetect(conexion)));

var app = builder.Build();
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
// No usar EnsureCreated: las tablas se crean con Add-Migration y Update-Database.
app.Run();
