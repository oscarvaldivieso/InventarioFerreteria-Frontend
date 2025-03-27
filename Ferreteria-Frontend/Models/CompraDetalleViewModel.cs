namespace Ferreteria_Frontend.Models
{
    public class CompraDetalleViewModel
    {
        public int CpDe_Id { get; set; }

        public int Comp_Id { get; set; }

        public int Prod_Id { get; set; }

        public int CpDe_Cantidad { get; set; }

        public double CpDe_Precio { get; set; }

        public int Usua_Creacion { get; set; }

        public DateTime Feca_Creacion { get; set; }

        public int? Usua_Modificacion { get; set; }

        public DateTime? Feca_Modificacion { get; set; }

        public bool? CpDe_Estado { get; set; }
    }
}
