using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using WpfClassLibrary.Model;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using WpfReportLibrary.ReportOptions;

namespace WpfCostn
{
    /// <summary>
    /// Interaction logic for CostnPrintOptionsWindow.xaml
    /// </summary>
    public partial class CostnPrintOptionsWindow : Window
    {
        private PrintCostnOptionsStructure printOptions;
        public PrintCostnOptionsStructure PrintOptions
        {
          get { return printOptions; }
          set { printOptions = value; }
        } 
        public CostnPrintOptionsWindow()
        {
            InitializeComponent();
            printOptions = new PrintCostnOptionsStructure();
            this.Icon = (BitmapImage)Application.Current.GetType().GetProperty("AppIcon").GetValue(Application.Current, null);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = printOptions;
            Mouse.OverrideCursor = null;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
            DialogResult = true;
            Close();
        }
    }
}
