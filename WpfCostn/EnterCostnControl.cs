//-----------------------------------------------------------------------
// <copyright file="EnterCostnControl.cs" company="ComputerLink">
//     Copyright (c) ComputerLink.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.IO;
using System.Xml;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using WpfClassLibrary;
using C1.WPF.DataGrid;
using System.Xml.Linq;
using WpfReportLibrary;
using System.Data.OleDb; // 
using System.Diagnostics;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using SQLDependancyService;
using WpfClassLibrary.Model;
using System.Windows.Shapes;
using System.Data.SqlClient;
using WpfReportLibrary.Model;
using System.Windows.Interop;
using System.Threading.Tasks;
using System.Windows.Controls;
using WpfWgmateControlLibrary;
using System.Windows.Documents;
using System.Windows.Navigation;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;

namespace WpfCostn
{
    /// <summary>
    /// Interaction logic for EnterCostnControl.xaml
    /// </summary>
    /// 
    public partial class EnterCostnControl : UserControl
    {
        AppViewState viewState = AppViewState.EMPTY;
        private const short ROWSCOUNT = 12;

        private const short DocketInstructType = 9;
        private const short DocketEmbellish = 28;
        private const short DocketPacking = 29;

        bool bLoaded = false;
        bool bSpinButton = false;
        bool bLoadByStyleDetails = false;

        int StyleVarnIndex = -1;
        string imageDirectory = "";
        SQLWriteClass SQLWrite = null;
        List<Visual> winCtrlList = null;
        List<StyleVarn> StyleVarnList = null;


        /// <summary>
        /// Wgamte Configuration 
        /// </summary>
        DependancyService depSvcWGmateConfig;

        /// <summary>
        /// Costdb
        /// </summary>
        DependancyService depSvcCostMrec;
        DependancyService depSvcCostCTMatsRec;
        DependancyService depSvcCostCTNotsRec;
        DependancyService depSvcCostCTOpersRec;
        DependancyService depSvcCostCTMatsFFRec;
        DependancyService depSvcCostCTCTInstsRec;

        /// <summary>
        /// Gmparams
        /// </summary>
        DependancyService depSvcGmparamsDescriptorsRec;
        DependancyService depSvcGmparamsParametersRec;

        /// <summary>
        /// Rollsdb
        /// </summary>
        // DependancyService depSvcRollsMRec;

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
        /// Wgmate Configuration
        /// </summary>
        List<WGmateConfig> listWGmateConfig;

        /// <summary>
        /// Costdb
        /// </summary>
        List<CostdbCostMRec> listcostMRec;
        List<CostdbCTMatsRec> listcostMatsRec;
        List<CostdbCTNotsRec> listcostNotsRec;
        List<CostdbCTMatsFFRec> listcostMatsFFRec;
        List<CostdbCTInstsRec> listcostInstsRec;
        List<CostdbCTOpersRec> listcostOpersRec;

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

        // costdb
        DataTable dtCostMrec;
        DataTable dtcostCTMatsRec;
        DataTable dtcostCTNotsRec;
        DataTable dtcostCTOpersRec;
        DataTable dtcostCTMatsFFRec;
        DataTable dtcostCTCTInstsRec;

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

        // TaxRates
        double[] ndTaxRates;

        // usercost
        UsercostUserCostMainRec UserCostMainRec;

        #region // Pop-up screens Data Objects Need for New Costing Sheet
        // GmcopersGMCOperMRec && GmcopersGMCOpersRec
        public MakeCostLists MakeCost { get; set; }

        // Gmcopers
        List<GmcopersGMCOperMRec> ListGmcopersGMCOperMRec;
        List<GmcopersGMCOpersRec> ListGmcopersGMCOpersRec;
        #endregion

        // Previous ProfMarginPerc
        double PrevCTProfMar1 = 0.0;
        double PrevCTSelPrice1 = 0.0;
        double PrevCTProfMarPer1 = 0.0;
   
        #region // Main Window DataTable Properties
        private StyleVarnEventArgs styleVarn;
        public StyleVarnEventArgs StyleVarnArg 
        {
            get { return styleVarn; }
            set { styleVarn = value; } 
        }
        #endregion

        public AppViewState MainWinViewState 
        {
            get { return viewState; }
        }

        private GmcurrsCURCurrenciesRec LastCurrcyRec { get; set; }
        public Window ContainerWindow { get; set; }
        public short MenuOption { get; set; }

        public EnterCostnControl()
        {
            InitializeComponent();

            MakeCost = null;
            styleVarn = new StyleVarnEventArgs();
            StyleVarnList = new List<StyleVarn>();
            listcostMRec = new List<CostdbCostMRec>();
            LastCurrcyRec = new GmcurrsCURCurrenciesRec();
            listcostMatsRec = new List<CostdbCTMatsRec>();
            listcostNotsRec = new List<CostdbCTNotsRec>();
            listcostInstsRec = new List<CostdbCTInstsRec>();
            listcostOpersRec = new List<CostdbCTOpersRec>();
            listcostMatsFFRec = new List<CostdbCTMatsFFRec>();

            UserCostMainRec = new UsercostUserCostMainRec();

            viewState = AppViewState.EMPTY;
            winCtrlList = new List<Visual>();
            datePickerDate.SelectedDate = DateTime.Now;
            SQLWrite = new SQLWriteClass(SQLConnection.SubscriberConnectionString);
            imageDirectory = (string)Application.Current.MainWindow.GetType().GetProperty("ImageDirectory").GetValue(Application.Current.MainWindow, null);
            dtGMSizesRec = (DataTable)Application.Current.MainWindow.GetType().GetProperty("TableGMSizesRec").GetValue(Application.Current.MainWindow, null);
            dtParametersRec = (DataTable)Application.Current.MainWindow.GetType().GetProperty("TableParameters").GetValue(Application.Current.MainWindow, null);
            dtCURCurrenciesRec = (DataTable)Application.Current.MainWindow.GetType().GetProperty("TableCURCurrenciesRec").GetValue(Application.Current.MainWindow, null);
            listnommodelNMODELMRec = (List<NommodelNMODELMRec>)Application.Current.MainWindow.GetType().GetProperty("ListnommodelNMODELMRec").GetValue(Application.Current.MainWindow, null);


            #region // Dependancy Objects
            depSvcCostMrec = new DependancyService();
            depSvcCostCTMatsRec = new DependancyService();
            depSvcCostCTNotsRec = new DependancyService();
            depSvcCostCTOpersRec = new DependancyService();
            depSvcCostCTCTInstsRec = new DependancyService();
            depSvcGmparamsDescriptorsRec = new DependancyService();
            depSvcGmparamsParametersRec = new DependancyService();
            #endregion
            #region // DataTables
            dtCostMrec = new DataTable();
            dtcostCTMatsRec = new DataTable();
            dtcostCTNotsRec = new DataTable();
            dtcostCTOpersRec = new DataTable();
            dtcostCTCTInstsRec = new DataTable();
            #endregion

            uscGmSizes.GmSizesEvent += uscGmSizes_GmSizesEvent;
            uscTypeParam.TypeEvent += uscTypeParam_TypeEvent;
            uscClientDetails.Loaded += uscClientDetails_Loaded;
            uscBrandParam.TypeEvent += uscBrandParam_TypeEvent;
            uscGenderParam.TypeEvent += uscGenderParam_TypeEvent;
            uscCountryParam.TypeEvent += uscCountryParam_TypeEvent;
            uscCatagoryParam.TypeEvent += uscCatagoryParam_TypeEvent;
            MagnifierOptionsControl.MagEvent += MagnifierOptionsControl_MagEvent;
            uscCostnStyleDetails.StyleVarnEvent += uscCostnStyleDetails_StyleVarnEvent;
        }
       
        #region // dropdown buttons Events
        void uscClientDetails_Loaded(object sender, RoutedEventArgs e)
        {
        }
        void uscGmSizes_GmSizesEvent(object sender, SizesEventArgs ev)
        {
            txtSize.Text = ev.Sizes.ItemArray[0].ToString();
        }
        void uscCatagoryParam_TypeEvent(object sender, TypeEventArgs ev)
        {
            txtCatagory.Text = ev.Type.ToString();
            txtblkCatagory.Text = ev.TypeDesc;
        }
        void uscCountryParam_TypeEvent(object sender, TypeEventArgs ev)
        {
            txtBrand.Text = ev.Type.ToString();
        }
        void uscGenderParam_TypeEvent(object sender, TypeEventArgs ev)
        {
            txtGender.Text = ev.Type.ToString();
            txtblkGender.Text = ev.TypeDesc;
        }
        void uscBrandParam_TypeEvent(object sender, TypeEventArgs ev)
        {
            txtBrand.Text = ev.Type.ToString();
            txtblkBrandDesc.Text = ev.TypeDesc;
        }
        void uscTypeParam_TypeEvent(object sender, TypeEventArgs ev)
        {
            txtType.Text = ev.Type.ToString();
            txtblkTypeLabel.Text = ev.TypeDesc;
        }
        #endregion

        /// <summary>
        /// Costdb Dependancy
        /// </summary>
        #region // Costdb Database Records
        private void DependancyCostMrec()
        {
            string sCommand = "dbo.GetCostdbStyleVarn";
            SqlParameter[] SqlParam = new SqlParameter[2];

            SqlParam[0] = DependancyService.SQLParameter("@CTStyle",
                        "System.String", DependancyService.ParamDirection.Input, (object)txtStyle.Text.Trim());

            SqlParam[1] = DependancyService.SQLParameter("@CTVarn",
                        "System.String", DependancyService.ParamDirection.Input, (object)txtVarn.Text.Trim());

            this.depSvcCostMrec = new DependancyService();
            this.depSvcCostMrec.Start(SQLConnection.StarterConnectionString, SQLConnection.SubscriberConnectionString, sCommand, SqlParam, SqlWatcher.SqlCmdType.PROCEDURE);
            this.depSvcCostMrec.GetSqlWatcher.OnChange += new SqlWatcher.SqlWatcherEventHandler(GetCostdbStyleVarn_OnChange);

            this.depSvcCostMrec.GetSqlWatcher.Start();
        }
        public void GetCostdbStyleVarn_OnChange(DataSet Result)
        {
            dtCostMrec = Result.Tables[0];

            if (dtCostMrec.Rows.Count > 0)
            { 
                DependancyCTMatsRec();
                DependancyCTMatsFFRec();
                DependancyCTNotsRec();
                DependancyCTInstsRec();
                DependancyCTOpersRec();
            }
            else /// if (viewState != AppViewState.NEW)
            {
                Mouse.OverrideCursor = null;

                switch (viewState)
                { 
                    case AppViewState.NEW:
                        if (txtStyle.Text.Trim().Length > 0)
                            DisplayStyleImage(txtStyle.Text.Trim());
                        break;
                    default:
                        imgStyle.Source = null;
                        listcostMatsRec.Clear();
                        listcostNotsRec.Clear();
                        listcostMatsFFRec.Clear();
                        listcostInstsRec.Clear();
                        listcostOpersRec.Clear();
                        ClearCostnSheet();

                        viewState = AppViewState.SEARCH;
                        EnableControls(true);

                        listcostMRec.Clear();
                        MainGrid.ItemsSource = Enumerable.Range(0, ROWSCOUNT).Select(i => new CostdbCTMatsRec());
                        break;
                } // end switch
            } // end else
        }
        private void DependancyCTMatsRec()
        {
            string sCommand = "dbo.GetCostdbCTMatsRec";
            SqlParameter[] SqlParam = new SqlParameter[1];

            SqlParam[0] = DependancyService.SQLParameter("@CostdbID",
                        "System.String", DependancyService.ParamDirection.Input, (object)dtCostMrec.Rows[0]["CostdbID"].ToString());

            this.depSvcCostCTMatsRec = new DependancyService();
            this.depSvcCostCTMatsRec.Start(SQLConnection.StarterConnectionString, SQLConnection.SubscriberConnectionString, sCommand, SqlParam, SqlWatcher.SqlCmdType.PROCEDURE);
            this.depSvcCostCTMatsRec.GetSqlWatcher.OnChange += new SqlWatcher.SqlWatcherEventHandler(GetCTMatsRecs_OnChange);

            this.depSvcCostCTMatsRec.GetSqlWatcher.Start();
        }
        public void GetCTMatsRecs_OnChange(DataSet Result)
        {
            dtcostCTMatsRec = Result.Tables[0];
        }
        private void DependancyCTMatsFFRec()
        {
            string sCommand = "dbo.GetCostdbCTMatsFFRec";
            SqlParameter[] SqlParam = new SqlParameter[1];

            SqlParam[0] = DependancyService.SQLParameter("@CostdbID",
                        "System.String", DependancyService.ParamDirection.Input, (object)dtCostMrec.Rows[0]["CostdbID"].ToString());

            this.depSvcCostCTMatsFFRec = new DependancyService();
            this.depSvcCostCTMatsFFRec.Start(SQLConnection.StarterConnectionString, SQLConnection.SubscriberConnectionString, sCommand, SqlParam, SqlWatcher.SqlCmdType.PROCEDURE);
            this.depSvcCostCTMatsFFRec.GetSqlWatcher.OnChange += new SqlWatcher.SqlWatcherEventHandler(GetCTMatsFFRec_OnChange);

            this.depSvcCostCTMatsFFRec.GetSqlWatcher.Start();
        }
        public void GetCTMatsFFRec_OnChange(DataSet Result)
        {
            dtcostCTMatsFFRec = Result.Tables[0];
        }
        private void DependancyCTNotsRec()
        {
            string sCommand = "dbo.GetCostdbCTNotsRec";
            SqlParameter[] SqlParam = new SqlParameter[1];

            SqlParam[0] = DependancyService.SQLParameter("@CostdbID",
                        "System.String", DependancyService.ParamDirection.Input, (object)dtCostMrec.Rows[0]["CostdbID"].ToString());

            this.depSvcCostCTNotsRec = new DependancyService();
            this.depSvcCostCTNotsRec.Start(SQLConnection.StarterConnectionString, SQLConnection.SubscriberConnectionString, sCommand, SqlParam, SqlWatcher.SqlCmdType.PROCEDURE);
            this.depSvcCostCTNotsRec.GetSqlWatcher.OnChange += new SqlWatcher.SqlWatcherEventHandler(GetCTNotsRec_OnChange);

            this.depSvcCostCTNotsRec.GetSqlWatcher.Start();
        }
        public void GetCTNotsRec_OnChange(DataSet Result)
        {
            dtcostCTNotsRec = Result.Tables[0];
        }
        private void DependancyCTInstsRec()
        {
            string sCommand = "dbo.GetCostdbCTInstsRec";
            SqlParameter[] SqlParam = new SqlParameter[1];

            SqlParam[0] = DependancyService.SQLParameter("@CostdbID",
                        "System.String", DependancyService.ParamDirection.Input, (object)dtCostMrec.Rows[0]["CostdbID"].ToString());

            this.depSvcCostCTCTInstsRec = new DependancyService();
            this.depSvcCostCTCTInstsRec.Start(SQLConnection.StarterConnectionString, SQLConnection.SubscriberConnectionString, sCommand, SqlParam, SqlWatcher.SqlCmdType.PROCEDURE);
            this.depSvcCostCTCTInstsRec.GetSqlWatcher.OnChange += new SqlWatcher.SqlWatcherEventHandler(GetCTInstsRec_OnChange);

            this.depSvcCostCTCTInstsRec.GetSqlWatcher.Start();
        }
        public void GetCTInstsRec_OnChange(DataSet Result)
        {
            dtcostCTCTInstsRec = Result.Tables[0];
        }
        private void DependancyCTOpersRec()
        {
            string sCommand = "dbo.GetCostdbCTOpersRec";
            SqlParameter[] SqlParam = new SqlParameter[1];

            SqlParam[0] = DependancyService.SQLParameter("@CostdbID",
                        "System.String", DependancyService.ParamDirection.Input, (object)dtCostMrec.Rows[0]["CostdbID"].ToString());

            this.depSvcCostCTOpersRec = new DependancyService();
            this.depSvcCostCTOpersRec.Start(SQLConnection.StarterConnectionString, SQLConnection.SubscriberConnectionString, sCommand, SqlParam, SqlWatcher.SqlCmdType.PROCEDURE);
            this.depSvcCostCTOpersRec.GetSqlWatcher.OnChange += new SqlWatcher.SqlWatcherEventHandler(GetCTOpersRec_OnChange);

            this.depSvcCostCTOpersRec.GetSqlWatcher.Start();
        }
        public void GetCTOpersRec_OnChange(DataSet Result)
        {
            dtcostCTOpersRec = Result.Tables[0];

            if (viewState != AppViewState.NEW) viewState = AppViewState.VIEW;
            Dispatcher.BeginInvoke(new Action(DisplayCostdbRecord));
        }
        #endregion

        private void EnableControls(bool bDisable)
        {
            Type_Panel.IsEnabled = bDisable;
            Middle_Left_Panel.IsEnabled = bDisable;
            Middle_Mid_Panel.IsEnabled = bDisable;
            txtYear.IsEnabled = bDisable;
            datePickerDate.IsEnabled = bDisable;

            switch (viewState)
            {
                case AppViewState.VIEW:
                    btnDelAllColrCombs.IsEnabled = false;
                    btnCareInstruct.IsEnabled = true;
                    Mid_Left_panel.IsEnabled = true;
                    MainGrid.CanUserEditRows = false;
                    btnOperations.IsEnabled = false;
                    btnChangeWhse.IsEnabled = false;
                    btnPackInstruc.IsEnabled = true;
                    btnDcknInstruc.IsEnabled = true;
                    btnCostBySize.IsEnabled = true;
                    btnOperations.IsEnabled = true;
                    btnPriceList.IsEnabled = true;
                    btnGarmetCol.IsEnabled = true;
                    btnDelStyle.IsEnabled = false;
                    btnMakeCost.IsEnabled = true;
                    btnDelLine.IsEnabled = false;
                    btnProcess.IsEnabled = false;
                    btnMatProp.IsEnabled = true;
                    btnCurrcy.IsEnabled = false;
                    btnColComb.IsEnabled = true;
                    btnEmblish.IsEnabled = true;
                    btnInsert.IsEnabled = false;
                    btnCusPref.IsEnabled = true;
                    btnWidth.IsEnabled = false;
                    btnConsum.IsEnabled = true;
                    btnCopy.IsEnabled = false;
                    btnExtra.IsEnabled = true;
                    btnNotes.IsEnabled = true;
                    btnSave.IsEnabled = false;
                    btnPrint.IsEnabled = true;
                    txtStyle.IsEnabled = true;
                    txtVarn.IsEnabled = true;
                    txtDesc.IsEnabled = false;
                    btnExit.IsEnabled = true;
                    btnSpec.IsEnabled = true;
                    btnEdit.IsEnabled = true;
                    btnNew.IsEnabled = true;

                    ((WPFMDIForm.MainWindow)ContainerWindow).MidiChildWndTitle(this, "View");
                    break;
                case AppViewState.EDIT:
                    btnDelAllColrCombs.IsEnabled = true;
                    btnCareInstruct.IsEnabled = true;
                    Mid_Left_panel.IsEnabled = true;
                    btnDcknInstruc.IsEnabled = true;
                    btnPackInstruc.IsEnabled = true;
                    MainGrid.CanUserEditRows = true;
                    btnModMaterial.IsEnabled = true;
                    btnCostBySize.IsEnabled = true;
                    btnChangeWhse.IsEnabled = true;
                    btnOperations.IsEnabled = true;
                    btnPriceList.IsEnabled = true;
                    btnGarmetCol.IsEnabled = true;
                    btnNonStock.IsEnabled = true;
                    btnMakeCost.IsEnabled = true;
                    btnDelStyle.IsEnabled = true;
                    btnProcess.IsEnabled = true;
                    btnDelLine.IsEnabled = true;
                    btnColComb.IsEnabled = true;
                    btnCusPref.IsEnabled = true;
                    btnEmblish.IsEnabled = true;
                    btnApprove.IsEnabled = true;
                    btnSizeMap.IsEnabled = true;
                    btnMatProp.IsEnabled = true;
                    btnCurrcy.IsEnabled = true;
                    btnConsum.IsEnabled = true;
                    btnInsert.IsEnabled = true;
                    txtStyle.IsEnabled = false;
                    txtVarn.IsEnabled = false;
                    btnGrade.IsEnabled = true;
                    btnEdit.IsEnabled = false;
                    btnWidth.IsEnabled = true;
                    btnPrint.IsEnabled = true;
                    btnNotes.IsEnabled = true;
                    btnSave.IsEnabled = true;
                    btnSpec.IsEnabled = true;
                    btnExit.IsEnabled = true;
                    btnNew.IsEnabled = false;
                    btnCopy.IsEnabled = true;
                    txtDesc.IsEnabled = true;
                    btnDtm.IsEnabled = true;

                    ((WPFMDIForm.MainWindow)ContainerWindow).MidiChildWndTitle(this, "Edit");
                    break;
                case AppViewState.NEW:
                    txtYear.Text = DateTime.Now.Year.ToString();
                    btnDelAllColrCombs.IsEnabled = false;
                    btnCareInstruct.IsEnabled = false;
                    Mid_Left_panel.IsEnabled = true;
                    btnModMaterial.IsEnabled = false;
                    btnPackInstruc.IsEnabled = false;
                    btnDcknInstruc.IsEnabled = false;
                    MainGrid.CanUserEditRows = true;
                    btnCostBySize.IsEnabled = false;
                    btnChangeWhse.IsEnabled = false;
                    btnOperations.IsEnabled = false;
                    btnGarmetCol.IsEnabled = false;
                    btnPriceList.IsEnabled = false;
                    btnMakeCost.IsEnabled = false;
                    btnDelStyle.IsEnabled = true;
                    btnCusPref.IsEnabled = false;
                    btnDelLine.IsEnabled = true;
                    btnColComb.IsEnabled = false;
                    btnEmblish.IsEnabled = false;
                    btnProcess.IsEnabled = false;
                    btnMatProp.IsEnabled = false;
                    btnSizeMap.IsEnabled = false;
                    btnCurrcy.IsEnabled = false;
                    btnConsum.IsEnabled = false;
                    btnPrint.IsEnabled = false;
                    btnInsert.IsEnabled = true;
                    btnWidth.IsEnabled = false;
                    btnExtra.IsEnabled = false;
                    btnNotes.IsEnabled = true;
                    btnGrade.IsEnabled = false;
                    btnEdit.IsEnabled = false;
                    btnSave.IsEnabled = true;
                    btnSpec.IsEnabled = false;
                    btnCopy.IsEnabled = true;
                    btnExit.IsEnabled = true;
                    btnNew.IsEnabled = false;
                    txtDesc.IsEnabled = true;
                    listcostNotsRec.Clear();
                    imgStyle.Source = null;

                    ((WPFMDIForm.MainWindow)ContainerWindow).MidiChildWndTitle(this, "New");
                    break;
                case AppViewState.EMPTY:
                case AppViewState.SEARCH:
                    btnDelAllColrCombs.IsEnabled = false;
                    btnCareInstruct.IsEnabled = false;
                    Mid_Left_panel.IsEnabled = false;
                    btnModMaterial.IsEnabled = false;
                    btnPackInstruc.IsEnabled = false;
                    btnDcknInstruc.IsEnabled = false;
                    MainGrid.CanUserEditRows = false;
                    btnOperations.IsEnabled = false;
                    btnCostBySize.IsEnabled = false;
                    btnChangeWhse.IsEnabled = false;
                    btnGarmetCol.IsEnabled = false;
                    btnPriceList.IsEnabled = false;
                    btnNonStock.IsEnabled = false;
                    btnMakeCost.IsEnabled = false;
                    btnDelStyle.IsEnabled = false;
                    btnCusPref.IsEnabled = false;
                    btnDelLine.IsEnabled = false;
                    btnColComb.IsEnabled = false;
                    btnEmblish.IsEnabled = false;
                    btnProcess.IsEnabled = false;
                    btnMatProp.IsEnabled = false;
                    btnApprove.IsEnabled = false;
                    btnSizeMap.IsEnabled = false;
                    btnCurrcy.IsEnabled = false;
                    btnConsum.IsEnabled = false;
                    btnGrade.IsEnabled = false;
                    btnPrint.IsEnabled = false;
                    btnInsert.IsEnabled = false;
                    btnWidth.IsEnabled = false;
                    btnExtra.IsEnabled = false;
                    btnNotes.IsEnabled = false;
                    btnSave.IsEnabled = false;
                    btnSpec.IsEnabled = false;
                    btnEdit.IsEnabled = false;
                    btnCopy.IsEnabled = false;
                    btnExit.IsEnabled = true;
                    txtDesc.IsEnabled = false;
                    btnNew.IsEnabled = true;
                    btnDtm.IsEnabled = false;
                    listcostNotsRec.Clear();
                    imgStyle.Source = null;

                    ((WPFMDIForm.MainWindow)ContainerWindow).MidiChildWndTitle(this, "");
                    break;
                default:
                    break;
            } // end switch
        }
        private void InitialiseControls()
        {
            txtblkCatagory.Text = "Home Currcy";
            txtCountry.Text = "0";
            txtCountryPerc.Text = "0.00";
            txtblkQprTot.Text = "0.000";
            txtblkMatCost.Text = "0.000";
            lblMatCostF.Text = "0.000";
            //lblMatCostX.Content = "0.000";
            txtMakePrice.Text = "0.000";
            txtMakePriceF.Text = "0.000";
            //txtMakePriceX.Text = "0.000";
            txtDutyPerc.Text = "0.00";
            txtDuty.Text = "0.000";
            txtDutyF.Text = "0.000";
            //txtDutyF.Text = "0.000";
            txtOverHeadsPer.Text = "0.00";
            txtOverHeads.Text = "0.000";
            txtOverHeadsF.Text = "0.000";
            //txtOverHeadsX.Text = "0.000";
            txtContinPer.Text = "0.00";
            txtContin.Text = "0.000";
            txtContinF.Text = "0.000";
            //txtContinX.Text = "0.000";
            txtManuFCost.Text = "0.000";
            txtManuFCostF.Text = "0.000";
            //txtManuFCostX.Text = "0.000";
            txtMarkUpPerc.Text = "0.00";
            txtMarkUp.Text = "0.000";
            txtMarkUpF.Text = "0.000";
            //txtMarkUpX.Text = "0.000";
            txtMargin.Text = "0.00";
            txtProfMarF.Text = "0.000";
            //txtCurrcyDiff.Text = "0.000";
            txtSelPrice.Text = "0.000";
            txtSelPrice_F.Text = "0.000";
            //txtSelPrice_X.Text = "0.000";
            txtSelPricePerc2.Text = "0.00";
            txtSelPricePerc3.Text = "0.00";
            txtSelPricePerc4.Text = "0.00";
            txtSelPrice2H.Text = "0.000";
            txtSelPrice3H.Text = "0.000";
            txtSelPrice4H.Text = "0.000";
            txtSelPrice2F.Text = "0.000";
            txtSelPrice3F.Text = "0.000";
            txtSelPrice4F.Text = "0.000";
            txtSelPricePlusVat.Text = "0.000";
            txtTimeMake.Text = "0.00";
            txtDiscountPerc.Text = "0.00";
            txtDiscount.Text = "0.000";
            txtStyleMarkUpPerc.Text = "0.00";
            lblProfitAfterDisc.Content = "0.000";
            txtMarginProfitDisc.Text = "0.00";
        }
        private void ClearCostnSheet()
        {
            ClearControls();
            MakeCost = null;
            bSpinButton = false;
            StyleVarnIndex = -1;
            PrevCTProfMar1 = 0.0;
            PrevCTSelPrice1 = 0.0;
            PrevCTProfMarPer1 = 0.0;
            StyleVarnArg.Varn = "";
            StyleVarnArg.Style = "";
            bLoadByStyleDetails = false;
            MainGrid.ItemsSource = null;
            rdoTradeDisc.IsChecked = true;
            ListGmcopersGMCOperMRec = null;
            ListGmcopersGMCOpersRec = null;
            chkBoxDiscFlag.IsChecked = false;
            MainGrid.ItemsSource = listcostMatsRec;
            datePickerDate.SelectedDate = DateTime.Now;
            UserCostMainRec = new UsercostUserCostMainRec();

            btnNotes.Background = btnPrint.Background;
            btnExtra.Background = btnPrint.Background;
            btnExtra.Foreground = btnPrint.Foreground;
            btnDcknInstruc.Background = btnPrint.Background;
            btnDcknInstruc.Foreground = btnPrint.Foreground;
            btnEmblish.Background = btnPrint.Background;
            btnPackInstruc.Background = btnPrint.Background;
            btnPackInstruc.Foreground = btnPrint.Foreground;
        }
        private bool IsCTMatsRecValid()
        {
            bool bValid = false;

            if (listcostMatsRec[MainGrid.SelectedIndex].CTMatType > 0 &&
                    listcostMatsRec[MainGrid.SelectedIndex].CTMatCode.Trim().Length > 0 &&
                        listcostMatsRec[MainGrid.SelectedIndex].CTMatColr > 0 &&
                            listcostMatsRec[MainGrid.SelectedIndex].CTMatSupl > 0 &&
                                listcostMatsRec[MainGrid.SelectedIndex].CTMatWhse > 0 &&
                                    listcostMatsRec[MainGrid.SelectedIndex].CTMatLocn > 0)
                bValid = true;

            return bValid;
        }
        private void ClearControls()
        {
            txtStyle.Text = "";
            txtVarn.Text = "";
            txtDesc.Text = "";
            //txtType.Text = "";
            //txtBrand.Text = "";
            //txtGender.Text = "";
            //txtSize.Text = "";
            //txtCatagory.Text = "";
            chkBoxDiscFlag.IsChecked = false;
            chkBoxDiscPercFlag.IsChecked = false;

            if (listcostMRec != null && listcostMRec.Count > 0)
            { 
                listcostMRec[0].CTGarType = null;
                listcostMRec[0].CTUkSzKey = null;
                listcostMRec[0].CTMinExpSz = null;
                listcostMRec[0].CTSpareShort1 = null;
                listcostMRec[0].CTSpareShort2 = null;
            }

            // Clear Color Flags before deleting
            (from CostdbCTMatsRec in listcostMatsRec
                         where CostdbCTMatsRec.CTMatType > 0
                         select CostdbCTMatsRec).Count(x =>
                        {
                            x.ColourCombinationsA = false;
                            x.CustomerPreferencesA = false;
                            x.MatComsumtionA = false;
                            x.MatPriceModifiedA = false;
                            return true;
                        });

            MainGrid.Refresh();

            foreach (Visual vs in winCtrlList)
            {
                string sType = vs.GetType().ToString();

                switch (vs.GetType().ToString())
                {
                    case "System.Windows.Controls.ComboBox":
                        ((ComboBox)vs).SelectedIndex = -1;
                        break;
                    case "System.Windows.Controls.TextBox":
                        // ((TextBox)vs).Text = "";
                        break;
                    case "System.Windows.Controls.Label":
                        if (!((Label)vs).Name.Contains("lblPerc"))
                            ((Label)vs).Content = "";
                        break;
                    case "System.Windows.Controls.TextBlock":
                        if (((TextBlock)vs).Name.Contains("txtblk") && ((TextBlock)vs).Name != "txtblkHomeCurrcy")
                            ((TextBlock)vs).Text = "";
                        break;
                    case "Xceed.Wpf.Toolkit.DropDownButton":
                        break;
                    case "Xceed.Wpf.DataGrid.Controls.MultiColumnComboBox":
                        //((Xceed.Wpf.DataGrid.Controls.MultiColumnComboBox)vs).SelectedIndex = -1;
                        break;
                    case "C1.WPF.DataGrid.C1DataGrid":
                        if (listcostMatsRec != null)
                        {
                            listcostMatsRec.Clear();
                            MainGrid.ItemsSource = null;
                            MainGrid.ItemsSource = listcostMatsRec;
                        }
                        break;
                    default:
                        break;
                }
            }
            InitialiseControls();
            ColorOneOfTrioButton();
        }
        private void GetStyleVarn()
        {
            DependancyCostMrec();
        }
        private void GetStyleList()
        {
            SQLWrite.SQLWriteCommand("GetCostdbStyle", SQLWriteClass.SqlCmdType.PROCEDURE);
            SqlParameter[] SqlParam = new SqlParameter[1];

            SqlParam[0] = DependancyService.SQLParameter("@Style",
                    "System.String", DependancyService.ParamDirection.Input, (object)txtStyle.Text.Trim());

            DataTable dtStyless = SQLWrite.ExecuteDataTableQuery(SqlParam);

            if (dtStyless.Rows.Count > 0)
            {
                StyleVarnIndex = 0;
                StyleVarnList.Clear();
                StyleVarnList.AddRange(WpfClassLibrary.Extensions.ToList<StyleVarn>(dtStyless));
            }
        }

