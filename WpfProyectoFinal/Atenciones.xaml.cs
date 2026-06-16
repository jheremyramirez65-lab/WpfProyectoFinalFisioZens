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
    /// Lógica de interacción para Atenciones.xaml
    /// </summary>
    public partial class Atenciones : UserControl
    {
        string conexion = @"Data Source=LAPTOP-L87KBM03\FISIOTERAPIA;Initial Catalog=BDFisioterapia;User ID=sa;Password=123456";

        public Atenciones()
        {
            InitializeComponent();
            CargarPacientes();
            CargarFisioterapeutasDisponibles();
            CargarTratamientos();
            CargarAtenciones();
        }

        private void CargarPacientes()
        {
            using (SqlConnection cn = new SqlConnection(conexion))
            {
                cn.Open();

                SqlDataAdapter da = new SqlDataAdapter(@"
                    SELECT IdPaciente,
                           Nombre + ' ' + Apellido + ' - CI: ' + CI AS NombreCompleto
                    FROM Paciente", cn);

                DataTable dt = new DataTable();
                da.Fill(dt);

                cbPaciente.ItemsSource = dt.DefaultView;
            }
        }

        private void CargarFisioterapeutasDisponibles()
        {
            using (SqlConnection cn = new SqlConnection(conexion))
            {
                cn.Open();

                SqlDataAdapter da = new SqlDataAdapter(@"
                    SELECT IdFisioterapeuta,
                           Nombre + ' ' + Apellido + ' - ' + Especialidad AS NombreCompleto
                    FROM Fisioterapeuta
                    WHERE Estado = 'Disponible'", cn);

                DataTable dt = new DataTable();
                da.Fill(dt);

                cbFisioterapeuta.ItemsSource = dt.DefaultView;
            }
        }

        private void CargarTratamientos()
        {
            using (SqlConnection cn = new SqlConnection(conexion))
            {
                cn.Open();

                SqlDataAdapter da = new SqlDataAdapter(@"
                    SELECT IdTratamiento, NombreTratamiento
                    FROM Tratamiento", cn);

                DataTable dt = new DataTable();
                da.Fill(dt);

                cbTratamiento.ItemsSource = dt.DefaultView;
            }
        }

        private void CargarAtenciones()
        {
            using (SqlConnection cn = new SqlConnection(conexion))
            {
                cn.Open();

                SqlDataAdapter da = new SqlDataAdapter(@"
                    SELECT 
                        A.IdAtencion,
                        P.Nombre + ' ' + P.Apellido AS Paciente,
                        ISNULL(F.Nombre + ' ' + F.Apellido, 'Sin asignar') AS Fisioterapeuta,
                        T.NombreTratamiento AS Tratamiento,
                        A.Fecha,
                        A.Hora,
                        A.Estado,
                        A.Observacion
                    FROM Atencion A
                    INNER JOIN Paciente P ON A.IdPaciente = P.IdPaciente
                    LEFT JOIN Fisioterapeuta F ON A.IdFisioterapeuta = F.IdFisioterapeuta
                    INNER JOIN Tratamiento T ON A.IdTratamiento = T.IdTratamiento", cn);

                DataTable dt = new DataTable();
                da.Fill(dt);

                dgAtenciones.ItemsSource = dt.DefaultView;
            }
        }

        private void btnGuardarAtencion_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cbPaciente.SelectedValue == null || cbTratamiento.SelectedValue == null)
                {
                    MessageBox.Show("Seleccione paciente y tratamiento.");
                    return;
                }

                int idPaciente = Convert.ToInt32(cbPaciente.SelectedValue);
                int idTratamiento = Convert.ToInt32(cbTratamiento.SelectedValue);

                object idFisioterapeuta = DBNull.Value;
                string estado = "En espera";

                if (cbFisioterapeuta.SelectedValue != null)
                {
                    idFisioterapeuta = Convert.ToInt32(cbFisioterapeuta.SelectedValue);
                    estado = "Atendido";
                }

                using (SqlConnection cn = new SqlConnection(conexion))
                {
                    cn.Open();

                    string query = @"
                        INSERT INTO Atencion
                        (IdPaciente, IdFisioterapeuta, IdTratamiento, Fecha, Hora, Estado, Observacion)
                        VALUES
                        (@IdPaciente, @IdFisioterapeuta, @IdTratamiento, @Fecha, @Hora, @Estado, @Observacion)";

                    SqlCommand cmd = new SqlCommand(query, cn);
                    cmd.Parameters.AddWithValue("@IdPaciente", idPaciente);
                    cmd.Parameters.AddWithValue("@IdFisioterapeuta", idFisioterapeuta);
                    cmd.Parameters.AddWithValue("@IdTratamiento", idTratamiento);
                    cmd.Parameters.AddWithValue("@Fecha", DateTime.Now.Date);
                    cmd.Parameters.AddWithValue("@Hora", DateTime.Now.ToString("HH:mm"));
                    cmd.Parameters.AddWithValue("@Estado", estado);
                    cmd.Parameters.AddWithValue("@Observacion", txtObservacion.Text);

                    cmd.ExecuteNonQuery();

                    if (idFisioterapeuta != DBNull.Value)
                    {
                        SqlCommand cmdEstado = new SqlCommand(@"
                            UPDATE Fisioterapeuta
                            SET Estado = 'Ocupado'
                            WHERE IdFisioterapeuta = @IdFisioterapeuta", cn);

                        cmdEstado.Parameters.AddWithValue("@IdFisioterapeuta", idFisioterapeuta);
                        cmdEstado.ExecuteNonQuery();
                    }
                }

                Directory.CreateDirectory("Archivos");

                string datos =
                    $"Fecha: {DateTime.Now}\n" +
                    $"Paciente: {cbPaciente.Text}\n" +
                    $"Fisioterapeuta: {(cbFisioterapeuta.SelectedValue == null ? "Sin asignar" : cbFisioterapeuta.Text)}\n" +
                    $"Tratamiento: {cbTratamiento.Text}\n" +
                    $"Estado: {estado}\n" +
                    $"Observación: {txtObservacion.Text}\n" +
                    "-----------------------------\n";

                File.AppendAllText("Archivos/atenciones.txt", datos);

                MessageBox.Show("Atención guardada correctamente.");

                txtObservacion.Clear();
                cbPaciente.SelectedIndex = -1;
                cbFisioterapeuta.SelectedIndex = -1;
                cbTratamiento.SelectedIndex = -1;

                CargarFisioterapeutasDisponibles();
                CargarAtenciones();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar atención: " + ex.Message);
            }
        }
    }
}
