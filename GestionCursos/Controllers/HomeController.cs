using GestionCursos.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionCursos.Controllers;

public class HomeController : Controller
{
    private readonly GestionCursosDbContext _db;
    public HomeController(GestionCursosDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        var resumen = new DashboardViewModel
        {
            TotalCursos = await _db.Cursos.CountAsync(),
            TotalEstudiantes = await _db.Estudiantes.CountAsync(),
            TotalInscripciones = await _db.Inscripciones.CountAsync()
        };
        return View(resumen);
    }

    public IActionResult Error() => View();
}
