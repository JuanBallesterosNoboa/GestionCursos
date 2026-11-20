using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionCursos.Models;

[Table("Inscripciones")]
public class InscripcionModel
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int inscripcion_id { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Seleccione un curso.")]
    [Display(Name = "Curso")]
    public int curso_id { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Seleccione un estudiante.")]
    [Display(Name = "Estudiante")]
    public int estudiante_id { get; set; }

    [Column(TypeName = "date")]
    [DataType(DataType.Date)]
    [Display(Name = "Fecha de inscripción")]
    public DateTime fecha_inscripcion { get; set; } = DateTime.Today;

    [ForeignKey(nameof(curso_id))]
    public CursoModel? Curso { get; set; }

    [ForeignKey(nameof(estudiante_id))]
    public EstudianteModel? Estudiante { get; set; }
}
