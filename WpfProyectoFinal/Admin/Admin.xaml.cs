using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using static WpfProyectoFinal.Admin;

namespace WpfProyectoFinal
{
    public partial class Admin : Window
    {
        ObservableCollection<Fisio> listaFisio = new ObservableCollection<Fisio>();
        Fisio fisioSeleccionado;

        public class Fisio
        {
            public int IdFisioterapeuta { get; set; }
            public string Nombre { get; set; }
            public string Apellido { get; set; }
            public string Especialidad { get; set; }
            public string Telefono { get; set; }
            public string Estado { get; set; }
        }

        public Admin()
        {
            InitializeComponent();

            PanelReportes.Visibility = Visibility.Visible;
            PanelFisioterapeuta.Visibility = Visibility.Hidden;

            cargarFisioterapeutas();
            cargarReportes();
        }

        void cargarFisioterapeutas()
        {
            try
            {
                listaFisio.Clear();

                using (SqlConnection cn = Conexion.ObtenerConexion())
                {
                    cn.Open();

                    string consulta = @"
                        SELECT IdFisioterapeuta, Nombre, Apellido, Especialidad, Telefono, Estado
                        FROM Fisioterapeuta";

                    SqlCommand comando = new SqlCommand(consulta, cn);
                    SqlDataReader reader = comando.ExecuteReader();

                    while (reader.Read())
                    {
                        listaFisio.Add(new Fisio()
                        {
                            IdFisioterapeuta = Convert.ToInt32(reader["IdFisioterapeuta"]),
                            Nombre = reader["Nombre"].ToString(),
                            Apellido = reader["Apellido"].ToString(),
                            Especialidad = reader["Especialidad"].ToString(),
                            Telefono = reader["Telefono"].ToString(),
                            Estado = reader["Estado"].ToString()
                        });
                    }

                    reader.Close();
                }

                dgHistorialPac.ItemsSource = listaFisio;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar fisioterapeutas: " + ex.Message);
            }
        }

