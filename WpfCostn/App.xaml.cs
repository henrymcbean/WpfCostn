using System;
using System.Data;
using System.Linq;
using System.Windows;
using SQLDependancyService;
using System.Configuration;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace WpfCostn
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        RegistryAccess Registry = null;

        #region  // StarterConnectionString

        private string starterConnectionString;
        public string StarterConnectionString
        {
            get { return starterConnectionString; }
            set { starterConnectionString = value; }
        }

        private string subscriberConnectionString;
        public string SubscriberConnectionString
        {
            get { return subscriberConnectionString; }
            set { subscriberConnectionString = value; }
        }

        public BitmapImage AppIcon 
        { 
            get
            {
                return new BitmapImage(new Uri("wcostn.png", UriKind.Relative)); 
            }
        }
        #endregion

        App()
        {
            if (Environment.Is64BitOperatingSystem)
                Registry = new RegistryAccess("Adm", RegistryAccess.MachineType.WIN64);
            else
                Registry = new RegistryAccess("Adm", RegistryAccess.MachineType.WIN32);

            string server = Registry.Read("SQLServer");

            if (server == null || server.Length == 0)
            {
                StarterConnectionString = string.Format("Data Source={0};Database=WGmate;Persist Security Info=false;Integrated Security=false;User Id=startUser;Password=startUser", "ANDREAS-PC\\SQLEXPRESS");
                SubscriberConnectionString = string.Format("Data Source={0};Database=WGmate;Persist Security Info=false;Integrated Security=false;User Id=Henry;Password=Fear4Change", "ANDREAS-PC\\SQLEXPRESS");
            }
            else
            {
                StarterConnectionString = string.Format("Data Source={0};Database=WGmate;Persist Security Info=false;Integrated Security=false;User Id=startUser;Password=startUser", server);
                SubscriberConnectionString = string.Format("Data Source={0};Database=WGmate;Persist Security Info=false;Integrated Security=false;User Id=Henry;Password=Fear4Change", server);
            }
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            //works for tab into textbox
            EventManager.RegisterClassHandler(typeof(TextBox),
                TextBox.GotFocusEvent,
                new RoutedEventHandler(TextBox_GotFocus));
            //works for click textbox
            EventManager.RegisterClassHandler(typeof(Window),
                Window.GotMouseCaptureEvent,
                new RoutedEventHandler(Window_MouseCapture));

            base.OnStartup(e);
        }
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            (sender as TextBox).SelectAll();
        }
        private void Window_MouseCapture(object sender, RoutedEventArgs e)
        {
            var textBox = e.OriginalSource as TextBox;
            if (textBox != null)
                textBox.SelectAll();
        }
    }
}
