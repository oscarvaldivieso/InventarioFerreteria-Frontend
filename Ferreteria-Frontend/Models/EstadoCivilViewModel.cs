using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ferreteria_Frontend.Models
{
    public class EstadoCivilViewModel
    {
        [Display(Name = "Id")]
        public int EsCv_Id { get; set; }

        [Display(Name = "Estado Civil")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string EsCv_Descripcion { get; set; }

        public int Usua_Creacion { get; set; }

        public DateTime Feca_Creacion { get; set; }

        public int? Usua_Modificacion { get; set; }

        public DateTime? Feca_Modificacion { get; set; }
        
        [NotMapped]
        public string? UsuaC_Nombre { get; set; }

        [NotMapped]
        public string? UsuaM_Nombre { get; set; }
    }
}
