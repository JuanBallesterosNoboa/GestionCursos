using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionCursos.Models;

[Table("Cursos")]
public class CursoModel : IValidatableObject
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int curso_id { get; set; }

    [Required(ErrorMessage = "Ingrese el nombre del curso.")]
    [StringLength(120)]
    [Display(Name = "Nombre del curso")]
    public string nombre { get; set; } = "";

    [Required(ErrorMessage = "Ingrese la descripción.")]
    [StringLength(600)]
    [Display(Name = "Descripción")]
    public string descripcion { get; set; } = "";

    [Required]
    [Column(TypeName = "date")]
    [DataType(DataType.Date)]
    [Display(Name = "Fecha de inicio")]
    public DateTime fecha_inicio { get; set; } = DateTime.Today;

    [Required]
    [Column(TypeName = "date")]
    [DataType(DataType.Date)]
    [Display(Name = "Fecha de finalización")]
    public DateTime fecha_fin { get; set; } = DateTime.Today.AddMonths(1);

    public ICollection<InscripcionModel> Inscripciones { get; set; } = new List<InscripcionModel>();

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (fecha_fin.Date < fecha_inicio.Date)
            yield return new ValidationResult(
                "La fecha de fin debe ser igual o posterior a la fecha de inicio.",
                new[] { nameof(fecha_fin) });
    }
}
