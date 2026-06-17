using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
using System.Windows.Shapes;


namespace WpfProyectoFinal
{
    /// <summary>
    /// Lógica de interacción para WinLogin.xaml
    /// </summary>
    public partial class WinLogin : Window
    {
        public WinLogin()
        {
            InitializeComponent();
        }

        private void btnIniciarSesion_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtCorreo.Text) ||
                    string.IsNullOrWhiteSpace(txtPassword.Password))
                {
                    MessageBox.Show("Ingrese usuario y contraseña.");
                    return;
                }

                string conexion = @"Data Source=LAPTOP-L87KBM03\FISIOTERAPIA;Initial Catalog=BDFisioterapia;User ID=sa;Password=123456";

                using (SqlConnection cn = new SqlConnection(conexion))
                {
                    cn.Open();

                    string query = @"
                        SELECT U.IdUsuario, U.Nombre, U.Apellido, R.NombreRol
                        FROM Usuario U
                        INNER JOIN Rol R ON U.IdRol = R.IdRol
                        WHERE U.Correo = @usuario
                        AND U.Contrasena = @contrasena
                        AND R.NombreRol = 'Recepcionista'";

                    SqlCommand cmd = new SqlCommand(query, cn);
                    cmd.Parameters.AddWithValue("@usuario", txtCorreo.Text);
                    cmd.Parameters.AddWithValue("@contrasena", txtPassword.Password);

                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        MainWindow menu = new MainWindow();
                        menu.Show();
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Usuario o contraseña incorrectos. Solo pueden ingresar recepcionistas.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al iniciar sesión: " + ex.Message);
            }
        }

        private void btnCrearCuenta_Click(object sender, RoutedEventArgs e)
        {
            WinSignUp registro = new WinSignUp();
            registro.ShowDialog();
        }
    }
}
