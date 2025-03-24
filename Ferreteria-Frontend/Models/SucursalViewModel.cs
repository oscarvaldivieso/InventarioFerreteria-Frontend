using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ferreteria_Frontend.Models
{
    public class SucursalViewModel
    {
        [Display(Name = "Id")]
        public int Sucu_Id { get; set; }

        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string Sucu_Nombre { get; set; }

        [NotMapped]
        [Display(Name = "Departamento")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string Depa_Codigo { get; set; }

        [Display(Name = "Municipio")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string Muni_Codigo { get; set; }

        [Display(Name = "Dirección Exacta")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string Sucu_DireccionExacta { get; set; }

        public int Usua_Creacion { get; set; }

        public DateTime? Feca_Creacion { get; set; }

        public int? Usua_Modificacion { get; set; }

        public DateTime? Feca_Modificacion { get; set; }

        public bool? Sucu_Estado { get; set; }

        [NotMapped]
        public string? UsuaC_Nombre { get; set; }
        [NotMapped]
        public string? UsuaM_Nombre { get; set; }
    }
}
