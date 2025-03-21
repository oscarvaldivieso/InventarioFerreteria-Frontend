using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ferreteria_Frontend.Models
{
    public class CategoriaViewModel
    {
        [Display(Name = "Id")]
        public int Cate_Id { get; set; }

        [Display(Name = "Categoría")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string Cate_Descripcion { get; set; }
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
