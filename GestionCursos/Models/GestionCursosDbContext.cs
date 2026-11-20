using Microsoft.EntityFrameworkCore;

namespace GestionCursos.Models;

public class GestionCursosDbContext : DbContext
{
    public GestionCursosDbContext(DbContextOptions<GestionCursosDbContext> opciones)
        : base(opciones) { }

    public DbSet<CursoModel> Cursos => Set<CursoModel>();
    public DbSet<EstudianteModel> Estudiantes => Set<EstudianteModel>();
    public DbSet<InscripcionModel> Inscripciones => Set<InscripcionModel>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<EstudianteModel>()
            .HasIndex(x => x.email).IsUnique();

        modelBuilder.Entity<InscripcionModel>()
            .HasIndex(x => new { x.curso_id, x.estudiante_id }).IsUnique();

        modelBuilder.Entity<InscripcionModel>()
            .HasOne(x => x.Curso)
            .WithMany(x => x.Inscripciones)
            .HasForeignKey(x => x.curso_id)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<InscripcionModel>()
            .HasOne(x => x.Estudiante)
            .WithMany(x => x.Inscripciones)
            .HasForeignKey(x => x.estudiante_id)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
