using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfProyectoFinal.Modelos
{
    public class Paciente : Persona
    {
        public int Edad { get; set; }
        public string Diagnostico { get; set; }
    }
}
