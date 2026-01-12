using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ferreteria_Frontend.Models
{
    public class CompraViewModel
    {
        [NotMapped]
        public DateOnly? Fecha_Inicio { get; set; }

        [NotMapped]
        public DateOnly? Fecha_Fin { get; set; }

        [Display(Name = "Id")]
        public int Comp_Id { get; set; }

        [Display(Name = "Proveedor")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public int Prov_Id { get; set; }

        [Display(Name = "Fecha Compra")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public DateTime Comp_Fecha { get; set; }

        public int Usua_Creacion { get; set; }

        public DateTime Feca_Creacion { get; set; }

        public int? Usua_Modificacion { get; set; }

        public DateTime? Feca_Modificacion { get; set; }

        public bool? Comp_Estado { get; set; }

        [NotMapped]
        public int? CpDe_Id { get; set; }

        [NotMapped]
        [Display(Name = "Producto")]
        public int? Prod_Id { get; set; }

        [NotMapped]
        [Display(Name = "Cantidad")]
        public int? CpDe_Cantidad { get; set; }

        [NotMapped]
        [Display(Name = "Precio")]
        public double? CpDe_Precio { get; set; }

        [NotMapped]
        public bool? CpDe_Estado { get; set; }

        [NotMapped]
        [Display(Name = "Producto")]
        public string? Prod_Descripcion { get; set; }

        [NotMapped]
        [Display(Name = "Proveedor")]
        public string? Prov_Nombre { get; set; }

        [NotMapped]
        public string? UsuarioCreacion { get; set; }

        [NotMapped]
        public string? UsuarioModificacion { get; set; }

        [NotMapped]
        public List<CompraDetalleViewModel>? Detalles { get; set; }
    }
}