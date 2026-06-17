using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

        public Pacientes()
        {
            InitializeComponent();
            CargarPacientes();
        }

        private void CargarPacientes()
        {
            using (SqlConnection cn = Conexion.ObtenerConexion())
            {
                cn.Open();

                SqlDataAdapter da = new SqlDataAdapter(
                    "SELECT IdPaciente, Nombre, Apellido, CI, Telefono, FechaNacimiento, Diagnostico FROM Paciente",
                    cn);

                DataTable dt = new DataTable();
                da.Fill(dt);

                dgPacientes.ItemsSource = dt.DefaultView;
            }
        }

        private void btnGuardarPaciente_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtNombre.Text) ||
                    string.IsNullOrWhiteSpace(txtApellido.Text) ||
                    string.IsNullOrWhiteSpace(txtCI.Text) ||
                    string.IsNullOrWhiteSpace(txtTelefono.Text))
                {
                    MessageBox.Show("Complete todos los campos obligatorios.");
                    return;
                }

                if (!Regex.IsMatch(txtNombre.Text, @"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$"))
                {
                    MessageBox.Show("Nombre inválido.");
                    return;
                }

                if (!Regex.IsMatch(txtApellido.Text, @"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$"))
                {
                    MessageBox.Show("Apellido inválido.");
                    return;
                }

                if (!Regex.IsMatch(txtCI.Text, @"^\d{8}$"))
                {
                    MessageBox.Show("CI inválido. Debe tener 8 dígitos.");
                    return;
                }

                if (!Regex.IsMatch(txtTelefono.Text, @"^\d{8}$"))
                {
                    MessageBox.Show("Teléfono inválido. Debe tener 8 dígitos.");
                    return;
                }

                using (SqlConnection cn = Conexion.ObtenerConexion())
                {
                    cn.Open();

                    string query = @"
                        INSERT INTO Paciente
                        (Nombre, Apellido, CI, Telefono, FechaNacimiento, Diagnostico)
                        VALUES
                        (@Nombre, @Apellido, @CI, @Telefono, @FechaNacimiento, @Diagnostico)";

                    SqlCommand cmd = new SqlCommand(query, cn);
                    cmd.Parameters.AddWithValue("@Nombre", txtNombre.Text);
                    cmd.Parameters.AddWithValue("@Apellido", txtApellido.Text);
                    cmd.Parameters.AddWithValue("@CI", txtCI.Text);
                    cmd.Parameters.AddWithValue("@Telefono", txtTelefono.Text);
                    cmd.Parameters.AddWithValue("@FechaNacimiento", dpFechaNacimiento.SelectedDate ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Diagnostico", txtDiagnostico.Text);

                    cmd.ExecuteNonQuery();
                }

                Directory.CreateDirectory("Archivos");

                string datos =
                    $"Nombre: {txtNombre.Text}\n" +
                    $"Apellido: {txtApellido.Text}\n" +
                    $"CI: {txtCI.Text}\n" +
                    $"Teléfono: {txtTelefono.Text}\n" +
                    $"Fecha nacimiento: {dpFechaNacimiento.SelectedDate}\n" +
                    $"Diagnóstico: {txtDiagnostico.Text}\n" +
                    $"Fecha registro: {DateTime.Now}\n" +
                    "-----------------------------\n";

                File.AppendAllText("Archivos/pacientes.txt", datos);

                MessageBox.Show("Paciente guardado correctamente.");

                LimpiarCampos();
                CargarPacientes();
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Error SQL: " + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar paciente: " + ex.Message);
            }
        }

        private void LimpiarCampos()
        {
            txtNombre.Clear();
            txtApellido.Clear();
            txtCI.Clear();
            txtTelefono.Clear();
            txtDiagnostico.Clear();
            dpFechaNacimiento.SelectedDate = null;
        }
    }
}