        void cargarReportes()
        {
            try
            {
                using (SqlConnection cn = Conexion.ObtenerConexion())
                {
                    cn.Open();

                    SqlCommand cmd1 = new SqlCommand("SELECT COUNT(*) FROM Atencion", cn);
                    tbCitas.Text = cmd1.ExecuteScalar().ToString();

                    SqlCommand cmd2 = new SqlCommand("SELECT COUNT(*) FROM Paciente", cn);
                    tbPacientes.Text = cmd2.ExecuteScalar().ToString();

                    SqlCommand cmd3 = new SqlCommand("SELECT COUNT(*) FROM Fisioterapeuta", cn);
                    tbFisio.Text = cmd3.ExecuteScalar().ToString();

                    SqlCommand cmd4 = new SqlCommand("SELECT ISNULL(SUM(Total),0) FROM Factura", cn);
                    tbIngresos.Text = "Bs. " + cmd4.ExecuteScalar().ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar reportes: " + ex.Message);
            }
        }

        private void btnCitasM_Click(object sender, RoutedEventArgs e)
        {
            PanelReportes.Visibility = Visibility.Visible;
            PanelFisioterapeuta.Visibility = Visibility.Hidden;
            cargarReportes();
        }

        private void btnAtencionesM_Click(object sender, RoutedEventArgs e)
        {
            PanelReportes.Visibility = Visibility.Hidden;
            PanelFisioterapeuta.Visibility = Visibility.Visible;
            cargarFisioterapeutas();
        }

        private void btnMCerrarSesion_Click(object sender, RoutedEventArgs e)
        {
            WinLogin ventana = new WinLogin();
            ventana.Show();
            this.Close();
        }

        private void btLimpiar_Click(object sender, RoutedEventArgs e)
        {
            tbNombre.Clear();
            tbApellido.Clear();
            tbTelefono.Clear();

            if (tbCorreo != null)
                tbCorreo.Clear();

            cbEspecialidad.SelectedIndex = -1;
            fisioSeleccionado = null;
        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (tbNombre.Text == "" ||
                    tbApellido.Text == "" ||
                    tbTelefono.Text == "" ||
                    cbEspecialidad.SelectedItem == null)
                {
                    MessageBox.Show("Complete todos los datos.");
                    return;
                }

                ComboBoxItem item = (ComboBoxItem)cbEspecialidad.SelectedItem;
                string especialidad = item.Content.ToString();

                using (SqlConnection cn = Conexion.ObtenerConexion())
                {
                    cn.Open();

                    string consulta = @"
                        INSERT INTO Fisioterapeuta
                        (Nombre, Apellido, Especialidad, Telefono, Estado)
                        VALUES
                        (@Nombre, @Apellido, @Especialidad, @Telefono, 'Disponible')";

                    SqlCommand cmd = new SqlCommand(consulta, cn);
                    cmd.Parameters.AddWithValue("@Nombre", tbNombre.Text);
                    cmd.Parameters.AddWithValue("@Apellido", tbApellido.Text);
                    cmd.Parameters.AddWithValue("@Especialidad", especialidad);
                    cmd.Parameters.AddWithValue("@Telefono", tbTelefono.Text);

                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Fisioterapeuta registrado correctamente.");
                cargarFisioterapeutas();
                btLimpiar_Click(null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar fisioterapeuta: " + ex.Message);
            }
        }

        private void dgHistorialPac_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            fisioSeleccionado = dgHistorialPac.SelectedItem as Fisio;

            if (fisioSeleccionado != null)
            {
                tbNombre.Text = fisioSeleccionado.Nombre;
                tbApellido.Text = fisioSeleccionado.Apellido;
                tbTelefono.Text = fisioSeleccionado.Telefono;

                if (tbCorreo != null)
                    tbCorreo.Clear();

                foreach (ComboBoxItem item in cbEspecialidad.Items)
                {
                    if (item.Content.ToString() == fisioSeleccionado.Especialidad)
                    {
                        cbEspecialidad.SelectedItem = item;
                        break;
                    }
                }
            }
        }

        private void btnNuevo_Click(object sender, RoutedEventArgs e)
        {
            btLimpiar_Click(null, null);
            tbNombre.Focus();

            MessageBox.Show("Ingrese los datos del nuevo fisioterapeuta.",
                "FisioZens",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void btnEditarFisio_Click(object sender, RoutedEventArgs e)
        {
            if (fisioSeleccionado == null)
            {
                MessageBox.Show("Seleccione un fisioterapeuta.");
                return;
            }

            try
            {
                if (cbEspecialidad.SelectedItem == null)
                {
                    MessageBox.Show("Seleccione una especialidad.");
                    return;
                }

                ComboBoxItem item = (ComboBoxItem)cbEspecialidad.SelectedItem;
                string especialidad = item.Content.ToString();

                using (SqlConnection cn = Conexion.ObtenerConexion())
                {
                    cn.Open();

                    string consulta = @"
                        UPDATE Fisioterapeuta
                        SET Nombre=@Nombre,
                            Apellido=@Apellido,
                            Especialidad=@Especialidad,
                            Telefono=@Telefono
                        WHERE IdFisioterapeuta=@IdFisioterapeuta";

                    SqlCommand cmd = new SqlCommand(consulta, cn);
                    cmd.Parameters.AddWithValue("@Nombre", tbNombre.Text);
                    cmd.Parameters.AddWithValue("@Apellido", tbApellido.Text);
                    cmd.Parameters.AddWithValue("@Especialidad", especialidad);
                    cmd.Parameters.AddWithValue("@Telefono", tbTelefono.Text);
                    cmd.Parameters.AddWithValue("@IdFisioterapeuta", fisioSeleccionado.IdFisioterapeuta);

                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Datos actualizados correctamente.");
                cargarFisioterapeutas();
                btLimpiar_Click(null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al editar fisioterapeuta: " + ex.Message);
            }
        }

        private void btnEliminarFisio_Click(object sender, RoutedEventArgs e)
        {
            if (fisioSeleccionado == null)
            {
                MessageBox.Show("Seleccione un fisioterapeuta.");
                return;
            }

            if (MessageBox.Show("¿Desea eliminar este fisioterapeuta?",
                "FisioZens",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question) == MessageBoxResult.No)
                return;

            try
            {
                using (SqlConnection cn = Conexion.ObtenerConexion())
                {
                    cn.Open();

                    SqlCommand verificar = new SqlCommand(
                        "SELECT COUNT(*) FROM Atencion WHERE IdFisioterapeuta=@IdFisioterapeuta", cn);

                    verificar.Parameters.AddWithValue("@IdFisioterapeuta", fisioSeleccionado.IdFisioterapeuta);

                    int cantidad = Convert.ToInt32(verificar.ExecuteScalar());

                    if (cantidad > 0)
                    {
                        MessageBox.Show("No se puede eliminar porque el fisioterapeuta tiene atenciones registradas.");
                        return;
                    }

                    SqlCommand cmd = new SqlCommand(
                        "DELETE FROM Fisioterapeuta WHERE IdFisioterapeuta=@IdFisioterapeuta", cn);

                    cmd.Parameters.AddWithValue("@IdFisioterapeuta", fisioSeleccionado.IdFisioterapeuta);
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Fisioterapeuta eliminado correctamente.");
                cargarFisioterapeutas();
                btLimpiar_Click(null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar fisioterapeuta: " + ex.Message);
            }
        }

        private void btnBusquedaPac_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                listaFisio.Clear();

                using (SqlConnection cn = Conexion.ObtenerConexion())
                {
                    cn.Open();

                    string consulta = @"
                        SELECT IdFisioterapeuta, Nombre, Apellido, Especialidad, Telefono, Estado
                        FROM Fisioterapeuta
                        WHERE Nombre LIKE @buscar
                        OR Apellido LIKE @buscar
                        OR Especialidad LIKE @buscar";

                    SqlCommand comando = new SqlCommand(consulta, cn);
                    comando.Parameters.AddWithValue("@buscar", "%" + txtBuscarPaciente.Text + "%");

                    SqlDataReader reader = comando.ExecuteReader();

                    while (reader.Read())
                    {
                        listaFisio.Add(new Fisio()
                        {
                            IdFisioterapeuta = Convert.ToInt32(reader["IdFisioterapeuta"]),
                            Nombre = reader["Nombre"].ToString(),
                            Apellido = reader["Apellido"].ToString(),
                            Especialidad = reader["Especialidad"].ToString(),
                            Telefono = reader["Telefono"].ToString(),
                            Estado = reader["Estado"].ToString()
                        });
                    }

                    reader.Close();
                }

                dgHistorialPac.ItemsSource = listaFisio;

                if (listaFisio.Count == 0)
                {
                    MessageBox.Show("No se encontraron resultados.");
                    cargarFisioterapeutas();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar: " + ex.Message);
            }
        }

        private void btnGenerarReporte_Click(object sender, RoutedEventArgs e)
        {
            if (cbMes.SelectedItem == null || cbAnio.SelectedItem == null)
            {
                MessageBox.Show("Seleccione un mes y un año.");
                return;
            }

            int mes = cbMes.SelectedIndex + 1;
            int anio = Convert.ToInt32(((ComboBoxItem)cbAnio.SelectedItem).Content);

            try
            {
                using (SqlConnection cn = Conexion.ObtenerConexion())
                {
                    cn.Open();

                    SqlCommand cmd1 = new SqlCommand(@"
                        SELECT COUNT(*)
                        FROM Atencion
                        WHERE MONTH(Fecha)=@Mes
                        AND YEAR(Fecha)=@Anio", cn);

                    cmd1.Parameters.AddWithValue("@Mes", mes);
                    cmd1.Parameters.AddWithValue("@Anio", anio);

                    tbCitas.Text = cmd1.ExecuteScalar().ToString();

                    SqlCommand cmd2 = new SqlCommand("SELECT COUNT(*) FROM Paciente", cn);
                    tbPacientes.Text = cmd2.ExecuteScalar().ToString();

                    SqlCommand cmd3 = new SqlCommand("SELECT COUNT(*) FROM Fisioterapeuta", cn);
                    tbFisio.Text = cmd3.ExecuteScalar().ToString();

                    SqlCommand cmd4 = new SqlCommand(@"
                        SELECT ISNULL(SUM(Total),0)
                        FROM Factura
                        WHERE MONTH(FechaEmision)=@Mes
                        AND YEAR(FechaEmision)=@Anio", cn);

                    cmd4.Parameters.AddWithValue("@Mes", mes);
                    cmd4.Parameters.AddWithValue("@Anio", anio);

                    decimal ingresos = Convert.ToDecimal(cmd4.ExecuteScalar());

                    tbIngresos.Text = "Bs. " + ingresos.ToString("N2");
                }

                MessageBox.Show("Reporte generado correctamente.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al generar reporte: " + ex.Message);
            }
        }
    }
}