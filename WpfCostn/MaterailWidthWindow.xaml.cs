using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using SQLDependancyService;
using System.Windows.Shapes;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace WpfCostn
{
    /// <summary>
    /// Interaction logic for MatWidthWindow.xaml
    /// </summary>
    public partial class MatWidthWindow : Window
    {
        SQLWriteClass SQLWrite = null;
        public short CTMatType { get; set; }
        public string CTMatCode { get; set; }
        public short CTMatColour { get; set; }
        public int CTMatSupl { get; set; }
        public short CTMatWhse { get; set; }
        public short CTMatLocn { get; set; }
        public MatWidthWindow()
        {
            InitializeComponent();
            SQLWrite = new SQLWriteClass(SQLConnection.SubscriberConnectionString);
        }

        private void RollsProperties()
        {
            SQLWrite.SQLWriteCommand("[dbo].[GetRollsdbPropertiesMatWidth]");
            SqlParameter[] SqlParam = new SqlParameter[6];

            SqlParam[0] = DependancyService.SQLParameter("@CTMatType",
                    "System.Int16", DependancyService.ParamDirection.Input, (object)CTMatType);

            SqlParam[1] = DependancyService.SQLParameter("@CTMatCode",
                    "System.String", DependancyService.ParamDirection.Input, (object)CTMatCode);

            SqlParam[2] = DependancyService.SQLParameter("@CTMatColr",
                    "System.Int16", DependancyService.ParamDirection.Input, (object)CTMatColour);

            SqlParam[3] = DependancyService.SQLParameter("@CTMatSupl",
                    "System.Int16", DependancyService.ParamDirection.Input, (object)CTMatSupl);

            SqlParam[4] = DependancyService.SQLParameter("@CTMatWhse",
                    "System.Int16", DependancyService.ParamDirection.Input, (object)CTMatWhse);

            SqlParam[5] = DependancyService.SQLParameter("@CTMatLocn",
                    "System.Int16", DependancyService.ParamDirection.Input, (object)CTMatLocn);

            object obj = SQLWrite.ExecuteQueryFunction(SqlParam);

            txtPresent.Text = string.Format("{0:0.00}", (double)obj);
        }
        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            RollsProperties();
        }
        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
