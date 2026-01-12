using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ferreteria_Frontend.Models
{
    public class ProveedoresViewModel
    {
        [Display(Name = "Id")]
        public int Prov_Id { get; set; }

        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string Prov_Nombre { get; set; }

        [Display(Name = "Contacto")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string Prov_Contacto { get; set; }

        [Display(Name = "Municipio")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string Muni_Codigo { get; set; }

        [Display(Name = "Dirección Exacta")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string Prov_DireccionExacta { get; set; }

        public int Usua_Creacion { get; set; }

        public DateTime Feca_Creacion { get; set; }

        public int? Usua_Modificacion { get; set; }

        public DateTime? Feca_Modificacion { get; set; }

        public bool? Prov_Estado { get; set; }

        [NotMapped]
        public string? UsuarioCreacion { get; set; }

        [NotMapped]
        public string? UsuarioModificacion { get; set; }

        [NotMapped]
        public string? Municipio{ get; set; }

        [NotMapped]
        [Display(Name = "Departamento")]
        public string? Depa_Codigo { get; set; }

        [NotMapped]
        public string? Departamento { get; set; }
    }
}