using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfProyectoFinal.Modelos
{
    public class Factura
    {
        public Atencion Atencion { get; set; }   // Composición / relación
        public double Monto { get; set; }
        public DateTime Fecha { get; set; }

        public double CalcularTotal()
        {
            return Monto;
        }
    }
}
