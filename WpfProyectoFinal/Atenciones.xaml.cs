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
    /// Lógica de interacción para Atenciones.xaml
    /// </summary>
    public partial class Atenciones : UserControl
    {
        public Atenciones()
        {
            InitializeComponent();
        }

        private void btnGuardarAtencion_Click(object sender, RoutedEventArgs e)
        {
            string datos =
                $"Fecha: {DateTime.Now}\n" +
                $"Paciente: {txtPaciente.Text}\n" +
                $"Fisioterapeuta: {txtFisioterapeuta.Text}\n" +
                $"Tratamiento: {txtTratamiento.Text}\n" +
                $"Observación: {txtObservacion.Text}\n" +
                "--------------------------\n";

            File.AppendAllText("atenciones.txt", datos);

            MessageBox.Show("Atención guardada correctamente.");
        }
    }
}
