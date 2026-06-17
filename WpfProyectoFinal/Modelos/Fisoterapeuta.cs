using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfProyectoFinal.Modelos
{
    public class Fisioterapeuta : Persona
    {
        public int IdFisioterapeuta { get; set; }
        public string Especialidad { get; set; }
        public string Estado { get; set; }
    }
}
