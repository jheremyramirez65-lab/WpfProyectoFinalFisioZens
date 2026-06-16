using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfProyectoFinal.Modelos
{
    public class Usuario : Persona
    {
        public string Correo { get; set; }
        public string Password { get; set; }
        public string Rol { get; set; }
    }
}
