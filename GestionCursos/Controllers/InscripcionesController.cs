using GestionCursos.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GestionCursos.Controllers;

public class InscripcionesController : Controller
{
    private readonly GestionCursosDbContext _db;
    public InscripcionesController(GestionCursosDbContext db) => _db = db;

    // READ: Inscripciones con nombre del curso y del estudiante
    public async Task<IActionResult> Index()
    {
        var lista = await _db.Inscripciones.AsNoTracking()
            .Include(x => x.Curso).Include(x => x.Estudiante)
            .OrderByDescending(x => x.fecha_inscripcion).ThenBy(x => x.inscripcion_id)
            .ToListAsync();
        return View(lista);
    }

    public async Task<IActionResult> Details(int id)
    {
        var item = await _db.Inscripciones.AsNoTracking()
            .Include(x => x.Curso).Include(x => x.Estudiante)
            .FirstOrDefaultAsync(x => x.inscripcion_id == id);
        return item is null ? NotFound() : View(item);
    }

    // Cargar listas desplegables para los formularios
    private async Task CargarCombos(int? cursoId = null, int? estudianteId = null)
    {
        var cursos = await _db.Cursos.AsNoTracking().OrderBy(x => x.nombre).ToListAsync();
        var estudiantes = await _db.Estudiantes.AsNoTracking()
            .OrderBy(x => x.apellido).ThenBy(x => x.nombre).ToListAsync();
        ViewBag.Cursos = new SelectList(cursos, "curso_id", "nombre", cursoId);
        ViewBag.Estudiantes = new SelectList(
            estudiantes.Select(x => new {
                x.estudiante_id,
                NombreCompleto = x.apellido + " " + x.nombre + " (" + x.email + ")"
            }), "estudiante_id", "NombreCompleto", estudianteId);
    }

    // CREATE
    public async Task<IActionResult> Create(int? cursoId)
    {
        await CargarCombos(cursoId);
        return View(new InscripcionModel { curso_id = cursoId ?? 0 });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind("curso_id,estudiante_id,fecha_inscripcion")] InscripcionModel item)
    {
        await ValidarInscripcion(item);
        if (!ModelState.IsValid)
        {
            await CargarCombos(item.curso_id, item.estudiante_id);
            return View(item);
        }
        _db.Inscripciones.Add(item);
        try { await _db.SaveChangesAsync(); }
        catch (DbUpdateException)
        {
            ModelState.AddModelError("", "No se pudo guardar. Compruebe que no esté inscrito dos veces.");
            await CargarCombos(item.curso_id, item.estudiante_id);
            return View(item);
        }
        TempData["Mensaje"] = "Inscripción registrada correctamente.";
        return RedirectToAction(nameof(Index));
    }

    // UPDATE
    public async Task<IActionResult> Edit(int id)
    {
        var item = await _db.Inscripciones.FindAsync(id);
        if (item is null) return NotFound();
        await CargarCombos(item.curso_id, item.estudiante_id);
        return View(item);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id,
        [Bind("inscripcion_id,curso_id,estudiante_id,fecha_inscripcion")] InscripcionModel datos)
    {
        if (id != datos.inscripcion_id) return BadRequest();
        await ValidarInscripcion(datos, id);
        if (!ModelState.IsValid)
        {
            await CargarCombos(datos.curso_id, datos.estudiante_id);
            return View(datos);
        }
        var actual = await _db.Inscripciones.FindAsync(id);
        if (actual is null) return NotFound();
        actual.curso_id = datos.curso_id;
        actual.estudiante_id = datos.estudiante_id;
        actual.fecha_inscripcion = datos.fecha_inscripcion;
        try { await _db.SaveChangesAsync(); }
        catch (DbUpdateException)
        {
            ModelState.AddModelError("", "No se pudo actualizar. Compruebe la inscripción.");
            await CargarCombos(datos.curso_id, datos.estudiante_id);
            return View(datos);
        }
        TempData["Mensaje"] = "Inscripción actualizada correctamente.";
        return RedirectToAction(nameof(Index));
    }

    private async Task ValidarInscripcion(InscripcionModel item, int? excepto = null)
    {
        if (item.curso_id > 0 && !await _db.Cursos.AnyAsync(x => x.curso_id == item.curso_id))
            ModelState.AddModelError(nameof(item.curso_id), "Curso no encontrado.");
        if (item.estudiante_id > 0 && !await _db.Estudiantes.AnyAsync(x => x.estudiante_id == item.estudiante_id))
            ModelState.AddModelError(nameof(item.estudiante_id), "Estudiante no encontrado.");
        if (item.curso_id > 0 && item.estudiante_id > 0 &&
            await _db.Inscripciones.AnyAsync(x => x.curso_id == item.curso_id &&
                x.estudiante_id == item.estudiante_id &&
                (!excepto.HasValue || x.inscripcion_id != excepto.Value)))
            ModelState.AddModelError("", "Este estudiante ya está inscrito en ese curso.");
    }

    // DELETE
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _db.Inscripciones.AsNoTracking()
            .Include(x => x.Curso).Include(x => x.Estudiante)
            .FirstOrDefaultAsync(x => x.inscripcion_id == id);
        return item is null ? NotFound() : View(item);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var item = await _db.Inscripciones.FindAsync(id);
        if (item is null) return NotFound();
        _db.Inscripciones.Remove(item);
        await _db.SaveChangesAsync();
        TempData["Mensaje"] = "Inscripción eliminada correctamente.";
        return RedirectToAction(nameof(Index));
    }
}
