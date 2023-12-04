using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistema.Ferreteria.Core.Venta.Dominio
{
    public class CuentaModel
    {

        public int Id { get; set; }
        public int? ClienteId { get; set; }
        public DateTime? FechaEmision { get; set; }
        public decimal Subtotal 
        { 
            get
            {
                return Detalles.Where(d => d.Tipo == TipoDetalle.Item).Sum(d => d.Total);
            }
        }
        public decimal Impuestos 
        { 
            get
            {
                return Detalles.Where(d => d.Tipo == TipoDetalle.Item).Sum(d => d.Total) * PorcentajeImpuesto;
            }
        }
        public decimal Total 
        { 
            get 
            {
                return Subtotal + Impuestos;
            } 
        }
        public List<DetalleCuentaModel> Detalles { get; set; }
        public decimal PorcentajeImpuesto { get; set; }

        public CuentaModel()
        {
            Detalles = new List<DetalleCuentaModel>();
        }

    }

    public class DetalleCuentaModel
    {

        public int Id { get; set; }
        public int CuentaId { get; set; }
        public TipoDetalle Tipo { get; set; }
        public int? ArticuloId { get; set; }
        public short Cantidad { get; set; }
        public decimal Total { get; set; }

    }

    public enum TipoDetalle
    {
        Item,
        Payment
    }
}