        private double TotalQPRMatCost()
        {
            double ndValue = 0.0;

            var sQuery = (from CostdbCTMatsRec in listcostMatsRec
                          where CostdbCTMatsRec.CTMatType > 0 && CostdbCTMatsRec.CTSpareFlag4 > 0
                          select CostdbCTMatsRec.CTCosting * CostdbCTMatsRec.CTMatPrice).Sum();

            ndValue = (double)sQuery;

            return ndValue;
        }
        private double CalculateDiscount()  
        {
            double nDisc = 0;

            nDisc = ((double)listcostMRec[0].CTSelPrice1) *
                            ((100.0 - ((double)listcostMRec[0].CTDiscountPer)) / 100.0);
            listcostMRec[0].DiscValue = nDisc;

            return nDisc;
        }
        private double GetVatRate(int VatNo)
        {
            double ndVatRate = 0;
            double[] VatRates = new double[24];

            if (listnommodelNMODELMRec.Count > 0)
            {
                SQLArrayConvert.StringToDoubleArray(listnommodelNMODELMRec[0].taxr, ref VatRates);
                ndVatRate = VatRates[VatNo - 1];
            }

            return ndVatRate;
        }

        // Used in code for txtMargin not sure when?
        // ((CalculateDiscount() - listcostMRec[0].CTTotManCost) / listcostMRec[0].CTSelPrice1) * 100.0)
        // CalculateSellPricePlusVat() = ((double)listcostMRec[0].CTSelPrice1) + ((((double)listcostMRec[0].CTSelPrice1) * ((double)listcostMRec[0].CTVatRate)) / 100.0)

        private double ProfitAterDiscount()
        {
            double ndValue = 0.0;

            ndValue = (double)listcostMRec[0].DiscValue - (double)listcostMRec[0].CTTotManCost;

            return ndValue;
        }
        private double CalculateProfitMargin()
        {
            double ndPofitMargin = 0.0;

            ndPofitMargin = (((double)listcostMRec[0].CTProfMar1) / ((double)listcostMRec[0].CTSelPrice1)) * 100.0;

            return ndPofitMargin;
        }
        private double CalculateMarkUpPercent()
        {
            double ndValue = 0.0;

            ndValue = (((double)listcostMRec[0].DiscValue - (double)listcostMRec[0].CTTotManCost) / (double)listcostMRec[0].CTTotManCost) * 100.0;

            return ndValue;
        }
        private double CalculateSellPricePlusVat()
        {
            double ndSellPricePlusVat = 0.0;

            // CostMain.CTSelPrice1 + ((CostMain.CTSelPrice1 * m_dblVatRate) / 100.0)
            ndSellPricePlusVat = ((double)listcostMRec[0].CTSelPrice1) + ((((double)listcostMRec[0].CTSelPrice1) * GetVatRate(1)) / 100.0);

            return ndSellPricePlusVat;
        }
        private double CaculateProfitMarginDiscount()
        {
            double ndValue = 0.0;

            //ndValue = (CalculateDiscount() - ((double)listcostMRec[0].CTTotManCost)) / ((double)listcostMRec[0].CTSelPrice1);
            ndValue = (((double)listcostMRec[0].DiscValue - ((double)listcostMRec[0].CTTotManCost)) / ((double)listcostMRec[0].CTSelPrice1)) * 100.0;

            return ndValue;
        }
        private void CalculateAndDisplayDiscountValues()
        {
            double ndValue = 0;

            if ((bool) rdoTradeDisc.IsChecked)
            {
                if ((bool)chkBoxDiscFlag.IsChecked)
                    txtDiscountPerc.Text = String.Format("{0:0.00}", (((double)listcostMRec[0].CTSelPrice1) -
                                                                    (double)listcostMRec[0].DiscValue) /
                                                                    (double) listcostMRec[0].CTSelPrice1 * 100.0); 
                else
                {
                    if (double.TryParse(txtDiscountPerc.Text, out ndValue))
                    {
                        if (ndValue != 100.0)
                        {
                            txtSelPrice.Text = String.Format("{0:0.000}", ((double)listcostMRec[0].DiscValue * 100.0) /
                                                                            (100.0 - (double)listcostMRec[0].CTDiscountPer));

                            listcostMRec[0].SelPricePlusVat = (double)listcostMRec[0].CTSelPrice1 +
                                                                            (((double)listcostMRec[0].CTSelPrice1 * GetVatRate(1)) / 100.0);
                        }
                    }
                }
            }
            else if ((bool) rdoSettleDisc2.IsChecked)
            {
                if ((bool)chkBoxDiscFlag.IsChecked)
                    txtDiscountPerc.Text = String.Format("{0:0.00}",  (((double)listcostMRec[0].CTSelPrice1 -
                                                                   (double)listcostMRec[0].DiscValue) /
										                            (double)listcostMRec[0].CTSelPrice1) * 100.0); 
                else
                {
                    if (double.TryParse(txtDiscountPerc.Text, out ndValue))
                    {
                        if (ndValue != 100.0)
                        {
                            listcostMRec[0].CTSelPrice1 = (double)listcostMRec[0].DiscValue * (100.0 - ((double)listcostMRec[0].CTDiscountPer)) / 100.0;

                            txtSelPrice.Text = String.Format("{0:0.000}", (double)listcostMRec[0].CTSelPrice1 + (((double)listcostMRec[0].CTSelPrice1 * GetVatRate(1)) / 100.0));

                        }
                    }
                }
            }
            else if ((bool) rdoSettleDisc3.IsChecked)
            {
                if ((bool)chkBoxDiscFlag.IsChecked)
                    if ((bool)chkBoxDiscFlag.IsChecked)
                        txtDiscountPerc.Text = String.Format("{0:0.00}", (((double)listcostMRec[0].CTSelPrice1 -
                                        (double)listcostMRec[0].DiscValue) /
                                        (double)listcostMRec[0].CTSelPrice1) * 100.0);
                else
                {
                        if (double.TryParse(txtDiscountPerc.Text, out ndValue))
                        {
                            if (ndValue != 100.0)
                            { 
                                txtDiscountPerc.Text = String.Format("{0:0.00}", ((double)listcostMRec[0].CTSelPrice1 * 100.0) /
                                                (100.0 - (double)listcostMRec[0].DiscValue));

                                lblProfitAfterDisc.Content = String.Format("{0:0.000}", (double)listcostMRec[0].DiscValue - listcostMRec[0].CTTotManCost);
                            }
                        }
                        
                }
            }
        }

