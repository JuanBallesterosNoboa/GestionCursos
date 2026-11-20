using GestionCursos.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionCursos.Controllers;

public class CursosController : Controller
{
    private readonly GestionCursosDbContext _db;
    public CursosController(GestionCursosDbContext db) => _db = db;

    // READ: Listar
    public async Task<IActionResult> Index() =>
        View(await _db.Cursos.AsNoTracking().OrderBy(x => x.nombre).ToListAsync());

    // READ: Ver detalle
    public async Task<IActionResult> Details(int id)
    {
        var curso = await _db.Cursos.AsNoTracking()
            .FirstOrDefaultAsync(x => x.curso_id == id);
        return curso is null ? NotFound() : View(curso);
    }

    // CREATE
    public IActionResult Create() => View(new CursoModel());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind("nombre,descripcion,fecha_inicio,fecha_fin")] CursoModel curso)
    {
        if (!ModelState.IsValid) return View(curso);
        _db.Cursos.Add(curso);
        await _db.SaveChangesAsync();
        TempData["Mensaje"] = "Curso registrado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    // UPDATE
    public async Task<IActionResult> Edit(int id)
    {
        var curso = await _db.Cursos.FindAsync(id);
        return curso is null ? NotFound() : View(curso);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id,
        [Bind("curso_id,nombre,descripcion,fecha_inicio,fecha_fin")] CursoModel datos)
    {
        if (id != datos.curso_id) return BadRequest();
        if (!ModelState.IsValid) return View(datos);
        var curso = await _db.Cursos.FindAsync(id);
        if (curso is null) return NotFound();
        curso.nombre = datos.nombre;
        curso.descripcion = datos.descripcion;
        curso.fecha_inicio = datos.fecha_inicio;
        curso.fecha_fin = datos.fecha_fin;
        await _db.SaveChangesAsync();
        TempData["Mensaje"] = "Curso actualizado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    // DELETE (confirmación por formulario)
    public async Task<IActionResult> Delete(int id)
    {
        var curso = await _db.Cursos.AsNoTracking()
            .FirstOrDefaultAsync(x => x.curso_id == id);
        return curso is null ? NotFound() : View(curso);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var curso = await _db.Cursos.FindAsync(id);
        if (curso is null) return NotFound();
        if (await _db.Inscripciones.AnyAsync(x => x.curso_id == id))
        {
            TempData["Error"] = "El curso tiene inscripciones. Elimínelas primero.";
            return RedirectToAction(nameof(Index));
        }
        _db.Cursos.Remove(curso);
        try { await _db.SaveChangesAsync(); }
        catch (DbUpdateException)
        {
            TempData["Error"] = "No se pudo eliminar: existen datos relacionados.";
            return RedirectToAction(nameof(Index));
        }
        TempData["Mensaje"] = "Curso eliminado correctamente.";
        return RedirectToAction(nameof(Index));
    }
}
