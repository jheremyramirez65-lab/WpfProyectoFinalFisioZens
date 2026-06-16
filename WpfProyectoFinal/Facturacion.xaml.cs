using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.IO;

namespace WpfProyectoFinal
{
    /// <summary>
    /// Lógica de interacción para Facturacion.xaml
    /// </summary>
    public partial class Facturacion : UserControl
    {
        public Facturacion()
        {
            InitializeComponent();
        }

        private void btnGuardarFactura_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog guardar = new SaveFileDialog();
            guardar.Filter = "Archivo de texto (*.txt)|*.txt";
            guardar.FileName = "factura_fisiozens.txt";

            if (guardar.ShowDialog() == true)
            {
                string contenido =
                    "FACTURA FISIOZENS\n" +
                    "----------------------\n" +
                    "Paciente: " + txtPaciente.Text + "\n" +
                    "Tratamiento: " + txtTratamiento.Text + "\n" +
                    "Monto: " + txtMonto.Text + " Bs.\n" +
                    "Fecha: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm");

                File.WriteAllText(guardar.FileName, contenido);

                MessageBox.Show("Factura guardada correctamente.");
            }
        }
    }
}
