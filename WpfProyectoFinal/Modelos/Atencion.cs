using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfProyectoFinal.Modelos
{
    public class Atencion
    {
        public Paciente Paciente { get; set; }   // Agregación
        public string Fisioterapeuta { get; set; }
        public string Tratamiento { get; set; }
        public string Observacion { get; set; }
        public DateTime Fecha { get; set; }
    }
}
