using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ferreteria_Frontend.Models
{
    public class MedidasViewModel
    {
        [Display(Name = "Id")]
        public int Medi_Id { get; set; }

        [Display(Name = "Medida")]

        public string Medi_Descripcion { get; set; }

        public int Usua_Creacion { get; set; }

        public DateTime Feca_Creacion { get; set; }

        public int? Usua_Modificacion { get; set; }

        public DateTime? Feca_Modificacion { get; set; }

        public bool? Medi_Estado { get; set; }


        [NotMapped]
        public string? UsuaC_Nombre { get; set; }
        [NotMapped]
        public string? UsuaM_Nombre { get; set; }

        //public virtual tbUsuarios Usua_CreacionNavigation { get; set; }

        //public virtual tbUsuarios Usua_ModificacionNavigation { get; set; }

        //public virtual ICollection<tbProductos> tbProductos { get; set; } = new List<tbProductos>();
    }
}
