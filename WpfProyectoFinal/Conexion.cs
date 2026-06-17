using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfProyectoFinal
{
    public class Conexion
    {
        private static string cadena = @"Data Source=LAPTOP-L87KBM03\FISIOTERAPIA;Initial Catalog=BDFisioterapia;User ID=sa;Password=123456";

        public static SqlConnection ObtenerConexion()
        {
            return new SqlConnection(cadena);
        }
    }
}