        private void LabelWindow()
        {/*
            switch(viewState)
            {
                case AppViewState.VIEW:
                    Costn.Title = "Costing Sheet / View";
                    break;
                case AppViewState.EDIT:
                    Costn.Title = "Costing Sheet / Edit";
                    break;
                case AppViewState.NEW:
                    Costn.Title = "Costing Sheet / New";
                    break;
                case AppViewState.SEARCH:
                    Costn.Title = "Costing Sheet / Search";
                    break;
                case AppViewState.EMPTY:
                    Costn.Title = "Costing Sheet";
                    break;
            } // end switch*/
        }
        private void DisplayCostdbRecord()
        {
            double ndCostValue = 0;
            IEnumerable<DataRow> dr = null;
            StyleVarnArg.Varn = txtVarn.Text;
            StyleVarnArg.Style = txtStyle.Text;
            OverHeadsControl.CostnStyle = txtStyle.Text;
            OverHeadsControl.CostnVarn = txtVarn.Text;
            if (!bLoadByStyleDetails && !bSpinButton) GetStyleList();
            listcostMRec = WpfClassLibrary.Extensions.ToList<CostdbCostMRec>(dtCostMrec);
            listcostMatsRec = WpfClassLibrary.Extensions.ToList<CostdbCTMatsRec>(dtcostCTMatsRec);
            listcostMatsFFRec = WpfClassLibrary.Extensions.ToList<CostdbCTMatsFFRec>(dtcostCTMatsFFRec);
            listcostNotsRec = WpfClassLibrary.Extensions.ToList<CostdbCTNotsRec>(dtcostCTNotsRec);
            listcostOpersRec = WpfClassLibrary.Extensions.ToList<CostdbCTOpersRec>(dtcostCTOpersRec);
            listcostInstsRec = WpfClassLibrary.Extensions.ToList<CostdbCTInstsRec>(dtcostCTCTInstsRec);


            if (viewState == AppViewState.NEW)
            {
                string sCode = string.Format("{0,-7}", txtVarn.Text.Trim()); ;

                listcostMRec[0].CTProfSelFlag = 0;
                WpfClassLibrary.Model.WadmdirWDIRMainRec WDIRMainRec = WpfClassLibrary.WgmateDBUtilClass.GetClientRecordByCode(1, sCode, ref SQLWrite);

                if (WDIRMainRec != null)
                    txtDiscountPerc.Text = string.Format("{0:0.00}", WDIRMainRec.WDIRDiscPer1);
            }
            else
            {
                chkBoxDiscPercFlag.IsChecked = true;
            }

            DisplayStyleImage(listcostMRec[0].CTStyle.Trim());

            #region // Costdb Main Record
            // txtYear.Text = listcostMRec[0].CTMinUkSz.ToString();
            // datePickerDate.SelectedDate = listcostMRec[0].CTDesignDate;

            if (listcostMRec[0].CTGarType > 0)
            {
                txtType.Text = listcostMRec[0].CTGarType.ToString();
                dr = WpfClassLibrary.DataTableAccess.GetTableRows(ref dtParametersRec,
                                        "PARParDescripKey", "PARParParamsKey", "7", listcostMRec[0].CTGarType.ToString());
                if (dr.Count() > 0) txtblkTypeLabel.Text = dr.ToList()[0]["PARParDescr"].ToString().Trim();
            }
            else
            {
                txtType.Text = "";
                txtblkTypeLabel.Text = ""; 
            }

            if (listcostMRec[0].CTMinExpSz > 0)
            {
                txtBrand.Text = listcostMRec[0].CTMinExpSz.ToString();
                dr = WpfClassLibrary.DataTableAccess.GetTableRows(ref dtParametersRec,
                                        "PARParDescripKey", "PARParParamsKey", "14", listcostMRec[0].CTMinExpSz.ToString());
                if (dr.Count() > 0) txtblkBrandDesc.Text = dr.ToList()[0]["PARParDescr"].ToString().Trim();
            }
            else
            {
                txtBrand.Text = "";
                txtblkBrandDesc.Text = "";
            }

            if (listcostMRec[0].CTSpareShort1 > 0)
                txtblkCatagory.Text = WpfClassLibrary.WgmateDBUtilClass.GetGmParamsDescr(11, (short)listcostMRec[0].CTSpareShort1, ref dtParametersRec);
            else
            {
                listcostMRec[0].CTSpareShort1 = null;
                txtblkCatagory.Text = "";
            }

            if (listcostMRec[0].CTSpareShort2 > 0)
                txtblkGender.Text = WpfClassLibrary.WgmateDBUtilClass.GetGmParamsDescr(27, (short)listcostMRec[0].CTSpareShort2, ref dtParametersRec);
            else
            {
                listcostMRec[0].CTSpareShort2 = null;
                txtblkGender.Text = "";
            }

            DataContext = listcostMRec[0];

            //txtGender.Text = String.Format("{0:0.00}", listcostMRec[0].CTGrading);
            txtSize.Text = listcostMRec[0].CTUkSzKey.ToString().Trim();
            txtblkHomeCurrcy.Text = WgmateDBUtilClass.GetGmcurrsCURCurrenciesDescr(1, ref dtCURCurrenciesRec);
            //txtblkMatCost.Text = String.Format("{0:0.000}", listcostMRec[0].CTTotMatCost);


            if (LastCurrcyRec.CURBuyingRate != null)
            {
                //lblMatCostF.Text = String.Format("{0:0.000}", listcostMRec[0].CTCostMakingF);
                listcostMRec[0].MaterialCostF = listcostMRec[0].CTTotMatCost * LastCurrcyRec.CURBuyingRate;
            }
            else
                listcostMRec[0].MaterialCostF = 0.0;

            txtMakePrice.Text = String.Format("{0:0.000}", listcostMRec[0].CTCostMaking);

            if (LastCurrcyRec.CURBuyingRate != null)
                txtMakePriceF.Text = String.Format("{0:0.000}", listcostMRec[0].CTTotMatCost * LastCurrcyRec.CURBuyingRate);
            else
                txtMakePriceF.Text = "0.000";

            txtDutyPerc.Text = String.Format("{0:0.00}", listcostMRec[0].CTDutyPer);
            txtDuty.Text = String.Format("{0:0.000}", listcostMRec[0].CTDuty);
            txtDutyF.Text = String.Format("{0:0.000}", listcostMRec[0].CTDutyF);
            txtOverHeadsPer.Text = String.Format("{0:0.00}", listcostMRec[0].CTOverHeadsPer);
            txtOverHeads.Text = String.Format("{0:0.000}", listcostMRec[0].CTOverHeads);
            txtOverHeadsF.Text = String.Format("{0:0.000}", listcostMRec[0].CTOverHeadsF);
            txtContinPer.Text = String.Format("{0:0.00}", listcostMRec[0].CTContinPer);
            txtContin.Text = String.Format("{0:0.000}", listcostMRec[0].CTContin);
            txtContinF.Text = String.Format("{0:0.000}", listcostMRec[0].CTContinF);
            txtManuFCost.Text = String.Format("{0:0.000}", listcostMRec[0].CTTotManCost);
            txtManuFCostF.Text = String.Format("{0:0.000}", listcostMRec[0].CTProfMar1F);
            txtMarkUpPerc.Text = String.Format("{0:0.00}", listcostMRec[0].CTProfMarPer1);
            txtMarkUp.Text = String.Format("{0:0.000}", listcostMRec[0].CTProfMar1);
            txtMarkUpF.Text = String.Format("{0:0.000}", listcostMRec[0].CTProfMar1F);
            txtMargin.Text = String.Format("{0:0.00}", CalculateProfitMargin());

            //txtProfMarF.Text = String.Format("{0:0.000}", listcostMRec[0].CTProfMar1F);
            listcostMRec[0].ProfirMarginF = listcostMRec[0].CTSelPrice1 + (LastCurrcyRec.CURSellingRate - LastCurrcyRec.CURBuyingRate);

            txtSelPrice.Text = String.Format("{0:0.000}", listcostMRec[0].CTSelPrice1);
            txtSelPrice_F.Text = String.Format("{0:0.000}", listcostMRec[0].CTVatRate);

            txtSelPricePerc2.Text = String.Format("{0:0.000}", 0);
            txtSelPrice2H.Text = String.Format("{0:0.000}", listcostMRec[0].CTSelPrice2);
            txtSelPrice2F.Text = String.Format("{0:0.000}", listcostMRec[0].CTSelPricePer2);

            txtSelPricePerc3.Text = String.Format("{0:0.000}", 0);
            txtSelPrice3H.Text = String.Format("{0:0.000}", listcostMRec[0].CTSelPrice3);
            txtSelPrice3F.Text = String.Format("{0:0.000}", listcostMRec[0].CTSelPricePer3);

            txtSelPricePerc4.Text = String.Format("{0:0.000}", 0);
            txtSelPrice4H.Text = String.Format("{0:0.000}", listcostMRec[0].CTSelPrice4);
            txtSelPrice4F.Text = String.Format("{0:0.000}", listcostMRec[0].CTSelPricePer4);

            listcostMRec[0].SelPricePlusVat = CalculateSellPricePlusVat();
            //txtPercSelPrice.Text = "";

            txtTimeMake.Text = String.Format("{0:0.00}", listcostMRec[0].CTTimeToMake);

            listcostMRec[0].QPRMatTotal = TotalQPRMatCost();
            listcostMRec[0].QprPerc = (TotalQPRMatCost() / listcostMRec[0].CTSelPrice1) * 100.0;

            if ((bool)chkBoxDiscPercFlag.IsChecked)
            {
                txtDiscountPerc.Text = String.Format("{0:0.00}", listcostMRec[0].CTDiscountPer);
                txtDiscount.Text = String.Format("{0:0.000}", CalculateDiscount());
                txtStyleMarkUpPerc.Text = String.Format("{0:0.00}", CalculateMarkUpPercent());
                lblProfitAfterDisc.Content = String.Format("{0:0.000}", ProfitAterDiscount());
                txtMarginProfitDisc.Text = String.Format("{0:0.00}", CaculateProfitMarginDiscount());
            }
            else
            {
                txtDiscountPerc.Text = "";
                txtDiscount.Text = "";
                txtStyleMarkUpPerc.Text = "";
                lblProfitAfterDisc.Content = "";
                txtMarginProfitDisc.Text = "";
            }

            #endregion
            #region // Fill MatsRec Description

            foreach (CostdbCTMatsRec MatsRec in listcostMatsRec)
            {
                //******************* Parts Desciption *************************************************************************
                if (MatsRec.CTMatPart.HasValue && MatsRec.CTMatPart > 0)
                    MatsRec.MatPartDesc = WpfClassLibrary.WgmateDBUtilClass.GetGmParamsDescr(4, (short)MatsRec.CTMatPart, ref dtParametersRec);

                //******************* Materail Desciption **********************************************************************

                if (MatsRec.CTMatType.HasValue && MatsRec.CTMatType > 0)
                    MatsRec.MatTypeDesc = WpfClassLibrary.WgmateDBUtilClass.GetGmParamsDescr(1, (short)MatsRec.CTMatType, ref dtParametersRec);

                //******************** Code Description******************************************************************

                SQLWrite.SQLWriteCommand("[dbo].[GetRollsdbRollsMRecByCode]", SQLWriteClass.SqlCmdType.PROCEDURE);
                SqlParameter[] SqlParam = new SqlParameter[2];

                SqlParam[0] = DependancyService.SQLParameter("@RMatType",
                        "System.Int16", DependancyService.ParamDirection.Input, (object)MatsRec.CTMatType.ToString().Trim());

                SqlParam[1] = DependancyService.SQLParameter("@RMatCode",
                        "System.String", DependancyService.ParamDirection.Input, (object)MatsRec.CTMatCode.ToString().Trim());

                MatsRec.MatCodeDesc = WpfClassLibrary.WgmateDBUtilClass.GetRollsdbRollsMRecCodeDesc(ref SqlParam, ref SQLWrite);

                //********************** Colour *****************************************************************************

                if (MatsRec.CTMatColr.HasValue && MatsRec.CTMatColr > 0)
                    MatsRec.MatColrDesc = WpfClassLibrary.WgmateDBUtilClass.GetGmParamsDescr(2, (short)MatsRec.CTMatColr, ref dtParametersRec);
                else MatsRec.MatColrDesc = "";

                //*********************** Supplier***************************************************************************

                WadmdirWDIRMainRec WDIRMainRec = WpfClassLibrary.WgmateDBUtilClass.GetClientRecord(2, (int)MatsRec.CTMatSupl, ref SQLWrite);
                // MatsRec.MatSuplDesc = WpfClassLibrary.WgmateDBUtilClass.GetClientName(2, (int)MatsRec.CTMatSupl, ref SQLWrite);
                MatsRec.MatSuplDesc = WDIRMainRec.WDIRClientName.Trim();

                MatsRec.CurrcyNo = (short)WDIRMainRec.WDIRCurrency;
                MatsRec.CountryCode = (short)WDIRMainRec.WDIRCountryCode;

                ndCostValue = (double)MatsRec.CTCosting * (double)MatsRec.CTMatPrice;
                MatsRec.Cost = ndCostValue;

                #region // AnyColourCombinations
                SQLWrite.SQLWriteCommand("[dbo].[IsColourCombinations]", SQLWriteClass.SqlCmdType.PROCEDURE);
                SqlParam = new SqlParameter[5];

                SqlParam[0] = DependancyService.SQLParameter("@Style",
                       "System.String", DependancyService.ParamDirection.Input, (object)listcostMRec[0].CTStyle);

                SqlParam[1] = DependancyService.SQLParameter("@Varn",
                       "System.String", DependancyService.ParamDirection.Input, (object)listcostMRec[0].CTVarn);

                SqlParam[2] = DependancyService.SQLParameter("@MatPart",
                        "System.Int16", DependancyService.ParamDirection.Input, (object)MatsRec.CTMatPart);

                SqlParam[3] = DependancyService.SQLParameter("@MatType",
                        "System.Int16", DependancyService.ParamDirection.Input, (object)MatsRec.CTMatType);

                SqlParam[4] = DependancyService.SQLParameter("@ColrNo",
                        "System.Int16", DependancyService.ParamDirection.Input, (object)MatsRec.CTMatColr);

                MatsRec.ColourCombinationsA = (bool)SQLWrite.ExecuteQueryFunction(SqlParam);
                #endregion

                #region // AnyCustomerPreferences
                SQLWrite.SQLWriteCommand("[dbo].[IsAnyCustomerPreferences]", SQLWriteClass.SqlCmdType.PROCEDURE);
                SqlParam = new SqlParameter[10];

                SqlParam[0] = DependancyService.SQLParameter("@Style",
                       "System.String", DependancyService.ParamDirection.Input, (object)listcostMRec[0].CTStyle);

                SqlParam[1] = DependancyService.SQLParameter("@Varn",
                       "System.String", DependancyService.ParamDirection.Input, (object)listcostMRec[0].CTVarn);

                SqlParam[2] = DependancyService.SQLParameter("@MatPart",
                        "System.Int16", DependancyService.ParamDirection.Input, (object)MatsRec.CTMatPart);

                SqlParam[3] = DependancyService.SQLParameter("@MatType",
                        "System.Int16", DependancyService.ParamDirection.Input, (object)MatsRec.CTMatType);

                SqlParam[4] = DependancyService.SQLParameter("@MatCode",
                       "System.String", DependancyService.ParamDirection.Input, (object)MatsRec.CTMatCode);

                SqlParam[5] = DependancyService.SQLParameter("@MatColr",
                        "System.Int16", DependancyService.ParamDirection.Input, (object)MatsRec.CTMatColr);

                SqlParam[6] = DependancyService.SQLParameter("@MatColr1",
                        "System.Int16", DependancyService.ParamDirection.Input, (object)0);

                SqlParam[7] = DependancyService.SQLParameter("@MatSupl",
                        "System.Int16", DependancyService.ParamDirection.Input, (object)MatsRec.CTMatSupl);

                SqlParam[8] = DependancyService.SQLParameter("@MatWhse",
                        "System.Int16", DependancyService.ParamDirection.Input, (object)MatsRec.CTMatWhse);

                SqlParam[9] = DependancyService.SQLParameter("@MatLocn",
                        "System.Int16", DependancyService.ParamDirection.Input, (object)MatsRec.CTMatLocn);

                MatsRec.CustomerPreferencesA = (bool)SQLWrite.ExecuteQueryFunction(SqlParam);
                #endregion

                #region // AnyMatConsumtion
                SQLWrite.SQLWriteCommand("[dbo].[IsAnyMatConsumtion]", SQLWriteClass.SqlCmdType.PROCEDURE);
                SqlParam = new SqlParameter[11];

                SqlParam[0] = DependancyService.SQLParameter("@Style",
                       "System.String", DependancyService.ParamDirection.Input, (object)listcostMRec[0].CTStyle);

                SqlParam[1] = DependancyService.SQLParameter("@Varn",
                       "System.String", DependancyService.ParamDirection.Input, (object)listcostMRec[0].CTVarn);

                SqlParam[2] = DependancyService.SQLParameter("@SizeKey",
                       "System.String", DependancyService.ParamDirection.Input, (object)listcostMRec[0].CTUkSzKey);

                SqlParam[3] = DependancyService.SQLParameter("@MatPart",
                        "System.Int16", DependancyService.ParamDirection.Input, (object)MatsRec.CTMatPart);

                SqlParam[4] = DependancyService.SQLParameter("@MatType",
                        "System.Int16", DependancyService.ParamDirection.Input, (object)MatsRec.CTMatType);

                SqlParam[5] = DependancyService.SQLParameter("@MatCode",
                       "System.String", DependancyService.ParamDirection.Input, (object)MatsRec.CTMatCode);

                SqlParam[6] = DependancyService.SQLParameter("@MatColr",
                        "System.Int16", DependancyService.ParamDirection.Input, (object)MatsRec.CTMatColr);

                SqlParam[7] = DependancyService.SQLParameter("@MatColr1",
                        "System.Int16", DependancyService.ParamDirection.Input, (object)0);

                SqlParam[8] = DependancyService.SQLParameter("@MatSupl",
                        "System.Int16", DependancyService.ParamDirection.Input, (object)MatsRec.CTMatSupl);

                SqlParam[9] = DependancyService.SQLParameter("@MatWhse",
                        "System.Int16", DependancyService.ParamDirection.Input, (object)MatsRec.CTMatWhse);

                SqlParam[10] = DependancyService.SQLParameter("@MatLocn",
                        "System.Int16", DependancyService.ParamDirection.Input, (object)MatsRec.CTMatLocn);

                MatsRec.MatComsumtionA = (bool)SQLWrite.ExecuteQueryFunction(SqlParam);
                #endregion

                #region // MatPriceModified
                SQLWrite.SQLWriteCommand("[dbo].[IsMatPriceModified]", SQLWriteClass.SqlCmdType.PROCEDURE);
                SqlParam = new SqlParameter[8];

                SqlParam[0] = DependancyService.SQLParameter("@MatType",
                        "System.Int16", DependancyService.ParamDirection.Input, (object)MatsRec.CTMatType);

                SqlParam[1] = DependancyService.SQLParameter("@MatCode",
                       "System.String", DependancyService.ParamDirection.Input, (object)MatsRec.CTMatCode);

                SqlParam[2] = DependancyService.SQLParameter("@MatColr",
                        "System.Int16", DependancyService.ParamDirection.Input, (object)MatsRec.CTMatColr);

                SqlParam[3] = DependancyService.SQLParameter("@MatColr1",
                        "System.Int16", DependancyService.ParamDirection.Input, (object)0);

                SqlParam[4] = DependancyService.SQLParameter("@MatSupl",
                        "System.Int16", DependancyService.ParamDirection.Input, (object)MatsRec.CTMatSupl);

                SqlParam[5] = DependancyService.SQLParameter("@MatWhse",
                        "System.Int16", DependancyService.ParamDirection.Input, (object)MatsRec.CTMatWhse);

                SqlParam[6] = DependancyService.SQLParameter("@MatLocn",
                        "System.Int16", DependancyService.ParamDirection.Input, (object)MatsRec.CTMatLocn);

                SqlParam[7] = DependancyService.SQLParameter("@NewPrice",
                        "System.String", DependancyService.ParamDirection.Input, (object)MatsRec.CTMatPrice);

                MatsRec.MatPriceModifiedA = (bool)SQLWrite.ExecuteQueryFunction(SqlParam);
                #endregion
            }

            listcostMatsRec.AddRange(Enumerable.Range(listcostMatsRec.Count, ROWSCOUNT).Select(i => new CostdbCTMatsRec()));
            MainGrid.ItemsSource = listcostMatsRec;

            if (listcostMatsRec.Count > 0)
                MainGrid.SelectedIndex = 0;
            #endregion

            if (viewState != AppViewState.NEW)
            {
                viewState = AppViewState.VIEW;
                EnableControls(false);
                bSpinButton = false;
            }

            LabelWindow();

            ButtonState();
            ColorOneOfTrioButton();
            OverHeadsControl.CostMRec = listcostMRec[0];

            if (listcostMRec[0].CTTotMatCost == 0.0)
            {
                CalculateCosting();
            }

            Mouse.OverrideCursor = null;
            btnEdit_Click(this, new RoutedEventArgs());     // Change to Ignore Edit & New
        }
        private void DisplayTotalMaterialCost()
        {
            listcostMRec[0].CTTotMatCost = CalculateTotalMaterialCost();
            listcostMRec[0].QPRMatTotal = TotalQPRMatCost();

            if (listcostMRec[0].CTSelPrice1.HasValue && listcostMRec[0].CTSelPrice1 > 0.001)
                listcostMRec[0].QprPerc = (listcostMRec[0].QPRMatTotal / listcostMRec[0].CTSelPrice1) * 100.0;
            else
                listcostMRec[0].QprPerc = 0.0;
        }
        private void DisplayStyleImage(string Style)
        {
            string sPath = imageDirectory + Style;
            bool bFileFound = false;
            string sFullPath = "";

            try
            {
                string[] ImageTypes = { ".bmp", ".jpg", ".gif" };

                foreach (string suffix in ImageTypes)
                {
                    sFullPath = sPath + suffix;

                    if (File.Exists(sFullPath))
                    {
                        bFileFound = true;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
            }
            finally
            {
                if (bFileFound)
                {
                    imgStyle.Source = new BitmapImage(new Uri(sFullPath));
                    imgStyle.Stretch = Stretch.Fill;
                }
                else
                    imgStyle.Source = null;
            }
        }

        // Notes UpdateBotPart(1) = true
        private void CalculateCosting(bool bOverHeads = true)
        {
            // Duty_TotalMatCost
            var sQuery = (from CostdbCTMatsRec in listcostMatsRec
                          where CostdbCTMatsRec.CTMatType > 0 && (short)CostdbCTMatsRec.CTMatDutyFlag > 0
                          select CostdbCTMatsRec.CTCosting * CostdbCTMatsRec.CTMatPrice).Sum();

            double ndDuty = (double)sQuery;

            listcostMRec[0].CTDuty = ((ndDuty + listcostMRec[0].CTCostMaking) * listcostMRec[0].CTDutyPer) / 100.0;
            //txtDuty.Text = string.Format("{0:0.000}", listcostMRec[0].CTDuty);

            listcostMRec[0].CTDutyF = listcostMRec[0].CTDuty * LastCurrcyRec.CURBuyingRate;
            //txtDutyF.Text = string.Format("{0:0.000}", listcostMRec[0].CTDutyF);

            // ***************************************************************************************

            double ndValue = (double)listcostMRec[0].CTTotMatCost + (double)listcostMRec[0].CTCostMaking + (double)listcostMRec[0].CTDuty;

            if (bOverHeads) // Overheads Per is fixed 
            {
                listcostMRec[0].CTOverHeads = (ndValue * listcostMRec[0].CTOverHeadsPer) / 100.0;
                //txtOverHeads.Text = string.Format("{0:0.000}", listcostMRec[0].CTOverHeads);

                listcostMRec[0].CTOverHeadsF = listcostMRec[0].CTOverHeads * LastCurrcyRec.CURBuyingRate;
                //txtOverHeadsF.Text = string.Format("{0:0.000}", listcostMRec[0].CTOverHeadsF);
            }
            else            // Overheads is fixed
            {
                if (ndValue > 0.001)
                    listcostMRec[0].CTOverHeadsPer = (listcostMRec[0].CTOverHeads * 100.0) / ndValue;
                else
                    listcostMRec[0].CTOverHeadsPer = 0.0;

                //txtOverHeadsPer.Text = string.Format("{0:0.00}", listcostMRec[0].CTOverHeadsPer);
            }

            ndValue = (double)listcostMRec[0].CTTotMatCost + (double)listcostMRec[0].CTCostMaking + (double)listcostMRec[0].CTDuty + (double)listcostMRec[0].CTOverHeads;

            if (bOverHeads) // Contingency Per is fixed 
            {
                listcostMRec[0].CTContin = (ndValue * listcostMRec[0].CTContinPer) / 100.0;
                //txtContin.Text = string.Format("{0:0.000}", listcostMRec[0].CTContin);

                listcostMRec[0].CTContinF = listcostMRec[0].CTContin * LastCurrcyRec.CURBuyingRate;
                //txtContinF.Text = string.Format("{0:0.000}", listcostMRec[0].CTContinF);
            }
            else            // Contingency is fixed
            {
                if (ndValue > 0.0)
                    listcostMRec[0].CTContinPer = (listcostMRec[0].CTContin * 100.0) / ndValue;
                else
                    listcostMRec[0].CTContinPer = 0.0;

                //txtContinPer.Text = string.Format("{0:0.00}", listcostMRec[0].CTContinPer);
            }

            listcostMRec[0].CTTotManCost = ndValue + listcostMRec[0].CTContin;
            //txtManuFCost.Text = string.Format("{0:0.000}", listcostMRec[0].CTTotManCost);

            listcostMRec[0].CTCostMakingF = listcostMRec[0].CTCostMaking * LastCurrcyRec.CURBuyingRate;
            listcostMRec[0].CTDutyF = listcostMRec[0].CTDuty * LastCurrcyRec.CURBuyingRate;
            listcostMRec[0].CTOverHeadsF = listcostMRec[0].CTOverHeads * LastCurrcyRec.CURBuyingRate;
            listcostMRec[0].CTContinF = listcostMRec[0].CTContin * LastCurrcyRec.CURBuyingRate;

            listcostMRec[0].CTTotManCostF = (listcostMRec[0].CTTotMatCost * LastCurrcyRec.CURBuyingRate) +
                                      listcostMRec[0].CTCostMakingF +
                                      listcostMRec[0].CTDutyF +
                                      listcostMRec[0].CTOverHeadsF +
                                      listcostMRec[0].CTContinF;

            //txtManuFCostF.Text = string.Format("{0:0.000}", listcostMRec[0].CTTotManCostF);

            switch (listcostMRec[0].CTProfSelFlag)
            {
                case 0:
                case 1:   // Markup Percent is fixed
                    listcostMRec[0].CTProfMar1 = (listcostMRec[0].CTTotManCost * listcostMRec[0].CTProfMarPer1) / 100.0;
                    //txtMarkUp.Text = string.Format("{0:0.000}", listcostMRec[0].CTProfMar1);

                    listcostMRec[0].CTSelPrice1 = listcostMRec[0].CTTotManCost + listcostMRec[0].CTProfMar1;
                    //txtSelPrice.Text = string.Format("{0:0.000}", listcostMRec[0].CTSelPrice1);

                    listcostMRec[0].SelPricePlusVat = (double)listcostMRec[0].CTSelPrice1 + (((double)listcostMRec[0].CTSelPrice1 * ndTaxRates[0]) / 100.0);

                    //===========================================================			
                    listcostMRec[0].CTProfMar1F = listcostMRec[0].CTProfMar1 * LastCurrcyRec.CURBuyingRate;
                    //txtProfMarF.Text = string.Format("{0:0.000}", listcostMRec[0].CTProfMar1F);

                    listcostMRec[0].CTVatRate = listcostMRec[0].CTTotManCostF + listcostMRec[0].CTProfMar1F;
                    //txtSelPrice_F.Text = string.Format("{0:0.000}", listcostMRec[0].CTVatRate);
                    //===========================================================	
                    break;
                case 2:    // Profit Margin Perecent is fixed 
                    if (listcostMRec[0].CTDate2 == System.DateTime.MinValue)
                    {
                        if (listcostMRec[0].CTTotManCost > 0.0)
                            listcostMRec[0].CTProfMarPer1 = (listcostMRec[0].CTProfMar1 * 100.0) / listcostMRec[0].CTTotManCost;
                        else
                            listcostMRec[0].CTProfMarPer1 = 0.0;

                        //txtMargin.Text = string.Format("{0:0.00}", listcostMRec[0].CTProfMarPer1);

                        listcostMRec[0].CTSelPrice1 = listcostMRec[0].CTTotManCost + listcostMRec[0].CTProfMar1;
                        //txtSelPrice.Text = string.Format("{0:0.000}", listcostMRec[0].CTSelPrice1);

                        listcostMRec[0].SelPricePlusVat = (double)listcostMRec[0].CTSelPrice1 + (((double)listcostMRec[0].CTSelPrice1 * ndTaxRates[0]) / 100.0);
                        listcostMRec[0].CTProfMar1F = listcostMRec[0].CTProfMar1 * LastCurrcyRec.CURBuyingRate;
                        //txtMarkUp.Text = string.Format("{0:0.000}", listcostMRec[0].CTProfMar1);

                        listcostMRec[0].CTVatRate = listcostMRec[0].CTTotManCostF + listcostMRec[0].CTProfMar1F;
                        //txtSelPrice_F.Text = string.Format("{0:0.000}", listcostMRec[0].CTVatRate);
                    }
                    break;
                case 3:    // Selling Price is fixed
                    listcostMRec[0].CTProfMar1 = listcostMRec[0].CTSelPrice1 - listcostMRec[0].CTTotManCost;
                    //txtMarkUp.Text = string.Format("{0:0.000}", listcostMRec[0].CTProfMar1);

                    if (listcostMRec[0].CTDate2 == DateTime.Parse("1900-01-01 00:00:00"))
                    {
                        if (listcostMRec[0].CTTotManCost > 0.0)
                            listcostMRec[0].CTProfMarPer1 =
                            (listcostMRec[0].CTProfMar1 * 100.0) / listcostMRec[0].CTTotManCost;
                        else
                            listcostMRec[0].CTProfMarPer1 = 0.0;

                        //txtMargin.Text = string.Format("{0:0.00}", listcostMRec[0].CTProfMarPer1);
                    }

                    listcostMRec[0].CTSelPrice1 = listcostMRec[0].CTTotManCost + listcostMRec[0].CTProfMar1;
                    //txtSelPrice.Text = string.Format("{0:0.000}", listcostMRec[0].CTSelPrice1);

                    listcostMRec[0].SelPricePlusVat = (double)listcostMRec[0].CTSelPrice1 + (((double)listcostMRec[0].CTSelPrice1 * ndTaxRates[0]) / 100.0);

                    //===========================================================			
                    listcostMRec[0].CTProfMar1F = listcostMRec[0].CTProfMar1 * LastCurrcyRec.CURBuyingRate;
                    //txtProfMarF.Text = string.Format("{0:0.000}", listcostMRec[0].CTProfMar1F);

                    listcostMRec[0].CTVatRate = listcostMRec[0].CTTotManCostF + listcostMRec[0].CTProfMar1F;
                    //txtSelPrice_F.Text = string.Format("{0:0.000}", listcostMRec[0].CTVatRate);
			        //===========================================================		
                    break;
            } // end switch
            listcostMRec[0].CTVatRate = listcostMRec[0].CTSelPrice1 * LastCurrcyRec.CURBuyingRate;
            //txtSelPrice.Text = string.Format("{0:0.000}", listcostMRec[0].CTSelPrice1);

            //=========================================================================================
            listcostMRec[0].CurrcyDiff = listcostMRec[0].CTSelPrice1 * (LastCurrcyRec.CURSellingRate - LastCurrcyRec.CURBuyingRate);

            listcostMRec[0].SelPricePlusVat = (double)listcostMRec[0].CTSelPrice1 + (((double)listcostMRec[0].CTSelPrice1 * ndTaxRates[0]) / 100.0);
            //=========================================================================================

            ReCalculateSellingPrices();
            OverHeadsControl.CostMRec = listcostMRec[0];
        }
        private void CalculateMaterialRowPrices(int Index)
        {
            SQLWrite.SQLWriteCommand("GetRollsdbRollsMRecByLocation", SQLWriteClass.SqlCmdType.PROCEDURE);
            SqlParameter[] SqlParam = new SqlParameter[7];

            SqlParam[0] = DependancyService.SQLParameter("@RMatType",
                    "System.Int16", DependancyService.ParamDirection.Input, (object)listcostMatsRec[Index].CTMatType);

            SqlParam[1] = DependancyService.SQLParameter("@RMatCode",
                    "System.String", DependancyService.ParamDirection.Input, (object)listcostMatsRec[Index].CTMatCode);

            SqlParam[2] = DependancyService.SQLParameter("@RMatColr",
                    "System.Int16", DependancyService.ParamDirection.Input, (object)listcostMatsRec[Index].CTMatColr);

            SqlParam[3] = DependancyService.SQLParameter("@RMatColr1",
                    "System.Int16", DependancyService.ParamDirection.Input, (object)0);

            SqlParam[4] = DependancyService.SQLParameter("@RMatSupl",
                    "System.Int16", DependancyService.ParamDirection.Input, (object)listcostMatsRec[Index].CTMatSupl);

            SqlParam[5] = DependancyService.SQLParameter("@RMatWhse",
                    "System.Int16", DependancyService.ParamDirection.Input, (object)listcostMatsRec[Index].CTMatWhse);

            SqlParam[6] = DependancyService.SQLParameter("@RMatLocn",
                    "System.Int16", DependancyService.ParamDirection.Input, (object)listcostMatsRec[Index].CTMatLocn);

            DataTable dtRollsMRec = SQLWrite.ExecuteDataTableQuery(SqlParam);

            if (dtRollsMRec.Rows.Count > 0)
            {
                List<RollsdbRollsMRec> listRollsMRec = WpfClassLibrary.Extensions.ToList<RollsdbRollsMRec>(dtRollsMRec);

                SQLWrite.SQLWriteCommand("GetRollsdbPropertiesByRollsID", SQLWriteClass.SqlCmdType.PROCEDURE);
                SqlParam = new SqlParameter[1];

                SqlParam[0] = DependancyService.SQLParameter("@RollsID",
                        "System.Int16", DependancyService.ParamDirection.Input, (object)listRollsMRec[0].RollsID);

                DataTable dtProperties = SQLWrite.ExecuteDataTableQuery(SqlParam);

                if (dtProperties.Rows.Count > 0)
                {
                    List<RollsdbProperties> listProperties = WpfClassLibrary.Extensions.ToList<RollsdbProperties>(dtProperties);

                    if (listProperties[0].OrderUnit > 0.001)
                        listcostMatsRec[Index].CTMatPrice = listRollsMRec[0].RMatCost / listProperties[0].OrderUnit;
                    else
                        listcostMatsRec[Index].CTMatPrice = listRollsMRec[0].RMatCost;

                    listcostMatsRec[Index].Cost = listcostMatsRec[Index].CTCosting * listcostMatsRec[Index].CTMatPrice;

                    DisplayTotalMaterialCost();
                    //listcostMRec[0].QPRMatTotal = TotalQPRMatCost();
                    //listcostMRec[0].CTTotMatCost = CalculateTotalMaterialCost();
                    //listcostMRec[0].QprPerc = (TotalQPRMatCost() / listcostMRec[0].CTSelPrice1) * 100.0;
                }
            }
        }
        private double CalculateTotalMaterialCost()
        {
            double ndValue = 0.0;

            var sQuery = (from CostdbCTMatsRec in listcostMatsRec
                          where CostdbCTMatsRec.CTCosting.HasValue && CostdbCTMatsRec.CTMatPrice.HasValue
                          select CostdbCTMatsRec.CTCosting * CostdbCTMatsRec.CTMatPrice).Sum();

            ndValue = (double)sQuery;

            return ndValue;
        }
        private void ReCalculateSellingPrices()
        {
            double ndValue = 0;

            if (!double.TryParse(txtSelPricePerc2.Text, out ndValue)) ndValue = 0;
            listcostMRec[0].CTSelPrice2 = (listcostMRec[0].CTTotManCost * ndValue / 100.0) + listcostMRec[0].CTTotManCost;

            //txtSelPrice2H.Text = string.Format("{0:0.000}", listcostMRec[0].CTSelPrice2);
            //txtSelPrice2F.Text = string.Format("{0:0.000}", listcostMRec[0].CTSelPrice2 * LastCurrcyRec.CURBuyingRate);

            if (!double.TryParse(txtSelPricePerc3.Text, out ndValue)) ndValue = 0;
            listcostMRec[0].CTSelPrice3 = (listcostMRec[0].CTTotManCost * ndValue / 100.0) + listcostMRec[0].CTTotManCost;
            //txtSelPrice3H.Text = string.Format("{0:0.000}", listcostMRec[0].CTSelPrice3);

            if (!double.TryParse(txtSelPricePerc4.Text, out ndValue)) ndValue = 0;
            listcostMRec[0].CTSelPrice4 = (listcostMRec[0].CTTotManCost * ndValue / 100.0) + listcostMRec[0].CTTotManCost;
            //txtSelPrice4H.Text = string.Format("{0:0.000}", listcostMRec[0].CTSelPrice4);

            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            if (!double.TryParse(txtPercSelPrice.Text, out ndValue)) ndValue = 0;

            if (ndValue != 0.0)
            {
                double ndSellPlusVat = double.Parse(txtSelPricePlusVat.Text.ToString());
                listcostMRec[0].CTSelPrice2 = ndSellPlusVat + ((ndSellPlusVat * ndValue) / 100.00);
                //txtSelPrice2H.Text = string.Format("{0:0.000}", listcostMRec[0].CTSelPrice2);
            }

            //-----------------------------------------------------------------------------------------

            //if (listcostMRec[0].CTSelPrice1 > 0.0001)
            //    txtMargin.Text = string.Format("{0:0.00}", (listcostMRec[0].CTProfMar1 / listcostMRec[0].CTSelPrice1) * 100.0);
            //else
            //    txtMargin.Text = "";
            listcostMRec[0].CTSelPrice1 = (listcostMRec[0].CTProfMar1 / listcostMRec[0].CTSelPrice1) * 100.0;

            CalculateDiscountValue();
        }
        private void CalculateDiscountValue()
        {
            if ((bool)chkBoxDiscPercFlag.IsChecked)
            {
                listcostMRec[0].DiscValue = (double)listcostMRec[0].CTSelPrice1 * ((100.0 - listcostMRec[0].CTDiscountPer) / 100.0);
                listcostMRec[0].CTTotManCost = double.Parse(txtManuFCost.Text);

                lblProfitAfterDisc.Content = string.Format("{0:0.000}", CalculateDiscount() - listcostMRec[0].CTTotManCost);
            }

            if ((double)listcostMRec[0].CTTotManCost > 0.0001)
            {
                if (listcostMRec[0].DiscValue.HasValue && listcostMRec[0].DiscValue > 0.001)
                    txtStyleMarkUpPerc.Text = string.Format("{0:0.00}", ((listcostMRec[0].DiscValue - listcostMRec[0].CTTotManCost) / listcostMRec[0].CTTotManCost) * 100.0);
                else
                    txtStyleMarkUpPerc.Text = "0.00";

                if (listcostMRec[0].CTSelPrice1 > 0.0001)
                    txtMarginProfitDisc.Text = string.Format("{0:0.00}", (((listcostMRec[0].DiscValue - listcostMRec[0].CTTotManCost) / listcostMRec[0].CTSelPrice1) * 100.0));
                else
                    txtMarginProfitDisc.Text = "0.00";
            }
        }
        private void ColorOneOfTrioButton()
        {
            if (listcostMRec == null || listcostMRec.Count == 0)
                return;

            switch (listcostMRec[0].CTProfSelFlag)
            {
                case 0:
                    txtMarkUpPerc.Background = txtDutyPerc.BorderBrush;
                    txtMarkUp.Background = txtDutyPerc.BorderBrush;
                    txtSelPrice.Background = txtDutyPerc.BorderBrush;
                    break;
                case 1: // MarkUpPerc Profitmargin
                    txtMarkUpPerc.Background = new SolidColorBrush(Colors.LightSalmon);
                    txtMarkUp.Background = txtDutyPerc.BorderBrush;
                    txtSelPrice.Background = txtDutyPerc.BorderBrush;
                    break;
                case 2: // Profitmargin
                    txtMarkUp.Background = new SolidColorBrush(Colors.LightSalmon); 
                    txtMarkUpPerc.Background = txtDutyPerc.BorderBrush;
                    txtSelPrice.Background = txtDutyPerc.BorderBrush;
                    break;
                case 3: // Sellingprice
                    txtSelPrice.Background = new SolidColorBrush(Colors.LightSalmon);
                    txtMarkUpPerc.Background = txtDutyPerc.BorderBrush;
                    txtMarkUp.Background = txtDutyPerc.BorderBrush;
                    break;
            } // end switch
        }
        private void ButtonState()
        {
            if (listcostNotsRec.Count > 0)
            {
                btnNotes.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF000096"));
                btnNotes.Foreground = Brushes.White;
            }
            else
            {
                btnNotes.Background = btnExit.Background;
                btnNotes.Foreground = btnExit.Foreground;
            }

            SQLWrite.SQLWriteCommand("[dbo].[IsExistCstextraCSTExtraRec]");
            SqlParameter[] SqlParam = new SqlParameter[2];

            SqlParam[0] = DependancyService.SQLParameter("@Style",
            "System.String", DependancyService.ParamDirection.Input, (object)txtStyle.Text.Trim());

            SqlParam[1] = DependancyService.SQLParameter("@Varn",
                    "System.String", DependancyService.ParamDirection.Input, (object)txtVarn.Text.Trim());

            bool? bFlag = (bool)SQLWrite.ExecuteQueryFunction(SqlParam);

            if (bFlag.HasValue && (bool)bFlag)
            {
                btnExtra.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF000096"));
                btnExtra.Foreground = Brushes.White;
            }
            else
            {
                btnExtra.Background = btnExit.Background;
                btnExtra.Foreground = btnExit.Foreground;
            }

            SQLWrite.SQLWriteCommand("[dbo].[IsExistCospinstCOIInstsRec]", SQLWriteClass.SqlCmdType.PROCEDURE);
            bFlag = (bool)SQLWrite.ExecuteQueryFunction(SqlParam);

            if (bFlag.HasValue && (bool)bFlag)
            {
                btnDcknInstruc.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF000096"));
                btnDcknInstruc.Foreground = Brushes.White;
            }
            else
            {
                btnDcknInstruc.Background = btnExit.Background;
                btnDcknInstruc.Foreground = btnExit.Foreground;
            }

            SQLWrite.SQLWriteCommand("[dbo].[IsExistEmbelishEMBInstsRec]", SQLWriteClass.SqlCmdType.PROCEDURE);
            bFlag = (bool)SQLWrite.ExecuteQueryFunction(SqlParam);

            if (bFlag.HasValue && (bool)bFlag)
            {
                btnEmblish.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF000096"));
                btnEmblish.Foreground = Brushes.White;
            }
            else
            {
                btnEmblish.Background = btnExit.Background;
                btnEmblish.Foreground = btnExit.Foreground;
            }

            SQLWrite.SQLWriteCommand("[dbo].[IsExistPackinstPACMainRec]", SQLWriteClass.SqlCmdType.PROCEDURE);
            bFlag = (bool)SQLWrite.ExecuteQueryFunction(SqlParam);

            if (bFlag.HasValue && (bool)bFlag)
            {
                btnPackInstruc.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF000096"));
                btnPackInstruc.Foreground = Brushes.White;
            }
            else
            {
                btnPackInstruc.Background = btnExit.Background;
                btnPackInstruc.Foreground = btnExit.Foreground;
            }
        }

        // Grid Material Functions
        private RollsdbRollsMRec GetRollsMRecByCode(short Type, string Code)
        {
            RollsdbRollsMRec RollsMRec = null;
            SqlParameter[] SqlParam = new SqlParameter[2];

            SqlParam[0] = DependancyService.SQLParameter("@RMatType", "System.Int16", DependancyService.ParamDirection.Input, (object)Type);
            SqlParam[1] = DependancyService.SQLParameter("@RMatCode", "System.String", DependancyService.ParamDirection.Input, (object)Code);

            RollsMRec = WpfClassLibrary.WgmateDBUtilClass.GetRollsdbRollsMRecByCode(ref SqlParam, ref SQLWrite);

            return RollsMRec;
        }
        private RollsdbRollsMRec GetRollsMRecByColour(short Type, string Code, short Colour)
        {
            RollsdbRollsMRec RollsMRec = null;
            SqlParameter[] SqlParam = new SqlParameter[3];

            SqlParam[0] = DependancyService.SQLParameter("@RMatType", "System.Int16", DependancyService.ParamDirection.Input, (object)Type);
            SqlParam[1] = DependancyService.SQLParameter("@RMatCode", "System.String", DependancyService.ParamDirection.Input, (object)Code);
            SqlParam[2] = DependancyService.SQLParameter("@RMatColr", "System.Int16", DependancyService.ParamDirection.Input, (object)Colour);

            RollsMRec = WpfClassLibrary.WgmateDBUtilClass.GetRollsdbRollsMRecByColour(ref SqlParam, ref SQLWrite);

            return RollsMRec;
        }
        private RollsdbRollsMRec GetRollsMRecBySuplr(short Type, string Code, short Colour, short Suplr)
        {
            RollsdbRollsMRec RollsMRec = null;
            SqlParameter[] SqlParam = new SqlParameter[4];

            SqlParam[0] = DependancyService.SQLParameter("@RMatType", "System.Int16", DependancyService.ParamDirection.Input, (object)Type);
            SqlParam[1] = DependancyService.SQLParameter("@RMatCode", "System.String", DependancyService.ParamDirection.Input, (object)Code);
            SqlParam[2] = DependancyService.SQLParameter("@RMatColr", "System.Int16", DependancyService.ParamDirection.Input, (object)Colour);
            SqlParam[3] = DependancyService.SQLParameter("@RMatSupl", "System.Int16", DependancyService.ParamDirection.Input, (object)Suplr);

            RollsMRec = WpfClassLibrary.WgmateDBUtilClass.GetRollsdbRollsMRecByColour(ref SqlParam, ref SQLWrite);

            return RollsMRec;
        }

        private void ReadyCostingSheet()
        {
            listcostMRec[0].CTStyle = txtStyle.Text.Trim();
            listcostMRec[0].CTVarn = txtVarn.Text.Trim();
            if (listcostMRec[0].CTSpareShort1 == null) listcostMRec[0].CTSpareShort1 = 0;
            if (listcostMRec[0].CTSpareShort2 == null) listcostMRec[0].CTSpareShort2 = 0;
            listcostMRec[0].CTStyleVarn = string.Format("{0,-13}{1,-9}", txtStyle.Text.Trim(), txtVarn.Text.Trim());

            if (listcostMRec[0].CTVatRate == null) listcostMRec[0].CTVatRate = 0.0;
            if (listcostMRec[0].CTCostMakingF == null) listcostMRec[0].CTCostMakingF = 0.0;
            if (listcostMRec[0].CTDutyF == null) listcostMRec[0].CTDutyF = 0.0;
            if (listcostMRec[0].CTOverHeadsF == null) listcostMRec[0].CTOverHeadsF = 0.0;
            if (listcostMRec[0].CTContinF == null) listcostMRec[0].CTContinF = 0.0;
            if (listcostMRec[0].CTTotManCostF == null) listcostMRec[0].CTTotManCostF = 0.0;
            if (listcostMRec[0].CTProfMar1F == null) listcostMRec[0].CTProfMar1F = 0.0;

            // Set Costsdb.CTMatsRec Ordinal
            for (int i = 0; i < listcostMatsRec.Count; i++)
                listcostMatsRec[i].Ordinal = i;

            UserCostMainRec.ID = 0;
            UserCostMainRec.UCRecordNumber = 0;
            UserCostMainRec.UCUserName = "adm";
            UserCostMainRec.UCUserCode = "adm";
            UserCostMainRec.UCDate = DateTime.Now;
            UserCostMainRec.UCTime = (int)UserCostMainRec.UCDate.Value.TimeOfDay.Ticks;
            UserCostMainRec.UCStyleVarn = string.Format("{0,-13}{1,-9}", txtStyle.Text.Trim(), txtVarn.Text.Trim());
            UserCostMainRec.UCStyle = txtStyle.Text.Trim();
            UserCostMainRec.UCVarn = txtVarn.Text.Trim();
        }
        private bool InsertCostingSheet()
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            StringBuilder sbXMLString = new StringBuilder();
            settings.OmitXmlDeclaration = true;
            bool bRtn = false;
            int? TranID = 0;

            // Costsdb - CostMRec & RMatSizes
            using (XmlWriter writer = XmlWriter.Create(sbXMLString, settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("ROOT");

                writer.WriteStartElement("Usercost.UserCostMainRec");
                string sUserCostMainRec = WpfClassLibrary.Model.UsercostUserCostMainRec.Serialize(UserCostMainRec);
                IEnumerable<XElement> Elenum = WpfClassLibrary.Model.UsercostUserCostMainRec.GetElementEnum(sUserCostMainRec);

                #region // Load Costsdb.CostMRec Model Xml Data
                foreach (XElement xElem in Elenum)
                    writer.WriteRaw(xElem.ToString());
                #endregion

                writer.WriteEndElement();

                // *************************************************************************************************** 

                writer.WriteStartElement("Costsdb.CostMRec");
                string sCostMRec = WpfClassLibrary.Model.CostdbCostMRec.Serialize(listcostMRec[0]);
                Elenum = WpfClassLibrary.Model.CostdbCostMRec.GetElementEnum(sCostMRec);

                IEnumerable<XElement> ElenumFilter = from XElement in Elenum
                                where XElement.Name != "SelPrice2Percent" &&
                                XElement.Name != "SelPrice3Percent" &&
                                XElement.Name != "SelPrice4Percent" &&
                                XElement.Name != "SelPricePlusVat" &&
                                XElement.Name != "ProfMarginPerc" &&
                                XElement.Name != "CurrcyDiff" &&
                                XElement.Name != "QPRMatTotal" &&
                                XElement.Name != "QprPerc" &&
                                XElement.Name != "SellPricePerc" &&
                                XElement.Name != "StyleMarkUpPerc" &&
                                XElement.Name != "ProfMarPercDisc" &&
                                XElement.Name != "DiscValue" &&
                                XElement.Name != "MaterialCostF" &&
                                XElement.Name != "ProfirMarginF" &&
                                XElement.Name != "SellingRate" &&
                                XElement.Name != "BuyingRate"
                                select XElement;

                #region // Load Costsdb.CostMRec Model Xml Data
                foreach (XElement xElem in ElenumFilter)
                    writer.WriteRaw(xElem.ToString());
                #endregion
                writer.WriteEndElement();

                #region // Load Costsdb.CTMatsRec Model Xml Data
                foreach (CostdbCTMatsRec CTMatsRec in listcostMatsRec)
                {
                    // Skip Empty Costsdb.CTMatsRec Row
                    if (/*CTMatsRec.CTMatPart > 0 && */CTMatsRec.CTMatType > 0)
                    {
                        writer.WriteStartElement("Costsdb.CTMatsRec");
                        string sCTMatsRec = WpfClassLibrary.Model.CostdbCTMatsRec.Serialize(CTMatsRec);
                        Elenum = WpfClassLibrary.Model.CostdbCTMatsRec.GetElementEnum(sCTMatsRec);

                        ElenumFilter = from XElement in Elenum
                                       where XElement.Name != "MatPartDesc" &&
                                       XElement.Name != "MatTypeDesc" &&
                                       XElement.Name != "MatCodeDesc" &&
                                       XElement.Name != "MatColrDesc" &&
                                       XElement.Name != "MatSuplDesc" &&
                                       XElement.Name != "Cost" &&
                                       XElement.Name != "CountryCode" &&
                                       XElement.Name != "CurrcyNo"
                                       select XElement;

                        #region // Load Costsdb.CTMatsRec Model Xml Data
                        foreach (XElement xElem in ElenumFilter)
                            writer.WriteRaw(xElem.ToString());
                        #endregion
                        writer.WriteEndElement();
                    }
                }
                #endregion

                #region // Load Costsdb.CTNotsRec Model Xml Data
                if (listcostNotsRec.Count > 0)
                { 
                    writer.WriteStartElement("Costsdb.CTNotsRec");

                    foreach (CostdbCTNotsRec CTNotsRec in listcostNotsRec)
                    {
                        string sCTNotsRec = WpfClassLibrary.Model.CostdbCTNotsRec.Serialize(CTNotsRec);
                        Elenum = WpfClassLibrary.Model.CostdbCTNotsRec.GetElementEnum(sCTNotsRec);

                        #region // Load Costsdb.CTNotsRec Model Xml Data
                        foreach (XElement xElem in Elenum)
                            writer.WriteRaw(xElem.ToString());
                        #endregion
                        writer.WriteEndElement();
                    }
                }
                #endregion

                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Flush();

                SQLWrite.SQLWriteCommand("InsertCostsdbCostMRec", SQLWriteClass.SqlCmdType.PROCEDURE);
                SqlParameter[] SqlParam = new SqlParameter[2];

                SqlParam[0] = DependancyService.SQLParameter("@lTranID",
                        "System.Int32", DependancyService.ParamDirection.Output, (object)TranID);

                SqlParam[1] = DependancyService.SQLParameter("@XmlString",
                        "System.String", DependancyService.ParamDirection.Input, (object)sbXMLString.ToString());

                bRtn = SQLWrite.ExecuteNonQuery(SqlParam);
            }
            return bRtn;
        }
        private bool UpdateCostingSheet()
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            StringBuilder sbXMLString = new StringBuilder();
            settings.OmitXmlDeclaration = true;
            bool bRtn = false;

            // Costdb Key Passed in to Procedure
            int? TranID = listcostMRec[0].CostdbID;

            // Costsdb - CostMRec & RMatSizes
            using (XmlWriter writer = XmlWriter.Create(sbXMLString, settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("ROOT");

                // ***************************************************************************************************

                writer.WriteStartElement("Costsdb.CostMRec");
                string sCostMRec = WpfClassLibrary.Model.CostdbCostMRec.Serialize(listcostMRec[0]);
                IEnumerable<XElement> Elenum = WpfClassLibrary.Model.CostdbCostMRec.GetElementEnum(sCostMRec);

                IEnumerable<XElement> ElenumFilter = from XElement in Elenum
                                                     where XElement.Name != "SelPrice2Percent" &&
                                                     XElement.Name != "SelPrice3Percent" &&
                                                     XElement.Name != "SelPrice4Percent" &&
                                                     XElement.Name != "SelPricePlusVat" &&
                                                     XElement.Name != "ProfMarginPerc" &&
                                                     XElement.Name != "CurrcyDiff" &&
                                                     XElement.Name != "QPRMatTotal" &&
                                                     XElement.Name != "QprPerc" &&
                                                     XElement.Name != "SellPricePerc" &&
                                                     XElement.Name != "StyleMarkUpPerc" &&
                                                     XElement.Name != "ProfMarPercDisc" &&
                                                     XElement.Name != "MaterialCostF" &&
                                                     XElement.Name != "ProfirMarginF" &&
                                                     XElement.Name != "SellingRate" &&
                                                     XElement.Name != "BuyingRate"
                                                     select XElement;

                #region // Load Costsdb.CostMRec Model Xml Data
                foreach (XElement xElem in ElenumFilter)
                    writer.WriteRaw(xElem.ToString());
                #endregion
                writer.WriteEndElement();

                #region // Load Costsdb.CTMatsRec Model Xml Data
                foreach (CostdbCTMatsRec CTMatsRec in listcostMatsRec)
                {
                    // Skip Empty Costsdb.CTMatsRec Row
                    if (CTMatsRec.CTMatType > 0)
                    {
                        writer.WriteStartElement("Costsdb.CTMatsRec");
                        string sCTMatsRec = WpfClassLibrary.Model.CostdbCTMatsRec.Serialize(CTMatsRec);
                        Elenum = WpfClassLibrary.Model.CostdbCTMatsRec.GetElementEnum(sCTMatsRec);

                        ElenumFilter = from XElement in Elenum
                                       where XElement.Name != "MatPartDesc" &&
                                       XElement.Name != "MatTypeDesc" &&
                                       XElement.Name != "MatCodeDesc" &&
                                       XElement.Name != "MatColrDesc" &&
                                       XElement.Name != "MatSuplDesc" &&
                                       XElement.Name != "Cost" &&
                                       XElement.Name != "CountryCode" &&
                                       XElement.Name != "CurrcyNo"
                                       select XElement;

                        #region // Load Costsdb.CTMatsRec Model Xml Data
                        foreach (XElement xElem in ElenumFilter)
                            writer.WriteRaw(xElem.ToString());
                        #endregion
                        writer.WriteEndElement();
                    }
                }
                #endregion

                #region // Load Costsdb.CTNotsRec Model Xml Data
                if (listcostNotsRec.Count > 0)
                {
                    writer.WriteStartElement("Costsdb.CTNotsRec");

                    foreach (CostdbCTNotsRec CTNotsRec in listcostNotsRec)
                    {
                        string sCTNotsRec = WpfClassLibrary.Model.CostdbCTNotsRec.Serialize(CTNotsRec);
                        Elenum = WpfClassLibrary.Model.CostdbCTNotsRec.GetElementEnum(sCTNotsRec);

                        #region // Load Costsdb.CTNotsRec Model Xml Data
                        foreach (XElement xElem in Elenum)
                            writer.WriteRaw(xElem.ToString());
                        #endregion
                        writer.WriteEndElement();
                    }
                }
                #endregion

                // ***************************************************************************************************

                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Flush();

                SQLWrite.SQLWriteCommand("UpdateCostsdbCostMRec", SQLWriteClass.SqlCmdType.PROCEDURE);
                SqlParameter[] SqlParam = new SqlParameter[2];

                SqlParam[0] = DependancyService.SQLParameter("@lTranID",
                        "System.Int32", DependancyService.ParamDirection.InputOutput, (object)TranID);

                SqlParam[1] = DependancyService.SQLParameter("@XmlString",
                        "System.String", DependancyService.ParamDirection.Input, (object)sbXMLString.ToString());

                bRtn = SQLWrite.ExecuteNonQuery(SqlParam);
            }
            return bRtn;
        }

        #region // Form Events
        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
            viewState = AppViewState.NEW;
            EnableControls(true);
            ClearCostnSheet();

            LabelWindow();
            DataContext = null;
            listcostMRec.Clear();
            MainGrid.ItemsSource = null;
            listcostMRec.Add(new CostdbCostMRec());

            DataContext = listcostMRec[0];

            for (int i = 0; i < 12; i++)
                listcostMatsRec.Add(new CostdbCTMatsRec());

            MainGrid.ItemsSource = listcostMatsRec;
            txtStyle.Focus();

            Mouse.OverrideCursor = null;
        }
        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            viewState = AppViewState.EDIT;
            EnableControls(true);
            LabelWindow();
        }
        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            switch (viewState)
            {
                case AppViewState.VIEW:
                    ClearCostnSheet();
                    viewState = AppViewState.SEARCH;

                    LabelWindow();
                    EnableControls(false);
                    listcostMatsRec.AddRange(Enumerable.Range(0, 12).Select(i => new CostdbCTMatsRec()));

                    MainGrid.ItemsSource = listcostMatsRec;
                    txtStyle.Focus();

                    btnNew_Click(this, new RoutedEventArgs());  // Change to Ignore Edit & New
                    break;
                case AppViewState.NEW: 
                    ClearCostnSheet();
                    viewState = AppViewState.SEARCH;

                    LabelWindow();
                    EnableControls(false);
                    MainGrid.ItemsSource = null;
                    listcostMatsRec.AddRange(Enumerable.Range(0, 12).Select(i => new CostdbCTMatsRec()));

                    MainGrid.ItemsSource = listcostMatsRec;
                    txtStyle.Focus();
                    break;
                case AppViewState.EDIT:
                    viewState = AppViewState.VIEW;
                    EnableControls(false);
                    LabelWindow();
                    break;
                case AppViewState.SEARCH:
                    ((WPFMDIForm.MainWindow)ContainerWindow).CloseMidiChildWnd(this);
                    break;
                case AppViewState.EMPTY:
                    ((WPFMDIForm.MainWindow)ContainerWindow).CloseMidiChildWnd(this);
                    break;
                default:
                    break;
            } // end switch
        }
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (txtStyle.Text.Trim().Length == 0 || listcostMatsRec[0].CTMatType == null || listcostMatsRec[0].CTMatType == 0)
            {
                MessageBoxEx.Show(ContainerWindow, "Unable to Save, Not valid Costing Sheet", "Save Costing Sheet", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            if (MessageBoxEx.Show(ContainerWindow, "Are you sure?", "Save Costing Sheet", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;

                // Hold Costing Sheet Values
                string sStyle = txtStyle.Text.Trim(), sVarn = txtVarn.Text.Trim();

                ReadyCostingSheet();

                switch (viewState)
                { 
                    case AppViewState.NEW:
                        if (InsertCostingSheet())
                        {
                            btnExit_Click(this, new RoutedEventArgs());
                            txtStyle.Text = sStyle;
                            txtVarn.Text = sVarn;
                            WpfClassLibrary.CommonUtilClass.DelayAction(2000, GetStyleVarn);
                        }
                        else
                        {
                            Mouse.OverrideCursor = null;
                            MessageBoxEx.Show(ContainerWindow, "Unable to Create Costing Sheet", "Save New Costing Sheet", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        }
                        break;
                    case AppViewState.EDIT:
                        if (UpdateCostingSheet())
                        {
                            btnExit_Click(this, new RoutedEventArgs());
                            txtStyle.Text = sStyle;
                            txtVarn.Text = sVarn;
                            txtVarn_LostFocus(this, new RoutedEventArgs());
                        }
                        else
                        {
                            Mouse.OverrideCursor = null;
                            MessageBoxEx.Show(ContainerWindow, "Unable to Update Costing Sheet", "Update Costing Sheet", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        }
                        break;
                } // end switch
            }
        }

        private void btnCopy_Click(object sender, RoutedEventArgs e)
        {
            CostnCopyAllWindow CostnCopyAll = new CostnCopyAllWindow();
            CostnCopyAll.Owner = ContainerWindow;
            CostnCopyAll.ShowDialog();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (!bLoaded)
            {
                UIElementUtility.EnumVisual(this, winCtrlList);

                txtblkHomeCurrcy.Text = WgmateDBUtilClass.GetGmcurrsCURCurrenciesDescr(1, ref dtCURCurrenciesRec);
                viewState = AppViewState.SEARCH;
                EnableControls(false);

                listcostMatsRec.AddRange(Enumerable.Range(0, 12).Select(i => new CostdbCTMatsRec()));
                MainGrid.ItemsSource = listcostMatsRec;
                bLoaded = true;
                LabelWindow();
            }

            txtStyle.Focus();
            // Change to Ignore Edit & New
            btnNew_Click(this, new RoutedEventArgs());

            Mouse.OverrideCursor = null;
        }
        private void btNotes_Click(object sender, RoutedEventArgs e)
        {
            string sNotes = "";

            // Load Notes
            foreach (CostdbCTNotsRec Note in listcostNotsRec)
                sNotes += Note.CTNotes;

            CostnNotesWindow CostnNotes = new CostnNotesWindow();
            CostnNotes.sNotes = sNotes;
            CostnNotes.Owner = ContainerWindow;
            CostnNotes.ShowDialog();


            if ((bool)CostnNotes.DialogResult)
            {
                listcostNotsRec.Clear();
                IEnumerable<string> NoteEnum = CommonUtilClass.WholeChunks(CostnNotes.sNotes, 80);

                foreach (string sNote in NoteEnum)
                {
                    CostdbCTNotsRec CTNotsRec = new CostdbCTNotsRec();
                    CTNotsRec.CTNotes = sNote;
                    listcostNotsRec.Add(CTNotsRec);
                }
            }
        }
        private void btnSpec_Click(object sender, RoutedEventArgs e)
        {
            short GarrType = 0;
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;

            if (!short.TryParse(txtType.Text, out GarrType))
                GarrType = 0;

            CostnGridSpecSheetWindow CostnGridSpecSheet = new CostnGridSpecSheetWindow(txtStyle.Text, txtVarn.Text, GarrType, txtSize.Text, txtDesc.Text);
            CostnGridSpecSheet.Owner = ContainerWindow;
            CostnGridSpecSheet.ShowDialog();
        }
        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
            CostnPrintOptionsWindow PrintOption = new CostnPrintOptionsWindow();
            PrintOption.Owner = ContainerWindow;

            if ((bool)PrintOption.ShowDialog())
            {
                PrintCostingSheet PrintOpt = new PrintCostingSheet(PrintOption.PrintOptions, ref listcostMRec, 
                                                                        ref listcostMatsRec, ref listcostMatsFFRec,
                                                                            ref listcostInstsRec, ref listcostOpersRec, ref listcostNotsRec);
                PrintOpt.ContainerWindow = ContainerWindow;
                PrintOpt.DisplayCostingReport();
            }
        }
        private void btnWidth_Click(object sender, RoutedEventArgs e)
        {
            if (IsCTMatsRecValid())
            {
                MatWidthWindow MatWidth = new MatWidthWindow();

                MatWidth.CTMatType = (short)listcostMatsRec[MainGrid.SelectedIndex].CTMatType;
                MatWidth.CTMatCode = listcostMatsRec[MainGrid.SelectedIndex].CTMatCode.Trim();
                MatWidth.CTMatColour = (short)listcostMatsRec[MainGrid.SelectedIndex].CTMatColr;
                MatWidth.CTMatSupl = (short)listcostMatsRec[MainGrid.SelectedIndex].CTMatSupl;
                MatWidth.CTMatWhse = (short)listcostMatsRec[MainGrid.SelectedIndex].CTMatWhse;
                MatWidth.CTMatLocn = (short)listcostMatsRec[MainGrid.SelectedIndex].CTMatLocn;

                MatWidth.Owner = ContainerWindow;
                MatWidth.ShowDialog();

                if ((bool)MatWidth.DialogResult)
                {

                }
            }
            else
                MessageBoxEx.Show(ContainerWindow, "Unable to set Material Width\nMissing Values", "Material Width", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }
        private void btnGrade_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
            CostnAverageCostingWindow AverageCosting = new CostnAverageCostingWindow(txtSize.Text.Trim());
            AverageCosting.Owner = ContainerWindow;
            AverageCosting.ShowDialog();
        }
        private void btnExtra_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
            CostnExtrasWindow Extra = Extra = new CostnExtrasWindow();

            Extra.CTSizeKey = txtSize.Text;
            Extra.CTStyle = txtStyle.Text;
            Extra.CTVarn = txtVarn.Text;
            Extra.Owner = ContainerWindow;
            Extra.ShowDialog();
        }
        private void btnCurrcy_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
            CostnCurrencyDetailsWindow CurrencyDetails = new CostnCurrencyDetailsWindow();
            CurrencyDetails.Owner = ContainerWindow;
            CurrencyDetails.ShowDialog();
        }
        private void btnConsum_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
            ColoursOptions ColoursOpt = new ColoursOptions();
            ColoursOpt.CTStyle = txtStyle.Text.Trim();
            ColoursOpt.CTVarn = txtVarn.Text.Trim();
            ColoursOpt.CTSizeKey = txtSize.Text.Trim();
            ColoursOpt.CTMatPart = (short)listcostMatsRec[MainGrid.SelectedIndex].CTMatPart;
            ColoursOpt.CTMatType = (short)listcostMatsRec[MainGrid.SelectedIndex].CTMatType;
            ColoursOpt.CTMatCode = listcostMatsRec[MainGrid.SelectedIndex].CTMatCode;
            ColoursOpt.CTMatPartDesc = listcostMatsRec[MainGrid.SelectedIndex].MatPartDesc;
            ColoursOpt.CTMatTypeDescr = listcostMatsRec[MainGrid.SelectedIndex].MatTypeDesc;
            ColoursOpt.CTMatDescription = listcostMatsRec[MainGrid.SelectedIndex].MatCodeDesc;
            ColoursOpt.CTMatSuplName = listcostMatsRec[MainGrid.SelectedIndex].MatSuplDesc;
            ColoursOpt.CTMatColrDescr = listcostMatsRec[MainGrid.SelectedIndex].MatColrDesc;
            ColoursOpt.CTMatColr = (short)listcostMatsRec[MainGrid.SelectedIndex].CTMatColr;
            ColoursOpt.CTMatSupl = (short)listcostMatsRec[MainGrid.SelectedIndex].CTMatSupl;
            ColoursOpt.CTMatWhse = (short)listcostMatsRec[MainGrid.SelectedIndex].CTMatWhse;
            ColoursOpt.CTMatLocn = (short)listcostMatsRec[MainGrid.SelectedIndex].CTMatLocn;

            CostnConsumeWindow CostnConsume = new CostnConsumeWindow(ColoursOpt);
            CostnConsume.Owner = ContainerWindow;
            CostnConsume.ShowDialog();
        }
        private void btnInsert_Click(object sender, RoutedEventArgs e)
        {
            listcostMatsRec.Insert(MainGrid.SelectedIndex, new CostdbCTMatsRec());
            MainGrid.ItemsSource = null;
            MainGrid.ItemsSource = listcostMatsRec;
        }
        private void btnDelLine_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBoxEx.Show(ContainerWindow, "Are you sure?", "Delete Line", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                listcostMatsRec.RemoveAt(MainGrid.SelectedIndex);
                MainGrid.ItemsSource = null;
                MainGrid.ItemsSource = listcostMatsRec;
            }
        }
        private void btnEmblish_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;

            CostnSpecialInstructionWindow SpecialInstruct = new CostnSpecialInstructionWindow(txtStyle.Text.Trim(), txtVarn.Text.Trim(), DocketEmbellish, 1);
            SpecialInstruct.Owner = ContainerWindow;
            SpecialInstruct.ShowDialog();
        }
        private void btnColComb_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
            CostdbCTMatsRec CTMatsRec = listcostMatsRec[MainGrid.SelectedIndex];
            CostnColourCombWindow ColourComb = new CostnColourCombWindow(listcostMRec[0].CTStyle.Trim(),
                                                    listcostMRec[0].CTVarn.Trim(), (short)CTMatsRec.CTMatPart,
                                                    (short)CTMatsRec.CTMatType, CTMatsRec.CTMatCode.Trim(),
                                                    (short)CTMatsRec.CTMatColr, (short)CTMatsRec.CTMatSupl);
            ColourComb.Owner = ContainerWindow;
            ColourComb.ShowDialog();
        }
        private void btnCusPref_Click(object sender, RoutedEventArgs e)
        {
            CusPrefrerence CustPref = new CusPrefrerence();
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;

            CustPref.Style = txtStyle.Text.Trim();
            CustPref.Varn = txtVarn.Text.Trim();
            CustPref.Part = (short)listcostMatsRec[MainGrid.SelectedIndex].CTMatPart;
            CustPref.PartDesc = listcostMatsRec[MainGrid.SelectedIndex].MatPartDesc;
            CustPref.StyleDesc = txtDesc.Text.Trim();
            CustPref.Part = (short)listcostMatsRec[MainGrid.SelectedIndex].CTMatPart;
            CustPref.Type = (short)listcostMatsRec[MainGrid.SelectedIndex].CTMatType;
            CustPref.TypeDesc = listcostMatsRec[MainGrid.SelectedIndex].MatTypeDesc;
            CustPref.Code = listcostMatsRec[MainGrid.SelectedIndex].CTMatCode.Trim();
            CustPref.CodeDesc = listcostMatsRec[MainGrid.SelectedIndex].MatCodeDesc;
            CustPref.Colr = (short)listcostMatsRec[MainGrid.SelectedIndex].CTMatColr;
            CustPref.ColrDesc = listcostMatsRec[MainGrid.SelectedIndex].MatColrDesc;
            CustPref.Colr1 = (short)listcostMatsRec[MainGrid.SelectedIndex].CTMatColr1;
            CustPref.Supl = (short)listcostMatsRec[MainGrid.SelectedIndex].CTMatSupl;
            CustPref.SuplName = listcostMatsRec[MainGrid.SelectedIndex].MatSuplDesc;
            CustPref.Whse = (short)listcostMatsRec[MainGrid.SelectedIndex].CTMatWhse;
            CustPref.Locn = (short)listcostMatsRec[MainGrid.SelectedIndex].CTMatLocn;
            CustPref.CustNo = 0;
            CustPref.CustName = "";
            CustPref.Price = listcostMatsRec[MainGrid.SelectedIndex].CTMatPrice;
            CustPref.Costing = listcostMatsRec[MainGrid.SelectedIndex].CTCosting;

            CostnCustomerPreferenceWindow CustomerPref = new CostnCustomerPreferenceWindow(CustPref);
            CustomerPref.Owner = ContainerWindow;
            CustomerPref.ShowDialog();
        }
        private void btnMatProp_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;

            CostnPropertiesWindow Properties = new CostnPropertiesWindow(
                (short)listcostMatsRec[MainGrid.SelectedIndex].CTMatType, 
                listcostMatsRec[MainGrid.SelectedIndex].CTMatCode.Trim(),
                listcostMatsRec[MainGrid.SelectedIndex].MatCodeDesc,
                (short)listcostMatsRec[MainGrid.SelectedIndex].CTMatColr, 0, 
                (short)listcostMatsRec[MainGrid.SelectedIndex].CTMatSupl, 
                (short)listcostMatsRec[MainGrid.SelectedIndex].CTMatWhse, 
                (short)listcostMatsRec[MainGrid.SelectedIndex].CTMatLocn);

            Properties.Owner = ContainerWindow;
            Properties.ShowDialog();
        }
        private void btnSizeMap_Click(object sender, RoutedEventArgs e)
        {
            if (listcostMatsRec.Count == 0 || listcostMatsRec == null) return;

            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;

            CostdbCTMatsRec CTMatsRec = listcostMatsRec[MainGrid.SelectedIndex];
            SQLWrite.SQLWriteCommand("[dbo].[RollsdbRollsMRecRMatSizeKey]");
            SqlParameter[] SqlParam = new SqlParameter[7];

            SqlParam[0] = DependancyService.SQLParameter("@CTMatType",
            "System.Int16", DependancyService.ParamDirection.Input, (object)(short)CTMatsRec.CTMatType);

            SqlParam[1] = DependancyService.SQLParameter("@CTMatCode",
                    "System.String", DependancyService.ParamDirection.Input, (object)CTMatsRec.CTMatCode.Trim());

            SqlParam[2] = DependancyService.SQLParameter("@CTMatColr",
                    "System.Int16", DependancyService.ParamDirection.Input, (object)(short)CTMatsRec.CTMatColr);

            SqlParam[3] = DependancyService.SQLParameter("@CTMatColr1",
                    "System.Int16", DependancyService.ParamDirection.Input, (object)0);

            SqlParam[4] = DependancyService.SQLParameter("@CTMatSupl",
                    "System.Int16", DependancyService.ParamDirection.Input, (object)(short)CTMatsRec.CTMatSupl);

            SqlParam[5] = DependancyService.SQLParameter("@CTMatWhse",
                    "System.Int16", DependancyService.ParamDirection.Input, (object)(short)CTMatsRec.CTMatWhse);

            SqlParam[6] = DependancyService.SQLParameter("@CTMatLocn",
                    "System.Int16", DependancyService.ParamDirection.Input, (object)(short)CTMatsRec.CTMatLocn);

            string sMatSize = (string)SQLWrite.ExecuteQueryFunction(SqlParam);

            if (sMatSize.Trim().Length > 0)
            {
                CTSizeMapping SizeMapping = new CTSizeMapping();

                SizeMapping.GarSizeKey = txtSize.Text;
                SizeMapping.CTMatSize = sMatSize;
                SizeMapping.CTStyle = txtStyle.Text;
                SizeMapping.CTVarn = txtVarn.Text;
                SizeMapping.CTMatPart = (short)CTMatsRec.CTMatPart;
                SizeMapping.CTMatType = (short)CTMatsRec.CTMatType;
                SizeMapping.CTMatTypeDesc = CTMatsRec.MatTypeDesc;
                SizeMapping.CTMatCode = CTMatsRec.CTMatCode.Trim();
                SizeMapping.CTMatCodeDesc = CTMatsRec.MatCodeDesc;
                SizeMapping.CTColour = (short)CTMatsRec.CTMatColr;
                SizeMapping.CTColourDesc = CTMatsRec.MatColrDesc;
                SizeMapping.CTColour1 = 0;
                SizeMapping.CTMatSuplr = (short)CTMatsRec.CTMatSupl;
                SizeMapping.CTMatWhse = (short)CTMatsRec.CTMatWhse;
                SizeMapping.CTLocn = (short)CTMatsRec.CTMatLocn;

                CostnSizeMappingWindow WinSizeMapping = new CostnSizeMappingWindow(SizeMapping);
                WinSizeMapping.Owner = ContainerWindow;
                WinSizeMapping.ShowDialog();
            }
            else
                Mouse.OverrideCursor = null;

        }
        private void btnDelStyle_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBoxEx.Show(ContainerWindow, "Are you sure?", "Delete Costing Sheet", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;

                SQLWrite.SQLWriteCommand("DeleteCostingSheet", SQLWriteClass.SqlCmdType.PROCEDURE);
                SqlParameter[] SqlParam = new SqlParameter[3];
                int? TranID = 0;

                SqlParam[0] = DependancyService.SQLParameter("@lTranID",
                        "System.Int32", DependancyService.ParamDirection.Input, (object)TranID);

                SqlParam[1] = DependancyService.SQLParameter("@Style",
                        "System.String", DependancyService.ParamDirection.Input, (object)txtStyle.Text.Trim());

                SqlParam[2] = DependancyService.SQLParameter("@Varn",
                        "System.String", DependancyService.ParamDirection.Input, (object)txtVarn.Text.Trim());

                bool bRtn = SQLWrite.ExecuteNonQuery(SqlParam);
                Mouse.OverrideCursor = null;

                if (!bRtn)
                    MessageBoxEx.Show(ContainerWindow, "Unable to Delete Style", "Delete Style", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
        private void txtType_LostFocus(object sender, RoutedEventArgs e)
        {
            Int16 Num;

            if (Int16.TryParse(txtType.Text, out Num))
            {
                txtblkTypeLabel.Text = WpfClassLibrary.WgmateDBUtilClass.GetGmParamsDescr(7, Num, ref dtParametersRec);
                if (txtblkTypeLabel.Text.Length == 0) txtType.Text = "";
            }
            else
            {
                txtType.Text = "";
                txtblkTypeLabel.Text = "";
            }
        }
        private void txtVarn_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtStyle.Text.Length > 0/* && txtVarn.Text.Length > 0*/)
            {
                Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;

                CostnStyleDetailsControl.CTStyle = txtStyle.Text.Trim();
                CostnStyleDetailsControl.CTVarn = txtVarn.Text.Trim();
                GetStyleVarn();
            }
        }
        private void btnMakeCost_Click(object sender, RoutedEventArgs e)
        {
            CostnMakeCostWindow MakeCostWin;
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;

            if (MakeCost != null)
                MakeCostWin = new CostnMakeCostWindow(MakeCost.listMakeCostDisplay, MakeCost.lisGmcopersGMCOperMRec);
            else
                MakeCostWin = new CostnMakeCostWindow(txtStyle.Text.Trim(), txtVarn.Text.Trim());

            MakeCostWin.Owner = ContainerWindow;
            
            if ((bool)MakeCostWin.ShowDialog())
                MakeCost = MakeCostWin.MakeCost;
        }
        private void txtSize_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtSize.Text.Trim().Length > 0)
            {
                if (!WpfClassLibrary.WgmateDBUtilClass.IsGmSizeKeyValid(txtSize.Text, ref dtGMSizesRec))
                {
                    MessageBoxEx.Show(ContainerWindow, "Unable to Find Size Entered", "Garrment Size", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    txtSize.Text = "";
                }
            }
        }
        private void btnGarmetCol_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;

            if (MainGrid.SelectedIndex > -1)
            {
                CostnColoursWindow ColoursWindow = new CostnColoursWindow(txtStyle.Text.Trim(), txtVarn.Text.Trim(), txtSize.Text.Trim(),
                    (short)listcostMatsRec[MainGrid.SelectedIndex].CTMatPart,
                    (short)listcostMatsRec[MainGrid.SelectedIndex].CTMatType,
                    listcostMatsRec[MainGrid.SelectedIndex].CTMatCode.Trim(),
                    listcostMatsRec[MainGrid.SelectedIndex].MatPartDesc,
                    listcostMatsRec[MainGrid.SelectedIndex].MatTypeDesc,
                    listcostMatsRec[MainGrid.SelectedIndex].MatCodeDesc,
                    listcostMatsRec[MainGrid.SelectedIndex].MatSuplDesc,
                    listcostMatsRec[MainGrid.SelectedIndex].MatColrDesc,
                    (short)listcostMatsRec[MainGrid.SelectedIndex].CTMatColr,
                    (short)listcostMatsRec[MainGrid.SelectedIndex].CTMatSupl,
                    (short)listcostMatsRec[MainGrid.SelectedIndex].CTMatWhse,
                    (short)listcostMatsRec[MainGrid.SelectedIndex].CTMatLocn,
                    listcostMRec[0], listcostMatsRec);
                ColoursWindow.ContainerWindow = ContainerWindow;
                ColoursWindow.Owner = ContainerWindow;
                ColoursWindow.ShowDialog();
            }
        }
        private void btnPriceList_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
            CostnPriceListWindow PriceList = new CostnPriceListWindow(txtStyle.Text.Trim(), txtVarn.Text.Trim(), txtDesc.Text.Trim());
            PriceList.Owner = ContainerWindow;
            PriceList.ShowDialog();
        }
        private void txtBrand_LostFocus(object sender, RoutedEventArgs e)
        {
            Int16 Num;

            if (Int16.TryParse(txtBrand.Text, out Num))
            {
                txtblkBrandDesc.Text = WpfClassLibrary.WgmateDBUtilClass.GetGmParamsDescr(14, Num, ref dtParametersRec);
                if (txtblkBrandDesc.Text.Length == 0) txtBrand.Text = "";
            }
            else
            {
                txtBrand.Text = "";
                txtblkBrandDesc.Text = "";
            }
        }
        private void txtStyle_LostFocus(object sender, RoutedEventArgs e)
        {
        }
        private void txtGender_LostFocus(object sender, RoutedEventArgs e)
        {
            Int16 Num;

            if (Int16.TryParse(txtGender.Text, out Num))
            {
                txtblkGender.Text = WpfClassLibrary.WgmateDBUtilClass.GetGmParamsDescr(27, Num, ref dtParametersRec);
                if (txtblkGender.Text.Length == 0) txtGender.Text = "";
            }
            else
            {
                txtGender.Text = "";
                txtblkGender.Text = "";
            }
        }
        private void btnCostBySize_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
            ColoursOptions ColoursOption = new ColoursOptions(txtStyle.Text.Trim(), txtVarn.Text.Trim(), txtSize.Text.Trim(),
                (short)listcostMatsRec[MainGrid.SelectedIndex].CTMatPart, (short)listcostMatsRec[MainGrid.SelectedIndex].CTMatType,
                    listcostMatsRec[MainGrid.SelectedIndex].CTMatCode, listcostMatsRec[MainGrid.SelectedIndex].MatPartDesc,
                    listcostMatsRec[MainGrid.SelectedIndex].MatTypeDesc, listcostMatsRec[MainGrid.SelectedIndex].MatCodeDesc,
                    listcostMatsRec[MainGrid.SelectedIndex].MatColrDesc, (short)listcostMatsRec[MainGrid.SelectedIndex].CTMatColr,
                    (short)listcostMatsRec[MainGrid.SelectedIndex].CTMatSupl, (short)listcostMatsRec[MainGrid.SelectedIndex].CTMatWhse,
                    (short)listcostMatsRec[MainGrid.SelectedIndex].CTMatLocn, listcostMRec[0], listcostMatsRec);

            CostnCostBySizeWindow CostnCostBySize = new CostnCostBySizeWindow(ColoursOption);
            CostnCostBySize.Owner = ContainerWindow;
            CostnCostBySize.ShowDialog();
        }
        private void btnOperations_Click(object sender, RoutedEventArgs e)
        {
            CostnOperationsWindow Operations = null;
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;

            if (viewState == AppViewState.NEW)
            {
                if (ListGmcopersGMCOperMRec != null && ListGmcopersGMCOpersRec != null)
                    Operations = new CostnOperationsWindow(ListGmcopersGMCOperMRec, ListGmcopersGMCOpersRec);
                else
                    Operations = new CostnOperationsWindow(txtStyle.Text.Trim(), txtVarn.Text.Trim());
            }
            else
                Operations = new CostnOperationsWindow(txtStyle.Text.Trim(), txtVarn.Text.Trim());

            Operations.Owner = ContainerWindow;

            if ((bool)Operations.ShowDialog())
            {
                if (viewState == AppViewState.NEW)
                {
                    ListGmcopersGMCOperMRec = Operations.ListGmcopersGMCOperMRec;
                    ListGmcopersGMCOpersRec = Operations.ListGmcopersGMCOpersRec;
                }
            }
        }
        private void txtCountry_LostFocus(object sender, RoutedEventArgs e)
        {
            Int16 Num;
            string sDesc = "";

            if (Int16.TryParse(txtGender.Text, out Num))
            {
                sDesc = WpfClassLibrary.WgmateDBUtilClass.GetGmParamsDescr(8, Num, ref dtParametersRec);

                if (sDesc.Length == 0)
                {
                    txtCountry.Text = "";
                    listcostMRec[0].CTCountryCode = null;
                }
                else
                {
                    listcostMRec[0].CTCountryCode = Num;
                    listcostMRec[0].QPRMatTotal = TotalQPRMatCost();

                    if (listcostMRec[0].CTSelPrice1 > 0.0001)
                        listcostMRec[0].QprPerc = (listcostMRec[0].QPRMatTotal / listcostMRec[0].CTSelPrice1) * 100.0;
                    else
                        listcostMRec[0].QprPerc = null;
                }
            }
            else
                txtCountry.Text = "";
        }
        private void btnModMaterial_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
            CostnMaterialEntryWindow MaterialEntry = new CostnMaterialEntryWindow((short)listcostMatsRec[MainGrid.SelectedIndex].CTMatType, listcostMatsRec[MainGrid.SelectedIndex].CTMatCode, listcostMatsRec[MainGrid.SelectedIndex].MatCodeDesc, (short)listcostMatsRec[MainGrid.SelectedIndex].CTMatColr, 0, (short)listcostMatsRec[MainGrid.SelectedIndex].CTMatSupl, (short)listcostMatsRec[MainGrid.SelectedIndex].CTMatWhse, (short)listcostMatsRec[MainGrid.SelectedIndex].CTMatLocn, false);

            MaterialEntry.Owner = ContainerWindow;
            MaterialEntry.ShowDialog();
        }
        private void btnDcknInstruc_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
            CostnSpecialInstructionWindow SpecialInstruct = new CostnSpecialInstructionWindow(txtStyle.Text.Trim(), txtVarn.Text.Trim(), DocketInstructType, 0);
            SpecialInstruct.Owner = ContainerWindow;
            SpecialInstruct.ShowDialog();
        }
        private void btnPackInstruc_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;

            CostnSpecialInstructionWindow SpecialInstruct = new CostnSpecialInstructionWindow(txtStyle.Text.Trim(), txtVarn.Text.Trim(), DocketPacking, 2);
            SpecialInstruct.Owner = ContainerWindow;
            SpecialInstruct.ShowDialog();
        }
        private void txtCatagory_LostFocus(object sender, RoutedEventArgs e)
        {
            Int16 Num;

            if (Int16.TryParse(txtCatagory.Text, out Num))
            {
                txtblkCatagory.Text = WpfClassLibrary.WgmateDBUtilClass.GetGmParamsDescr(11, Num, ref dtParametersRec);
                if (txtblkCatagory.Text.Length == 0) txtCatagory.Text = "";
            }
            else
            {
                txtCatagory.Text = "";
                txtblkCatagory.Text = "";
            }
        }
        private void txtStyle_TextChanged(object sender, TextChangedEventArgs e)
        {
            bLoadByStyleDetails = false;
            StyleVarnArg.Varn = txtVarn.Text;
            StyleVarnArg.Style = txtStyle.Text;
        }
        private void MagnifierOptionsControl_MagEvent(object sender, MagniferEvent e)
        {
            _magnifier.FrameType = e.MagFrame;
            _magnifier.Width = e.MagHeight;
            _magnifier.Height = e.MagHeight;
            _magnifier.Radius = e.Radius;
            _magnifier.ZoomFactor = e.Zoom;
            _magnifier.FrameType = e.MagFrame;
            _magnifier.BorderThickness = new Thickness(e.Border);
            _magnifier.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(((Color)e.MagBrush).ToString()));
        }
        private void btnSpinStyle_Spin(object sender, Xceed.Wpf.Toolkit.SpinEventArgs e)
        {
            if (StyleVarnList == null || StyleVarnList.Count == 0) return;
            bSpinButton = true;

            if (e.Direction == Xceed.Wpf.Toolkit.SpinDirection.Increase)
            {
                if ((StyleVarnIndex + 1) <= (StyleVarnList.Count - 1))
                {
                    StyleVarnIndex += 1;
                    txtStyle.Text = StyleVarnList[StyleVarnIndex].CTStyle.Trim();
                    txtVarn.Text = StyleVarnList[StyleVarnIndex].CTVarn.Trim();
                    txtVarn_LostFocus(this, new RoutedEventArgs());
                }
            }
            else
            {
                if (!((StyleVarnIndex - 1) < 0))
                {
                    StyleVarnIndex -= 1;
                    txtStyle.Text = StyleVarnList[StyleVarnIndex].CTStyle.Trim();
                    txtVarn.Text = StyleVarnList[StyleVarnIndex].CTVarn.Trim();
                    txtVarn_LostFocus(this, new RoutedEventArgs());
                }
            }
        }
        private void uscCostnStyleDetails_StyleVarnEvent(object sender, StyleVarnEventArgs e)
        {
            //Cursor = Cursors.Wait;

            StyleVarnList = e.listStyleVarn;
            bLoadByStyleDetails = true;
            txtStyle.Text = e.Style;
            txtVarn.Text = e.Varn;
            StyleVarnIndex = 0;

            txtVarn_LostFocus(this, new RoutedEventArgs());
            MainGrid.Refresh();
        }

        private void MainGrid_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F7)
            {
                RollsdbRollsMRec RollsMRec;
                ParamsWindow Params = null;

                switch (MainGrid.CurrentColumn.Index)
                {
                    case 1: // Part
                        Params = new ParamsWindow(4);
                        Params.Owner = ContainerWindow;

                        if ((bool)Params.ShowDialog())
                        {
                            listcostMatsRec[MainGrid.CurrentRow.Index].CTMatPart = Params.Type;
                            listcostMatsRec[MainGrid.CurrentRow.Index].MatPartDesc = Params.TypeDesc;
                        }
                        break;
                    case 2: // Materail
                        Params = new ParamsWindow(1);
                        Params.Owner = ContainerWindow;

                        if ((bool)Params.ShowDialog())
                        {
                            listcostMatsRec[MainGrid.CurrentRow.Index].CTMatType = Params.Type;
                            listcostMatsRec[MainGrid.CurrentRow.Index].MatTypeDesc = Params.TypeDesc;
                        }
                        break;
                    case 4: // Code
                        CostnMaterialDetailsWindow MaterialDetails = new CostnMaterialDetailsWindow((short)listcostMatsRec[MainGrid.CurrentRow.Index].CTMatType);
                        MaterialDetails.Owner = ContainerWindow;
                        if ((bool)MaterialDetails.ShowDialog())
                        {
                            listcostMatsRec[MainGrid.CurrentRow.Index].CTMatType = (short?)MaterialDetails.MatType;
                            listcostMatsRec[MainGrid.CurrentRow.Index].CTMatCode = MaterialDetails.MatCode.Trim();
                            listcostMatsRec[MainGrid.CurrentRow.Index].MatCodeDesc = MaterialDetails.MatDescr;
                            listcostMatsRec[MainGrid.CurrentRow.Index].CTMatColr = (short?)MaterialDetails.MatColr;
                            listcostMatsRec[MainGrid.CurrentRow.Index].MatColrDesc = MaterialDetails.MatColrDesc;
                            listcostMatsRec[MainGrid.CurrentRow.Index].CTMatSupl = (short?)MaterialDetails.MatSupl;
                            listcostMatsRec[MainGrid.CurrentRow.Index].CTMatWhse = (short?)MaterialDetails.MatWhse;
                            listcostMatsRec[MainGrid.CurrentRow.Index].CTMatLocn = (short?)MaterialDetails.MatLocn;
                            listcostMatsRec[MainGrid.CurrentRow.Index].CTMatPrice = (double?)MaterialDetails.MatCost;
                            listcostMatsRec[MainGrid.CurrentRow.Index].MatSuplDesc = MaterialDetails.Supplier;

                            //if (MainGrid.CurrentCell.Column.Index != MainGrid.Columns.Count - 1)
                            //{
                                MainGrid[MainGrid.CurrentRow.Index, MainGrid.CurrentColumn.Index + 10].Presenter.Focus();
                                MainGrid[MainGrid.CurrentRow.Index, MainGrid.CurrentColumn.Index + 10].Presenter.IsSelected = true;
                                MainGrid[MainGrid.CurrentRow.Index, MainGrid.CurrentColumn.Index + 10].Presenter.IsCurrent = true;
                                MainGrid.BeginEdit(MainGrid[MainGrid.CurrentRow.Index, MainGrid.CurrentColumn.Index + 10]);
                            //}

                            LastCurrcyRec.CURCurrencyKey = MaterialDetails.CurrcyNo;
                            if (LastCurrcyRec.CURCurrencyKey == 1) LastCurrcyRec.CURBuyingRate = 0.0;
                            else LastCurrcyRec.CURBuyingRate = MaterialDetails.CurrcyBuyRate;

                            CalculateMaterialRowPrices(MainGrid.CurrentRow.Index);
                            CalculateTotalMaterialCost();
                            CalculateCosting();

                            DisplayTotalMaterialCost();
                        }
                        break;
                    case 6: // colour 
                        Params = new ParamsWindow(2);
                        Params.Owner = ContainerWindow;

                        if ((bool)Params.ShowDialog())
                        {
                            listcostMatsRec[MainGrid.CurrentRow.Index].CTMatColr = Params.Type;
                            listcostMatsRec[MainGrid.CurrentRow.Index].MatColrDesc = Params.TypeDesc;

                            // Rollsdb Information Exist
                            RollsMRec = GetRollsMRecByColour((short)listcostMatsRec[MainGrid.CurrentRow.Index].CTMatType,
                                                                        listcostMatsRec[MainGrid.CurrentRow.Index].CTMatCode, (short) Params.Type);

                            if (RollsMRec != null)
                            {
                                if (MessageBoxResult.Yes == MessageBoxEx.Show(ContainerWindow, "Material code does not exist. Enter.", "Enter New Material", MessageBoxButton.YesNo, MessageBoxImage.Question))
                                {
                                    CostnMaterialEntryWindow MaterialEntry = new CostnMaterialEntryWindow((short)listcostMatsRec[MainGrid.SelectedIndex].CTMatType, listcostMatsRec[MainGrid.SelectedIndex].MatTypeDesc, "", 0, 0, 0, 0, 0);
                                    MaterialEntry.Owner = ContainerWindow;

                                    if ((bool)MaterialEntry.ShowDialog())
                                    {
                                        listcostMatsRec[MainGrid.CurrentRow.Index].CTMatType = MaterialEntry.MaterialEntry.RMatType;
                                        listcostMatsRec[MainGrid.CurrentRow.Index].MatTypeDesc = MaterialEntry.MaterialEntry.RMatTypeDesc.Trim();
                                        listcostMatsRec[MainGrid.CurrentRow.Index].CTMatCode = MaterialEntry.MaterialEntry.RMatCode.Trim();
                                        listcostMatsRec[MainGrid.CurrentRow.Index].MatCodeDesc = MaterialEntry.MaterialEntry.RMatCodeDesc.Trim();
                                        listcostMatsRec[MainGrid.CurrentRow.Index].CTMatColr = MaterialEntry.MaterialEntry.RMatColr;
                                        listcostMatsRec[MainGrid.CurrentRow.Index].MatColrDesc = MaterialEntry.MaterialEntry.RMatColrDesc.Trim();
                                        listcostMatsRec[MainGrid.CurrentRow.Index].CTMatSupl = MaterialEntry.MaterialEntry.RMatSupl;
                                        listcostMatsRec[MainGrid.CurrentRow.Index].MatSuplDesc = MaterialEntry.MaterialEntry.RMatSuplDesc.Trim();
                                        listcostMatsRec[MainGrid.CurrentRow.Index].CTMatWhse = MaterialEntry.MaterialEntry.RMatWhse;
                                        listcostMatsRec[MainGrid.CurrentRow.Index].CTMatLocn = MaterialEntry.MaterialEntry.RMatLocn;

                                        if (MaterialEntry.MaterialEntry.OrderUnit > 0.001)
                                            listcostMatsRec[MainGrid.CurrentRow.Index].CTMatPrice = MaterialEntry.MaterialEntry.RMatCost / MaterialEntry.MaterialEntry.OrderUnit;
                                        else
                                            listcostMatsRec[MainGrid.CurrentRow.Index].CTMatPrice = MaterialEntry.MaterialEntry.RMatCost;


                                        LastCurrcyRec.CURCurrencyKey = MaterialEntry.MaterialEntry.CurrcyNo;
                                        if (LastCurrcyRec.CURCurrencyKey == 1) LastCurrcyRec.CURBuyingRate = 0.0;
                                        else LastCurrcyRec.CURBuyingRate = MaterialEntry.MaterialEntry.CurrcyBuyRate;

                                        MainGrid[MainGrid.CurrentRow.Index, MainGrid.CurrentColumn.Index + 10].Presenter.Focus();
                                        MainGrid[MainGrid.CurrentRow.Index, MainGrid.CurrentColumn.Index + 10].Presenter.IsSelected = true;
                                        MainGrid[MainGrid.CurrentRow.Index, MainGrid.CurrentColumn.Index + 10].Presenter.IsCurrent = true;
                                        MainGrid.BeginEdit(MainGrid[MainGrid.CurrentRow.Index, MainGrid.CurrentColumn.Index + 10]);

                                        CalculateMaterialRowPrices(MainGrid.CurrentRow.Index);
                                        CalculateTotalMaterialCost();
                                        CalculateCosting();

                                        DisplayTotalMaterialCost();
                                    }
                                }
                            }
                            else
                            {
                                listcostMatsRec[MainGrid.CurrentRow.Index].CTMatSupl = RollsMRec.RMatSupl;
                                listcostMatsRec[MainGrid.CurrentRow.Index].MatSuplDesc = WpfClassLibrary.WgmateDBUtilClass.GetClientName(2, (short)RollsMRec.RMatSupl, ref SQLWrite);
                                listcostMatsRec[MainGrid.CurrentRow.Index].CTMatWhse = RollsMRec.RMatWhse;
                                listcostMatsRec[MainGrid.CurrentRow.Index].CTMatLocn = RollsMRec.RMatLocn;

                                CalculateMaterialRowPrices(MainGrid.CurrentRow.Index);
                                CalculateTotalMaterialCost();
                                CalculateCosting();

                                DisplayTotalMaterialCost();
                            }
                        }
                        break;
                    case 8: // Supplier
                        CostnClientStyleDetailsWindow ClientDetails = new CostnClientStyleDetailsWindow(2);
                        ClientDetails.Owner = ContainerWindow;

                        if ((bool)ClientDetails.ShowDialog())
                        {
                            listcostMatsRec[MainGrid.CurrentRow.Index].CTMatSupl = short.Parse(ClientDetails.ClientNo);
                            listcostMatsRec[MainGrid.CurrentRow.Index].MatSuplDesc = ClientDetails.ClientName;

                            // Rollsdb Information Exist
                            RollsMRec = GetRollsMRecBySuplr((short) listcostMatsRec[MainGrid.CurrentRow.Index].CTMatType, 
                                                                        listcostMatsRec[MainGrid.CurrentRow.Index].CTMatCode,
                                                                            (short)listcostMatsRec[MainGrid.CurrentRow.Index].CTMatColr, 
                                                                                (short) listcostMatsRec[MainGrid.CurrentRow.Index].CTMatSupl);

                            if (RollsMRec == null)
                            {
                                if (MessageBoxResult.Yes == MessageBoxEx.Show(ContainerWindow, "Material code does not exist. Enter.", "Enter New Material", MessageBoxButton.YesNo, MessageBoxImage.Question))
                                {
                                    CostnMaterialEntryWindow MaterialEntry = new CostnMaterialEntryWindow((short)listcostMatsRec[MainGrid.SelectedIndex].CTMatType, listcostMatsRec[MainGrid.SelectedIndex].MatTypeDesc, "", 0, 0, 0, 0, 0);
                                    MaterialEntry.Owner = ContainerWindow;

                                    if ((bool)MaterialEntry.ShowDialog())
                                    {
                                        listcostMatsRec[MainGrid.CurrentRow.Index].CTMatType = MaterialEntry.MaterialEntry.RMatType;
                                        listcostMatsRec[MainGrid.CurrentRow.Index].MatTypeDesc = MaterialEntry.MaterialEntry.RMatTypeDesc.Trim();
                                        listcostMatsRec[MainGrid.CurrentRow.Index].CTMatCode = MaterialEntry.MaterialEntry.RMatCode.Trim();
                                        listcostMatsRec[MainGrid.CurrentRow.Index].MatCodeDesc = MaterialEntry.MaterialEntry.RMatCodeDesc.Trim();
                                        listcostMatsRec[MainGrid.CurrentRow.Index].CTMatColr = MaterialEntry.MaterialEntry.RMatColr;
                                        listcostMatsRec[MainGrid.CurrentRow.Index].MatColrDesc = MaterialEntry.MaterialEntry.RMatColrDesc.Trim();
                                        listcostMatsRec[MainGrid.CurrentRow.Index].CTMatSupl = MaterialEntry.MaterialEntry.RMatSupl;
                                        listcostMatsRec[MainGrid.CurrentRow.Index].MatSuplDesc = MaterialEntry.MaterialEntry.RMatSuplDesc.Trim();
                                        listcostMatsRec[MainGrid.CurrentRow.Index].CTMatWhse = MaterialEntry.MaterialEntry.RMatWhse;
                                        listcostMatsRec[MainGrid.CurrentRow.Index].CTMatLocn = MaterialEntry.MaterialEntry.RMatLocn;

                                        if (MaterialEntry.MaterialEntry.OrderUnit > 0.001)
                                            listcostMatsRec[MainGrid.CurrentRow.Index].CTMatPrice = MaterialEntry.MaterialEntry.RMatCost / MaterialEntry.MaterialEntry.OrderUnit;
                                        else
                                            listcostMatsRec[MainGrid.CurrentRow.Index].CTMatPrice = MaterialEntry.MaterialEntry.RMatCost;


                                        LastCurrcyRec.CURCurrencyKey = MaterialEntry.MaterialEntry.CurrcyNo;
                                        if (LastCurrcyRec.CURCurrencyKey == 1) LastCurrcyRec.CURBuyingRate = 0.0;
                                        else LastCurrcyRec.CURBuyingRate = MaterialEntry.MaterialEntry.CurrcyBuyRate;

                                        MainGrid[MainGrid.CurrentRow.Index, MainGrid.CurrentColumn.Index + 10].Presenter.Focus();
                                        MainGrid[MainGrid.CurrentRow.Index, MainGrid.CurrentColumn.Index + 10].Presenter.IsSelected = true;
                                        MainGrid[MainGrid.CurrentRow.Index, MainGrid.CurrentColumn.Index + 10].Presenter.IsCurrent = true;
                                        MainGrid.BeginEdit(MainGrid[MainGrid.CurrentRow.Index, MainGrid.CurrentColumn.Index + 10]);

                                        CalculateMaterialRowPrices(MainGrid.CurrentRow.Index);
                                        CalculateTotalMaterialCost();
                                        CalculateCosting();

                                        DisplayTotalMaterialCost();
                                    }
                                }
                            }
                            else
                            {
                                listcostMatsRec[MainGrid.CurrentRow.Index].CTMatWhse = RollsMRec.RMatWhse;
                                listcostMatsRec[MainGrid.CurrentRow.Index].CTMatLocn = RollsMRec.RMatLocn;

                                CalculateMaterialRowPrices(MainGrid.CurrentRow.Index);
                                CalculateTotalMaterialCost();
                                CalculateCosting();

                                DisplayTotalMaterialCost();
                            }
                        }
                        break;
                } // end switch;
            }
        }
        private void MainGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
            {
                e.Handled = true;
                int currRow, currCol;
                currRow = MainGrid.CurrentRow.Index;
                currCol = MainGrid.CurrentColumn.Index;
                if (currCol == MainGrid.Columns.Count - 1)
                {
                    currCol = -1;
                    currRow++;
                }

                if (currRow == MainGrid.Rows.Count - 1)
                {
                    currRow = 0;
                }

                if (currCol != -1)
                {
                    for (int i = currCol; i <= MainGrid.Columns.Count - 2; i++)
                    {
                        if (MainGrid.Columns[i + 1].Visibility != System.Windows.Visibility.Collapsed)
                        {
                            currCol = i + 1;
                            break;
                        }
                        else
                        {
                            currCol = currCol + 1;
                        }
                    }
                }
                else
                {
                    currCol = 0;
                }

                MainGrid.Selection.Clear();
                MainGrid.Selection.Add(MainGrid[currRow, currCol]);
                MainGrid.CurrentCell = MainGrid[currRow, currCol];
                MainGrid.ScrollIntoView(currRow, currCol);
            }
        }
        private void MainGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Only allow pop-ups in New & Edit Mode
            if (viewState != AppViewState.NEW && viewState != AppViewState.EDIT) return;

            short MatType = 0;
            RollsdbRollsMRec RollsMRec;
            ParamsWindow Params = null;

                switch (MainGrid.CurrentColumn.Index)
                {
                    case 1: // Part
                        Params = new ParamsWindow(4);
                        Params.Owner = ContainerWindow;

                        if ((bool)Params.ShowDialog())
                        {
                            listcostMatsRec[MainGrid.CurrentRow.Index].CTMatPart = Params.Type;
                            listcostMatsRec[MainGrid.CurrentRow.Index].MatPartDesc = Params.TypeDesc;
                        }
                        break;
                    case 2: // Materail
                        Params = new ParamsWindow(1);
                        Params.Owner = ContainerWindow;

                        if ((bool)Params.ShowDialog())
                        {
                            listcostMatsRec[MainGrid.CurrentRow.Index].CTMatType = Params.Type;
                            listcostMatsRec[MainGrid.CurrentRow.Index].MatTypeDesc = Params.TypeDesc;
                        }
                        break;
                    case 4: // Code
                        if (listcostMatsRec[MainGrid.CurrentRow.Index].CTMatType.HasValue)
                            MatType = (short)listcostMatsRec[MainGrid.CurrentRow.Index].CTMatType;
                        else
                            MatType = 1;
                        CostnMaterialDetailsWindow MaterialDetails = new CostnMaterialDetailsWindow(MatType, listcostMatsRec[MainGrid.CurrentRow.Index].CTMatCode,
                                                                                                        listcostMatsRec[MainGrid.CurrentRow.Index].MatCodeDesc,
                                                                                                        listcostMatsRec[MainGrid.CurrentRow.Index].CTMatColr,
                                                                                                        listcostMatsRec[MainGrid.CurrentRow.Index].MatColrDesc, 0,
                                                                                                        listcostMatsRec[MainGrid.CurrentRow.Index].CTMatSupl,
                                                                                                        listcostMatsRec[MainGrid.CurrentRow.Index].MatSuplDesc,
                                                                                                        listcostMatsRec[MainGrid.CurrentRow.Index].CTMatWhse, 
                                                                                                        listcostMatsRec[MainGrid.CurrentRow.Index].CTMatLocn);
                        MaterialDetails.Owner = ContainerWindow;
                        if ((bool)MaterialDetails.ShowDialog())
                        {
                            listcostMatsRec[MainGrid.CurrentRow.Index].CTMatType = (short?)MaterialDetails.MatType;
                            listcostMatsRec[MainGrid.CurrentRow.Index].CTMatCode = MaterialDetails.MatCode.Trim();
                            listcostMatsRec[MainGrid.CurrentRow.Index].MatCodeDesc = MaterialDetails.MatDescr.Trim();
                            listcostMatsRec[MainGrid.CurrentRow.Index].CTMatColr = (short?)MaterialDetails.MatColr;
                            listcostMatsRec[MainGrid.CurrentRow.Index].MatColrDesc = MaterialDetails.MatColrDesc.Trim();
                            listcostMatsRec[MainGrid.CurrentRow.Index].CTMatSupl = (short?)MaterialDetails.MatSupl;
                            listcostMatsRec[MainGrid.CurrentRow.Index].CTMatWhse = (short?)MaterialDetails.MatWhse;
                            listcostMatsRec[MainGrid.CurrentRow.Index].CTMatLocn = (short?)MaterialDetails.MatLocn;
                            listcostMatsRec[MainGrid.CurrentRow.Index].CTMatPrice = (double?)MaterialDetails.MatCost;
                            listcostMatsRec[MainGrid.CurrentRow.Index].MatSuplDesc = MaterialDetails.Supplier;

                            if (MainGrid.CurrentCell.Column.Index != MainGrid.Columns.Count - 1)
                            {
                                MainGrid[MainGrid.CurrentRow.Index, MainGrid.CurrentColumn.Index + 10].Presenter.Focus();
                                MainGrid[MainGrid.CurrentRow.Index, MainGrid.CurrentColumn.Index + 10].Presenter.IsSelected = true;
                                MainGrid[MainGrid.CurrentRow.Index, MainGrid.CurrentColumn.Index + 10].Presenter.IsCurrent = true;
                                MainGrid.BeginEdit(MainGrid[MainGrid.CurrentRow.Index, MainGrid.CurrentColumn.Index + 10]);
                            }

                            LastCurrcyRec.CURCurrencyKey = MaterialDetails.CurrcyNo;
                            if (LastCurrcyRec.CURCurrencyKey == 1) LastCurrcyRec.CURBuyingRate = 0.0;
                            else LastCurrcyRec.CURBuyingRate = MaterialDetails.CurrcyBuyRate;

                            CalculateMaterialRowPrices(MainGrid.CurrentRow.Index);
                            CalculateTotalMaterialCost();
                            CalculateCosting();

                            DisplayTotalMaterialCost();
                        }
                        break;
                    case 6: // colour 
                        Params = new ParamsWindow(2);
                        Params.Owner = ContainerWindow;

                        if ((bool)Params.ShowDialog())
                        {
                            listcostMatsRec[MainGrid.CurrentRow.Index].CTMatColr = Params.Type;
                            listcostMatsRec[MainGrid.CurrentRow.Index].MatColrDesc = Params.TypeDesc;

                            // Rollsdb Information Exist
                            RollsMRec = GetRollsMRecByColour((short) listcostMatsRec[MainGrid.CurrentRow.Index].CTMatType,
                                                                listcostMatsRec[MainGrid.CurrentRow.Index].CTMatCode, Params.Type);

                            if (RollsMRec == null)
                            {
                                if (MessageBoxResult.Yes == MessageBoxEx.Show(ContainerWindow, "Material code does not exist. Enter.", "Enter New Material", MessageBoxButton.YesNo, MessageBoxImage.Question))
                                {
                                    CostnMaterialEntryWindow MaterialEntry = new CostnMaterialEntryWindow((short)listcostMatsRec[MainGrid.SelectedIndex].CTMatType, listcostMatsRec[MainGrid.SelectedIndex].MatTypeDesc, "", 0, 0, 0, 0, 0);
                                    MaterialEntry.Owner = ContainerWindow;

                                    if ((bool)MaterialEntry.ShowDialog())
                                    {
                                        listcostMatsRec[MainGrid.CurrentRow.Index].CTMatType = MaterialEntry.MaterialEntry.RMatType;
                                        listcostMatsRec[MainGrid.CurrentRow.Index].MatTypeDesc = MaterialEntry.MaterialEntry.RMatTypeDesc.Trim();
                                        listcostMatsRec[MainGrid.CurrentRow.Index].CTMatCode = MaterialEntry.MaterialEntry.RMatCode.Trim();
                                        listcostMatsRec[MainGrid.CurrentRow.Index].MatCodeDesc = MaterialEntry.MaterialEntry.RMatCodeDesc.Trim();
                                        listcostMatsRec[MainGrid.CurrentRow.Index].CTMatColr = MaterialEntry.MaterialEntry.RMatColr;
                                        listcostMatsRec[MainGrid.CurrentRow.Index].MatColrDesc = MaterialEntry.MaterialEntry.RMatColrDesc.Trim();
                                        listcostMatsRec[MainGrid.CurrentRow.Index].CTMatSupl = MaterialEntry.MaterialEntry.RMatSupl;
                                        listcostMatsRec[MainGrid.CurrentRow.Index].MatSuplDesc = MaterialEntry.MaterialEntry.RMatSuplDesc.Trim();
                                        listcostMatsRec[MainGrid.CurrentRow.Index].CTMatWhse = MaterialEntry.MaterialEntry.RMatWhse;
                                        listcostMatsRec[MainGrid.CurrentRow.Index].CTMatLocn = MaterialEntry.MaterialEntry.RMatLocn;

                                        if (MaterialEntry.MaterialEntry.OrderUnit > 0.001)
                                            listcostMatsRec[MainGrid.CurrentRow.Index].CTMatPrice = MaterialEntry.MaterialEntry.RMatCost / MaterialEntry.MaterialEntry.OrderUnit;
                                        else
                                            listcostMatsRec[MainGrid.CurrentRow.Index].CTMatPrice = MaterialEntry.MaterialEntry.RMatCost;


                                        LastCurrcyRec.CURCurrencyKey = MaterialEntry.MaterialEntry.CurrcyNo;
                                        if (LastCurrcyRec.CURCurrencyKey == 1) LastCurrcyRec.CURBuyingRate = 0.0;
                                        else LastCurrcyRec.CURBuyingRate = MaterialEntry.MaterialEntry.CurrcyBuyRate;

                                        MainGrid[MainGrid.CurrentRow.Index, MainGrid.CurrentColumn.Index + 10].Presenter.Focus();
                                        MainGrid[MainGrid.CurrentRow.Index, MainGrid.CurrentColumn.Index + 10].Presenter.IsSelected = true;
                                        MainGrid[MainGrid.CurrentRow.Index, MainGrid.CurrentColumn.Index + 10].Presenter.IsCurrent = true;
                                        MainGrid.BeginEdit(MainGrid[MainGrid.CurrentRow.Index, MainGrid.CurrentColumn.Index + 10]);

                                        CalculateMaterialRowPrices(MainGrid.CurrentRow.Index);
                                        CalculateTotalMaterialCost();
                                        CalculateCosting();

                                        DisplayTotalMaterialCost();
                                    }
                                }
                            }
                            else
                            {
                                listcostMatsRec[MainGrid.CurrentRow.Index].CTMatSupl = RollsMRec.RMatSupl;
                                listcostMatsRec[MainGrid.CurrentRow.Index].MatSuplDesc = WpfClassLibrary.WgmateDBUtilClass.GetClientName(2, (short)RollsMRec.RMatSupl, ref SQLWrite);
                                listcostMatsRec[MainGrid.CurrentRow.Index].CTMatWhse = RollsMRec.RMatWhse;
                                listcostMatsRec[MainGrid.CurrentRow.Index].CTMatLocn = RollsMRec.RMatLocn;

                                CalculateMaterialRowPrices(MainGrid.CurrentRow.Index);
                                CalculateTotalMaterialCost();
                                CalculateCosting();

                                DisplayTotalMaterialCost();
                            }
                        }
                        break;
                    case 8: // Supplier
                        CostnClientStyleDetailsWindow ClientDetails = new CostnClientStyleDetailsWindow(2);
                        ClientDetails.Owner = ContainerWindow;

                        if ((bool)ClientDetails.ShowDialog())
                        {
                            listcostMatsRec[MainGrid.CurrentRow.Index].CTMatSupl = short.Parse(ClientDetails.ClientNo);
                            listcostMatsRec[MainGrid.CurrentRow.Index].MatSuplDesc = ClientDetails.ClientName;

                            // Rollsdb Information Exist
                            RollsMRec = GetRollsMRecBySuplr((short)listcostMatsRec[MainGrid.CurrentRow.Index].CTMatType,
                                                                        listcostMatsRec[MainGrid.CurrentRow.Index].CTMatCode,
                                                                            (short)listcostMatsRec[MainGrid.CurrentRow.Index].CTMatColr,
                                                                                (short)listcostMatsRec[MainGrid.CurrentRow.Index].CTMatSupl);

                            if (RollsMRec == null)
                            {
                                if (MessageBoxResult.Yes == MessageBoxEx.Show(ContainerWindow, "Material code does not exist. Enter.", "Enter New Material", MessageBoxButton.YesNo, MessageBoxImage.Question))
                                {
                                    CostnMaterialEntryWindow MaterialEntry = new CostnMaterialEntryWindow((short)listcostMatsRec[MainGrid.SelectedIndex].CTMatType, listcostMatsRec[MainGrid.SelectedIndex].MatTypeDesc, "", 0, 0, 0, 0, 0);
                                    MaterialEntry.Owner = ContainerWindow;

                                    if ((bool)MaterialEntry.ShowDialog())
                                    {
                                        listcostMatsRec[MainGrid.CurrentRow.Index].CTMatType = MaterialEntry.MaterialEntry.RMatType;
                                        listcostMatsRec[MainGrid.CurrentRow.Index].MatTypeDesc = MaterialEntry.MaterialEntry.RMatTypeDesc.Trim();
                                        listcostMatsRec[MainGrid.CurrentRow.Index].CTMatCode = MaterialEntry.MaterialEntry.RMatCode.Trim();
                                        listcostMatsRec[MainGrid.CurrentRow.Index].MatCodeDesc = MaterialEntry.MaterialEntry.RMatCodeDesc.Trim();
                                        listcostMatsRec[MainGrid.CurrentRow.Index].CTMatColr = MaterialEntry.MaterialEntry.RMatColr;
                                        listcostMatsRec[MainGrid.CurrentRow.Index].MatColrDesc = MaterialEntry.MaterialEntry.RMatColrDesc.Trim();
                                        listcostMatsRec[MainGrid.CurrentRow.Index].CTMatSupl = MaterialEntry.MaterialEntry.RMatSupl;
                                        listcostMatsRec[MainGrid.CurrentRow.Index].MatSuplDesc = MaterialEntry.MaterialEntry.RMatSuplDesc.Trim();
                                        listcostMatsRec[MainGrid.CurrentRow.Index].CTMatWhse = MaterialEntry.MaterialEntry.RMatWhse;
                                        listcostMatsRec[MainGrid.CurrentRow.Index].CTMatLocn = MaterialEntry.MaterialEntry.RMatLocn;

                                        if (MaterialEntry.MaterialEntry.OrderUnit > 0.001)
                                            listcostMatsRec[MainGrid.CurrentRow.Index].CTMatPrice = MaterialEntry.MaterialEntry.RMatCost / MaterialEntry.MaterialEntry.OrderUnit;
                                        else
                                            listcostMatsRec[MainGrid.CurrentRow.Index].CTMatPrice = MaterialEntry.MaterialEntry.RMatCost;


                                        LastCurrcyRec.CURCurrencyKey = MaterialEntry.MaterialEntry.CurrcyNo;
                                        if (LastCurrcyRec.CURCurrencyKey == 1) LastCurrcyRec.CURBuyingRate = 0.0;
                                        else LastCurrcyRec.CURBuyingRate = MaterialEntry.MaterialEntry.CurrcyBuyRate;

                                        MainGrid[MainGrid.CurrentRow.Index, MainGrid.CurrentColumn.Index + 10].Presenter.Focus();
                                        MainGrid[MainGrid.CurrentRow.Index, MainGrid.CurrentColumn.Index + 10].Presenter.IsSelected = true;
                                        MainGrid[MainGrid.CurrentRow.Index, MainGrid.CurrentColumn.Index + 10].Presenter.IsCurrent = true;
                                        MainGrid.BeginEdit(MainGrid[MainGrid.CurrentRow.Index, MainGrid.CurrentColumn.Index + 10]);

                                        CalculateMaterialRowPrices(MainGrid.CurrentRow.Index);
                                        CalculateTotalMaterialCost();
                                        CalculateCosting();

                                        DisplayTotalMaterialCost();
                                    }
                                }
                            }
                            else
                            {
                                listcostMatsRec[MainGrid.CurrentRow.Index].CTMatWhse = RollsMRec.RMatWhse;
                                listcostMatsRec[MainGrid.CurrentRow.Index].CTMatLocn = RollsMRec.RMatLocn;

                                CalculateMaterialRowPrices(MainGrid.CurrentRow.Index);
                                CalculateTotalMaterialCost();
                                CalculateCosting();

                                DisplayTotalMaterialCost();
                            }
                        }
                        break;
                } // end switch;
        }
        private void MainGrid_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Only allow pop-ups in New & Edit Mode
            if (viewState != AppViewState.NEW && viewState != AppViewState.EDIT) return;

            if (e.RightButton == MouseButtonState.Pressed)
            {
                RollsdbRollsMRec RollsMRec;
                ParamsWindow Params = null;

                switch (MainGrid.CurrentColumn.Index)
                {
                    case 1: // Part
                        Params = new ParamsWindow(4);
                        Params.Owner = ContainerWindow;

                        if ((bool)Params.ShowDialog())
                        {
                            listcostMatsRec[MainGrid.CurrentRow.Index].CTMatPart = Params.Type;
                            listcostMatsRec[MainGrid.CurrentRow.Index].MatPartDesc = Params.TypeDesc;
                        }
                        break;
                    case 2: // Materail
                        Params = new ParamsWindow(1);
                        Params.Owner = ContainerWindow;

                        if ((bool)Params.ShowDialog())
                        {
                            listcostMatsRec[MainGrid.CurrentRow.Index].CTMatType = Params.Type;
                            listcostMatsRec[MainGrid.CurrentRow.Index].MatTypeDesc = Params.TypeDesc;
                        }
                        break;
                    case 4: // Code
                        CostnMaterialDetailsWindow MaterialDetails = new CostnMaterialDetailsWindow();
                        MaterialDetails.Owner = ContainerWindow;
                        if ((bool)MaterialDetails.ShowDialog())
                        {
                            listcostMatsRec[MainGrid.CurrentRow.Index].CTMatType = (short?)MaterialDetails.MatType;
                            listcostMatsRec[MainGrid.CurrentRow.Index].CTMatCode = MaterialDetails.MatCode.Trim();
                            listcostMatsRec[MainGrid.CurrentRow.Index].MatCodeDesc = MaterialDetails.MatDescr.Trim();
                            listcostMatsRec[MainGrid.CurrentRow.Index].CTMatColr = (short?)MaterialDetails.MatColr;
                            listcostMatsRec[MainGrid.CurrentRow.Index].MatColrDesc = MaterialDetails.MatColrDesc.Trim();
                            listcostMatsRec[MainGrid.CurrentRow.Index].CTMatSupl = (short?)MaterialDetails.MatSupl;
                            listcostMatsRec[MainGrid.CurrentRow.Index].CTMatWhse = (short?)MaterialDetails.MatWhse;
                            listcostMatsRec[MainGrid.CurrentRow.Index].CTMatLocn = (short?)MaterialDetails.MatLocn;
                            listcostMatsRec[MainGrid.CurrentRow.Index].CTMatPrice = (double?)MaterialDetails.MatCost;
                            listcostMatsRec[MainGrid.CurrentRow.Index].MatSuplDesc = MaterialDetails.Supplier;

                            //if (MainGrid.CurrentCell.Column.Index != MainGrid.Columns.Count - 1)
                            //{
                                MainGrid[MainGrid.CurrentRow.Index, MainGrid.CurrentColumn.Index + 10].Presenter.Focus();
                                MainGrid[MainGrid.CurrentRow.Index, MainGrid.CurrentColumn.Index + 10].Presenter.IsSelected = true;
                                MainGrid[MainGrid.CurrentRow.Index, MainGrid.CurrentColumn.Index + 10].Presenter.IsCurrent = true;
                                MainGrid.BeginEdit(MainGrid[MainGrid.CurrentRow.Index, MainGrid.CurrentColumn.Index + 10]);
                            //}

                            LastCurrcyRec.CURCurrencyKey = MaterialDetails.CurrcyNo;
                            if (LastCurrcyRec.CURCurrencyKey == 1) LastCurrcyRec.CURBuyingRate = 0.0;
                            else LastCurrcyRec.CURBuyingRate = MaterialDetails.CurrcyBuyRate;

                            CalculateMaterialRowPrices(MainGrid.CurrentRow.Index);
                            CalculateTotalMaterialCost();
                            CalculateCosting();

                            DisplayTotalMaterialCost();
                        }
                        break;
                    case 6: // colour 
                        Params = new ParamsWindow(2);
                        Params.Owner = ContainerWindow;

                        if ((bool)Params.ShowDialog())
                        {
                            listcostMatsRec[MainGrid.CurrentRow.Index].CTMatColr = Params.Type;
                            listcostMatsRec[MainGrid.CurrentRow.Index].MatColrDesc = Params.TypeDesc;

                            // Rollsdb Information Exist
                            RollsMRec = GetRollsMRecByColour((short)listcostMatsRec[MainGrid.CurrentRow.Index].CTMatType,
                                                                        listcostMatsRec[MainGrid.CurrentRow.Index].CTMatCode, Params.Type);

                            if (RollsMRec == null)
                            {
                                if (MessageBoxResult.Yes == MessageBoxEx.Show(ContainerWindow, "Material code does not exist. Enter.", "Enter New Material", MessageBoxButton.YesNo, MessageBoxImage.Question))
                                {
                                    CostnMaterialEntryWindow MaterialEntry = new CostnMaterialEntryWindow((short)listcostMatsRec[MainGrid.SelectedIndex].CTMatType, listcostMatsRec[MainGrid.SelectedIndex].MatTypeDesc, "", 0, 0, 0, 0, 0);
                                    MaterialEntry.Owner = ContainerWindow;

                                    if ((bool)MaterialEntry.ShowDialog())
                                    {
                                        listcostMatsRec[MainGrid.CurrentRow.Index].CTMatType = MaterialEntry.MaterialEntry.RMatType;
                                        listcostMatsRec[MainGrid.CurrentRow.Index].MatTypeDesc = MaterialEntry.MaterialEntry.RMatTypeDesc.Trim();
                                        listcostMatsRec[MainGrid.CurrentRow.Index].CTMatCode = MaterialEntry.MaterialEntry.RMatCode.Trim();
                                        listcostMatsRec[MainGrid.CurrentRow.Index].MatCodeDesc = MaterialEntry.MaterialEntry.RMatCodeDesc.Trim();
                                        listcostMatsRec[MainGrid.CurrentRow.Index].CTMatColr = MaterialEntry.MaterialEntry.RMatColr;
                                        listcostMatsRec[MainGrid.CurrentRow.Index].MatColrDesc = MaterialEntry.MaterialEntry.RMatColrDesc.Trim();
                                        listcostMatsRec[MainGrid.CurrentRow.Index].CTMatSupl = MaterialEntry.MaterialEntry.RMatSupl;
                                        listcostMatsRec[MainGrid.CurrentRow.Index].MatSuplDesc = MaterialEntry.MaterialEntry.RMatSuplDesc.Trim();
                                        listcostMatsRec[MainGrid.CurrentRow.Index].CTMatWhse = MaterialEntry.MaterialEntry.RMatWhse;
                                        listcostMatsRec[MainGrid.CurrentRow.Index].CTMatLocn = MaterialEntry.MaterialEntry.RMatLocn;

                                        if (MaterialEntry.MaterialEntry.OrderUnit > 0.001)
                                            listcostMatsRec[MainGrid.CurrentRow.Index].CTMatPrice = MaterialEntry.MaterialEntry.RMatCost / MaterialEntry.MaterialEntry.OrderUnit;
                                        else
                                            listcostMatsRec[MainGrid.CurrentRow.Index].CTMatPrice = MaterialEntry.MaterialEntry.RMatCost;


                                        LastCurrcyRec.CURCurrencyKey = MaterialEntry.MaterialEntry.CurrcyNo;
                                        if (LastCurrcyRec.CURCurrencyKey == 1) LastCurrcyRec.CURBuyingRate = 0.0;
                                        else LastCurrcyRec.CURBuyingRate = MaterialEntry.MaterialEntry.CurrcyBuyRate;

                                        MainGrid[MainGrid.CurrentRow.Index, MainGrid.CurrentColumn.Index + 10].Presenter.Focus();
                                        MainGrid[MainGrid.CurrentRow.Index, MainGrid.CurrentColumn.Index + 10].Presenter.IsSelected = true;
                                        MainGrid[MainGrid.CurrentRow.Index, MainGrid.CurrentColumn.Index + 10].Presenter.IsCurrent = true;
                                        MainGrid.BeginEdit(MainGrid[MainGrid.CurrentRow.Index, MainGrid.CurrentColumn.Index + 10]);

                                        CalculateMaterialRowPrices(MainGrid.CurrentRow.Index);
                                        CalculateTotalMaterialCost();
                                        CalculateCosting();

                                        DisplayTotalMaterialCost();
                                    }
                                }
                            }
                            else
                            {
                                listcostMatsRec[MainGrid.CurrentRow.Index].CTMatSupl = RollsMRec.RMatSupl;
                                listcostMatsRec[MainGrid.CurrentRow.Index].MatSuplDesc = WpfClassLibrary.WgmateDBUtilClass.GetClientName(2, (short)RollsMRec.RMatSupl, ref SQLWrite);
                                listcostMatsRec[MainGrid.CurrentRow.Index].CTMatWhse = RollsMRec.RMatWhse;
                                listcostMatsRec[MainGrid.CurrentRow.Index].CTMatLocn = RollsMRec.RMatLocn;

                                CalculateMaterialRowPrices(MainGrid.CurrentRow.Index);
                                CalculateTotalMaterialCost();
                                CalculateCosting();

                                DisplayTotalMaterialCost();
                            }
                        }
                        break;
                    case 8: // Supplier
                        CostnClientStyleDetailsWindow ClientDetails = new CostnClientStyleDetailsWindow(2);
                        ClientDetails.Owner = ContainerWindow;

                        if ((bool)ClientDetails.ShowDialog())
                        {
                            listcostMatsRec[MainGrid.CurrentRow.Index].CTMatSupl = short.Parse(ClientDetails.ClientNo);
                            listcostMatsRec[MainGrid.CurrentRow.Index].MatSuplDesc = ClientDetails.ClientName;

                            // Rollsdb Information Exist
                            RollsMRec = GetRollsMRecBySuplr((short)listcostMatsRec[MainGrid.CurrentRow.Index].CTMatType,
                                                                        listcostMatsRec[MainGrid.CurrentRow.Index].CTMatCode,
                                                                            (short)listcostMatsRec[MainGrid.CurrentRow.Index].CTMatColr,
                                                                                (short)listcostMatsRec[MainGrid.CurrentRow.Index].CTMatSupl);

                            if (RollsMRec == null)
                            {
                                if (MessageBoxResult.Yes == MessageBoxEx.Show(ContainerWindow, "Material code does not exist. Enter.", "Enter New Material", MessageBoxButton.YesNo, MessageBoxImage.Question))
                                {
                                    CostnMaterialEntryWindow MaterialEntry = new CostnMaterialEntryWindow((short)listcostMatsRec[MainGrid.SelectedIndex].CTMatType, listcostMatsRec[MainGrid.SelectedIndex].MatTypeDesc, "", 0, 0, 0, 0, 0);
                                    MaterialEntry.Owner = ContainerWindow;

                                    if ((bool)MaterialEntry.ShowDialog())
                                    {
                                        listcostMatsRec[MainGrid.CurrentRow.Index].CTMatType = MaterialEntry.MaterialEntry.RMatType;
                                        listcostMatsRec[MainGrid.CurrentRow.Index].MatTypeDesc = MaterialEntry.MaterialEntry.RMatTypeDesc.Trim();
                                        listcostMatsRec[MainGrid.CurrentRow.Index].CTMatCode = MaterialEntry.MaterialEntry.RMatCode.Trim();
                                        listcostMatsRec[MainGrid.CurrentRow.Index].MatCodeDesc = MaterialEntry.MaterialEntry.RMatCodeDesc.Trim();
                                        listcostMatsRec[MainGrid.CurrentRow.Index].CTMatColr = MaterialEntry.MaterialEntry.RMatColr;
                                        listcostMatsRec[MainGrid.CurrentRow.Index].MatColrDesc = MaterialEntry.MaterialEntry.RMatColrDesc.Trim();
                                        listcostMatsRec[MainGrid.CurrentRow.Index].CTMatSupl = MaterialEntry.MaterialEntry.RMatSupl;
                                        listcostMatsRec[MainGrid.CurrentRow.Index].MatSuplDesc = MaterialEntry.MaterialEntry.RMatSuplDesc.Trim();
                                        listcostMatsRec[MainGrid.CurrentRow.Index].CTMatWhse = MaterialEntry.MaterialEntry.RMatWhse;
                                        listcostMatsRec[MainGrid.CurrentRow.Index].CTMatLocn = MaterialEntry.MaterialEntry.RMatLocn;

                                        if (MaterialEntry.MaterialEntry.OrderUnit > 0.001)
                                            listcostMatsRec[MainGrid.CurrentRow.Index].CTMatPrice = MaterialEntry.MaterialEntry.RMatCost / MaterialEntry.MaterialEntry.OrderUnit;
                                        else
                                            listcostMatsRec[MainGrid.CurrentRow.Index].CTMatPrice = MaterialEntry.MaterialEntry.RMatCost;


                                        LastCurrcyRec.CURCurrencyKey = MaterialEntry.MaterialEntry.CurrcyNo;
                                        if (LastCurrcyRec.CURCurrencyKey == 1) LastCurrcyRec.CURBuyingRate = 0.0;
                                        else LastCurrcyRec.CURBuyingRate = MaterialEntry.MaterialEntry.CurrcyBuyRate;

                                        MainGrid[MainGrid.CurrentRow.Index, MainGrid.CurrentColumn.Index + 10].Presenter.Focus();
                                        MainGrid[MainGrid.CurrentRow.Index, MainGrid.CurrentColumn.Index + 10].Presenter.IsSelected = true;
                                        MainGrid[MainGrid.CurrentRow.Index, MainGrid.CurrentColumn.Index + 10].Presenter.IsCurrent = true;
                                        MainGrid.BeginEdit(MainGrid[MainGrid.CurrentRow.Index, MainGrid.CurrentColumn.Index + 10]);

                                        CalculateMaterialRowPrices(MainGrid.CurrentRow.Index);
                                        CalculateTotalMaterialCost();
                                        CalculateCosting();

                                        DisplayTotalMaterialCost();
                                    }
                                }
                            }
                            else
                            {
                                listcostMatsRec[MainGrid.CurrentRow.Index].CTMatWhse = RollsMRec.RMatWhse;
                                listcostMatsRec[MainGrid.CurrentRow.Index].CTMatLocn = RollsMRec.RMatLocn;

                                CalculateMaterialRowPrices(MainGrid.CurrentRow.Index);
                                CalculateTotalMaterialCost();
                                CalculateCosting();

                                DisplayTotalMaterialCost();
                            }
                        }
                        break;
                } // end switch;
            }
        }
        private void MainGrid_CommittedEdit(object sender, C1.WPF.DataGrid.DataGridCellEventArgs e)
        {
            short nsValue;
            double ndValue;
            SqlParameter[] SqlParam;
            RollsdbRollsMRec RollsMRec;
            TextBlock CellTxtBlk = null;

            // Only allow pop-ups in New & Edit Mode
            if (viewState == AppViewState.NEW || viewState == AppViewState.EDIT)
            { 
                switch (e.Cell.Column.Index)
                {
                    case 1: // Parts Desciption
                        if (listcostMatsRec.Count > 0 && short.TryParse(e.Cell.Text, out nsValue))
                        {
                            listcostMatsRec[e.Cell.Row.Index].CTMatPart = nsValue;
                            listcostMatsRec[e.Cell.Row.Index].MatPartDesc = WpfClassLibrary.WgmateDBUtilClass.GetGmParamsDescr(4, nsValue, ref dtParametersRec);
                        }
                        else if (listcostMatsRec.Count > 0 && listcostMatsRec.Count <= e.Cell.Row.Index && (CellTxtBlk = e.Cell.Presenter.Content as TextBlock) != null)
                        {
                            listcostMatsRec[e.Cell.Row.Index].MatPartDesc = "";
                            listcostMatsRec[e.Cell.Row.Index].CTMatPart = 0;
                        }
                        break;
                    case 2: // Materail Desciption
                        if (listcostMatsRec.Count > 0 && short.TryParse(e.Cell.Text, out nsValue))
                        {
                            listcostMatsRec[e.Cell.Row.Index].MatTypeDesc = WpfClassLibrary.WgmateDBUtilClass.GetGmParamsDescr(1, nsValue, ref dtParametersRec);
                            if (listcostMatsRec[e.Cell.Row.Index].MatTypeDesc.Trim().Length == 0) listcostMatsRec[e.Cell.Row.Index].CTMatType = 0;
                        }
                        else listcostMatsRec[e.Cell.Row.Index].MatTypeDesc = "";
                        break;
                    case 4: // Code Description
                        if (e.Cell.Text.Trim().Length > 0 && listcostMatsRec.Count > 0 && listcostMatsRec[e.Cell.Row.Index].CTMatType > 0)
                        {
                            RollsMRec = GetRollsMRecByCode((short)listcostMatsRec[e.Cell.Row.Index].CTMatType, e.Cell.Text.Trim());

                            if (RollsMRec == null)
                            {
                                if (MessageBoxResult.Yes == MessageBoxEx.Show(ContainerWindow, "Material code does not exist. Enter.", "Enter New Material", MessageBoxButton.YesNo, MessageBoxImage.Question))
                                {
                                    CostnMaterialEntryWindow MaterialEntry = new CostnMaterialEntryWindow((short)listcostMatsRec[MainGrid.SelectedIndex].CTMatType, e.Cell.Text.Trim(), "", 0, 0, 0, 0, 0);
                                    MaterialEntry.Owner = ContainerWindow;

                                    if ((bool)MaterialEntry.ShowDialog())
                                    {
                                        listcostMatsRec[e.Cell.Row.Index].CTMatType = MaterialEntry.MaterialEntry.RMatType;
                                        listcostMatsRec[e.Cell.Row.Index].MatTypeDesc = MaterialEntry.MaterialEntry.RMatTypeDesc.Trim();
                                        listcostMatsRec[e.Cell.Row.Index].CTMatCode = MaterialEntry.MaterialEntry.RMatCode.Trim();
                                        listcostMatsRec[e.Cell.Row.Index].MatCodeDesc = MaterialEntry.MaterialEntry.RMatCodeDesc.Trim();
                                        listcostMatsRec[e.Cell.Row.Index].CTMatColr = MaterialEntry.MaterialEntry.RMatColr;
                                        listcostMatsRec[e.Cell.Row.Index].MatColrDesc = MaterialEntry.MaterialEntry.RMatColrDesc.Trim();
                                        listcostMatsRec[e.Cell.Row.Index].CTMatSupl = MaterialEntry.MaterialEntry.RMatSupl;
                                        listcostMatsRec[e.Cell.Row.Index].MatSuplDesc = MaterialEntry.MaterialEntry.RMatSuplDesc.Trim();
                                        listcostMatsRec[e.Cell.Row.Index].CTMatWhse = MaterialEntry.MaterialEntry.RMatWhse;
                                        listcostMatsRec[e.Cell.Row.Index].CTMatLocn = MaterialEntry.MaterialEntry.RMatLocn;

                                        if (MaterialEntry.MaterialEntry.OrderUnit > 0.001)
                                            listcostMatsRec[e.Cell.Row.Index].CTMatPrice = MaterialEntry.MaterialEntry.RMatCost / MaterialEntry.MaterialEntry.OrderUnit;
                                        else
                                            listcostMatsRec[e.Cell.Row.Index].CTMatPrice = MaterialEntry.MaterialEntry.RMatCost;
                                        

                                        LastCurrcyRec.CURCurrencyKey = MaterialEntry.MaterialEntry.CurrcyNo;
                                        if (LastCurrcyRec.CURCurrencyKey == 1) LastCurrcyRec.CURBuyingRate = 0.0;
                                        else LastCurrcyRec.CURBuyingRate = MaterialEntry.MaterialEntry.CurrcyBuyRate;

                                        //MainGrid[e.Cell.Row.Index, e.Cell.Column.Index + 10].Presenter.Focus();
                                        //MainGrid[e.Cell.Row.Index, e.Cell.Column.Index + 10].Presenter.IsSelected = true;
                                        //MainGrid[e.Cell.Row.Index, e.Cell.Column.Index + 10].Presenter.IsCurrent = true;
                                        //MainGrid.BeginEdit(e.Cell.Row.Index, e.Cell.Column.Index + 10);

                                        CalculateMaterialRowPrices(e.Cell.Row.Index);
                                        DisplayTotalMaterialCost();
                                        CalculateTotalMaterialCost();
                                        CalculateCosting();
                                    }
                                }
                            }
                            else
                            {
                                listcostMatsRec[e.Cell.Row.Index].MatCodeDesc = RollsMRec.RMatDescr;
                                listcostMatsRec[e.Cell.Row.Index].CTMatSupl = RollsMRec.RMatSupl;
                                listcostMatsRec[e.Cell.Row.Index].MatSuplDesc = WpfClassLibrary.WgmateDBUtilClass.GetClientName(2, (short)RollsMRec.RMatSupl, ref SQLWrite);
                                listcostMatsRec[e.Cell.Row.Index].CTMatColr = RollsMRec.RMatColr;
                                listcostMatsRec[e.Cell.Row.Index].MatColrDesc = WpfClassLibrary.WgmateDBUtilClass.GetGmParamsDescr(2, (short)RollsMRec.RMatColr, ref dtParametersRec);
                                listcostMatsRec[e.Cell.Row.Index].CTMatWhse = RollsMRec.RMatWhse;
                                listcostMatsRec[e.Cell.Row.Index].CTMatLocn = RollsMRec.RMatLocn;

                                CalculateMaterialRowPrices(MainGrid.CurrentRow.Index);
                                DisplayTotalMaterialCost();
                                CalculateTotalMaterialCost();
                                CalculateCosting();

                                //MainGrid[e.Cell.Row.Index, e.Cell.Column.Index + 10].Presenter.Focus();
                                //MainGrid[e.Cell.Row.Index, e.Cell.Column.Index + 10].Presenter.IsSelected = true;
                                //MainGrid[e.Cell.Row.Index, e.Cell.Column.Index + 10].Presenter.IsCurrent = true;
                                //MainGrid.BeginEdit(e.Cell.Row.Index, e.Cell.Column.Index + 10);
                            }
                        }
                        else
                        {
                            // Event is Triggered if Exit App
                            // Check that Material row Exist
                            if (listcostMatsRec.Count > 0)
                            {
                                if ((CellTxtBlk = e.Cell.Presenter.Content as TextBlock) != null)
                                {
                                    (e.Cell.Presenter.Content as TextBlock).Text = "";
                                    listcostMatsRec[e.Cell.Row.Index].MatCodeDesc = "";
                                }
                            }
                        }
                        break;
                    case 6: // Colour
                        if (listcostMatsRec.Count > 0 && short.TryParse(e.Cell.Text, out nsValue))
                        {
                            listcostMatsRec[e.Cell.Row.Index].MatColrDesc = WpfClassLibrary.WgmateDBUtilClass.GetGmParamsDescr(2, nsValue, ref dtParametersRec);

                            // Rollsdb Information Exist
                            RollsMRec = GetRollsMRecByColour((short)listcostMatsRec[MainGrid.CurrentRow.Index].CTMatType,
                                                                        listcostMatsRec[MainGrid.CurrentRow.Index].CTMatCode, nsValue);

                            if (RollsMRec == null)
                            {
                                if (MessageBoxResult.Yes == MessageBoxEx.Show(ContainerWindow, "Material code does not exist. Enter.", "Enter New Material", MessageBoxButton.YesNo, MessageBoxImage.Question))
                                {
                                    CostnMaterialEntryWindow MaterialEntry = new CostnMaterialEntryWindow((short)listcostMatsRec[MainGrid.SelectedIndex].CTMatType, listcostMatsRec[MainGrid.SelectedIndex].MatTypeDesc, "", 0, 0, 0, 0, 0);
                                    MaterialEntry.Owner = ContainerWindow;

                                    if ((bool)MaterialEntry.ShowDialog())
                                    {
                                        listcostMatsRec[e.Cell.Row.Index].CTMatType = MaterialEntry.MaterialEntry.RMatType;
                                        listcostMatsRec[e.Cell.Row.Index].MatTypeDesc = MaterialEntry.MaterialEntry.RMatTypeDesc.Trim();
                                        listcostMatsRec[e.Cell.Row.Index].CTMatCode = MaterialEntry.MaterialEntry.RMatCode.Trim();
                                        listcostMatsRec[e.Cell.Row.Index].MatCodeDesc = MaterialEntry.MaterialEntry.RMatCodeDesc.Trim();
                                        listcostMatsRec[e.Cell.Row.Index].CTMatColr = MaterialEntry.MaterialEntry.RMatColr;
                                        listcostMatsRec[e.Cell.Row.Index].MatColrDesc = MaterialEntry.MaterialEntry.RMatColrDesc.Trim();
                                        listcostMatsRec[e.Cell.Row.Index].CTMatSupl = MaterialEntry.MaterialEntry.RMatSupl;
                                        listcostMatsRec[e.Cell.Row.Index].MatSuplDesc = MaterialEntry.MaterialEntry.RMatSuplDesc.Trim();
                                        listcostMatsRec[e.Cell.Row.Index].CTMatWhse = MaterialEntry.MaterialEntry.RMatWhse;
                                        listcostMatsRec[e.Cell.Row.Index].CTMatLocn = MaterialEntry.MaterialEntry.RMatLocn;

                                        if (MaterialEntry.MaterialEntry.OrderUnit > 0.001)
                                            listcostMatsRec[e.Cell.Row.Index].CTMatPrice = MaterialEntry.MaterialEntry.RMatCost / MaterialEntry.MaterialEntry.OrderUnit;
                                        else
                                            listcostMatsRec[e.Cell.Row.Index].CTMatPrice = MaterialEntry.MaterialEntry.RMatCost;


                                        LastCurrcyRec.CURCurrencyKey = MaterialEntry.MaterialEntry.CurrcyNo;
                                        if (LastCurrcyRec.CURCurrencyKey == 1) LastCurrcyRec.CURBuyingRate = 0.0;
                                        else LastCurrcyRec.CURBuyingRate = MaterialEntry.MaterialEntry.CurrcyBuyRate;

                                        MainGrid[e.Cell.Row.Index, MainGrid.CurrentColumn.Index + 10].Presenter.Focus();
                                        MainGrid[e.Cell.Row.Index, MainGrid.CurrentColumn.Index + 10].Presenter.IsSelected = true;
                                        MainGrid[e.Cell.Row.Index, MainGrid.CurrentColumn.Index + 10].Presenter.IsCurrent = true;
                                        MainGrid.BeginEdit(MainGrid[e.Cell.Row.Index, MainGrid.CurrentColumn.Index + 10]);

                                        CalculateMaterialRowPrices(e.Cell.Row.Index);
                                        DisplayTotalMaterialCost();
                                        CalculateTotalMaterialCost();
                                        CalculateCosting();
                                    }
                                }
                            }
                            else
                            {
                                listcostMatsRec[e.Cell.Row.Index].MatCodeDesc = RollsMRec.RMatDescr;
                                listcostMatsRec[e.Cell.Row.Index].CTMatSupl = RollsMRec.RMatSupl;
                                listcostMatsRec[e.Cell.Row.Index].MatSuplDesc = WpfClassLibrary.WgmateDBUtilClass.GetClientName(2, (short)RollsMRec.RMatSupl, ref SQLWrite);
                                listcostMatsRec[e.Cell.Row.Index].CTMatWhse = RollsMRec.RMatWhse;
                                listcostMatsRec[e.Cell.Row.Index].CTMatLocn = RollsMRec.RMatLocn;

                                CalculateMaterialRowPrices(e.Cell.Row.Index);
                                DisplayTotalMaterialCost();
                                CalculateTotalMaterialCost();
                                CalculateCosting();
                            }
                        }
                        else
                        {
                            // Event is Triggered if Exit App
                            // Check that Material row Exist
                            if (listcostMatsRec.Count > 0)
                            {
                                if (listcostMatsRec.Count <= e.Cell.Row.Index && (CellTxtBlk = e.Cell.Presenter.Content as TextBlock) != null)
                                {
                                    (e.Cell.Presenter.Content as TextBlock).Text = "";
                                    listcostMatsRec[e.Cell.Row.Index].MatColrDesc = "";
                                }
                            }
                        }
                        break;
                    case 9: // Supplier
                        if (listcostMatsRec.Count > 0 && short.TryParse(e.Cell.Text, out nsValue))
                        {
                            listcostMatsRec[e.Cell.Row.Index].CTMatSupl = nsValue;
                            WadmdirWDIRMainRec WDIRMainRec = WpfClassLibrary.WgmateDBUtilClass.GetClientRecord(2, (int)nsValue, ref SQLWrite);

                            if (WDIRMainRec != null)
                            {
                                listcostMatsRec[e.Cell.Row.Index].MatSuplDesc = WDIRMainRec.WDIRClientName.Trim();

                                // Rollsdb Information Exist
                                RollsMRec = GetRollsMRecBySuplr((short)listcostMatsRec[e.Cell.Row.Index].CTMatType,
                                                                            listcostMatsRec[e.Cell.Row.Index].CTMatCode,
                                                                                (short)listcostMatsRec[e.Cell.Row.Index].CTMatColr,
                                                                                    (short)listcostMatsRec[e.Cell.Row.Index].CTMatSupl);

                                if (RollsMRec == null)
                                {
                                    if (MessageBoxResult.Yes == MessageBoxEx.Show(ContainerWindow, "Material code does not exist. Enter.", "Enter New Material", MessageBoxButton.YesNo, MessageBoxImage.Question))
                                    {
                                        CostnMaterialEntryWindow MaterialEntry = new CostnMaterialEntryWindow((short)listcostMatsRec[MainGrid.SelectedIndex].CTMatType, listcostMatsRec[MainGrid.SelectedIndex].MatTypeDesc, "", 0, 0, 0, 0, 0);
                                        MaterialEntry.Owner = ContainerWindow;

                                        if ((bool)MaterialEntry.ShowDialog())
                                        {
                                            listcostMatsRec[e.Cell.Row.Index].CTMatType = MaterialEntry.MaterialEntry.RMatType;
                                            listcostMatsRec[e.Cell.Row.Index].MatTypeDesc = MaterialEntry.MaterialEntry.RMatTypeDesc.Trim();
                                            listcostMatsRec[e.Cell.Row.Index].CTMatCode = MaterialEntry.MaterialEntry.RMatCode.Trim();
                                            listcostMatsRec[e.Cell.Row.Index].MatCodeDesc = MaterialEntry.MaterialEntry.RMatCodeDesc.Trim();
                                            listcostMatsRec[e.Cell.Row.Index].CTMatColr = MaterialEntry.MaterialEntry.RMatColr;
                                            listcostMatsRec[e.Cell.Row.Index].MatColrDesc = MaterialEntry.MaterialEntry.RMatColrDesc.Trim();
                                            listcostMatsRec[e.Cell.Row.Index].CTMatSupl = MaterialEntry.MaterialEntry.RMatSupl;
                                            listcostMatsRec[e.Cell.Row.Index].MatSuplDesc = MaterialEntry.MaterialEntry.RMatSuplDesc.Trim();
                                            listcostMatsRec[e.Cell.Row.Index].CTMatWhse = MaterialEntry.MaterialEntry.RMatWhse;
                                            listcostMatsRec[e.Cell.Row.Index].CTMatLocn = MaterialEntry.MaterialEntry.RMatLocn;

                                            if (MaterialEntry.MaterialEntry.OrderUnit > 0.001)
                                                listcostMatsRec[e.Cell.Row.Index].CTMatPrice = MaterialEntry.MaterialEntry.RMatCost / MaterialEntry.MaterialEntry.OrderUnit;
                                            else
                                                listcostMatsRec[e.Cell.Row.Index].CTMatPrice = MaterialEntry.MaterialEntry.RMatCost;


                                            LastCurrcyRec.CURCurrencyKey = MaterialEntry.MaterialEntry.CurrcyNo;
                                            if (LastCurrcyRec.CURCurrencyKey == 1) LastCurrcyRec.CURBuyingRate = 0.0;
                                            else LastCurrcyRec.CURBuyingRate = MaterialEntry.MaterialEntry.CurrcyBuyRate;

                                            MainGrid[e.Cell.Row.Index, MainGrid.CurrentColumn.Index + 10].Presenter.Focus();
                                            MainGrid[e.Cell.Row.Index, MainGrid.CurrentColumn.Index + 10].Presenter.IsSelected = true;
                                            MainGrid[e.Cell.Row.Index, MainGrid.CurrentColumn.Index + 10].Presenter.IsCurrent = true;
                                            MainGrid.BeginEdit(MainGrid[e.Cell.Row.Index, MainGrid.CurrentColumn.Index + 10]);

                                            CalculateMaterialRowPrices(e.Cell.Row.Index);
                                            DisplayTotalMaterialCost();
                                            CalculateTotalMaterialCost();
                                            CalculateCosting();
                                        }
                                    } // end if
                                } 
                                else
                                {
                                    listcostMatsRec[e.Cell.Row.Index].MatCodeDesc = RollsMRec.RMatDescr;
                                    listcostMatsRec[e.Cell.Row.Index].CTMatSupl = RollsMRec.RMatSupl;
                                    listcostMatsRec[e.Cell.Row.Index].MatSuplDesc = WpfClassLibrary.WgmateDBUtilClass.GetClientName(2, (short)RollsMRec.RMatSupl, ref SQLWrite);
                                    listcostMatsRec[e.Cell.Row.Index].CTMatWhse = RollsMRec.RMatWhse;
                                    listcostMatsRec[e.Cell.Row.Index].CTMatLocn = RollsMRec.RMatLocn;

                                    CalculateMaterialRowPrices(e.Cell.Row.Index);
                                    DisplayTotalMaterialCost();
                                    CalculateTotalMaterialCost();
                                    CalculateCosting();
                                }
                            }
                        }
                        else
                        {
                            // Event is Triggered if Exit App
                            // Check that Material row Exist
                            if (listcostMatsRec.Count > 0)
                            {
                                if (listcostMatsRec.Count <= e.Cell.Row.Index && (CellTxtBlk = e.Cell.Presenter.Content as TextBlock) != null)
                                { 
                                    (e.Cell.Presenter.Content as TextBlock).Text = "";
                                    listcostMatsRec[e.Cell.Row.Index].MatSuplDesc = "";
                                }
                            }
                        }
                        break;
                    case 13: // Price
                        if (listcostMatsRec.Count > 0 && double.TryParse(e.Cell.Text, out ndValue))
                        {
                            (e.Cell.Presenter.Content as TextBlock).Text = string.Format("{0:0.000}", ndValue);
                            listcostMatsRec[e.Cell.Row.Index].CTMatPrice = ndValue;

                            #region // MatPriceModified
                            SQLWrite.SQLWriteCommand("[dbo].[IsMatPriceModified]", SQLWriteClass.SqlCmdType.PROCEDURE);
                            SqlParam = new SqlParameter[8];

                            SqlParam[0] = DependancyService.SQLParameter("@MatType",
                                    "System.Int16", DependancyService.ParamDirection.Input, (object)listcostMatsRec[e.Cell.Row.Index].CTMatType);

                            SqlParam[1] = DependancyService.SQLParameter("@MatCode",
                                   "System.String", DependancyService.ParamDirection.Input, (object)listcostMatsRec[e.Cell.Row.Index].CTMatCode);

                            SqlParam[2] = DependancyService.SQLParameter("@MatColr",
                                    "System.Int16", DependancyService.ParamDirection.Input, (object)listcostMatsRec[e.Cell.Row.Index].CTMatColr);

                            SqlParam[3] = DependancyService.SQLParameter("@MatColr1",
                                    "System.Int16", DependancyService.ParamDirection.Input, (object)0);

                            SqlParam[4] = DependancyService.SQLParameter("@MatSupl",
                                    "System.Int16", DependancyService.ParamDirection.Input, (object)listcostMatsRec[e.Cell.Row.Index].CTMatSupl);

                            SqlParam[5] = DependancyService.SQLParameter("@MatWhse",
                                    "System.Int16", DependancyService.ParamDirection.Input, (object)listcostMatsRec[e.Cell.Row.Index].CTMatWhse);

                            SqlParam[6] = DependancyService.SQLParameter("@MatLocn",
                                    "System.Int16", DependancyService.ParamDirection.Input, (object)listcostMatsRec[e.Cell.Row.Index].CTMatLocn);

                            SqlParam[7] = DependancyService.SQLParameter("@NewPrice",
                                    "System.String", DependancyService.ParamDirection.Input, (object)listcostMatsRec[e.Cell.Row.Index].CTMatPrice);

                            listcostMatsRec[e.Cell.Row.Index].MatComsumtionA = (bool)SQLWrite.ExecuteQueryFunction(SqlParam);
                            #endregion


                            if (listcostMatsRec.Count > 0 && listcostMatsRec[e.Cell.Row.Index].CTCosting.HasValue && (double)listcostMatsRec[e.Cell.Row.Index].CTCosting > 0.0001)
                            {
                                ndValue = ((double)listcostMatsRec[e.Cell.Row.Index].CTCosting * ndValue);
                                listcostMatsRec[e.Cell.Row.Index].Cost = ndValue;

                                DisplayTotalMaterialCost();
                                CalculateTotalMaterialCost();
                                CalculateCosting();
                            }
                            /*else
                            {
                                if ((CellTxtBlk = e.Cell.Presenter.Content as TextBlock) != null)
                                    (e.Cell.Presenter.Content as TextBlock).Text = "";
                            }*/
                        }
                        else
                        { 
                            // Event is Triggered if Exit App
                            // Check that Material row Exist
                            if (listcostMatsRec.Count > 0)
                            {
                                if ((CellTxtBlk = e.Cell.Presenter.Content as TextBlock) != null)
                                {
                                    (e.Cell.Presenter.Content as TextBlock).Text = "";
                                    listcostMatsRec[e.Cell.Row.Index].CTMatPrice = 0;
                                }
                            }
                        }
                        break;
                    case 14: // Costing
                        if (listcostMatsRec.Count > 0 && listcostMatsRec[e.Cell.Row.Index].CTMatPrice.HasValue && double.TryParse(e.Cell.Text, out ndValue))
                        {
                            listcostMatsRec[e.Cell.Row.Index].Cost = ((double)listcostMatsRec[e.Cell.Row.Index].CTMatPrice * ndValue);
                            (e.Cell.Presenter.Content as TextBlock).Text = string.Format("{0:0.000}", listcostMatsRec[e.Cell.Row.Index].CTCosting);
                            (MainGrid[e.Cell.Row.Index, e.Cell.Column.Index + 1].Presenter.Content as TextBlock).Text = string.Format("{0:0.000}", listcostMatsRec[e.Cell.Row.Index].Cost);

                            DisplayTotalMaterialCost();
                            CalculateTotalMaterialCost();
                            CalculateCosting();
                        }
                        else
                        {
                            // Event is Triggered if Exit App
                            // Check that Material row Exist
                            if (listcostMatsRec.Count > 0)
                            {
                                if ((CellTxtBlk = e.Cell.Presenter.Content as TextBlock) != null)
                                {
                                    (e.Cell.Presenter.Content as TextBlock).Text = "";
                                    listcostMatsRec[e.Cell.Row.Index].CTCosting = 0;
                                }
                            }
                        }
                        break;
                    case 15: // Cost
                        if (double.TryParse(e.Cell.Text, out ndValue))
                            (e.Cell.Presenter.Content as TextBlock).Text = string.Format("{0:0.000}", ndValue);
                        else
                            (e.Cell.Presenter.Content as TextBlock).Text = "";

                        DisplayTotalMaterialCost();
                        break;
                } // end switch
            } // end for
        }
        private void MainGrid_LoadedCellPresenter(object sender, C1.WPF.DataGrid.DataGridCellEventArgs e)
        {
            if (e.Cell.Column.HeaderPresenter != null)
            {
                switch (e.Cell.Column.Header.ToString())
                {
                    case " ":
                    case "Wh":
                    case "Lc":
                    case "Price":
                    case "Costing":
                    case "Cost":
                        e.Cell.Column.HeaderPresenter.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Right;
                        break;
                    default:
                        break;
                }
            }

            // double ndValue;

            if (viewState == AppViewState.VIEW || viewState == AppViewState.EDIT)
            {
                switch(e.Cell.Column.Index)
                {
                    case 3: // Material
                        if (listcostMatsRec[e.Cell.Row.Index].CustomerPreferencesA)
                            e.Cell.Presenter.Background = new SolidColorBrush(Colors.LightBlue);
                        else
                            e.Cell.Presenter.Background = MainGrid.Background;
                        break;
                    case 4: // Material code
                        if (listcostMatsRec[e.Cell.Row.Index].CTSpareFlag2 == 1)
                            e.Cell.Presenter.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00FF00"));
                        else
                            e.Cell.Presenter.Background = MainGrid.Background;

                        if (e.Cell.Presenter.Content.GetType() == typeof(TextBlock))
                            ToolTipService.SetToolTip(e.Cell.Presenter, e.Cell.Value);
                        break;
                    case 5: // Material Description
                        if (listcostMatsRec[e.Cell.Row.Index].CTSpareFlag3 == 1)
                            e.Cell.Presenter.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF81C0"));
                        else
                            e.Cell.Presenter.Background = MainGrid.Background;

                        if (e.Cell.Presenter.Content.GetType() == typeof(TextBlock))
                            ToolTipService.SetToolTip(e.Cell.Presenter, e.Cell.Value);
                        break;
                    case 7: // Colour
                        if (listcostMatsRec[e.Cell.Row.Index].ColourCombinationsA)
                        {
                            e.Cell.Presenter.Background = new SolidColorBrush(Colors.Blue);
                            e.Cell.Presenter.Foreground = new SolidColorBrush(Colors.White);
                        }
                        else
                        {
                            switch (listcostMatsRec[e.Cell.Row.Index].CTSpareFlag1)
                            {
                                case 0:
                                    e.Cell.Presenter.Foreground = MainGrid.Foreground;
                                    e.Cell.Presenter.Background = MainGrid.Background;
                                    break;
                                case 1: // DTM
                                    e.Cell.Presenter.Background = new SolidColorBrush(Colors.Yellow);
                                    e.Cell.Presenter.Foreground = new SolidColorBrush(Colors.Black);
                                    break;
                                case 2: // Non Stock
                                    e.Cell.Presenter.Background = new SolidColorBrush(Colors.Brown);
                                    e.Cell.Presenter.Foreground = new SolidColorBrush(Colors.White);
                                    break;
                                default:
                                    e.Cell.Presenter.Foreground = MainGrid.Foreground;
                                    e.Cell.Presenter.Background = MainGrid.Background;
                                    break;
                            } // end switch
                        }
                        if (e.Cell.Presenter.Content.GetType() == typeof(TextBlock))
                            ToolTipService.SetToolTip(e.Cell.Presenter, e.Cell.Value);
                        break;
                    case 9: // Supplier
                        if (listcostMatsRec[e.Cell.Row.Index].CurrcyNo > 1)
                        {
                            e.Cell.Presenter.Background = new SolidColorBrush(Colors.Orange);
                            e.Cell.Presenter.Foreground = new SolidColorBrush(Colors.White);
                        }
                        else
                        {
                            e.Cell.Presenter.Foreground = MainGrid.Foreground;
                            e.Cell.Presenter.Background = MainGrid.Background;
                        }

                        if (e.Cell.Presenter.Content.GetType() == typeof(TextBlock))
                            ToolTipService.SetToolTip(e.Cell.Presenter, e.Cell.Value);
                        break;
                    case 13: // Price
                        /*if ((e.Cell.Presenter.Content as TextBlock) != null)
                        {
                            if (double.TryParse(e.Cell.Text, out ndValue))
                                (e.Cell.Presenter.Content as TextBlock).Text = string.Format("{0:0.000}", ndValue);
                            else
                                (e.Cell.Presenter.Content as TextBlock).Text = "";
                        }*/

                        if (listcostMatsRec[e.Cell.Row.Index].MatPriceModifiedA)
                            e.Cell.Presenter.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00FFFF"));
                        else
                        {
                            e.Cell.Presenter.Foreground = MainGrid.Foreground;
                            e.Cell.Presenter.Background = MainGrid.Background;
                        }
                        break;
                    case 14: // Costing
                        /*if ((e.Cell.Presenter.Content as TextBlock) != null)
                        { 
                            if (double.TryParse(e.Cell.Text, out ndValue))
                                (e.Cell.Presenter.Content as TextBlock).Text = string.Format("{0:0.000}", ndValue);
                            else
                                (e.Cell.Presenter.Content as TextBlock).Text = "";
                        }*/

                        if (listcostMatsRec[e.Cell.Row.Index].MatComsumtionA)
                        {
                            e.Cell.Presenter.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF0080"));
                            e.Cell.Presenter.Foreground = new SolidColorBrush(Colors.White);
                        }
                        else
                        {
                            e.Cell.Presenter.Foreground = MainGrid.Foreground;
                            e.Cell.Presenter.Background = MainGrid.Background;
                        }
                        break;
                    case 15: // Cost
                        /*if ((e.Cell.Presenter.Content as TextBlock) != null)
                        {
                            if (double.TryParse(e.Cell.Text, out ndValue))
                                (e.Cell.Presenter.Content as TextBlock).Text = string.Format("{0:0.000}", ndValue);
                            else
                                (e.Cell.Presenter.Content as TextBlock).Text = "";
                        }*/

                        if (listcostMRec[0].CTCountryCode == 0)
                            listcostMatsRec[e.Cell.Row.Index].CTSpareFlag4 = 0;
			            else
			            {
                            if (listcostMatsRec[e.Cell.Row.Index].CountryCode == listcostMRec[0].CTCountryCode)
                                listcostMatsRec[e.Cell.Row.Index].CTSpareFlag4 = 1;
				            else
                                listcostMatsRec[e.Cell.Row.Index].CTSpareFlag4 = 0;
			            }

                        if (listcostMatsRec[e.Cell.Row.Index].CTSpareFlag4 == 1  && listcostMatsRec[e.Cell.Row.Index].CTMatType > 0)
			            {
                            e.Cell.Presenter.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF8080FF"));
                            e.Cell.Presenter.Foreground = new SolidColorBrush(Colors.White);
			            }
			            else
			            {
                            e.Cell.Presenter.Foreground = MainGrid.Foreground;
                            e.Cell.Presenter.Background = MainGrid.Background;
			            }
                        break;
                    default:
                        e.Cell.Presenter.Foreground = MainGrid.Foreground;
                        e.Cell.Presenter.Background = MainGrid.Background;
                        break;
                } // end switch
            }
        }
        #endregion

        #region // First Column
        private void txtDutyPerc_LostFocus(object sender, RoutedEventArgs e)
        {
            double ndValue = 0.0;

            if (double.TryParse(txtDutyPerc.Text, out ndValue))
                listcostMRec[0].CTDutyPer =  ndValue;
            else
                listcostMRec[0].CTDutyPer = 0.0;

            CalculateCosting();

            DisplayTotalMaterialCost();
        }
        private void txtContinPer_LostFocus(object sender, RoutedEventArgs e)
        {
            double ndValue = 0.0;

            if (double.TryParse(txtContinPer.Text, out ndValue))
                listcostMRec[0].CTContinPer = ndValue;
            else
                listcostMRec[0].CTContinPer = 0.0;

            CalculateCosting();

            DisplayTotalMaterialCost();
        }
        private void txtMarkUpPerc_LostFocus(object sender, RoutedEventArgs e)
        {
            double ndValue = 0.0;

            if (double.TryParse(txtMarkUpPerc.Text, out ndValue))
            {
                listcostMRec[0].CTProfMarPer1 = ndValue;

                if (Math.Round((double)listcostMRec[0].CTProfMarPer1, 2) != Math.Round(PrevCTProfMarPer1, 2))
                {
                    // Commission
                    if (listcostMRec[0].CTDate2.HasValue && listcostMRec[0].CTDate2 == DateTime.Parse(WpfClassLibrary.GlobalConstants.NULL_DATE))
                    {
                        listcostMRec[0].CTProfSelFlag = 1;

                        listcostMRec[0].SelPricePlusVat = (double)listcostMRec[0].CTSelPrice1 + (((double)listcostMRec[0].CTSelPrice1 * ndTaxRates[0]) / 100.0);
                        PrevCTProfMarPer1 = (double)listcostMRec[0].CTProfMarPer1;

                        // **************************************************************************************************************

                        listcostMRec[0].CTProfMar1 = (listcostMRec[0].CTTotManCost * listcostMRec[0].CTProfMarPer1) / 100.0;
                        PrevCTProfMar1 = (double)listcostMRec[0].CTProfMar1;

                        listcostMRec[0].CTSelPrice1 = listcostMRec[0].CTTotManCost + listcostMRec[0].CTProfMar1;
                        listcostMRec[0].SelPricePlusVat = (double)listcostMRec[0].CTSelPrice1 + (((double)listcostMRec[0].CTSelPrice1 * ndTaxRates[0]) / 100.0);

                        // **************************************************************************************************************

                        listcostMRec[0].CTProfMar1F = listcostMRec[0].CTProfMar1 * LastCurrcyRec.CURBuyingRate;
                        listcostMRec[0].CTVatRate = listcostMRec[0].CTTotManCostF + listcostMRec[0].CTProfMar1F;

                        if (listcostMRec[0].CTSelPrice1 > 0.0001)
                            listcostMRec[0].ProfMarginPerc = ((double)listcostMRec[0].CTProfMar1 / (double)listcostMRec[0].CTSelPrice1) * 100.0;
                        else
                            listcostMRec[0].ProfMarginPerc = 0.0;

                        ColorOneOfTrioButton();
                    }
                }
                else
                {
                    listcostMRec[0].CTCostMaking = ((100 - listcostMRec[0].CTProfMarPer1) * listcostMRec[0].CTSelPrice1) / 100.0;
                    CalculateCosting();

                    DisplayTotalMaterialCost();
                }
            }
            else
                listcostMRec[0].CTContinPer = 0.0;
        }
        private void txtMargin_LostFocus(object sender, RoutedEventArgs e)
        {
            double ndValue = 0;

            if (double.TryParse(txtMargin.Text, out ndValue))
                listcostMRec[0].ProfMarginPerc = ndValue;
            else
                listcostMRec[0].ProfMarginPerc = 0.0;

            if (listcostMRec[0].ProfMarginPerc != 100.0)
                listcostMRec[0].CTProfMar1 = (listcostMRec[0].CTTotManCost * listcostMRec[0].ProfMarginPerc) / (100.00 - listcostMRec[0].ProfMarginPerc);
        }
        private void txtOverHeadsPer_LostFocus(object sender, RoutedEventArgs e)
        {
            double ndValue = 0.0;

            if (double.TryParse(txtOverHeadsPer.Text, out ndValue))
                listcostMRec[0].CTOverHeadsPer = ndValue;
            else
                listcostMRec[0].CTOverHeadsPer = 0.0;

            CalculateCosting();

            DisplayTotalMaterialCost();
        }
        #endregion

        #region // Second Column
        private void txtDuty_LostFocus(object sender, RoutedEventArgs e)
        {
            double ndValue = 0.0;

            if (double.TryParse(txtDuty.Text, out ndValue))
                listcostMRec[0].CTDuty = ndValue;
            else
                listcostMRec[0].CTDuty = 0.0;

            CalculateCosting();

            DisplayTotalMaterialCost();
        }
        private void txtContin_LostFocus(object sender, RoutedEventArgs e)
        {
            double ndValue = 0.0;

            if (double.TryParse(txtContin.Text, out ndValue))
                listcostMRec[0].CTContin = ndValue;
            else
                listcostMRec[0].CTContin = 0.0;

            CalculateCosting();

            DisplayTotalMaterialCost();
        }
        private void txtMarkUp_LostFocus(object sender, RoutedEventArgs e)
        {
            double ndValue = 0.0;

            if (double.TryParse(txtMarkUp.Text, out ndValue))
                listcostMRec[0].CTProfMar1 = ndValue;
            else
                listcostMRec[0].CTProfMar1 = 0.0;

            if (Math.Round((double)listcostMRec[0].CTProfMar1, 2) != Math.Round(PrevCTProfMar1, 2))
            {
                listcostMRec[0].CTProfSelFlag = 2;
                listcostMRec[0].SelPricePlusVat = (double)listcostMRec[0].CTSelPrice1 + (((double)listcostMRec[0].CTSelPrice1 * ndTaxRates[0]) / 100.0);

                PrevCTProfMar1 = (double)listcostMRec[0].CTProfMar1;

                // Commission
                if (listcostMRec[0].CTDate2.HasValue && listcostMRec[0].CTDate2 != default(DateTime))
                {
                    if (listcostMRec[0].CTTotManCost > 0.0)
                        listcostMRec[0].CTProfMarPer1 = (listcostMRec[0].CTProfMar1 * 100.0) / listcostMRec[0].CTTotManCost;
                    else
                        listcostMRec[0].CTProfMarPer1 = 0.0;
                }

                PrevCTProfMarPer1 = listcostMRec[0].ProfMarginPerc;
                listcostMRec[0].CTSelPrice1 = listcostMRec[0].CTTotManCost + listcostMRec[0].CTProfMar1;

                PrevCTSelPrice1 = (double)listcostMRec[0].CTSelPrice1;
                listcostMRec[0].SelPricePlusVat = (double)listcostMRec[0].CTSelPrice1 + (((double)listcostMRec[0].CTSelPrice1 * ndTaxRates[0]) / 100.0);

                if (listcostMRec[0].CTSelPrice1 > 0.0001)
                    listcostMRec[0].ProfMarginPerc = ((double)listcostMRec[0].CTProfMar1 / (double)listcostMRec[0].CTSelPrice1) * 100.0;
                else
                    listcostMRec[0].ProfMarginPerc = 0.0;

                listcostMRec[0].CTProfMar1F = listcostMRec[0].CTProfMar1 * LastCurrcyRec.CURBuyingRate;
                listcostMRec[0].CTVatRate = listcostMRec[0].CTTotManCostF + listcostMRec[0].CTProfMar1F;

                ColorOneOfTrioButton();
            }
        }
        private void txtSelPrice_LostFocus(object sender, RoutedEventArgs e)
        {
            double ndValue = 0.0;

            if (double.TryParse(txtSelPrice.Text, out ndValue))
                listcostMRec[0].CTSelPrice1 = ndValue;
            else
                listcostMRec[0].CTSelPrice1 = 0.0;

            listcostMRec[0].SelPricePlusVat = (double)listcostMRec[0].CTSelPrice1 + (((double)listcostMRec[0].CTSelPrice1 * ndTaxRates[0]) / 100.0);

            // Commission
            if (listcostMRec[0].CTDate2.HasValue && listcostMRec[0].CTDate2 == DateTime.Parse(WpfClassLibrary.GlobalConstants.NULL_DATE))
            {
                listcostMRec[0].CTProfSelFlag = 3;

                // ===============================================
                PrevCTSelPrice1 = (double)listcostMRec[0].CTSelPrice1;
                listcostMRec[0].CTProfMar1 = listcostMRec[0].CTSelPrice1 - listcostMRec[0].CTTotManCost;
                PrevCTProfMar1 = (double)listcostMRec[0].CTProfMar1;

                if (listcostMRec[0].CTTotManCost > 0.0)
                    listcostMRec[0].CTProfMarPer1 = (listcostMRec[0].CTProfMar1 * 100.0) / listcostMRec[0].CTTotManCost;
                else
                    listcostMRec[0].CTProfMarPer1 = 0.0;

                PrevCTProfMarPer1 = (double)listcostMRec[0].CTProfMarPer1;
                //-------------------------------------------------------------------------	
                //-------------------------------------------------------------------------	

                if ((double)listcostMRec[0].CTSelPrice1 > 0.0001)
                    listcostMRec[0].ProfMarginPerc = ((double)listcostMRec[0].CTProfMar1 / (double)listcostMRec[0].CTSelPrice1) * 100.0;
                else
                    listcostMRec[0].ProfMarginPerc = 0.0;

                if (listcostMRec[0].CTVatNo > 0 && LastCurrcyRec.CURBuyingRate > 0.0)
                {
                    listcostMRec[0].CTProfMar1F = listcostMRec[0].CTProfMar1 * LastCurrcyRec.CURBuyingRate;
                    listcostMRec[0].CTVatRate = listcostMRec[0].CTTotManCostF + listcostMRec[0].CTProfMar1F;
                }
                ColorOneOfTrioButton();
            }
            ReCalculateSellingPrices();
        }
        private void txtMakePrice_LostFocus(object sender, RoutedEventArgs e)
        {
            double ndValue = 0.0;

            if (double.TryParse(txtMakePrice.Text, out ndValue))
                listcostMRec[0].CTCostMaking = ndValue;
            else
                listcostMRec[0].CTCostMaking = 0.0;

            if (listcostMRec[0].CTVatNo > 0 && LastCurrcyRec.CURBuyingRate > 0.0)
                listcostMRec[0].CTCostMakingF = listcostMRec[0].CTCostMaking * LastCurrcyRec.CURBuyingRate;

            CalculateCosting();

            DisplayTotalMaterialCost();
        }
        private void txtOverHeads_LostFocus(object sender, RoutedEventArgs e)
        {
            double ndValue = 0.0;

            if (double.TryParse(txtOverHeads.Text, out ndValue))
                listcostMRec[0].CTOverHeads = ndValue;
            else
                listcostMRec[0].CTOverHeads = 0.0;

            CalculateCosting();

            DisplayTotalMaterialCost();
        }
        #endregion

        #region // Third Column
        private void txtContinF_LostFocus(object sender, RoutedEventArgs e)
        {
            double ndValue = 0.0;

            if (double.TryParse(txtContinF.Text, out ndValue))
                listcostMRec[0].CTContinF = ndValue;
            else
                listcostMRec[0].CTContinF = 0.0;

            if (listcostMRec[0].CTVatNo > 0 && LastCurrcyRec.CURBuyingRate > 0.0)
                listcostMRec[0].CTContin = listcostMRec[0].CTContinF / LastCurrcyRec.CURBuyingRate;

            CalculateCosting();

            DisplayTotalMaterialCost();
        }
        private void txtMarkUpF_LostFocus(object sender, RoutedEventArgs e)
        {
            double ndValue = 0.0;

            if (double.TryParse(txtMarkUpF.Text, out ndValue))
                listcostMRec[0].CTProfMar1F = ndValue;
            else
                listcostMRec[0].CTProfMar1F = 0.0;

            listcostMRec[0].CTVatRate = listcostMRec[0].CTTotManCostF + listcostMRec[0].CTProfMar1F;

            if (listcostMRec[0].CTVatNo > 0 && LastCurrcyRec.CURBuyingRate > 0.0)
                listcostMRec[0].CTProfMar1 = listcostMRec[0].CTProfMar1F / LastCurrcyRec.CURBuyingRate;
        }
        private void txtMakePriceF_LostFocus(object sender, RoutedEventArgs e)
        {
            if (listcostMRec[0].CTVatNo > 0 && LastCurrcyRec.CURBuyingRate > 0.0)
                listcostMRec[0].CTCostMaking = listcostMRec[0].CTCostMakingF / LastCurrcyRec.CURBuyingRate;

            CalculateCosting(false);

            DisplayTotalMaterialCost();
        }
        private void txtOverHeadsF_LostFocus(object sender, RoutedEventArgs e)
        {
            double ndValue = 0.0;

            if (double.TryParse(txtOverHeadsF.Text, out ndValue))
                listcostMRec[0].CTOverHeadsF = ndValue;
            else
                listcostMRec[0].CTOverHeadsF = 0.0;

            if (listcostMRec[0].CTVatNo > 0 && LastCurrcyRec.CURBuyingRate > 0.0)
                listcostMRec[0].CTOverHeads = listcostMRec[0].CTOverHeadsF / LastCurrcyRec.CURBuyingRate;


            CalculateCosting();

            DisplayTotalMaterialCost();
        }
        private void txtSelPrice_F_LostFocus(object sender, RoutedEventArgs e)
        {
            double ndValue = 0.0;

            if (double.TryParse(txtSelPrice_F.Text, out ndValue))
                listcostMRec[0].CTVatRate = ndValue;
            else
                listcostMRec[0].CTVatRate = 0.0;

            if (listcostMRec[0].CTVatNo > 0 && LastCurrcyRec.CURBuyingRate > 0.0)
            {
                listcostMRec[0].CTSelPrice1 = listcostMRec[0].CTVatRate / LastCurrcyRec.CURBuyingRate;
                listcostMRec[0].SelPricePlusVat = (double)listcostMRec[0].CTSelPrice1 + (((double)listcostMRec[0].CTSelPrice1 * ndTaxRates[0]) / 100.0);
            }

            txtSelPrice_LostFocus(this, new RoutedEventArgs());
        }
        #endregion

        private void txtSelPricePerc2_LostFocus(object sender, RoutedEventArgs e)
        {
            listcostMRec[0].CTSelPrice2 = (listcostMRec[0].CTTotManCost * listcostMRec[0].SelPrice2Percent / 100.0) + listcostMRec[0].CTTotManCost;
            listcostMRec[0].CTSelPricePer2 = (double)listcostMRec[0].CTSelPrice2 * LastCurrcyRec.CURBuyingRate;
        }
        private void txtDiscountPerc_LostFocus(object sender, RoutedEventArgs e)
        {
            double ndValue = 0.0;

            if (double.TryParse(txtDiscountPerc.Text, out ndValue))
            {
                txtDiscountPerc.Text = string.Format("{0:0.00}", ndValue);
                listcostMRec[0].CTDiscountPer = ndValue;
            }
            else
            {
                txtDiscountPerc.Text = "0.00";
                listcostMRec[0].CTDiscountPer = 0.0;
            }

            CalculateDiscountValue();
        }
        private void txtSelPricePerc3_LostFocus(object sender, RoutedEventArgs e)
        {
            listcostMRec[0].CTSelPrice3 = (listcostMRec[0].CTTotManCost * listcostMRec[0].SelPrice3Percent / 100.0) + listcostMRec[0].CTTotManCost;
        }
        private void txtPercSelPrice_LostFocus(object sender, RoutedEventArgs e)
        {
            listcostMRec[0].CTSelPrice2 = listcostMRec[0].SelPricePlusVat + ((listcostMRec[0].SelPricePlusVat * listcostMRec[0].SellPricePerc) / 100.00);
        }
        private void txtSelPricePerc4_LostFocus(object sender, RoutedEventArgs e)
        {
            listcostMRec[0].CTSelPrice4 = (listcostMRec[0].CTTotManCost * listcostMRec[0].SelPrice4Percent / 100.0) + listcostMRec[0].CTTotManCost;
            listcostMRec[0].CTSelPricePer4 = listcostMRec[0].CTSelPrice4 * LastCurrcyRec.CURBuyingRate;
        }
        private void txtSelPrice4H_LostFocus(object sender, RoutedEventArgs e)
        {
            if (listcostMRec[0].CTTotManCost > 0.0)
                listcostMRec[0].SelPrice4Percent = (listcostMRec[0].CTSelPrice4 - listcostMRec[0].CTTotManCost) * 100.0 / listcostMRec[0].CTTotManCost;

            listcostMRec[0].CTSelPricePer4 = listcostMRec[0].CTSelPrice3 * LastCurrcyRec.CURBuyingRate;
        }
        private void txtDiscount_LostFocus(object sender, RoutedEventArgs e)
        {
            double ndValue = 0.0;

            if (double.TryParse(txtDiscount.Text, out ndValue))
                listcostMRec[0].DiscValue = ndValue;
            else
                listcostMRec[0].DiscValue = 0.0;

            if (listcostMRec[0].CTTotManCost > 0.0001)
            {
                listcostMRec[0].StyleMarkUpPerc = listcostMRec[0].DiscValue - listcostMRec[0].CTTotManCost;

                if (listcostMRec[0].CTSelPrice1 > 0.0001)
                    listcostMRec[0].ProfMarPercDisc = ((listcostMRec[0].DiscValue - listcostMRec[0].CTTotManCost) / listcostMRec[0].CTSelPrice1) * 100.0;
                else
                    listcostMRec[0].ProfMarPercDisc = 0.0;
            }
        }
        private void btnZoom_Click(object sender, RoutedEventArgs e)
        {
            if (txtStyle.Text.Trim().Length == 0) return;

            Wgmate.ImageViewer.WpfPhotoViewer PhotoViewer = new Wgmate.ImageViewer.WpfPhotoViewer();
            PhotoViewer.CTStyle = txtStyle.Text;
            PhotoViewer.CTVarn = ""; // txtVarn.Text;
            PhotoViewer.Owner = Window.GetWindow(this);
            PhotoViewer.ShowDialog();
        }
    }
}