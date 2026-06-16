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

namespace WpfProyectoFinal
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MostrarInicio();
        }

        private void MostrarInicio()
        {
            contenedorPrincipal.Content = new Image
            {
                Source = new BitmapImage(new Uri("imgs/logo.png", UriKind.Relative)),
                Stretch = Stretch.Uniform,
                Margin = new Thickness(40)
            };
        }

        private void btnInicio_Click(object sender, RoutedEventArgs e)
        {
            MostrarInicio();
        }

        private void btnAgregarPacientes_Click(object sender, RoutedEventArgs e)
        {
            contenedorPrincipal.Content = new TextBlock
            {
                Text = "Aquí irá el módulo Pacientes",
                FontSize = 30
            };
            contenedorPrincipal.Content = new Pacientes();
        }

        private void btnAgregarAtenciones_Click(object sender, RoutedEventArgs e)
        {
            contenedorPrincipal.Content = new TextBlock
            {
                Text = "Aquí irá el módulo Atenciones",
                FontSize = 30
            };
            contenedorPrincipal.Content = new Atenciones();
        }

        private void btnFacturacion_Click(object sender, RoutedEventArgs e)
        {
            contenedorPrincipal.Content = new Facturacion();
        }

        private void CerrarSesion_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
