using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionCursos.Models;

[Table("Estudiantes")]
public class EstudianteModel : IValidatableObject
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int estudiante_id { get; set; }

    [Required(ErrorMessage = "Ingrese el nombre.")]
    [StringLength(100)]
    [Display(Name = "Nombre")]
    public string nombre { get; set; } = "";

    [Required(ErrorMessage = "Ingrese el apellido.")]
    [StringLength(100)]
    [Display(Name = "Apellido")]
    public string apellido { get; set; } = "";

    [Required(ErrorMessage = "Ingrese el correo electrónico.")]
    [EmailAddress(ErrorMessage = "Ingrese un correo válido.")]
    [StringLength(150)]
    [Display(Name = "Correo electrónico")]
    public string email { get; set; } = "";

    [Column(TypeName = "date")]
    [DataType(DataType.Date)]
    [Display(Name = "Fecha de nacimiento")]
    public DateTime fecha_nacimiento { get; set; } = DateTime.Today.AddYears(-18);

    public ICollection<InscripcionModel> Inscripciones { get; set; } = new List<InscripcionModel>();

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (fecha_nacimiento.Date > DateTime.Today)
            yield return new ValidationResult(
                "La fecha de nacimiento no puede ser futura.",
                new[] { nameof(fecha_nacimiento) });
    }
}
