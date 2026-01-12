using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ferreteria_Frontend.Models
{
    public class ProductoViewModel
    {
        [Display(Name = "Id")]
        public int Prod_Id { get; set; }

        [Display(Name = "Producto")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string Prod_Descripcion { get; set; }

        [Display(Name = "Marca")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int Marc_Id { get; set; }

        [Display(Name = "Categoria")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int Cate_Id { get; set; }

        [Display(Name = "Proveedor")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int Prov_Id { get; set; }

        [Display(Name = "Modelo")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string Prod_Modelo { get; set; }

        [Display(Name = "Cantidad")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int Prod_Cantidad { get; set; }

        public int Usua_Creacion { get; set; }

        public DateTime Feca_Creacion { get; set; }

        public int? Usua_Modificacion { get; set; }

        public DateTime? Feca_Modificacion { get; set; }

        public bool? Prod_Estado { get; set; }

        [Display(Name = "Imagen")]
        public string? Prod_URLImg { get; set; }

        public IFormFile Prod_Imagen { get; set; }


        [Display(Name = "Medida")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int? Medi_Id { get; set; }

        [NotMapped]
        public string? UsuarioCreacion { get; set; }

        [NotMapped]
        public string? UsuarioModificacion { get; set; }

        [NotMapped]
        public string? Marc_Descripcion { get; set; }

        [NotMapped]
        public string? Medi_Descripcion { get; set; }

        [NotMapped]
        public string? Cate_Descripcion { get; set; }

        [NotMapped]
        public string? Prov_Nombre { get; set; }
    }
}