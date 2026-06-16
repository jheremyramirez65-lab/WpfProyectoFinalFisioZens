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
    /// Lógica de interacción para Pacientes.xaml
    /// </summary>
    public partial class Pacientes : UserControl
    {
        public Pacientes()
        {
            InitializeComponent();
        }

        private void btnGuardarPaciente_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("Ingrese el nombre del paciente.");
                return;
            }

            string datos =
                $"Nombre: {txtNombre.Text}\n" +
                $"CI: {txtCi.Text}\n" +
                $"Teléfono: {txtTelefono.Text}\n" +
                $"Edad: {txtEdad.Text}\n" +
                "--------------------------\n";

            File.AppendAllText("pacientes.txt", datos);

            MessageBox.Show("Paciente guardado correctamente.");
        }
    }
}
