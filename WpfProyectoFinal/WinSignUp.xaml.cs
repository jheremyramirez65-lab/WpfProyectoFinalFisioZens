using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
using System.Windows.Shapes;

namespace WpfProyectoFinal
{
    /// <summary>
    /// Lógica de interacción para WinSignUp.xaml
    /// </summary>
    public partial class WinSignUp : Window
    {
        string conexion = @"Data Source=LAPTOP-L87KBM03\FISIOTERAPIA;Initial Catalog=BDFisioterapia;User ID=sa;Password=123456";

        public WinSignUp()
        {
            InitializeComponent();
        }

        private void btnRegistrar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string genero = "";

                if (rbMasculino.IsChecked == true) genero = "Masculino";
                else if (rbFemenino.IsChecked == true) genero = "Femenino";
                else if (rbOtro.IsChecked == true) genero = "Otro";

                if (string.IsNullOrWhiteSpace(txtNombre.Text) ||
                    string.IsNullOrWhiteSpace(txtApellido.Text) ||
                    string.IsNullOrWhiteSpace(txtNombreUsuario.Text) ||
                    string.IsNullOrWhiteSpace(txtPassword.Password) ||
                    string.IsNullOrWhiteSpace(txtCelular.Text) ||
                    string.IsNullOrWhiteSpace(txtCorreo.Text) ||
                    cbPais.SelectedItem == null ||
                    string.IsNullOrWhiteSpace(genero))
                {
                    MessageBox.Show("Complete todos los campos.");
                    return;
                }

                if (!Regex.IsMatch(txtCorreo.Text, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                {
                    MessageBox.Show("Ingrese un correo válido.");
                    return;
                }

                if (!Regex.IsMatch(txtCelular.Text, @"^\d{8,10}$"))
                {
                    MessageBox.Show("El celular debe tener entre 8 y 10 números.");
                    return;
                }

                if (txtPassword.Password.Length < 6)
                {
                    MessageBox.Show("La contraseña debe tener mínimo 6 caracteres.");
                    return;
                }

                string pais = ((ComboBoxItem)cbPais.SelectedItem).Content.ToString();

                using (SqlConnection cn = new SqlConnection(conexion))
                {
                    cn.Open();

                    string query = @"
                        INSERT INTO Usuario
                        (Nombre, Apellido, Correo, Celular, Pais, Genero, NombreUsuario, Contrasena, IdRol)
                        VALUES
                        (@Nombre, @Apellido, @Correo, @Celular, @Pais, @Genero, @NombreUsuario, @Contrasena,
                        (SELECT IdRol FROM Rol WHERE NombreRol = 'Recepcionista'))";

                    SqlCommand cmd = new SqlCommand(query, cn);
                    cmd.Parameters.AddWithValue("@Nombre", txtNombre.Text);
                    cmd.Parameters.AddWithValue("@Apellido", txtApellido.Text);
                    cmd.Parameters.AddWithValue("@Correo", txtCorreo.Text);
                    cmd.Parameters.AddWithValue("@Celular", txtCelular.Text);
                    cmd.Parameters.AddWithValue("@Pais", pais);
                    cmd.Parameters.AddWithValue("@Genero", genero);
                    cmd.Parameters.AddWithValue("@NombreUsuario", txtNombreUsuario.Text);
                    cmd.Parameters.AddWithValue("@Contrasena", txtPassword.Password);

                    cmd.ExecuteNonQuery();

                    System.IO.Directory.CreateDirectory("Archivos");

                    string datosUsuario =
                        $"Nombre: {txtNombre.Text}\n" +
                        $"Apellido: {txtApellido.Text}\n" +
                        $"Usuario: {txtNombreUsuario.Text}\n" +
                        $"Correo: {txtCorreo.Text}\n" +
                        $"Celular: {txtCelular.Text}\n" +
                        $"País: {pais}\n" +
                        $"Género: {genero}\n" +
                        $"Rol: Recepcionista\n" +
                        $"Fecha registro: {DateTime.Now}\n" +
                        "-----------------------------\n";

                    System.IO.File.AppendAllText("Archivos/usuarios.txt", datosUsuario);
                }

                MessageBox.Show("Cuenta de recepcionista creada correctamente.");
                this.Close();
            }
            catch (SqlException ex)
            {
                if (ex.Message.Contains("UQ_Usuario_Correo"))
                    MessageBox.Show("Ese correo ya está registrado.");
                else if (ex.Message.Contains("UQ_Usuario_NombreUsuario"))
                    MessageBox.Show("Ese nombre de usuario ya existe.");
                else
                    MessageBox.Show("Error SQL: " + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al registrar: " + ex.Message);
            }
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
