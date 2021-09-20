//-----------------------------------------------------------------------
// <copyright file="MainWindow.cs" company="ComputerLink">
//     Copyright (c) ComputerLink.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using WPF.MDI;
using WpfCostn;
using System.IO;
using System.Data;
using System.Linq;
using System.Text;
//using WpfDcksn.Model;
using System.Windows;
using WpfClassLibrary;
using C1.WPF.DataGrid;
using System.Xml.Linq;
using System.Threading;
using System.Diagnostics;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using SQLDependancyService;
using WpfClassLibrary.Model;
using System.Windows.Shapes;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Windows.Interop;
using System.Windows.Controls;
using WpfWgmateControlLibrary;
using System.Windows.Documents;
using System.Collections.Generic;
using System.Windows.Navigation;
using System.Windows.Media.Imaging;
using System.Windows.Controls.Primitives;

namespace WPFMDIForm
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window, System.Windows.Forms.IWin32Window
    {
        AppViewState viewState = AppViewState.EMPTY;
        public event EventHandler CloseEvent;
        string imageDirectory = "";
        string sapDirectory = "";

        /// <summary>
        /// Gmparams
        /// </summary>
        DependancyService depSvcGmparamsDescriptorsRec;
        DependancyService depSvcGmparamsParametersRec;

        /// <summary>
        /// GmCurrs
        /// </summary>
        DependancyService depSvcGmcurrsCURCurrenciesRec;

        /// <summary>
        /// Gmsizes
        /// </summary>
        DependancyService depSvcGmsizesGMSizesRec;

        /// <summary>
        /// Nommodel
        /// </summary>
        DependancyService depSvcNommodelNMODELMRec;

        /// <summary>
        /// CompanyInfo Configuration 
        /// </summary>
        DependancyService depSvcCompanyInfo;

        /// <summary>
        /// Wgamte Configuration 
        /// </summary>
        DependancyService depSvcWGmateConfig;

        /// <summary>
        /// Wgmate Configuration
        /// </summary>
        List<WGmateConfig> listWGmateConfig;

        /// <summary>
        /// userdckt UserDcktMainRec
        /// </summary>
        UserdcktUserDcktMainRec UserDcktMainRec;

        /// <summary>
        /// Usercost UserCostMainRec
        /// </summary>
        UsercostUserCostMainRec UserCostMainRec;

        // gmparams
        DataTable dtDescriptorsRec;
        DataTable dtParametersRec;

        // gmcurrs
        DataTable dtCURCurrenciesRec = null;

        // Gmsizes
        DataTable dtGMSizesRec = null;
        DataTable dtGMSizesGrid = null;

        // nommodel
        DataTable dtnommodelNMODELMRec;

        // CompanyInfo
        DataTable dtCompanyInfo;

        // TaxRates
        double[] ndTaxRates;

        /// <summary>
        /// Gmcurrs
        /// </summary>
        List<GmcurrsCURCurrenciesRec> listgmcurrsCURCurrenciesRec;

        /// <summary>
        /// Nommodel
        /// </summary>
        List<NommodelNMODELMRec> listnommodelNMODELMRec;

        // WGmateConfig
        DataTable dtWGmateConfig;

        #region // Main Window DataTable Properties
        public DataTable TableDescriptors
        {
            get { return dtDescriptorsRec; }
        }
        public DataTable TableParameters
        {
            get { return dtParametersRec; }
        }
        public DataTable TableGMSizesRec
        {
            get { return dtGMSizesRec; }
        }
        public DataTable TableGridGMSizes
        {
            get { return dtGMSizesGrid; }
        }
        public DataTable TableCURCurrenciesRec
        {
            get { return dtCURCurrenciesRec; }
        }
        public DataTable TableCompanyInfo
        {
            get { return dtCompanyInfo; }
        }
        public List<GmcurrsCURCurrenciesRec> ListCURCurrenciesRec
        {
            get { return listgmcurrsCURCurrenciesRec; }
        }
        public List<NommodelNMODELMRec> ListnommodelNMODELMRec
        {
            get { return listnommodelNMODELMRec; }
        }

        public UserdcktUserDcktMainRec RecUserdcktUserDckt
        {
            get { return UserDcktMainRec; }
        }
        public UsercostUserCostMainRec RecUsercostUser
        {
            get { return UserCostMainRec; }
        }
        public string ImageDirectory
        {
            get { return imageDirectory; }
        }
        public string SapDirectory
        {
            get { return sapDirectory; }
        }
        public WGmateConfig TableWGmateConfig
        {
            get { return listWGmateConfig[0]; }
        }
        #endregion

        public AppViewState MainWinViewState
        {
            get { return viewState; }
        }


        public MainWindow()
        {
            string Server = "";
            string[] args = Environment.GetCommandLineArgs();
           
            Server = CommandLine.GetParameter("SQLServer", args);

            if (Server.Length == 0)
            {
                SQLConnection.StarterConnectionString = ((App)Application.Current).StarterConnectionString;
                SQLConnection.SubscriberConnectionString = ((App)Application.Current).SubscriberConnectionString;
            }
            else
            {
                SQLConnection.StarterConnectionString = string.Format("Data Source={0};Database=WGmate;Persist Security Info=false;Integrated Security=false;User Id=startUser;Password=startUser", Server);
                SQLConnection.SubscriberConnectionString = string.Format("Data Source={0};Database=WGmate;Persist Security Info=false;Integrated Security=false;User Id=Henry;Password=Fear4Change", Server);
            }

            InitializeComponent();
            MainMdiContainer.Children.CollectionChanged += (o, e) => Menu_RefreshWindows();
        }
        public IntPtr Handle
        {
            get { return new WindowInteropHelper(this).Handle; }
        }

        /// <summary>
        /// Gmcurrs Dependancy
        /// </summary>
        #region // Gmcurrs Records
        private void DependancyCURCurrenciesRec()
        {
            string sCommand = "dbo.GetGmcurrsCURCurrenciesRec";
            this.depSvcGmcurrsCURCurrenciesRec = new DependancyService();
            this.depSvcGmcurrsCURCurrenciesRec.Start(SQLConnection.StarterConnectionString, SQLConnection.SubscriberConnectionString, sCommand, null, SqlWatcher.SqlCmdType.PROCEDURE);
            this.depSvcGmcurrsCURCurrenciesRec.GetSqlWatcher.OnChange += new SqlWatcher.SqlWatcherEventHandler(GetGmcurrsCURCurrenciesRec_OnChange);

            this.depSvcGmcurrsCURCurrenciesRec.GetSqlWatcher.Start();
        }
        public void GetGmcurrsCURCurrenciesRec_OnChange(DataSet Result)
        {
            dtCURCurrenciesRec = Result.Tables[0];
            listgmcurrsCURCurrenciesRec = WpfClassLibrary.Extensions.ToList<GmcurrsCURCurrenciesRec>(dtCURCurrenciesRec);
        }
        #endregion

        /// <summary>
        ///  Gmparams Dependancy
        /// </summary>
        #region Gmparams // Gmparams Records
        private void DependancyDescriptorsRec()
        {
            string sCommand = "dbo.GetGmparamsDescriptorsRec";
            this.depSvcGmparamsDescriptorsRec = new DependancyService();
            this.depSvcGmparamsDescriptorsRec.Start(SQLConnection.StarterConnectionString, SQLConnection.SubscriberConnectionString, sCommand, null, SqlWatcher.SqlCmdType.PROCEDURE);
            this.depSvcGmparamsDescriptorsRec.GetSqlWatcher.OnChange += new SqlWatcher.SqlWatcherEventHandler(GetDescriptorsRec_OnChange);

            this.depSvcGmparamsDescriptorsRec.GetSqlWatcher.Start();
        }
        public void GetDescriptorsRec_OnChange(DataSet Result)
        {
            dtDescriptorsRec = Result.Tables[0];
        }
        private void DependancyParametersRec()
        {
            string sCommand = "dbo.GetGmparamsParametersRec";
            this.depSvcGmparamsParametersRec = new DependancyService();
            this.depSvcGmparamsParametersRec.Start(SQLConnection.StarterConnectionString, SQLConnection.SubscriberConnectionString, sCommand, null, SqlWatcher.SqlCmdType.PROCEDURE);
            this.depSvcGmparamsParametersRec.GetSqlWatcher.OnChange += new SqlWatcher.SqlWatcherEventHandler(GetParametersRec_OnChange);

            this.depSvcGmparamsParametersRec.GetSqlWatcher.Start();
        }
        public void GetParametersRec_OnChange(DataSet Result)
        {
            dtParametersRec = Result.Tables[0];
        }
        #endregion

        /// <summary>
        /// WgamteConfig Dependancy
        /// </summary>
        #region // WGmateConfig
        private void DependancyWGmateConfig()
        {
            string sCommand = "dbo.GetWGmateConfig";
            this.depSvcWGmateConfig = new DependancyService();
            this.depSvcWGmateConfig.Start(SQLConnection.StarterConnectionString, SQLConnection.SubscriberConnectionString, sCommand, null, SqlWatcher.SqlCmdType.PROCEDURE);
            this.depSvcWGmateConfig.GetSqlWatcher.OnChange += new SqlWatcher.SqlWatcherEventHandler(GetWGmateConfig_OnChange);

            this.depSvcWGmateConfig.GetSqlWatcher.Start();
        }
        public void GetWGmateConfig_OnChange(DataSet Result)
        {
            dtWGmateConfig = Result.Tables[0];

            if (dtWGmateConfig.Rows.Count > 0)
            {
                listWGmateConfig = WpfClassLibrary.Extensions.ToList<WGmateConfig>(dtWGmateConfig);
                imageDirectory = listWGmateConfig[0].ImagePath + @"\";
                sapDirectory = listWGmateConfig[0].SAPDir + @"\";
            }
        }
        #endregion

        /// <summary>
        /// Nommodel Dependancy
        /// </summary>
        #region // Nommodel Records
        private void DependancyNMODELMRec()
        {
            string sCommand = "dbo.GetNommodelNMODELMRec";
            this.depSvcNommodelNMODELMRec = new DependancyService();
            this.depSvcNommodelNMODELMRec.Start(SQLConnection.StarterConnectionString, SQLConnection.SubscriberConnectionString, sCommand, null, SqlWatcher.SqlCmdType.PROCEDURE);
            this.depSvcNommodelNMODELMRec.GetSqlWatcher.OnChange += new SqlWatcher.SqlWatcherEventHandler(GetNommodelNMODELMRec_OnChange);

            this.depSvcNommodelNMODELMRec.GetSqlWatcher.Start();
        }
        public void GetNommodelNMODELMRec_OnChange(DataSet Result)
        {
            dtnommodelNMODELMRec = Result.Tables[0];

            if (dtnommodelNMODELMRec.Rows.Count > 0)
            {
                listnommodelNMODELMRec = WpfClassLibrary.Extensions.ToList<NommodelNMODELMRec>(dtnommodelNMODELMRec);
                WpfClassLibrary.SQLArrayConvert.StringToDoubleArray(listnommodelNMODELMRec[0].taxr, ref ndTaxRates);
            }
        }
        #endregion

        /// <summary>
        ///  Gmsizes Dependancy
        /// </summary>
        #region Gmcurrs // Gmcurrs Records
        private void DependancyGMSizesRec()
        {
            string sCommand = "dbo.GetGmsizesGMSizesRec";
            this.depSvcGmsizesGMSizesRec = new DependancyService();
            this.depSvcGmsizesGMSizesRec.Start(SQLConnection.StarterConnectionString, SQLConnection.SubscriberConnectionString, sCommand, null, SqlWatcher.SqlCmdType.PROCEDURE);
            this.depSvcGmsizesGMSizesRec.GetSqlWatcher.OnChange += new SqlWatcher.SqlWatcherEventHandler(GetGMSizesRec_OnChange);

            this.depSvcGmsizesGMSizesRec.GetSqlWatcher.Start();
        }
        public void GetGMSizesRec_OnChange(DataSet Result)
        {
            dtGMSizesRec = Result.Tables[0];
            dtGMSizesGrid = CommonUtilClass.GetGmSizes(dtGMSizesRec);
        }
        #endregion

        /// <summary>
        ///  CompanyInfo Dependancy
        /// </summary>
        #region Gmcurrs // CompanyInfo Records
        private void DependancyCompanyInfo()
        {
            string sCommand = "dbo.GetCompanyInfo";
            this.depSvcCompanyInfo = new DependancyService();
            this.depSvcCompanyInfo.Start(SQLConnection.StarterConnectionString, SQLConnection.SubscriberConnectionString, sCommand, null, SqlWatcher.SqlCmdType.PROCEDURE);
            this.depSvcCompanyInfo.GetSqlWatcher.OnChange += new SqlWatcher.SqlWatcherEventHandler(GetCompanyInfo_OnChange);

            this.depSvcCompanyInfo.GetSqlWatcher.Start();
        }
        public void GetCompanyInfo_OnChange(DataSet Result)
        {
            dtCompanyInfo = Result.Tables[0];
        }
        #endregion

        /// <summary>
        /// Refresh windows list
        /// </summary>
        void Menu_RefreshWindows()
        {/*
            WindowsMenu.Items.Clear();
            MenuItem mi;
            for (int i = 0; i < MainMdiContainer.Children.Count; i++)
            {
                MdiChild child = MainMdiContainer.Children[i];
                mi = new MenuItem { Header = child.Title };
                mi.Click += (o, e) => child.Focus();
                WindowsMenu.Items.Add(mi);
            }
            WindowsMenu.Items.Add(new Separator());
            WindowsMenu.Items.Add(mi = new MenuItem { Header = "Cascade" });
            mi.Click += (o, e) => MainMdiContainer.MdiLayout = MdiLayout.Cascade;
            WindowsMenu.Items.Add(mi = new MenuItem { Header = "Horizontally" });
            mi.Click += (o, e) => MainMdiContainer.MdiLayout = MdiLayout.TileHorizontal;
            WindowsMenu.Items.Add(mi = new MenuItem { Header = "Vertically" });
            mi.Click += (o, e) => MainMdiContainer.MdiLayout = MdiLayout.TileVertical;

            WindowsMenu.Items.Add(new Separator());
            WindowsMenu.Items.Add(mi = new MenuItem { Header = "Close all" });
            mi.Click += (o, e) => MainMdiContainer.Children.Clear();*/
        }

        private void EnableMenu(MdiChild midi)
        {
            if (midi.Title.Trim().Contains("Enter/Examine"))
                userEnterCosting.IsEnabled = true;
            /*else if (midi.Title.Trim().Contains("Complete"))
            {
                if (midi.Title.Trim().Contains("Reverse Complete"))
                    userReverseDocket.IsEnabled = true;
                else
                    userCompleteDocket.IsEnabled = true;
            }
            else if (midi.Title.Trim().Contains("Reverse"))
            {
                if (midi.Title.Trim().Contains("Reverse Cancel Collation"))
                    userReverseCancelCollate.IsEnabled = true;
                else if (midi.Title.Trim().Contains("Reverse Cancel"))
                    userReverseCancelDocket.IsEnabled = true;
                else
                    userReverseDocket.IsEnabled = true;
            }
            else if (midi.Title.Trim().Contains("Cancel Collation"))
                userCancelCollate.IsEnabled = true;
            else if (midi.Title.Trim().Contains("Cancel"))
                userCancelDocket.IsEnabled = true;
            else if (midi.Title.Trim().Contains("Receive Garments"))
                userReceiveGarments.IsEnabled = true;
            else if (midi.Title.Trim().Contains("Collate Orders"))
                userCollate.IsEnabled = true;
            else if (midi.Title.Trim().Contains("Docketd Quantities"))
                userDockQuantities.IsEnabled = true;
            else if (midi.Title.Trim().Contains("Docket Deliveries"))
                userDockDeliveries.IsEnabled = true;
            else if (midi.Title.Trim().Contains("Materials Issued"))
                userMatIssued.IsEnabled = true;*/
        }
        public void CloseMidiChildWnd(object sender)
        {
            foreach (MdiChild midi in MainMdiContainer.Children)
            {
                if (midi.Content == sender)
                {
                    EnableMenu(midi);
                    midi.Close();
                    break;
                }
            }
        }
        public void SysExitMidiChildWnd(object sender)
        {
            string[] sTitle = ((MdiChild)sender).Title.Trim().Split(' ');

            switch (sTitle[0].Trim())
            {
                case "Enter/Examine":
                    userEnterCosting.IsEnabled = true;
                    break;
                /*case "Complete":
                    userCompleteDocket.IsEnabled = true;
                    break;
                case "Reverse":
                    if (((MdiChild)sender).Title.Trim().Contains("Reverse Complete Docket"))
                        userReverseDocket.IsEnabled = true;
                    else if (((MdiChild)sender).Title.Trim().Contains("Reverse Cancel Docket"))
                        userReverseCancelDocket.IsEnabled = true;
                    else if (((MdiChild)sender).Title.Trim().Contains("Reverse Cancel Collation"))
                        userReverseCancelCollate.IsEnabled = true;
                    break;
                case "Cancel":
                    if (((MdiChild)sender).Title.Trim() == "Cancel Docket")
                        userCancelDocket.IsEnabled = true;
                    else if (((MdiChild)sender).Title.Trim() == "Cancel Collation")
                        userCancelCollate.IsEnabled = true;
                    break;
                case "Receive":     //  Garments
                    userReceiveGarments.IsEnabled = true;
                    break;
                case "Collate":     //  Orders
                    if (((MdiChild)sender).Title.Trim().Contains("Collate Orders"))
                        userCollate.IsEnabled = true;
                    break;
                case "Docketd":
                    userDockQuantities.IsEnabled = true;
                    break;
                case "Docket":
                    userDockDeliveries.IsEnabled = true;
                    break;
                case "Materials":
                    userMatIssued.IsEnabled = true;
                    break;*/
                default:
                    break;
            } // end switch
        }
        public void MidiChildWndTitle(object sender, string Label)
        {
            foreach (MdiChild midi in MainMdiContainer.Children)
            {
                if (midi.Content == sender)
                {
                    if (midi.Title.Trim().Contains("Enter/Examine"))
                        midi.Title = "Enter/Examine" + " /  " + Label;
                    /*else if (midi.Title.Trim().Contains("Receive Garments"))
                        midi.Title = "Receive Garments" + " / " + Label;
                    else if (midi.Title.Trim().Contains("Collate Orders"))
                        midi.Title = "Collate Orders" + " /  " + Label;
                    else if (midi.Title.Trim().Contains("Cancel Collation"))
                        midi.Title = "Cancel Collation" + " /  " + Label;
                    else if (midi.Title.Trim().Contains("Cancel Collation"))
                        midi.Title = "Cancel Collation" + " /  " + Label;*/
                }
            }
        }

        void MainWindow_Closing(object sender, RoutedEventArgs e)
        {
            if (sender is MdiChild)
                SysExitMidiChildWnd(sender);
        }
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DependancyCURCurrenciesRec();
            DependancyDescriptorsRec();
            DependancyParametersRec();
            DependancyWGmateConfig();
            DependancyCompanyInfo();
            DependancyNMODELMRec();
            DependancyGMSizesRec();
        }
        private void userEnteruserEnterCosting_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;

            //MainMdiContainer.Children.Clear();
            userEnterCosting.IsEnabled = false;
            MainMdiContainer.Children.Add(new MdiChild()
            {
                Title = " Enter/Examine",
                Height = 688,
                Width = 910,
                //Style = null,
                Resizable = true,
                //MinimizeBox = false,
                //Here UserRegistration is the class that you have created for mainWindow.xaml user control.
                Content = new EnterCostnControl()
            });

            MainMdiContainer.Width = 916;
            MainMdiContainer.Height = 696;
            MainMdiContainer.Children[MainMdiContainer.Children.Count - 1].Position = new Point(0, 0);
            ((EnterCostnControl)MainMdiContainer.Children[MainMdiContainer.Children.Count - 1].Content).ContainerWindow = this;
            ((EnterCostnControl)MainMdiContainer.Children[MainMdiContainer.Children.Count - 1].Content).MenuOption = 1;
            MainMdiContainer.Children[MainMdiContainer.Children.Count - 1].Closing += MainWindow_Closing;
        }
    }
}