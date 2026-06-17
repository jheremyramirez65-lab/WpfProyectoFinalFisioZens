using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
            CargarAtenciones();
            CargarFacturas();
        }

        private void CargarAtenciones()
        {
            using (SqlConnection cn = Conexion.ObtenerConexion())
            {
                cn.Open();

                SqlDataAdapter da = new SqlDataAdapter(@"
                    SELECT 
                        A.IdAtencion,
                        P.Nombre + ' ' + P.Apellido + ' - ' + T.NombreTratamiento + ' - Bs. ' + CAST(T.Costo AS VARCHAR) AS Detalle,
                        T.Costo
                    FROM Atencion A
                    INNER JOIN Paciente P ON A.IdPaciente = P.IdPaciente
                    INNER JOIN Tratamiento T ON A.IdTratamiento = T.IdTratamiento
                    WHERE A.Estado = 'Atendido'
                    AND A.IdAtencion NOT IN (SELECT IdAtencion FROM Factura)", cn);

                DataTable dt = new DataTable();
                da.Fill(dt);

                cbAtencion.ItemsSource = dt.DefaultView;
            }
        }

        private void CargarFacturas()
        {
            using (SqlConnection cn = Conexion.ObtenerConexion())
            {
                cn.Open();

                SqlDataAdapter da = new SqlDataAdapter(@"
                    SELECT 
                        F.IdFactura,
                        P.Nombre + ' ' + P.Apellido AS Paciente,
                        T.NombreTratamiento AS Tratamiento,
                        F.FechaEmision,
                        F.Subtotal,
                        F.Descuento,
                        F.Total,
                        F.MetodoPago
                    FROM Factura F
                    INNER JOIN Atencion A ON F.IdAtencion = A.IdAtencion
                    INNER JOIN Paciente P ON A.IdPaciente = P.IdPaciente
                    INNER JOIN Tratamiento T ON A.IdTratamiento = T.IdTratamiento", cn);

                DataTable dt = new DataTable();
                da.Fill(dt);

                dgFacturas.ItemsSource = dt.DefaultView;
            }
        }

        private void cbAtencion_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbAtencion.SelectedItem is DataRowView fila)
            {
                txtSubtotal.Text = fila["Costo"].ToString();
            }
        }

        private void btnGuardarFactura_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cbAtencion.SelectedValue == null)
                {
                    MessageBox.Show("Seleccione una atención.");
                    return;
                }

                if (!decimal.TryParse(txtSubtotal.Text, out decimal subtotal))
                {
                    MessageBox.Show("Subtotal inválido.");
                    return;
                }

                if (!decimal.TryParse(txtDescuento.Text, out decimal descuento))
                {
                    MessageBox.Show("El descuento debe ser numérico.");
                    return;
                }

                if (descuento < 0 || descuento > subtotal)
                {
                    MessageBox.Show("El descuento no puede ser negativo ni mayor al subtotal.");
                    return;
                }

                if (cbMetodoPago.SelectedItem == null)
                {
                    MessageBox.Show("Seleccione método de pago.");
                    return;
                }

                int idAtencion = Convert.ToInt32(cbAtencion.SelectedValue);
                decimal total = subtotal - descuento;
                string metodoPago = ((ComboBoxItem)cbMetodoPago.SelectedItem).Content.ToString();

                using (SqlConnection cn = Conexion.ObtenerConexion())
                {
                    cn.Open();

                    string query = @"
                        INSERT INTO Factura
                        (IdAtencion, FechaEmision, Subtotal, Descuento, Total, MetodoPago)
                        VALUES
                        (@IdAtencion, @FechaEmision, @Subtotal, @Descuento, @Total, @MetodoPago)";

                    SqlCommand cmd = new SqlCommand(query, cn);
                    cmd.Parameters.AddWithValue("@IdAtencion", idAtencion);
                    cmd.Parameters.AddWithValue("@FechaEmision", DateTime.Now);
                    cmd.Parameters.AddWithValue("@Subtotal", subtotal);
                    cmd.Parameters.AddWithValue("@Descuento", descuento);
                    cmd.Parameters.AddWithValue("@Total", total);
                    cmd.Parameters.AddWithValue("@MetodoPago", metodoPago);

                    cmd.ExecuteNonQuery();
                }

                Directory.CreateDirectory("Archivos");

                string contenido =
                    "FACTURA FISIOZENS\n" +
                    "-----------------------------\n" +
                    $"Atención: {cbAtencion.Text}\n" +
                    $"Subtotal: {subtotal} Bs.\n" +
                    $"Descuento: {descuento} Bs.\n" +
                    $"Total: {total} Bs.\n" +
                    $"Método de pago: {metodoPago}\n" +
                    $"Fecha: {DateTime.Now:dd/MM/yyyy HH:mm}\n" +
                    "-----------------------------\n";

                SaveFileDialog guardar = new SaveFileDialog();
                guardar.Filter = "Archivo de texto (*.txt)|*.txt";
                guardar.FileName = "factura_fisiozens.txt";

                if (guardar.ShowDialog() == true)
                {
                    File.WriteAllText(guardar.FileName, contenido);
                }

                MessageBox.Show("Factura generada correctamente.");

                txtSubtotal.Clear();
                txtDescuento.Text = "0";
                cbMetodoPago.SelectedIndex = -1;

                CargarAtenciones();
                CargarFacturas();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al generar factura: " + ex.Message);
            }
        }
    }
}