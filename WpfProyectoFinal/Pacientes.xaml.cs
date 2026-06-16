using System;
using System.Collections.Generic;
using System.IO;
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
using WpfProyectoFinal.Modelos;

namespace WpfProyectoFinal
{
    /// <summary>
    /// Lógica de interacción para Pacientes.xaml
    /// </summary>
    public partial class Pacientes : UserControl
    {
        private List<Paciente> listaPacientes = new List<Paciente>();
        public Pacientes()
        {
            InitializeComponent();
            dgPacientes.ItemsSource = listaPacientes;
        }

        private void btnGuardarPaciente_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtNombre.Text) ||
                    string.IsNullOrWhiteSpace(txtCi.Text) ||
                    string.IsNullOrWhiteSpace(txtTelefono.Text) ||
                    string.IsNullOrWhiteSpace(txtEdad.Text) ||
                    string.IsNullOrWhiteSpace(txtDiagnostico.Text))
                {
                    MessageBox.Show("Complete todos los campos.");
                    return;
                }

                if (!int.TryParse(txtEdad.Text, out int edad))
                {
                    MessageBox.Show("La edad debe ser un número.");
                    return;
                }

                Paciente paciente = new Paciente
                {
                    Nombre = txtNombre.Text,
                    Ci = txtCi.Text,
                    Telefono = txtTelefono.Text,
                    Edad = edad,
                    Diagnostico = txtDiagnostico.Text
                };

                listaPacientes.Add(paciente);
                dgPacientes.Items.Refresh();

                string datos = $"{paciente.Nombre};{paciente.Ci};{paciente.Telefono};{paciente.Edad};{paciente.Diagnostico}\n";
                File.AppendAllText("pacientes.txt", datos);

                MessageBox.Show("Paciente guardado correctamente.");
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Error al guardar paciente: " + ex.Message);
            }
        }
    }
}
