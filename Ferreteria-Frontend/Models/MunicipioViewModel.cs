using System.ComponentModel.DataAnnotations.Schema;

namespace Ferreteria_Frontend.Models
{
    public class MunicipioViewModel
    {
        public string Muni_Codigo { get; set; }

        public string Muni_Descripcion { get; set; }

        public string Depa_Codigo { get; set; }

        public int Usua_Creacion { get; set; }

        public DateTime Feca_Creacion { get; set; }

        public int? Usua_Modificacion { get; set; }

        public DateTime? Feca_Modificacion { get; set; }

        [NotMapped]
        public string? UsuaC_Nombre { get; set; }
        [NotMapped]
        public string? UsuaM_Nombre { get; set; }

        //public virtual tbDepartamentos Depa_CodigoNavigation { get; set; }

        //public virtual tbUsuarios Usua_CreacionNavigation { get; set; }

        //public virtual tbUsuarios Usua_ModificacionNavigation { get; set; }

        //public virtual ICollection<tbClientes> tbClientes { get; set; } = new List<tbClientes>();

        //public virtual ICollection<tbEmpleados> tbEmpleados { get; set; } = new List<tbEmpleados>();

        //public virtual ICollection<tbProveedores> tbProveedores { get; set; } = new List<tbProveedores>();

        //public virtual ICollection<tbSucursales> tbSucursales { get; set; } = new List<tbSucursales>();

    }
}
