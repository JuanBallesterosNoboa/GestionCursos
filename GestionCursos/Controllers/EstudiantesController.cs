using GestionCursos.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionCursos.Controllers;

public class EstudiantesController : Controller
{
    private readonly GestionCursosDbContext _db;
    public EstudiantesController(GestionCursosDbContext db) => _db = db;

    // READ
    public async Task<IActionResult> Index() =>
        View(await _db.Estudiantes.AsNoTracking()
            .OrderBy(x => x.apellido).ThenBy(x => x.nombre).ToListAsync());

    public async Task<IActionResult> Details(int id)
    {
        var e = await _db.Estudiantes.AsNoTracking()
            .FirstOrDefaultAsync(x => x.estudiante_id == id);
        return e is null ? NotFound() : View(e);
    }

    // CREATE
    public IActionResult Create() => View(new EstudianteModel());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind("nombre,apellido,email,fecha_nacimiento")] EstudianteModel estudiante)
    {
        estudiante.email = (estudiante.email ?? "").Trim();
        if (await _db.Estudiantes.AnyAsync(x => x.email == estudiante.email))
            ModelState.AddModelError(nameof(estudiante.email), "Este correo ya está registrado.");
        if (!ModelState.IsValid) return View(estudiante);
        _db.Estudiantes.Add(estudiante);
        try { await _db.SaveChangesAsync(); }
        catch (DbUpdateException)
        {
            ModelState.AddModelError(nameof(estudiante.email), "No se pudo guardar; revise si el correo ya existe.");
            return View(estudiante);
        }
        TempData["Mensaje"] = "Estudiante registrado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    // UPDATE
    public async Task<IActionResult> Edit(int id)
    {
        var e = await _db.Estudiantes.FindAsync(id);
        return e is null ? NotFound() : View(e);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id,
        [Bind("estudiante_id,nombre,apellido,email,fecha_nacimiento")] EstudianteModel datos)
    {
        if (id != datos.estudiante_id) return BadRequest();
        datos.email = (datos.email ?? "").Trim();
        if (await _db.Estudiantes.AnyAsync(x => x.email == datos.email && x.estudiante_id != id))
            ModelState.AddModelError(nameof(datos.email), "Este correo ya está registrado.");
        if (!ModelState.IsValid) return View(datos);
        var e = await _db.Estudiantes.FindAsync(id);
        if (e is null) return NotFound();
        e.nombre = datos.nombre;
        e.apellido = datos.apellido;
        e.email = datos.email;
        e.fecha_nacimiento = datos.fecha_nacimiento;
        try { await _db.SaveChangesAsync(); }
        catch (DbUpdateException)
        {
            ModelState.AddModelError(nameof(datos.email), "No se pudo actualizar; compruebe el correo.");
            return View(datos);
        }
        TempData["Mensaje"] = "Estudiante actualizado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    // DELETE
    public async Task<IActionResult> Delete(int id)
    {
        var e = await _db.Estudiantes.AsNoTracking()
            .FirstOrDefaultAsync(x => x.estudiante_id == id);
        return e is null ? NotFound() : View(e);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var e = await _db.Estudiantes.FindAsync(id);
        if (e is null) return NotFound();
        if (await _db.Inscripciones.AnyAsync(x => x.estudiante_id == id))
        {
            TempData["Error"] = "El estudiante tiene inscripciones. Elimínelas primero.";
            return RedirectToAction(nameof(Index));
        }
        _db.Estudiantes.Remove(e);
        try { await _db.SaveChangesAsync(); }
        catch (DbUpdateException)
        {
            TempData["Error"] = "No se pudo eliminar: existen datos relacionados.";
            return RedirectToAction(nameof(Index));
        }
        TempData["Mensaje"] = "Estudiante eliminado correctamente.";
        return RedirectToAction(nameof(Index));
    }
}
