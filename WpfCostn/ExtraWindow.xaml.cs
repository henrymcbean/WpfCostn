using System;
using System.Linq;
using System.Text;
using System.Windows;
using WpfClassLibrary;
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

namespace WpfCostn
{
    /// <summary>
    /// Interaction logic for ExtraWindow.xaml
    /// </summary>
    public partial class ExtraWindow : Window
    {
        #region Class Properties
        public string CTSizeKey { get; set; }
        public string CTStyle { get; set; }
        public string CTVarn { get; set; }
        #endregion

        public List<CstextraCSTExtraRec> ListCSTExtraRec { get; set; }
        public List<CstextraCSTEModifyRec> ListCSTCSTEModifyRec { get; set; }

        public ExtraWindow()
        {
            InitializeComponent();
            uscExtra.ContainerWindow = this;
            uscExtra.CloseUsrCtrlEvent += uscExtra_CloseUsrCtrlEvent;
        }

        void uscExtra_CloseUsrCtrlEvent(object sender, EventArgs e)
        {
            Close();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            uscExtra.CTSizeKey = CTSizeKey;
            uscExtra.CTStyle = CTStyle;
            uscExtra.CTVarn = CTVarn;
        }
    }
}
