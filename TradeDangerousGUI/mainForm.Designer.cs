namespace TDHelper
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.btnStart = new System.Windows.Forms.Button();
            this.lblRunOptionsDestination = new System.Windows.Forms.Label();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.cboRunOptionsDestination = new System.Windows.Forms.ComboBox();
            this.mnuSetValues = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnuLadenLY = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuUnladenLY = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSep2 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuCapacity = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSep3 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuReset = new System.Windows.Forms.ToolStripMenuItem();
            this.lblStopWatch = new System.Windows.Forms.Label();
            this.backgroundWorker2 = new System.ComponentModel.BackgroundWorker();
            this.lblCommandersCredits = new System.Windows.Forms.Label();
            this.lblRouteOptionsShipCapacity = new System.Windows.Forms.Label();
            this.lblUnladenLy = new System.Windows.Forms.Label();
            this.lblLadenLy = new System.Windows.Forms.Label();
            this.chkRunOptionsTowards = new System.Windows.Forms.CheckBox();
            this.chkRunOptionsLoop = new System.Windows.Forms.CheckBox();
            this.mnuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnuCut = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuPaste = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.sepSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuSavePage1 = new System.Windows.Forms.ToolStripMenuItem();
            this.nmuClearSaved1 = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSavePage2 = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuClearSaved2 = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSavePage3 = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuClearSaved3 = new System.Windows.Forms.ToolStripMenuItem();
            this.sepSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuPushNotes = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuNotesClear = new System.Windows.Forms.ToolStripMenuItem();
            this.sepSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuSelectAll = new System.Windows.Forms.ToolStripMenuItem();
            this.chkRunOptionsDirect = new System.Windows.Forms.CheckBox();
            this.numRunOptionsRoutes = new System.Windows.Forms.NumericUpDown();
            this.numRunOptionsEndJumps = new System.Windows.Forms.NumericUpDown();
            this.numRunOptionsStartJumps = new System.Windows.Forms.NumericUpDown();
            this.numRouteOptionsLimit = new System.Windows.Forms.NumericUpDown();
            this.numRouteOptionsPruneHops = new System.Windows.Forms.NumericUpDown();
            this.numRouteOptionsPruneScore = new System.Windows.Forms.NumericUpDown();
            this.numRouteOptionsLsPenalty = new System.Windows.Forms.NumericUpDown();
            this.numRouteOptionsMaxLSDistance = new System.Windows.Forms.NumericUpDown();
            this.numRouteOptionsGpt = new System.Windows.Forms.NumericUpDown();
            this.chkRunOptionsUnique = new System.Windows.Forms.CheckBox();
            this.numShipInsurance = new System.Windows.Forms.NumericUpDown();
            this.numRouteOptionsShipCapacity = new System.Windows.Forms.NumericUpDown();
            this.numRouteOptionsJumps = new System.Windows.Forms.NumericUpDown();
            this.numRouteOptionsHops = new System.Windows.Forms.NumericUpDown();
            this.numCommandersCredits = new System.Windows.Forms.NumericUpDown();
            this.numUnladenLy = new System.Windows.Forms.NumericUpDown();
            this.numLadenLy = new System.Windows.Forms.NumericUpDown();
            this.lblRunOptionsRoutes = new System.Windows.Forms.Label();
            this.cboMethod = new System.Windows.Forms.ComboBox();
            this.txtVia = new System.Windows.Forms.TextBox();
            this.lblVia = new System.Windows.Forms.Label();
            this.lblRunOptionsStartJumps = new System.Windows.Forms.Label();
            this.lblRunOptionsEndJumps = new System.Windows.Forms.Label();
            this.txtAvoid = new System.Windows.Forms.TextBox();
            this.lblAvoid = new System.Windows.Forms.Label();
            this.chkRunOptionsBlkMkt = new System.Windows.Forms.CheckBox();
            this.lblRouteOptionsHops = new System.Windows.Forms.Label();
            this.tipToolTips = new System.Windows.Forms.ToolTip(this.components);
            this.btnDbUpdate = new System.Windows.Forms.Button();
            this.btnGetSystem = new System.Windows.Forms.Button();
            this.lblSourceSystem = new System.Windows.Forms.Label();
            this.lblPadSize = new System.Windows.Forms.Label();
            this.txtPadSize = new System.Windows.Forms.TextBox();
            this.lblRouteOptionsAge = new System.Windows.Forms.Label();
            this.numRouteOptionsAge = new System.Windows.Forms.NumericUpDown();
            this.lblRouteOptionsGpt = new System.Windows.Forms.Label();
            this.cboSourceSystem = new System.Windows.Forms.ComboBox();
            this.lblRouteOptionsMargin = new System.Windows.Forms.Label();
            this.numRouteOptionsMargin = new System.Windows.Forms.NumericUpDown();
            this.lblRouteOptionsCargoLimit = new System.Windows.Forms.Label();
            this.lblRouteOptionsMaxGpt = new System.Windows.Forms.Label();
            this.numRouteOptionsMaxGpt = new System.Windows.Forms.NumericUpDown();
            this.btnMiniMode = new System.Windows.Forms.Button();
            this.cboCommandersShips = new System.Windows.Forms.ComboBox();
            this.icoUpdateNotify = new System.Windows.Forms.PictureBox();
            this.numRouteOptionsStock = new System.Windows.Forms.NumericUpDown();
            this.lblStock = new System.Windows.Forms.Label();
            this.lblRunOptionsLoopInt = new System.Windows.Forms.Label();
            this.numRunOptionsLoopInt = new System.Windows.Forms.NumericUpDown();
            this.chkRunOptionsShorten = new System.Windows.Forms.CheckBox();
            this.numRouteOptionsDemand = new System.Windows.Forms.NumericUpDown();
            this.lblRouteOptionsDemand = new System.Windows.Forms.Label();
            this.chkRunOptionsJumps = new System.Windows.Forms.CheckBox();
            this.btnCmdrProfile = new System.Windows.Forms.Button();
            this.btnSaveSettings = new System.Windows.Forms.Button();
            this.lblRunOptionsPlanetary = new System.Windows.Forms.Label();
            this.txtRunOptionsPlanetary = new System.Windows.Forms.TextBox();
            this.btnSettings = new System.Windows.Forms.Button();
            this.btnRunOptionsSwap = new System.Windows.Forms.Button();
            this.lblBuyOptionsSupply = new System.Windows.Forms.Label();
            this.numBuyOptionsSupply = new System.Windows.Forms.NumericUpDown();
            this.numBuyOptionsLimit = new System.Windows.Forms.NumericUpDown();
            this.lblBuyOptionsLimit = new System.Windows.Forms.Label();
            this.lblBuyOptionsNearLy = new System.Windows.Forms.Label();
            this.numBuyOptionsNearLy = new System.Windows.Forms.NumericUpDown();
            this.txtBuyOptionsAvoid = new System.Windows.Forms.TextBox();
            this.lblBuyOptionsAvoid = new System.Windows.Forms.Label();
            this.lblBuyOptionsPlanetary = new System.Windows.Forms.Label();
            this.txtBuyOptionsPlanetary = new System.Windows.Forms.TextBox();
            this.chkBuyOptionsBlkMkt = new System.Windows.Forms.CheckBox();
            this.lblBuyOptionsPads = new System.Windows.Forms.Label();
            this.txtBuyOptionsPads = new System.Windows.Forms.TextBox();
            this.chkBuyOptionsOneStop = new System.Windows.Forms.CheckBox();
            this.numBuyOptionsAbove = new System.Windows.Forms.NumericUpDown();
            this.lblBuyOptionsAbove = new System.Windows.Forms.Label();
            this.numBuyOptionsBelow = new System.Windows.Forms.NumericUpDown();
            this.lblBuyOptionsBelow = new System.Windows.Forms.Label();
            this.cboBuyOptionsCommodities = new System.Windows.Forms.ComboBox();
            this.lblBuyOptionsCommodity = new System.Windows.Forms.Label();
            this.optBuyOptionsDistance = new System.Windows.Forms.RadioButton();
            this.optBuyOptionsPrice = new System.Windows.Forms.RadioButton();
            this.optBuyOptionsSupply = new System.Windows.Forms.RadioButton();
            this.lblBuyOptionsSort = new System.Windows.Forms.Label();
            this.lblSellOptionsSort = new System.Windows.Forms.Label();
            this.optSellOptionsSupply = new System.Windows.Forms.RadioButton();
            this.optSellOptionsPrice = new System.Windows.Forms.RadioButton();
            this.cboSellOptionsCommodities = new System.Windows.Forms.ComboBox();
            this.lblSellOptionsCommodity = new System.Windows.Forms.Label();
            this.numSellOptionsBelow = new System.Windows.Forms.NumericUpDown();
            this.lblSellOptionsBelow = new System.Windows.Forms.Label();
            this.numSellOptionsAbove = new System.Windows.Forms.NumericUpDown();
            this.lblSellOptionsAbove = new System.Windows.Forms.Label();
            this.lblSellOptionsPads = new System.Windows.Forms.Label();
            this.numSellOptionsNearLy = new System.Windows.Forms.NumericUpDown();
            this.txtSellOptionsPads = new System.Windows.Forms.TextBox();
            this.lblSellOptionsLimit = new System.Windows.Forms.Label();
            this.lblSellOptionsPlanetary = new System.Windows.Forms.Label();
            this.txtSellOptionsPlanetary = new System.Windows.Forms.TextBox();
            this.numSellOptionsLimit = new System.Windows.Forms.NumericUpDown();
            this.txtSellOptionsAvoid = new System.Windows.Forms.TextBox();
            this.lblSellOptionsAvoid = new System.Windows.Forms.Label();
            this.lblSellOptionsNearLy = new System.Windows.Forms.Label();
            this.numSellOptionsDemand = new System.Windows.Forms.NumericUpDown();
            this.lblSellOptionsDemand = new System.Windows.Forms.Label();
            this.lblRaresOptionsSort = new System.Windows.Forms.Label();
            this.optRaresOptionsDistance = new System.Windows.Forms.RadioButton();
            this.optRaresOptionsPrice = new System.Windows.Forms.RadioButton();
            this.lblRaresOptionsPads = new System.Windows.Forms.Label();
            this.numRaresOptionsLy = new System.Windows.Forms.NumericUpDown();
            this.txtRaresOptionsPads = new System.Windows.Forms.TextBox();
            this.lblRaresOptionsLimit = new System.Windows.Forms.Label();
            this.lblRaresOptionsPlanetary = new System.Windows.Forms.Label();
            this.chkRaresOptionsReverse = new System.Windows.Forms.CheckBox();
            this.txtRaresOptionsPlanetary = new System.Windows.Forms.TextBox();
            this.chkRaresOptionsQuiet = new System.Windows.Forms.CheckBox();
            this.numRaresOptionsLimit = new System.Windows.Forms.NumericUpDown();
            this.txtRaresOptionsFrom = new System.Windows.Forms.TextBox();
            this.lblRaresOptionsFrom = new System.Windows.Forms.Label();
            this.lblRaresOptionsLy = new System.Windows.Forms.Label();
            this.numRaresOptionsAway = new System.Windows.Forms.NumericUpDown();
            this.lblRaresOptionsType = new System.Windows.Forms.Label();
            this.optRaresOptionsLegal = new System.Windows.Forms.RadioButton();
            this.optRaresOptionsIllegal = new System.Windows.Forms.RadioButton();
            this.optRaresOptionsAll = new System.Windows.Forms.RadioButton();
            this.btnTradeOptionsSwap = new System.Windows.Forms.Button();
            this.lblTradeOptionDestination = new System.Windows.Forms.Label();
            this.cboTradeOptionDestination = new System.Windows.Forms.ComboBox();
            this.lblMarketOptionsType = new System.Windows.Forms.Label();
            this.optMarketOptionsBuy = new System.Windows.Forms.RadioButton();
            this.optMarketOptionsSell = new System.Windows.Forms.RadioButton();
            this.optMarketOptionsAll = new System.Windows.Forms.RadioButton();
            this.cboShipVendorOptionShips = new System.Windows.Forms.ComboBox();
            this.lblShipVendorOptionShips = new System.Windows.Forms.Label();
            this.btnNavOptionsSwap = new System.Windows.Forms.Button();
            this.chkNavOptionsStations = new System.Windows.Forms.CheckBox();
            this.lblNavOptionsDestination = new System.Windows.Forms.Label();
            this.lblNavOptionsRefuelJumps = new System.Windows.Forms.Label();
            this.numNavOptionsRefuelJumps = new System.Windows.Forms.NumericUpDown();
            this.cboNavOptionsDestination = new System.Windows.Forms.ComboBox();
            this.txtNavOptionsAvoid = new System.Windows.Forms.TextBox();
            this.lblNavOptionsAvoid = new System.Windows.Forms.Label();
            this.txtNavOptionsVia = new System.Windows.Forms.TextBox();
            this.lblNavOptionsVia = new System.Windows.Forms.Label();
            this.lblNavOptionsPads = new System.Windows.Forms.Label();
            this.txtNavOptionsPads = new System.Windows.Forms.TextBox();
            this.lblNavOptionsPlanetary = new System.Windows.Forms.Label();
            this.txtNavOptionsPlanetary = new System.Windows.Forms.TextBox();
            this.numNavOptionsLy = new System.Windows.Forms.NumericUpDown();
            this.lblNavOptionsLy = new System.Windows.Forms.Label();
            this.numOldDataOptionsNearLy = new System.Windows.Forms.NumericUpDown();
            this.lblOldDataOptionsLimit = new System.Windows.Forms.Label();
            this.numOldDataOptionsLimit = new System.Windows.Forms.NumericUpDown();
            this.lblOldDataOptionsNearLy = new System.Windows.Forms.Label();
            this.chkOldDataOptionsRoute = new System.Windows.Forms.CheckBox();
            this.lblOldDataOptionsMinAge = new System.Windows.Forms.Label();
            this.numOldDataOptionsMinAge = new System.Windows.Forms.NumericUpDown();
            this.chkLocalOptionsTrading = new System.Windows.Forms.CheckBox();
            this.numLocalOptionsLy = new System.Windows.Forms.NumericUpDown();
            this.lblLocalOptionsLy = new System.Windows.Forms.Label();
            this.lblLocalOptionsPads = new System.Windows.Forms.Label();
            this.txtLocalOptionsPads = new System.Windows.Forms.TextBox();
            this.lblLocalOptionsPlanetary = new System.Windows.Forms.Label();
            this.txtLocalOptionsPlanetary = new System.Windows.Forms.TextBox();
            this.btnLocalOptionsReset = new System.Windows.Forms.Button();
            this.chkLocalOptionsStations = new System.Windows.Forms.CheckBox();
            this.chkLocalOptionsOutfitting = new System.Windows.Forms.CheckBox();
            this.chkLocalOptionsRearm = new System.Windows.Forms.CheckBox();
            this.chkLocalOptionsCommodities = new System.Windows.Forms.CheckBox();
            this.chkLocalOptionsRepair = new System.Windows.Forms.CheckBox();
            this.chkLocalOptionsBlkMkt = new System.Windows.Forms.CheckBox();
            this.chkLocalOptionsRefuel = new System.Windows.Forms.CheckBox();
            this.chkLocalOptionsShipyard = new System.Windows.Forms.CheckBox();
            this.btnLocalOptionsAll = new System.Windows.Forms.Button();
            this.chkSellOptionsBlkMkt = new System.Windows.Forms.CheckBox();
            this.backgroundWorker3 = new System.ComponentModel.BackgroundWorker();
            this.backgroundWorker4 = new System.ComponentModel.BackgroundWorker();
            this.panRunOptions = new System.Windows.Forms.Panel();
            this.lblUpdateNotify = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.pagOutput = new System.Windows.Forms.TabPage();
            this.rtbOutput = new System.Windows.Forms.RichTextBox();
            this.tabSavedPage1 = new System.Windows.Forms.TabPage();
            this.rtbSaved1 = new System.Windows.Forms.RichTextBox();
            this.tabSavedPage2 = new System.Windows.Forms.TabPage();
            this.rtbSaved2 = new System.Windows.Forms.RichTextBox();
            this.tabSavedPage3 = new System.Windows.Forms.TabPage();
            this.rtbSaved3 = new System.Windows.Forms.RichTextBox();
            this.tabNotesPage = new System.Windows.Forms.TabPage();
            this.txtNotes = new System.Windows.Forms.RichTextBox();
            this.tabLogPage = new System.Windows.Forms.TabPage();
            this.grdPilotsLog = new System.Windows.Forms.DataGridView();
            this.mnuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnuInsertAtGridRow = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuRemoveAtGridRow = new System.Windows.Forms.ToolStripMenuItem();
            this.sepSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuForceRefreshGridView = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuForceResortMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sepSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuCopySystemToSrc = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuCopySystemToDest = new System.Windows.Forms.ToolStripMenuItem();
            this.panSellOptions = new System.Windows.Forms.Panel();
            this.grpSellOptionsSort = new System.Windows.Forms.GroupBox();
            this.panBuyOptions = new System.Windows.Forms.Panel();
            this.grpBuyOptionsSort = new System.Windows.Forms.GroupBox();
            this.lblRouteOptionsJumps = new System.Windows.Forms.Label();
            this.lblRouteOptionsMaxLS = new System.Windows.Forms.Label();
            this.lblRouteOptionsPruneHops = new System.Windows.Forms.Label();
            this.lblRouteOptionsPruneScore = new System.Windows.Forms.Label();
            this.lblRouteOptionsLsPenalty = new System.Windows.Forms.Label();
            this.lblShipInsurance = new System.Windows.Forms.Label();
            this.backgroundWorker5 = new System.ComponentModel.BackgroundWorker();
            this.backgroundWorker6 = new System.ComponentModel.BackgroundWorker();
            this.lblTrackerLink = new System.Windows.Forms.LinkLabel();
            this.lblFaqLink = new System.Windows.Forms.LinkLabel();
            this.backgroundWorker7 = new System.ComponentModel.BackgroundWorker();
            this.panShipData = new System.Windows.Forms.Panel();
            this.panStock = new System.Windows.Forms.Panel();
            this.panHops = new System.Windows.Forms.Panel();
            this.panProfit = new System.Windows.Forms.Panel();
            this.panOther = new System.Windows.Forms.Panel();
            this.panMisc = new System.Windows.Forms.Panel();
            this.panRouteOptions = new System.Windows.Forms.Panel();
            this.panMethods = new System.Windows.Forms.Panel();
            this.panRaresOptions = new System.Windows.Forms.Panel();
            this.grpRaresOptionsType = new System.Windows.Forms.GroupBox();
            this.grpRaresOptionsSort = new System.Windows.Forms.GroupBox();
            this.panTradeOptions = new System.Windows.Forms.Panel();
            this.panMarketOptions = new System.Windows.Forms.Panel();
            this.grpMarketOptionsType = new System.Windows.Forms.GroupBox();
            this.panShipVendorOptions = new System.Windows.Forms.Panel();
            this.panNavOptions = new System.Windows.Forms.Panel();
            this.panOldDataOptions = new System.Windows.Forms.Panel();
            this.panLocalOptions = new System.Windows.Forms.Panel();
            this.panOptions = new System.Windows.Forms.Panel();
            this.mnuSetValues.SuspendLayout();
            this.mnuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numRunOptionsRoutes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRunOptionsEndJumps)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRunOptionsStartJumps)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRouteOptionsLimit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRouteOptionsPruneHops)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRouteOptionsPruneScore)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRouteOptionsLsPenalty)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRouteOptionsMaxLSDistance)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRouteOptionsGpt)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numShipInsurance)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRouteOptionsShipCapacity)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRouteOptionsJumps)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRouteOptionsHops)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCommandersCredits)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUnladenLy)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numLadenLy)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRouteOptionsAge)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRouteOptionsMargin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRouteOptionsMaxGpt)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.icoUpdateNotify)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRouteOptionsStock)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRunOptionsLoopInt)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRouteOptionsDemand)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBuyOptionsSupply)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBuyOptionsLimit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBuyOptionsNearLy)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBuyOptionsAbove)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBuyOptionsBelow)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSellOptionsBelow)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSellOptionsAbove)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSellOptionsNearLy)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSellOptionsLimit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSellOptionsDemand)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRaresOptionsLy)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRaresOptionsLimit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRaresOptionsAway)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numNavOptionsRefuelJumps)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numNavOptionsLy)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numOldDataOptionsNearLy)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numOldDataOptionsLimit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numOldDataOptionsMinAge)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numLocalOptionsLy)).BeginInit();
            this.panRunOptions.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.pagOutput.SuspendLayout();
            this.tabSavedPage1.SuspendLayout();
            this.tabSavedPage2.SuspendLayout();
            this.tabSavedPage3.SuspendLayout();
            this.tabNotesPage.SuspendLayout();
            this.tabLogPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdPilotsLog)).BeginInit();
            this.mnuStrip2.SuspendLayout();
            this.panSellOptions.SuspendLayout();
            this.grpSellOptionsSort.SuspendLayout();
            this.panBuyOptions.SuspendLayout();
            this.grpBuyOptionsSort.SuspendLayout();
            this.panShipData.SuspendLayout();
            this.panStock.SuspendLayout();
            this.panHops.SuspendLayout();
            this.panProfit.SuspendLayout();
            this.panOther.SuspendLayout();
            this.panMisc.SuspendLayout();
            this.panRouteOptions.SuspendLayout();
            this.panMethods.SuspendLayout();
            this.panRaresOptions.SuspendLayout();
            this.grpRaresOptionsType.SuspendLayout();
            this.grpRaresOptionsSort.SuspendLayout();
            this.panTradeOptions.SuspendLayout();
            this.panMarketOptions.SuspendLayout();
            this.grpMarketOptionsType.SuspendLayout();
            this.panShipVendorOptions.SuspendLayout();
            this.panNavOptions.SuspendLayout();
            this.panOldDataOptions.SuspendLayout();
            this.panLocalOptions.SuspendLayout();
            this.panOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnStart
            // 
            this.btnStart.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStart.Location = new System.Drawing.Point(787, 1);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(69, 23);
            this.btnStart.TabIndex = 1;
            this.btnStart.TabStop = false;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.BtnStart_Click);
            // 
            // lblRunOptionsDestination
            // 
            this.lblRunOptionsDestination.AllowDrop = true;
            this.lblRunOptionsDestination.AutoSize = true;
            this.lblRunOptionsDestination.Location = new System.Drawing.Point(32, 8);
            this.lblRunOptionsDestination.Name = "lblRunOptionsDestination";
            this.lblRunOptionsDestination.Size = new System.Drawing.Size(63, 13);
            this.lblRunOptionsDestination.TabIndex = 1;
            this.lblRunOptionsDestination.Text = "Destination:";
            this.tipToolTips.SetToolTip(this.lblRunOptionsDestination, "Destination point in the form of system or system/station");
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerReportsProgress = true;
            this.backgroundWorker1.WorkerSupportsCancellation = true;
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BackgroundWorker1_DoWork);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.BackgroundWorker1_RunWorkerCompleted);
            // 
            // cboRunOptionsDestination
            // 
            this.cboRunOptionsDestination.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cboRunOptionsDestination.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.cboRunOptionsDestination.ContextMenuStrip = this.mnuSetValues;
            this.cboRunOptionsDestination.Location = new System.Drawing.Point(101, 5);
            this.cboRunOptionsDestination.Name = "cboRunOptionsDestination";
            this.cboRunOptionsDestination.Size = new System.Drawing.Size(212, 21);
            this.cboRunOptionsDestination.TabIndex = 0;
            this.tipToolTips.SetToolTip(this.cboRunOptionsDestination, "Destination point in the form of system or system/station\r\nCtrl+Enter adds a Syst" +
        "em/Station to the favorites\r\nShift+Enter removes a System/Station from the favor" +
        "ites");
            this.cboRunOptionsDestination.DropDown += new System.EventHandler(this.ComboBox_DropDown);
            this.cboRunOptionsDestination.SelectedIndexChanged += new System.EventHandler(this.DestinationChanged);
            this.cboRunOptionsDestination.DropDownClosed += new System.EventHandler(this.ComboBox_DropDownClosed);
            this.cboRunOptionsDestination.TextChanged += new System.EventHandler(this.DestinationChanged);
            this.cboRunOptionsDestination.KeyDown += new System.Windows.Forms.KeyEventHandler(this.DestSystemComboBox_KeyDown);
            // 
            // mnuSetValues
            // 
            this.mnuSetValues.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuLadenLY,
            this.mnuUnladenLY,
            this.mnuSep2,
            this.mnuCapacity,
            this.mnuSep3,
            this.mnuReset});
            this.mnuSetValues.Name = "contextMenuStrip1";
            this.mnuSetValues.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.mnuSetValues.ShowImageMargin = false;
            this.mnuSetValues.Size = new System.Drawing.Size(110, 104);
            this.mnuSetValues.Opening += new System.ComponentModel.CancelEventHandler(this.SetValuesMenu_Opening);
            // 
            // mnuLadenLY
            // 
            this.mnuLadenLY.Name = "mnuLadenLY";
            this.mnuLadenLY.Size = new System.Drawing.Size(109, 22);
            this.mnuLadenLY.Text = "Laden LY";
            this.mnuLadenLY.Click += new System.EventHandler(this.DistanceMenuItem_Click);
            // 
            // mnuUnladenLY
            // 
            this.mnuUnladenLY.Name = "mnuUnladenLY";
            this.mnuUnladenLY.Size = new System.Drawing.Size(109, 22);
            this.mnuUnladenLY.Text = "Unladen LY";
            this.mnuUnladenLY.Click += new System.EventHandler(this.DistanceMenuItem_Click);
            // 
            // mnuSep2
            // 
            this.mnuSep2.Name = "mnuSep2";
            this.mnuSep2.Size = new System.Drawing.Size(106, 6);
            // 
            // mnuCapacity
            // 
            this.mnuCapacity.Name = "mnuCapacity";
            this.mnuCapacity.Size = new System.Drawing.Size(109, 22);
            this.mnuCapacity.Text = "Capacity";
            this.mnuCapacity.Click += new System.EventHandler(this.DistanceMenuItem_Click);
            // 
            // mnuSep3
            // 
            this.mnuSep3.Name = "mnuSep3";
            this.mnuSep3.Size = new System.Drawing.Size(106, 6);
            // 
            // mnuReset
            // 
            this.mnuReset.Name = "mnuReset";
            this.mnuReset.Size = new System.Drawing.Size(109, 22);
            this.mnuReset.Text = "Reset";
            this.mnuReset.Click += new System.EventHandler(this.DistanceMenuItem_Click);
            // 
            // lblStopWatch
            // 
            this.lblStopWatch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblStopWatch.AutoSize = true;
            this.lblStopWatch.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStopWatch.Location = new System.Drawing.Point(14, 655);
            this.lblStopWatch.Name = "lblStopWatch";
            this.lblStopWatch.Size = new System.Drawing.Size(0, 15);
            this.lblStopWatch.TabIndex = 9;
            // 
            // backgroundWorker2
            // 
            this.backgroundWorker2.WorkerReportsProgress = true;
            this.backgroundWorker2.WorkerSupportsCancellation = true;
            this.backgroundWorker2.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BackgroundWorker2_DoWork);
            this.backgroundWorker2.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.BackgroundWorker2_RunWorkerCompleted);
            // 
            // lblCommandersCredits
            // 
            this.lblCommandersCredits.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCommandersCredits.AutoSize = true;
            this.lblCommandersCredits.Location = new System.Drawing.Point(354, 34);
            this.lblCommandersCredits.Name = "lblCommandersCredits";
            this.lblCommandersCredits.Size = new System.Drawing.Size(42, 13);
            this.lblCommandersCredits.TabIndex = 0;
            this.lblCommandersCredits.Text = "Credits:";
            this.tipToolTips.SetToolTip(this.lblCommandersCredits, "Current credits");
            // 
            // lblRouteOptionsShipCapacity
            // 
            this.lblRouteOptionsShipCapacity.AutoSize = true;
            this.lblRouteOptionsShipCapacity.Location = new System.Drawing.Point(242, 34);
            this.lblRouteOptionsShipCapacity.Name = "lblRouteOptionsShipCapacity";
            this.lblRouteOptionsShipCapacity.Size = new System.Drawing.Size(51, 13);
            this.lblRouteOptionsShipCapacity.TabIndex = 1;
            this.lblRouteOptionsShipCapacity.Text = "Capacity:";
            this.tipToolTips.SetToolTip(this.lblRouteOptionsShipCapacity, "Total cargo space in your ship");
            // 
            // lblUnladenLy
            // 
            this.lblUnladenLy.AutoSize = true;
            this.lblUnladenLy.Location = new System.Drawing.Point(118, 34);
            this.lblUnladenLy.Name = "lblUnladenLy";
            this.lblUnladenLy.Size = new System.Drawing.Size(66, 13);
            this.lblUnladenLy.TabIndex = 3;
            this.lblUnladenLy.Text = "Unladen LY:";
            this.tipToolTips.SetToolTip(this.lblUnladenLy, "Distance that can be travelled while unladen (including fuel)");
            // 
            // lblLadenLy
            // 
            this.lblLadenLy.AutoSize = true;
            this.lblLadenLy.Location = new System.Drawing.Point(4, 34);
            this.lblLadenLy.Name = "lblLadenLy";
            this.lblLadenLy.Size = new System.Drawing.Size(56, 13);
            this.lblLadenLy.TabIndex = 2;
            this.lblLadenLy.Text = "Laden LY:";
            this.tipToolTips.SetToolTip(this.lblLadenLy, "Distance that can be travelled while fully laden (including fuel)");
            // 
            // chkRunOptionsTowards
            // 
            this.chkRunOptionsTowards.AutoSize = true;
            this.chkRunOptionsTowards.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkRunOptionsTowards.Location = new System.Drawing.Point(220, 55);
            this.chkRunOptionsTowards.Name = "chkRunOptionsTowards";
            this.chkRunOptionsTowards.Size = new System.Drawing.Size(67, 17);
            this.chkRunOptionsTowards.TabIndex = 7;
            this.chkRunOptionsTowards.TabStop = false;
            this.chkRunOptionsTowards.Text = "Towards";
            this.tipToolTips.SetToolTip(this.chkRunOptionsTowards, "Favors distance covered over profit generated during routing (requires a Destinat" +
        "ion)");
            this.chkRunOptionsTowards.UseVisualStyleBackColor = true;
            this.chkRunOptionsTowards.Click += new System.EventHandler(this.ChkTowards_Click);
            // 
            // chkRunOptionsLoop
            // 
            this.chkRunOptionsLoop.AutoSize = true;
            this.chkRunOptionsLoop.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkRunOptionsLoop.Location = new System.Drawing.Point(3, 35);
            this.chkRunOptionsLoop.Name = "chkRunOptionsLoop";
            this.chkRunOptionsLoop.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.chkRunOptionsLoop.Size = new System.Drawing.Size(50, 17);
            this.chkRunOptionsLoop.TabIndex = 6;
            this.chkRunOptionsLoop.TabStop = false;
            this.chkRunOptionsLoop.Text = "Loop";
            this.tipToolTips.SetToolTip(this.chkRunOptionsLoop, "Attempts to get a round-trip route");
            this.chkRunOptionsLoop.UseVisualStyleBackColor = true;
            this.chkRunOptionsLoop.Click += new System.EventHandler(this.ChkLoop_Click);
            // 
            // mnuStrip1
            // 
            this.mnuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuCut,
            this.mnuCopy,
            this.mnuPaste,
            this.mnuDelete,
            this.sepSeparator3,
            this.mnuSavePage1,
            this.nmuClearSaved1,
            this.mnuSavePage2,
            this.mnuClearSaved2,
            this.mnuSavePage3,
            this.mnuClearSaved3,
            this.sepSeparator2,
            this.mnuPushNotes,
            this.mnuNotesClear,
            this.sepSeparator1,
            this.mnuSelectAll});
            this.mnuStrip1.Name = "contextMenuStrip1";
            this.mnuStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.mnuStrip1.ShowImageMargin = false;
            this.mnuStrip1.Size = new System.Drawing.Size(123, 308);
            this.mnuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.ContextMenuStrip1_Opening);
            // 
            // mnuCut
            // 
            this.mnuCut.Enabled = false;
            this.mnuCut.Name = "mnuCut";
            this.mnuCut.Size = new System.Drawing.Size(122, 22);
            this.mnuCut.Text = "Cut";
            this.mnuCut.Click += new System.EventHandler(this.CutMenuItem_Click);
            // 
            // mnuCopy
            // 
            this.mnuCopy.Name = "mnuCopy";
            this.mnuCopy.Size = new System.Drawing.Size(122, 22);
            this.mnuCopy.Text = "Copy";
            this.mnuCopy.Click += new System.EventHandler(this.CopyMenuItem_Click);
            // 
            // mnuPaste
            // 
            this.mnuPaste.Enabled = false;
            this.mnuPaste.Name = "mnuPaste";
            this.mnuPaste.Size = new System.Drawing.Size(122, 22);
            this.mnuPaste.Text = "Paste";
            this.mnuPaste.Click += new System.EventHandler(this.PasteMenuItem_Click);
            // 
            // mnuDelete
            // 
            this.mnuDelete.Enabled = false;
            this.mnuDelete.Name = "mnuDelete";
            this.mnuDelete.Size = new System.Drawing.Size(122, 22);
            this.mnuDelete.Text = "Delete";
            this.mnuDelete.Click += new System.EventHandler(this.DeleteMenuItem_Click);
            // 
            // sepSeparator3
            // 
            this.sepSeparator3.Name = "sepSeparator3";
            this.sepSeparator3.Size = new System.Drawing.Size(119, 6);
            // 
            // mnuSavePage1
            // 
            this.mnuSavePage1.Enabled = false;
            this.mnuSavePage1.Name = "mnuSavePage1";
            this.mnuSavePage1.Size = new System.Drawing.Size(122, 22);
            this.mnuSavePage1.Text = "Save to #1";
            this.mnuSavePage1.Click += new System.EventHandler(this.SavePage1MenuItem_Click);
            // 
            // nmuClearSaved1
            // 
            this.nmuClearSaved1.Name = "nmuClearSaved1";
            this.nmuClearSaved1.Size = new System.Drawing.Size(122, 22);
            this.nmuClearSaved1.Text = "Clear #1";
            this.nmuClearSaved1.Visible = false;
            this.nmuClearSaved1.Click += new System.EventHandler(this.ClearSaved1MenuItem_Click);
            // 
            // mnuSavePage2
            // 
            this.mnuSavePage2.Enabled = false;
            this.mnuSavePage2.Name = "mnuSavePage2";
            this.mnuSavePage2.Size = new System.Drawing.Size(122, 22);
            this.mnuSavePage2.Text = "Save to #2";
            this.mnuSavePage2.Click += new System.EventHandler(this.SavePage2MenuItem_Click);
            // 
            // mnuClearSaved2
            // 
            this.mnuClearSaved2.Name = "mnuClearSaved2";
            this.mnuClearSaved2.Size = new System.Drawing.Size(122, 22);
            this.mnuClearSaved2.Text = "Clear #2";
            this.mnuClearSaved2.Visible = false;
            this.mnuClearSaved2.Click += new System.EventHandler(this.ClearSaved2MenuItem_Click);
            // 
            // mnuSavePage3
            // 
            this.mnuSavePage3.Enabled = false;
            this.mnuSavePage3.Name = "mnuSavePage3";
            this.mnuSavePage3.Size = new System.Drawing.Size(122, 22);
            this.mnuSavePage3.Text = "Save to #3";
            this.mnuSavePage3.Click += new System.EventHandler(this.SavePage3MenuItem_Click);
            // 
            // mnuClearSaved3
            // 
            this.mnuClearSaved3.Name = "mnuClearSaved3";
            this.mnuClearSaved3.Size = new System.Drawing.Size(122, 22);
            this.mnuClearSaved3.Text = "Clear #3";
            this.mnuClearSaved3.Visible = false;
            this.mnuClearSaved3.Click += new System.EventHandler(this.ClearSaved3MenuItem_Click);
            // 
            // sepSeparator2
            // 
            this.sepSeparator2.Name = "sepSeparator2";
            this.sepSeparator2.Size = new System.Drawing.Size(119, 6);
            // 
            // mnuPushNotes
            // 
            this.mnuPushNotes.Name = "mnuPushNotes";
            this.mnuPushNotes.Size = new System.Drawing.Size(122, 22);
            this.mnuPushNotes.Text = "Add To Notes";
            this.mnuPushNotes.Click += new System.EventHandler(this.PushNotesMenuItem_Click);
            // 
            // mnuNotesClear
            // 
            this.mnuNotesClear.Name = "mnuNotesClear";
            this.mnuNotesClear.Size = new System.Drawing.Size(122, 22);
            this.mnuNotesClear.Text = "Clear Notes";
            this.mnuNotesClear.Visible = false;
            this.mnuNotesClear.Click += new System.EventHandler(this.NotesClearMenuItem_Click);
            // 
            // sepSeparator1
            // 
            this.sepSeparator1.Name = "sepSeparator1";
            this.sepSeparator1.Size = new System.Drawing.Size(119, 6);
            // 
            // mnuSelectAll
            // 
            this.mnuSelectAll.Name = "mnuSelectAll";
            this.mnuSelectAll.Size = new System.Drawing.Size(122, 22);
            this.mnuSelectAll.Text = "Select All";
            this.mnuSelectAll.Click += new System.EventHandler(this.SelectMenuItem_Click);
            // 
            // chkRunOptionsDirect
            // 
            this.chkRunOptionsDirect.AutoSize = true;
            this.chkRunOptionsDirect.Location = new System.Drawing.Point(3, 81);
            this.chkRunOptionsDirect.Name = "chkRunOptionsDirect";
            this.chkRunOptionsDirect.Size = new System.Drawing.Size(54, 17);
            this.chkRunOptionsDirect.TabIndex = 3;
            this.chkRunOptionsDirect.TabStop = false;
            this.chkRunOptionsDirect.Text = "Direct";
            this.tipToolTips.SetToolTip(this.chkRunOptionsDirect, "Attempts to calculate a 1 hop route, ignoring distance (volatile)");
            this.chkRunOptionsDirect.UseVisualStyleBackColor = true;
            this.chkRunOptionsDirect.Click += new System.EventHandler(this.DirectCheckBox_Click);
            // 
            // numRunOptionsRoutes
            // 
            this.numRunOptionsRoutes.ContextMenuStrip = this.mnuSetValues;
            this.numRunOptionsRoutes.Location = new System.Drawing.Point(131, 112);
            this.numRunOptionsRoutes.Maximum = new decimal(new int[] {
            50000,
            0,
            0,
            0});
            this.numRunOptionsRoutes.Name = "numRunOptionsRoutes";
            this.numRunOptionsRoutes.Size = new System.Drawing.Size(46, 20);
            this.numRunOptionsRoutes.TabIndex = 5;
            this.numRunOptionsRoutes.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numRunOptionsRoutes.ThousandsSeparator = true;
            this.tipToolTips.SetToolTip(this.numRunOptionsRoutes, "Generates this many routes for a Run");
            this.numRunOptionsRoutes.Enter += new System.EventHandler(this.NumericUpDown_Enter);
            this.numRunOptionsRoutes.MouseUp += new System.Windows.Forms.MouseEventHandler(this.NumericUpDown_MouseUp);
            // 
            // numRunOptionsEndJumps
            // 
            this.numRunOptionsEndJumps.ContextMenuStrip = this.mnuSetValues;
            this.numRunOptionsEndJumps.Location = new System.Drawing.Point(131, 86);
            this.numRunOptionsEndJumps.Name = "numRunOptionsEndJumps";
            this.numRunOptionsEndJumps.Size = new System.Drawing.Size(46, 20);
            this.numRunOptionsEndJumps.TabIndex = 1;
            this.numRunOptionsEndJumps.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tipToolTips.SetToolTip(this.numRunOptionsEndJumps, "Try to finish this many jumps away from the destination, this requires a destinat" +
        "ion (volatile)");
            this.numRunOptionsEndJumps.Enter += new System.EventHandler(this.NumericUpDown_Enter);
            this.numRunOptionsEndJumps.MouseUp += new System.Windows.Forms.MouseEventHandler(this.NumericUpDown_MouseUp);
            // 
            // numRunOptionsStartJumps
            // 
            this.numRunOptionsStartJumps.ContextMenuStrip = this.mnuSetValues;
            this.numRunOptionsStartJumps.Location = new System.Drawing.Point(131, 60);
            this.numRunOptionsStartJumps.Name = "numRunOptionsStartJumps";
            this.numRunOptionsStartJumps.Size = new System.Drawing.Size(46, 20);
            this.numRunOptionsStartJumps.TabIndex = 2;
            this.numRunOptionsStartJumps.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tipToolTips.SetToolTip(this.numRunOptionsStartJumps, "Try to route starting from this many jumps away from the source system (volatile)" +
        "");
            this.numRunOptionsStartJumps.Enter += new System.EventHandler(this.NumericUpDown_Enter);
            this.numRunOptionsStartJumps.MouseUp += new System.Windows.Forms.MouseEventHandler(this.NumericUpDown_MouseUp);
            // 
            // numRouteOptionsLimit
            // 
            this.numRouteOptionsLimit.ContextMenuStrip = this.mnuSetValues;
            this.numRouteOptionsLimit.Location = new System.Drawing.Point(67, 48);
            this.numRouteOptionsLimit.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numRouteOptionsLimit.Name = "numRouteOptionsLimit";
            this.numRouteOptionsLimit.Size = new System.Drawing.Size(71, 20);
            this.numRouteOptionsLimit.TabIndex = 2;
            this.numRouteOptionsLimit.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tipToolTips.SetToolTip(this.numRouteOptionsLimit, "Limit each commodity purchased to this amount on a hop");
            this.numRouteOptionsLimit.Enter += new System.EventHandler(this.NumericUpDown_Enter);
            this.numRouteOptionsLimit.MouseUp += new System.Windows.Forms.MouseEventHandler(this.NumericUpDown_MouseUp);
            // 
            // numRouteOptionsPruneHops
            // 
            this.numRouteOptionsPruneHops.ContextMenuStrip = this.mnuSetValues;
            this.numRouteOptionsPruneHops.Location = new System.Drawing.Point(78, 26);
            this.numRouteOptionsPruneHops.Name = "numRouteOptionsPruneHops";
            this.numRouteOptionsPruneHops.Size = new System.Drawing.Size(60, 20);
            this.numRouteOptionsPruneHops.TabIndex = 1;
            this.numRouteOptionsPruneHops.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tipToolTips.SetToolTip(this.numRouteOptionsPruneHops, "Number of hops before pruning starts, to enable set >=2");
            this.numRouteOptionsPruneHops.Enter += new System.EventHandler(this.NumericUpDown_Enter);
            this.numRouteOptionsPruneHops.MouseUp += new System.Windows.Forms.MouseEventHandler(this.NumericUpDown_MouseUp);
            // 
            // numRouteOptionsPruneScore
            // 
            this.numRouteOptionsPruneScore.ContextMenuStrip = this.mnuSetValues;
            this.numRouteOptionsPruneScore.Location = new System.Drawing.Point(78, 3);
            this.numRouteOptionsPruneScore.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
            this.numRouteOptionsPruneScore.Name = "numRouteOptionsPruneScore";
            this.numRouteOptionsPruneScore.Size = new System.Drawing.Size(60, 20);
            this.numRouteOptionsPruneScore.TabIndex = 0;
            this.numRouteOptionsPruneScore.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tipToolTips.SetToolTip(this.numRouteOptionsPruneScore, "Percentage of route score change, below which pruning occurs");
            this.numRouteOptionsPruneScore.Enter += new System.EventHandler(this.NumericUpDown_Enter);
            this.numRouteOptionsPruneScore.MouseUp += new System.Windows.Forms.MouseEventHandler(this.NumericUpDown_MouseUp);
            // 
            // numRouteOptionsLsPenalty
            // 
            this.numRouteOptionsLsPenalty.ContextMenuStrip = this.mnuSetValues;
            this.numRouteOptionsLsPenalty.DecimalPlaces = 1;
            this.numRouteOptionsLsPenalty.Location = new System.Drawing.Point(78, 72);
            this.numRouteOptionsLsPenalty.Maximum = new decimal(new int[] {
            20000,
            0,
            0,
            0});
            this.numRouteOptionsLsPenalty.Name = "numRouteOptionsLsPenalty";
            this.numRouteOptionsLsPenalty.Size = new System.Drawing.Size(60, 20);
            this.numRouteOptionsLsPenalty.TabIndex = 3;
            this.numRouteOptionsLsPenalty.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tipToolTips.SetToolTip(this.numRouteOptionsLsPenalty, "Scoring penalty per LS traveled to station");
            this.numRouteOptionsLsPenalty.Enter += new System.EventHandler(this.NumericUpDown_Enter);
            this.numRouteOptionsLsPenalty.MouseUp += new System.Windows.Forms.MouseEventHandler(this.NumericUpDown_MouseUp);
            // 
            // numRouteOptionsMaxLSDistance
            // 
            this.numRouteOptionsMaxLSDistance.ContextMenuStrip = this.mnuSetValues;
            this.numRouteOptionsMaxLSDistance.Location = new System.Drawing.Point(78, 49);
            this.numRouteOptionsMaxLSDistance.Maximum = new decimal(new int[] {
            20000,
            0,
            0,
            0});
            this.numRouteOptionsMaxLSDistance.Name = "numRouteOptionsMaxLSDistance";
            this.numRouteOptionsMaxLSDistance.Size = new System.Drawing.Size(60, 20);
            this.numRouteOptionsMaxLSDistance.TabIndex = 2;
            this.numRouteOptionsMaxLSDistance.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tipToolTips.SetToolTip(this.numRouteOptionsMaxLSDistance, "Maximum distance station can be from system drop");
            this.numRouteOptionsMaxLSDistance.Enter += new System.EventHandler(this.NumericUpDown_Enter);
            this.numRouteOptionsMaxLSDistance.MouseUp += new System.Windows.Forms.MouseEventHandler(this.NumericUpDown_MouseUp);
            // 
            // numRouteOptionsGpt
            // 
            this.numRouteOptionsGpt.ContextMenuStrip = this.mnuSetValues;
            this.numRouteOptionsGpt.Location = new System.Drawing.Point(61, 2);
            this.numRouteOptionsGpt.Maximum = new decimal(new int[] {
            20000,
            0,
            0,
            0});
            this.numRouteOptionsGpt.Name = "numRouteOptionsGpt";
            this.numRouteOptionsGpt.Size = new System.Drawing.Size(60, 20);
            this.numRouteOptionsGpt.TabIndex = 0;
            this.numRouteOptionsGpt.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numRouteOptionsGpt.ThousandsSeparator = true;
            this.tipToolTips.SetToolTip(this.numRouteOptionsGpt, "Minimum profit in credits per ton on any hop");
            this.numRouteOptionsGpt.Enter += new System.EventHandler(this.NumericUpDown_Enter);
            this.numRouteOptionsGpt.MouseUp += new System.Windows.Forms.MouseEventHandler(this.NumericUpDown_MouseUp);
            // 
            // chkRunOptionsUnique
            // 
            this.chkRunOptionsUnique.AutoSize = true;
            this.chkRunOptionsUnique.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkRunOptionsUnique.Location = new System.Drawing.Point(227, 78);
            this.chkRunOptionsUnique.Name = "chkRunOptionsUnique";
            this.chkRunOptionsUnique.Size = new System.Drawing.Size(60, 17);
            this.chkRunOptionsUnique.TabIndex = 5;
            this.chkRunOptionsUnique.TabStop = false;
            this.chkRunOptionsUnique.Text = "Unique";
            this.tipToolTips.SetToolTip(this.chkRunOptionsUnique, "Require that stations on a route only be visited once");
            this.chkRunOptionsUnique.UseVisualStyleBackColor = true;
            this.chkRunOptionsUnique.Click += new System.EventHandler(this.UniqueCheckBox_Click);
            // 
            // numShipInsurance
            // 
            this.numShipInsurance.Location = new System.Drawing.Point(421, 4);
            this.numShipInsurance.Maximum = new decimal(new int[] {
            -2147483648,
            2,
            0,
            0});
            this.numShipInsurance.Name = "numShipInsurance";
            this.numShipInsurance.Size = new System.Drawing.Size(100, 20);
            this.numShipInsurance.TabIndex = 5;
            this.numShipInsurance.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numShipInsurance.ThousandsSeparator = true;
            this.tipToolTips.SetToolTip(this.numShipInsurance, "Keep at least this much in credits during routing");
            this.numShipInsurance.Enter += new System.EventHandler(this.NumericUpDown_Enter);
            this.numShipInsurance.MouseUp += new System.Windows.Forms.MouseEventHandler(this.NumericUpDown_MouseUp);
            // 
            // numRouteOptionsShipCapacity
            // 
            this.numRouteOptionsShipCapacity.Location = new System.Drawing.Point(292, 30);
            this.numRouteOptionsShipCapacity.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numRouteOptionsShipCapacity.Name = "numRouteOptionsShipCapacity";
            this.numRouteOptionsShipCapacity.Size = new System.Drawing.Size(60, 20);
            this.numRouteOptionsShipCapacity.TabIndex = 6;
            this.numRouteOptionsShipCapacity.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tipToolTips.SetToolTip(this.numRouteOptionsShipCapacity, "Total cargo space in your ship");
            this.numRouteOptionsShipCapacity.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numRouteOptionsShipCapacity.Enter += new System.EventHandler(this.NumericUpDown_Enter);
            this.numRouteOptionsShipCapacity.MouseUp += new System.Windows.Forms.MouseEventHandler(this.NumericUpDown_MouseUp);
            // 
            // numRouteOptionsJumps
            // 
            this.numRouteOptionsJumps.ContextMenuStrip = this.mnuSetValues;
            this.numRouteOptionsJumps.Location = new System.Drawing.Point(45, 26);
            this.numRouteOptionsJumps.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numRouteOptionsJumps.Name = "numRouteOptionsJumps";
            this.numRouteOptionsJumps.Size = new System.Drawing.Size(39, 20);
            this.numRouteOptionsJumps.TabIndex = 1;
            this.numRouteOptionsJumps.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tipToolTips.SetToolTip(this.numRouteOptionsJumps, "A jump is any system between two hops");
            this.numRouteOptionsJumps.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numRouteOptionsJumps.Enter += new System.EventHandler(this.NumericUpDown_Enter);
            this.numRouteOptionsJumps.MouseUp += new System.Windows.Forms.MouseEventHandler(this.NumericUpDown_MouseUp);
            // 
            // numRouteOptionsHops
            // 
            this.numRouteOptionsHops.ContextMenuStrip = this.mnuSetValues;
            this.numRouteOptionsHops.Location = new System.Drawing.Point(45, 3);
            this.numRouteOptionsHops.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numRouteOptionsHops.Name = "numRouteOptionsHops";
            this.numRouteOptionsHops.Size = new System.Drawing.Size(39, 20);
            this.numRouteOptionsHops.TabIndex = 0;
            this.numRouteOptionsHops.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tipToolTips.SetToolTip(this.numRouteOptionsHops, "A hop is a station to load/unload from");
            this.numRouteOptionsHops.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numRouteOptionsHops.Enter += new System.EventHandler(this.NumericUpDown_Enter);
            this.numRouteOptionsHops.MouseUp += new System.Windows.Forms.MouseEventHandler(this.NumericUpDown_MouseUp);
            // 
            // numCommandersCredits
            // 
            this.numCommandersCredits.Location = new System.Drawing.Point(396, 30);
            this.numCommandersCredits.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numCommandersCredits.Name = "numCommandersCredits";
            this.numCommandersCredits.Size = new System.Drawing.Size(125, 20);
            this.numCommandersCredits.TabIndex = 3;
            this.numCommandersCredits.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numCommandersCredits.ThousandsSeparator = true;
            this.tipToolTips.SetToolTip(this.numCommandersCredits, "Current credits");
            this.numCommandersCredits.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numCommandersCredits.Enter += new System.EventHandler(this.NumericUpDown_Enter);
            this.numCommandersCredits.MouseUp += new System.Windows.Forms.MouseEventHandler(this.NumericUpDown_MouseUp);
            // 
            // numUnladenLy
            // 
            this.numUnladenLy.DecimalPlaces = 2;
            this.numUnladenLy.Location = new System.Drawing.Point(184, 30);
            this.numUnladenLy.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            131072});
            this.numUnladenLy.Name = "numUnladenLy";
            this.numUnladenLy.Size = new System.Drawing.Size(53, 20);
            this.numUnladenLy.TabIndex = 4;
            this.numUnladenLy.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tipToolTips.SetToolTip(this.numUnladenLy, "Distance that can be travelled while unladen (including fuel)");
            this.numUnladenLy.Enter += new System.EventHandler(this.NumericUpDown_Enter);
            this.numUnladenLy.MouseUp += new System.Windows.Forms.MouseEventHandler(this.NumericUpDown_MouseUp);
            // 
            // numLadenLy
            // 
            this.numLadenLy.DecimalPlaces = 2;
            this.numLadenLy.Location = new System.Drawing.Point(60, 30);
            this.numLadenLy.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            131072});
            this.numLadenLy.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numLadenLy.Name = "numLadenLy";
            this.numLadenLy.Size = new System.Drawing.Size(53, 20);
            this.numLadenLy.TabIndex = 2;
            this.numLadenLy.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tipToolTips.SetToolTip(this.numLadenLy, "Distance that can be travelled while fully laden (including fuel)");
            this.numLadenLy.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numLadenLy.Enter += new System.EventHandler(this.NumericUpDown_Enter);
            this.numLadenLy.MouseUp += new System.Windows.Forms.MouseEventHandler(this.NumericUpDown_MouseUp);
            // 
            // lblRunOptionsRoutes
            // 
            this.lblRunOptionsRoutes.AutoSize = true;
            this.lblRunOptionsRoutes.Location = new System.Drawing.Point(81, 114);
            this.lblRunOptionsRoutes.Name = "lblRunOptionsRoutes";
            this.lblRunOptionsRoutes.Size = new System.Drawing.Size(44, 13);
            this.lblRunOptionsRoutes.TabIndex = 58;
            this.lblRunOptionsRoutes.Tag = "";
            this.lblRunOptionsRoutes.Text = "Routes:";
            this.tipToolTips.SetToolTip(this.lblRunOptionsRoutes, "Generates this many routes for a Run");
            // 
            // cboMethod
            // 
            this.cboMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboMethod.Items.AddRange(new object[] {
            "Run",
            "Buy",
            "Sell",
            "Rares",
            "Trade",
            "Market",
            "ShipVendor",
            "Navigation",
            "OldData",
            "Local",
            "Station"});
            this.cboMethod.Location = new System.Drawing.Point(699, 2);
            this.cboMethod.Name = "cboMethod";
            this.cboMethod.Size = new System.Drawing.Size(82, 21);
            this.cboMethod.TabIndex = 1;
            this.cboMethod.TabStop = false;
            this.tipToolTips.SetToolTip(this.cboMethod, "Select the command to run");
            this.cboMethod.SelectedIndexChanged += new System.EventHandler(this.MethodComboBox_SelectedIndexChanged);
            // 
            // txtVia
            // 
            this.txtVia.ContextMenuStrip = this.mnuSetValues;
            this.txtVia.Location = new System.Drawing.Point(61, 29);
            this.txtVia.Name = "txtVia";
            this.txtVia.Size = new System.Drawing.Size(311, 20);
            this.txtVia.TabIndex = 19;
            this.txtVia.TabStop = false;
            this.tipToolTips.SetToolTip(this.txtVia, "Attempt to route through these systems, delimited by comma");
            // 
            // lblVia
            // 
            this.lblVia.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblVia.AutoSize = true;
            this.lblVia.Location = new System.Drawing.Point(31, 31);
            this.lblVia.Name = "lblVia";
            this.lblVia.Size = new System.Drawing.Size(25, 13);
            this.lblVia.TabIndex = 45;
            this.lblVia.Text = "Via:";
            this.tipToolTips.SetToolTip(this.lblVia, "Attempt to route through these systems, delimited by comma");
            // 
            // lblRunOptionsStartJumps
            // 
            this.lblRunOptionsStartJumps.AutoSize = true;
            this.lblRunOptionsStartJumps.Location = new System.Drawing.Point(60, 63);
            this.lblRunOptionsStartJumps.Name = "lblRunOptionsStartJumps";
            this.lblRunOptionsStartJumps.Size = new System.Drawing.Size(65, 13);
            this.lblRunOptionsStartJumps.TabIndex = 43;
            this.lblRunOptionsStartJumps.Text = "Start Jumps:";
            this.tipToolTips.SetToolTip(this.lblRunOptionsStartJumps, "Try to route starting from this many jumps away from the source system (volatile)" +
        "");
            // 
            // lblRunOptionsEndJumps
            // 
            this.lblRunOptionsEndJumps.AutoSize = true;
            this.lblRunOptionsEndJumps.Enabled = false;
            this.lblRunOptionsEndJumps.Location = new System.Drawing.Point(63, 88);
            this.lblRunOptionsEndJumps.Name = "lblRunOptionsEndJumps";
            this.lblRunOptionsEndJumps.Size = new System.Drawing.Size(62, 13);
            this.lblRunOptionsEndJumps.TabIndex = 41;
            this.lblRunOptionsEndJumps.Text = "End Jumps:";
            this.tipToolTips.SetToolTip(this.lblRunOptionsEndJumps, "Try to finish this many jumps away from the destination, this requires a destinat" +
        "ion (volatile)");
            // 
            // txtAvoid
            // 
            this.txtAvoid.ContextMenuStrip = this.mnuSetValues;
            this.txtAvoid.Location = new System.Drawing.Point(61, 5);
            this.txtAvoid.Name = "txtAvoid";
            this.txtAvoid.Size = new System.Drawing.Size(311, 20);
            this.txtAvoid.TabIndex = 18;
            this.txtAvoid.TabStop = false;
            this.tipToolTips.SetToolTip(this.txtAvoid, "Avoids can include system/station and items delimited by comma");
            this.txtAvoid.TextChanged += new System.EventHandler(this.TxtAvoid_TextChanged);
            // 
            // lblAvoid
            // 
            this.lblAvoid.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblAvoid.AutoSize = true;
            this.lblAvoid.Location = new System.Drawing.Point(22, 8);
            this.lblAvoid.Name = "lblAvoid";
            this.lblAvoid.Size = new System.Drawing.Size(37, 13);
            this.lblAvoid.TabIndex = 39;
            this.lblAvoid.Text = "Avoid:";
            this.tipToolTips.SetToolTip(this.lblAvoid, "Avoids can include system/station and items delimited by comma");
            // 
            // chkRunOptionsBlkMkt
            // 
            this.chkRunOptionsBlkMkt.AutoSize = true;
            this.chkRunOptionsBlkMkt.Location = new System.Drawing.Point(3, 58);
            this.chkRunOptionsBlkMkt.Name = "chkRunOptionsBlkMkt";
            this.chkRunOptionsBlkMkt.Size = new System.Drawing.Size(51, 17);
            this.chkRunOptionsBlkMkt.TabIndex = 2;
            this.chkRunOptionsBlkMkt.TabStop = false;
            this.chkRunOptionsBlkMkt.Text = "BMkt";
            this.tipToolTips.SetToolTip(this.chkRunOptionsBlkMkt, "Require stations with a black market");
            this.chkRunOptionsBlkMkt.UseVisualStyleBackColor = true;
            this.chkRunOptionsBlkMkt.Click += new System.EventHandler(this.BmktCheckBox_Click);
            // 
            // lblRouteOptionsHops
            // 
            this.lblRouteOptionsHops.AutoSize = true;
            this.lblRouteOptionsHops.Location = new System.Drawing.Point(10, 7);
            this.lblRouteOptionsHops.Name = "lblRouteOptionsHops";
            this.lblRouteOptionsHops.Size = new System.Drawing.Size(35, 13);
            this.lblRouteOptionsHops.TabIndex = 29;
            this.lblRouteOptionsHops.Text = "Hops:";
            this.tipToolTips.SetToolTip(this.lblRouteOptionsHops, "A hop is a station to load/unload from");
            // 
            // btnDbUpdate
            // 
            this.btnDbUpdate.Location = new System.Drawing.Point(419, 0);
            this.btnDbUpdate.Name = "btnDbUpdate";
            this.btnDbUpdate.Size = new System.Drawing.Size(68, 23);
            this.btnDbUpdate.TabIndex = 24;
            this.btnDbUpdate.TabStop = false;
            this.btnDbUpdate.Text = "Update DB";
            this.tipToolTips.SetToolTip(this.btnDbUpdate, "Update the database based on time from previous update");
            this.btnDbUpdate.UseVisualStyleBackColor = true;
            this.btnDbUpdate.Click += new System.EventHandler(this.BtnDbUpdate_Click);
            // 
            // btnGetSystem
            // 
            this.btnGetSystem.Location = new System.Drawing.Point(334, 0);
            this.btnGetSystem.Name = "btnGetSystem";
            this.btnGetSystem.Size = new System.Drawing.Size(23, 23);
            this.btnGetSystem.TabIndex = 7;
            this.btnGetSystem.TabStop = false;
            this.btnGetSystem.Text = "C";
            this.tipToolTips.SetToolTip(this.btnGetSystem, "Get the most recent systems from the network logs (Ctrl+Click for a full refresh)" +
        "");
            this.btnGetSystem.UseVisualStyleBackColor = true;
            this.btnGetSystem.Click += new System.EventHandler(this.GetSystemButton_Click);
            // 
            // lblSourceSystem
            // 
            this.lblSourceSystem.AutoSize = true;
            this.lblSourceSystem.Location = new System.Drawing.Point(47, 5);
            this.lblSourceSystem.Name = "lblSourceSystem";
            this.lblSourceSystem.Size = new System.Drawing.Size(44, 13);
            this.lblSourceSystem.TabIndex = 2;
            this.lblSourceSystem.Text = "Source:";
            this.tipToolTips.SetToolTip(this.lblSourceSystem, "Starting point in the form of system or system/station");
            // 
            // lblPadSize
            // 
            this.lblPadSize.AutoSize = true;
            this.lblPadSize.Location = new System.Drawing.Point(284, 8);
            this.lblPadSize.Name = "lblPadSize";
            this.lblPadSize.Size = new System.Drawing.Size(34, 13);
            this.lblPadSize.TabIndex = 34;
            this.lblPadSize.Text = "Pads:";
            this.tipToolTips.SetToolTip(this.lblPadSize, "Minimum pad sizes to consider a hop, can be M, L, and/or ?");
            // 
            // txtPadSize
            // 
            this.txtPadSize.Location = new System.Drawing.Point(320, 5);
            this.txtPadSize.MaxLength = 3;
            this.txtPadSize.Name = "txtPadSize";
            this.txtPadSize.Size = new System.Drawing.Size(32, 20);
            this.txtPadSize.TabIndex = 3;
            this.txtPadSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tipToolTips.SetToolTip(this.txtPadSize, "Minimum pad sizes to consider a hop, can be M, L, and/or ?");
            this.txtPadSize.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.PadSize_KeyPress);
            // 
            // lblRouteOptionsAge
            // 
            this.lblRouteOptionsAge.AutoSize = true;
            this.lblRouteOptionsAge.Location = new System.Drawing.Point(43, 99);
            this.lblRouteOptionsAge.Name = "lblRouteOptionsAge";
            this.lblRouteOptionsAge.Size = new System.Drawing.Size(29, 13);
            this.lblRouteOptionsAge.TabIndex = 51;
            this.lblRouteOptionsAge.Text = "Age:";
            this.tipToolTips.SetToolTip(this.lblRouteOptionsAge, "Filter any hops based on the age of their recent data, up to 30 days");
            // 
            // numRouteOptionsAge
            // 
            this.numRouteOptionsAge.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.numRouteOptionsAge.ContextMenuStrip = this.mnuSetValues;
            this.numRouteOptionsAge.Location = new System.Drawing.Point(78, 95);
            this.numRouteOptionsAge.Maximum = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.numRouteOptionsAge.Name = "numRouteOptionsAge";
            this.numRouteOptionsAge.Size = new System.Drawing.Size(60, 20);
            this.numRouteOptionsAge.TabIndex = 4;
            this.numRouteOptionsAge.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tipToolTips.SetToolTip(this.numRouteOptionsAge, "Filter any hops based on the age of their recent data, up to 30 days");
            this.numRouteOptionsAge.Enter += new System.EventHandler(this.NumericUpDown_Enter);
            this.numRouteOptionsAge.MouseUp += new System.Windows.Forms.MouseEventHandler(this.NumericUpDown_MouseUp);
            // 
            // lblRouteOptionsGpt
            // 
            this.lblRouteOptionsGpt.AutoSize = true;
            this.lblRouteOptionsGpt.Location = new System.Drawing.Point(6, 6);
            this.lblRouteOptionsGpt.Name = "lblRouteOptionsGpt";
            this.lblRouteOptionsGpt.Size = new System.Drawing.Size(52, 13);
            this.lblRouteOptionsGpt.TabIndex = 27;
            this.lblRouteOptionsGpt.Text = "Min GPT:";
            this.tipToolTips.SetToolTip(this.lblRouteOptionsGpt, "Minimum profit in credits per ton on any hop");
            // 
            // cboSourceSystem
            // 
            this.cboSourceSystem.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cboSourceSystem.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.cboSourceSystem.ContextMenuStrip = this.mnuSetValues;
            this.cboSourceSystem.Location = new System.Drawing.Point(101, 1);
            this.cboSourceSystem.Name = "cboSourceSystem";
            this.cboSourceSystem.Size = new System.Drawing.Size(213, 21);
            this.cboSourceSystem.TabIndex = 0;
            this.tipToolTips.SetToolTip(this.cboSourceSystem, "Starting point in the form of system or system/station\r\nCtrl+Enter adds a System/" +
        "Station to the favorites\r\nShift+Enter removes a System/Station from the favorite" +
        "s");
            this.cboSourceSystem.DropDown += new System.EventHandler(this.ComboBox_DropDown);
            this.cboSourceSystem.DropDownClosed += new System.EventHandler(this.ComboBox_DropDownClosed);
            this.cboSourceSystem.TextChanged += new System.EventHandler(this.SrcSystemComboBox_TextChanged);
            this.cboSourceSystem.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SrcSystemComboBox_KeyDown);
            // 
            // lblRouteOptionsMargin
            // 
            this.lblRouteOptionsMargin.AutoSize = true;
            this.lblRouteOptionsMargin.Location = new System.Drawing.Point(16, 52);
            this.lblRouteOptionsMargin.Name = "lblRouteOptionsMargin";
            this.lblRouteOptionsMargin.Size = new System.Drawing.Size(42, 13);
            this.lblRouteOptionsMargin.TabIndex = 53;
            this.lblRouteOptionsMargin.Text = "Margin:";
            this.tipToolTips.SetToolTip(this.lblRouteOptionsMargin, "Profit margin variance");
            // 
            // numRouteOptionsMargin
            // 
            this.numRouteOptionsMargin.ContextMenuStrip = this.mnuSetValues;
            this.numRouteOptionsMargin.DecimalPlaces = 2;
            this.numRouteOptionsMargin.Location = new System.Drawing.Point(61, 48);
            this.numRouteOptionsMargin.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            131072});
            this.numRouteOptionsMargin.Name = "numRouteOptionsMargin";
            this.numRouteOptionsMargin.Size = new System.Drawing.Size(60, 20);
            this.numRouteOptionsMargin.TabIndex = 2;
            this.numRouteOptionsMargin.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tipToolTips.SetToolTip(this.numRouteOptionsMargin, "Profit margin variance (<1.00)");
            this.numRouteOptionsMargin.Enter += new System.EventHandler(this.NumericUpDown_Enter);
            this.numRouteOptionsMargin.MouseUp += new System.Windows.Forms.MouseEventHandler(this.NumericUpDown_MouseUp);
            // 
            // lblRouteOptionsCargoLimit
            // 
            this.lblRouteOptionsCargoLimit.AutoSize = true;
            this.lblRouteOptionsCargoLimit.Location = new System.Drawing.Point(3, 52);
            this.lblRouteOptionsCargoLimit.Name = "lblRouteOptionsCargoLimit";
            this.lblRouteOptionsCargoLimit.Size = new System.Drawing.Size(62, 13);
            this.lblRouteOptionsCargoLimit.TabIndex = 13;
            this.lblRouteOptionsCargoLimit.Text = "Cargo Limit:";
            this.tipToolTips.SetToolTip(this.lblRouteOptionsCargoLimit, "Limit each commodity purchased to this amount on a hop");
            // 
            // lblRouteOptionsMaxGpt
            // 
            this.lblRouteOptionsMaxGpt.AutoSize = true;
            this.lblRouteOptionsMaxGpt.Location = new System.Drawing.Point(3, 29);
            this.lblRouteOptionsMaxGpt.Name = "lblRouteOptionsMaxGpt";
            this.lblRouteOptionsMaxGpt.Size = new System.Drawing.Size(55, 13);
            this.lblRouteOptionsMaxGpt.TabIndex = 55;
            this.lblRouteOptionsMaxGpt.Text = "Max GPT:";
            this.tipToolTips.SetToolTip(this.lblRouteOptionsMaxGpt, "Maximum profit in credits per ton on any hop");
            // 
            // numRouteOptionsMaxGpt
            // 
            this.numRouteOptionsMaxGpt.ContextMenuStrip = this.mnuSetValues;
            this.numRouteOptionsMaxGpt.Location = new System.Drawing.Point(61, 25);
            this.numRouteOptionsMaxGpt.Maximum = new decimal(new int[] {
            20000,
            0,
            0,
            0});
            this.numRouteOptionsMaxGpt.Name = "numRouteOptionsMaxGpt";
            this.numRouteOptionsMaxGpt.Size = new System.Drawing.Size(60, 20);
            this.numRouteOptionsMaxGpt.TabIndex = 1;
            this.numRouteOptionsMaxGpt.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numRouteOptionsMaxGpt.ThousandsSeparator = true;
            this.tipToolTips.SetToolTip(this.numRouteOptionsMaxGpt, "Maximum profit in credits per ton on any hop");
            this.numRouteOptionsMaxGpt.Enter += new System.EventHandler(this.NumericUpDown_Enter);
            this.numRouteOptionsMaxGpt.MouseUp += new System.Windows.Forms.MouseEventHandler(this.NumericUpDown_MouseUp);
            // 
            // btnMiniMode
            // 
            this.btnMiniMode.Enabled = false;
            this.btnMiniMode.Location = new System.Drawing.Point(363, 0);
            this.btnMiniMode.Name = "btnMiniMode";
            this.btnMiniMode.Size = new System.Drawing.Size(23, 23);
            this.btnMiniMode.TabIndex = 1;
            this.btnMiniMode.TabStop = false;
            this.btnMiniMode.Text = "&T";
            this.tipToolTips.SetToolTip(this.btnMiniMode, "Switch to a minimal TreeView mode for Run output (ESC to exit)");
            this.btnMiniMode.UseVisualStyleBackColor = true;
            this.btnMiniMode.Click += new System.EventHandler(this.MiniModeButton_Click);
            // 
            // cboCommandersShips
            // 
            this.cboCommandersShips.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboCommandersShips.Location = new System.Drawing.Point(6, 4);
            this.cboCommandersShips.Name = "cboCommandersShips";
            this.cboCommandersShips.Size = new System.Drawing.Size(272, 21);
            this.cboCommandersShips.TabIndex = 56;
            this.cboCommandersShips.TabStop = false;
            this.tipToolTips.SetToolTip(this.cboCommandersShips, "Select a ship");
            this.cboCommandersShips.SelectionChangeCommitted += new System.EventHandler(this.AltConfigBox_SelectionChangeCommitted);
            // 
            // icoUpdateNotify
            // 
            this.icoUpdateNotify.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.icoUpdateNotify.Image = global::TDHelper.Properties.Resources.LightningBolt;
            this.icoUpdateNotify.Location = new System.Drawing.Point(167, 655);
            this.icoUpdateNotify.Name = "icoUpdateNotify";
            this.icoUpdateNotify.Size = new System.Drawing.Size(21, 21);
            this.icoUpdateNotify.TabIndex = 59;
            this.icoUpdateNotify.TabStop = false;
            this.tipToolTips.SetToolTip(this.icoUpdateNotify, "An update to TDHelper is available! Restart to update.");
            this.icoUpdateNotify.Visible = false;
            // 
            // numRouteOptionsStock
            // 
            this.numRouteOptionsStock.ContextMenuStrip = this.mnuSetValues;
            this.numRouteOptionsStock.Location = new System.Drawing.Point(67, 3);
            this.numRouteOptionsStock.Maximum = new decimal(new int[] {
            9999999,
            0,
            0,
            0});
            this.numRouteOptionsStock.Name = "numRouteOptionsStock";
            this.numRouteOptionsStock.Size = new System.Drawing.Size(71, 20);
            this.numRouteOptionsStock.TabIndex = 8;
            this.numRouteOptionsStock.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numRouteOptionsStock.ThousandsSeparator = true;
            this.tipToolTips.SetToolTip(this.numRouteOptionsStock, "Filter hops below this level of stock");
            this.numRouteOptionsStock.Enter += new System.EventHandler(this.NumericUpDown_Enter);
            this.numRouteOptionsStock.MouseUp += new System.Windows.Forms.MouseEventHandler(this.NumericUpDown_MouseUp);
            // 
            // lblStock
            // 
            this.lblStock.AutoSize = true;
            this.lblStock.Location = new System.Drawing.Point(15, 7);
            this.lblStock.Name = "lblStock";
            this.lblStock.Size = new System.Drawing.Size(50, 13);
            this.lblStock.TabIndex = 57;
            this.lblStock.Text = "    Stock:";
            this.tipToolTips.SetToolTip(this.lblStock, "Filter hops below this level of stock");
            // 
            // lblRunOptionsLoopInt
            // 
            this.lblRunOptionsLoopInt.AutoSize = true;
            this.lblRunOptionsLoopInt.Location = new System.Drawing.Point(76, 36);
            this.lblRunOptionsLoopInt.Name = "lblRunOptionsLoopInt";
            this.lblRunOptionsLoopInt.Size = new System.Drawing.Size(49, 13);
            this.lblRunOptionsLoopInt.TabIndex = 59;
            this.lblRunOptionsLoopInt.Text = "Loop Int:";
            this.tipToolTips.SetToolTip(this.lblRunOptionsLoopInt, "Minimum hops between visiting the same station");
            // 
            // numRunOptionsLoopInt
            // 
            this.numRunOptionsLoopInt.ContextMenuStrip = this.mnuSetValues;
            this.numRunOptionsLoopInt.Location = new System.Drawing.Point(131, 34);
            this.numRunOptionsLoopInt.Name = "numRunOptionsLoopInt";
            this.numRunOptionsLoopInt.Size = new System.Drawing.Size(46, 20);
            this.numRunOptionsLoopInt.TabIndex = 10;
            this.numRunOptionsLoopInt.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tipToolTips.SetToolTip(this.numRunOptionsLoopInt, "Minimum hops between visiting the same station");
            this.numRunOptionsLoopInt.Enter += new System.EventHandler(this.NumericUpDown_Enter);
            this.numRunOptionsLoopInt.MouseUp += new System.Windows.Forms.MouseEventHandler(this.NumericUpDown_MouseUp);
            // 
            // chkRunOptionsShorten
            // 
            this.chkRunOptionsShorten.AutoSize = true;
            this.chkRunOptionsShorten.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkRunOptionsShorten.Enabled = false;
            this.chkRunOptionsShorten.Location = new System.Drawing.Point(224, 32);
            this.chkRunOptionsShorten.Name = "chkRunOptionsShorten";
            this.chkRunOptionsShorten.Size = new System.Drawing.Size(63, 17);
            this.chkRunOptionsShorten.TabIndex = 61;
            this.chkRunOptionsShorten.TabStop = false;
            this.chkRunOptionsShorten.Text = "Shorten";
            this.tipToolTips.SetToolTip(this.chkRunOptionsShorten, "Finds the highest gainful route with the least hops (requires a Destination)");
            this.chkRunOptionsShorten.UseVisualStyleBackColor = true;
            this.chkRunOptionsShorten.Click += new System.EventHandler(this.ShortenCheckBox_Click);
            // 
            // numRouteOptionsDemand
            // 
            this.numRouteOptionsDemand.ContextMenuStrip = this.mnuSetValues;
            this.numRouteOptionsDemand.Location = new System.Drawing.Point(67, 26);
            this.numRouteOptionsDemand.Maximum = new decimal(new int[] {
            9999999,
            0,
            0,
            0});
            this.numRouteOptionsDemand.Name = "numRouteOptionsDemand";
            this.numRouteOptionsDemand.Size = new System.Drawing.Size(71, 20);
            this.numRouteOptionsDemand.TabIndex = 1;
            this.numRouteOptionsDemand.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numRouteOptionsDemand.ThousandsSeparator = true;
            this.tipToolTips.SetToolTip(this.numRouteOptionsDemand, "Filter hops below this level of demand");
            this.numRouteOptionsDemand.Enter += new System.EventHandler(this.NumericUpDown_Enter);
            this.numRouteOptionsDemand.MouseUp += new System.Windows.Forms.MouseEventHandler(this.NumericUpDown_MouseUp);
            // 
            // lblRouteOptionsDemand
            // 
            this.lblRouteOptionsDemand.AutoSize = true;
            this.lblRouteOptionsDemand.Location = new System.Drawing.Point(15, 30);
            this.lblRouteOptionsDemand.Name = "lblRouteOptionsDemand";
            this.lblRouteOptionsDemand.Size = new System.Drawing.Size(50, 13);
            this.lblRouteOptionsDemand.TabIndex = 61;
            this.lblRouteOptionsDemand.Text = "Demand:";
            this.tipToolTips.SetToolTip(this.lblRouteOptionsDemand, "Filter hops below this level of demand");
            // 
            // chkRunOptionsJumps
            // 
            this.chkRunOptionsJumps.AutoSize = true;
            this.chkRunOptionsJumps.Location = new System.Drawing.Point(3, 104);
            this.chkRunOptionsJumps.Name = "chkRunOptionsJumps";
            this.chkRunOptionsJumps.Size = new System.Drawing.Size(56, 17);
            this.chkRunOptionsJumps.TabIndex = 62;
            this.chkRunOptionsJumps.TabStop = false;
            this.chkRunOptionsJumps.Text = "Jumps";
            this.tipToolTips.SetToolTip(this.chkRunOptionsJumps, "Show jumps between hops during a multi-hop route");
            this.chkRunOptionsJumps.UseVisualStyleBackColor = true;
            // 
            // btnCmdrProfile
            // 
            this.btnCmdrProfile.Location = new System.Drawing.Point(493, 0);
            this.btnCmdrProfile.Name = "btnCmdrProfile";
            this.btnCmdrProfile.Size = new System.Drawing.Size(75, 23);
            this.btnCmdrProfile.TabIndex = 68;
            this.btnCmdrProfile.TabStop = false;
            this.btnCmdrProfile.Text = "Cmdr Profile";
            this.tipToolTips.SetToolTip(this.btnCmdrProfile, "Download the commander\'s profile from Frontier and populate the available values." +
        "");
            this.btnCmdrProfile.UseVisualStyleBackColor = true;
            this.btnCmdrProfile.Click += new System.EventHandler(this.BtnCmdrProfile_Click);
            // 
            // btnSaveSettings
            // 
            this.btnSaveSettings.Location = new System.Drawing.Point(574, 0);
            this.btnSaveSettings.Name = "btnSaveSettings";
            this.btnSaveSettings.Size = new System.Drawing.Size(54, 23);
            this.btnSaveSettings.TabIndex = 69;
            this.btnSaveSettings.TabStop = false;
            this.btnSaveSettings.Text = "Save Settings";
            this.tipToolTips.SetToolTip(this.btnSaveSettings, "Save the settings immediately.");
            this.btnSaveSettings.UseVisualStyleBackColor = true;
            this.btnSaveSettings.Click += new System.EventHandler(this.BtnSaveSettings_Click);
            // 
            // lblRunOptionsPlanetary
            // 
            this.lblRunOptionsPlanetary.AutoSize = true;
            this.lblRunOptionsPlanetary.Location = new System.Drawing.Point(195, 114);
            this.lblRunOptionsPlanetary.Name = "lblRunOptionsPlanetary";
            this.lblRunOptionsPlanetary.Size = new System.Drawing.Size(54, 13);
            this.lblRunOptionsPlanetary.TabIndex = 69;
            this.lblRunOptionsPlanetary.Text = "Planetary:";
            this.tipToolTips.SetToolTip(this.lblRunOptionsPlanetary, "Limit result to stations with one of the specified planetary. Can be Y, N and/or " +
        "?");
            // 
            // txtRunOptionsPlanetary
            // 
            this.txtRunOptionsPlanetary.ContextMenuStrip = this.mnuSetValues;
            this.txtRunOptionsPlanetary.Location = new System.Drawing.Point(255, 111);
            this.txtRunOptionsPlanetary.MaxLength = 3;
            this.txtRunOptionsPlanetary.Name = "txtRunOptionsPlanetary";
            this.txtRunOptionsPlanetary.Size = new System.Drawing.Size(32, 20);
            this.txtRunOptionsPlanetary.TabIndex = 68;
            this.txtRunOptionsPlanetary.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tipToolTips.SetToolTip(this.txtRunOptionsPlanetary, "Limit result to stations with one of the specified planetary. Can be Y, N and/or " +
        "?");
            this.txtRunOptionsPlanetary.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Planetary_KeyPress);
            // 
            // btnSettings
            // 
            this.btnSettings.Image = ((System.Drawing.Image)(resources.GetObject("btnSettings.Image")));
            this.btnSettings.Location = new System.Drawing.Point(390, 0);
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.Size = new System.Drawing.Size(23, 23);
            this.btnSettings.TabIndex = 70;
            this.btnSettings.TabStop = false;
            this.tipToolTips.SetToolTip(this.btnSettings, "Configuration settings");
            this.btnSettings.UseVisualStyleBackColor = true;
            this.btnSettings.Click += new System.EventHandler(this.BtnSettings_Click);
            // 
            // btnRunOptionsSwap
            // 
            this.btnRunOptionsSwap.Location = new System.Drawing.Point(3, 3);
            this.btnRunOptionsSwap.Name = "btnRunOptionsSwap";
            this.btnRunOptionsSwap.Size = new System.Drawing.Size(23, 23);
            this.btnRunOptionsSwap.TabIndex = 70;
            this.btnRunOptionsSwap.TabStop = false;
            this.btnRunOptionsSwap.Text = "S";
            this.tipToolTips.SetToolTip(this.btnRunOptionsSwap, "Swap the contents of Source/Destination");
            this.btnRunOptionsSwap.UseVisualStyleBackColor = true;
            this.btnRunOptionsSwap.Click += new System.EventHandler(this.SwapSourceAndDestination);
            // 
            // lblBuyOptionsSupply
            // 
            this.lblBuyOptionsSupply.AutoSize = true;
            this.lblBuyOptionsSupply.Location = new System.Drawing.Point(202, 60);
            this.lblBuyOptionsSupply.Name = "lblBuyOptionsSupply";
            this.lblBuyOptionsSupply.Size = new System.Drawing.Size(51, 13);
            this.lblBuyOptionsSupply.TabIndex = 60;
            this.lblBuyOptionsSupply.Text = "   Supply:";
            this.tipToolTips.SetToolTip(this.lblBuyOptionsSupply, "Limit to stations known to have at least this much supply");
            // 
            // numBuyOptionsSupply
            // 
            this.numBuyOptionsSupply.ContextMenuStrip = this.mnuSetValues;
            this.numBuyOptionsSupply.Location = new System.Drawing.Point(259, 58);
            this.numBuyOptionsSupply.Maximum = new decimal(new int[] {
            9999999,
            0,
            0,
            0});
            this.numBuyOptionsSupply.Name = "numBuyOptionsSupply";
            this.numBuyOptionsSupply.Size = new System.Drawing.Size(55, 20);
            this.numBuyOptionsSupply.TabIndex = 61;
            this.numBuyOptionsSupply.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numBuyOptionsSupply.ThousandsSeparator = true;
            this.tipToolTips.SetToolTip(this.numBuyOptionsSupply, "Limit to stations known to have at least this much supply");
            // 
            // numBuyOptionsLimit
            // 
            this.numBuyOptionsLimit.ContextMenuStrip = this.mnuSetValues;
            this.numBuyOptionsLimit.Location = new System.Drawing.Point(139, 83);
            this.numBuyOptionsLimit.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numBuyOptionsLimit.Name = "numBuyOptionsLimit";
            this.numBuyOptionsLimit.Size = new System.Drawing.Size(53, 20);
            this.numBuyOptionsLimit.TabIndex = 66;
            this.numBuyOptionsLimit.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tipToolTips.SetToolTip(this.numBuyOptionsLimit, "Limit each commodity purchased to this amount on a hop");
            // 
            // lblBuyOptionsLimit
            // 
            this.lblBuyOptionsLimit.AutoSize = true;
            this.lblBuyOptionsLimit.Location = new System.Drawing.Point(88, 85);
            this.lblBuyOptionsLimit.Name = "lblBuyOptionsLimit";
            this.lblBuyOptionsLimit.Size = new System.Drawing.Size(45, 13);
            this.lblBuyOptionsLimit.TabIndex = 67;
            this.lblBuyOptionsLimit.Text = "Results:";
            this.tipToolTips.SetToolTip(this.lblBuyOptionsLimit, "Limit the number of results shown.");
            // 
            // lblBuyOptionsNearLy
            // 
            this.lblBuyOptionsNearLy.AutoSize = true;
            this.lblBuyOptionsNearLy.Location = new System.Drawing.Point(84, 59);
            this.lblBuyOptionsNearLy.Name = "lblBuyOptionsNearLy";
            this.lblBuyOptionsNearLy.Size = new System.Drawing.Size(49, 13);
            this.lblBuyOptionsNearLy.TabIndex = 68;
            this.lblBuyOptionsNearLy.Text = "Near LY:";
            this.tipToolTips.SetToolTip(this.lblBuyOptionsNearLy, "Distance to search for local system/station info.");
            // 
            // numBuyOptionsNearLy
            // 
            this.numBuyOptionsNearLy.ContextMenuStrip = this.mnuSetValues;
            this.numBuyOptionsNearLy.DecimalPlaces = 2;
            this.numBuyOptionsNearLy.Location = new System.Drawing.Point(139, 57);
            this.numBuyOptionsNearLy.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            131072});
            this.numBuyOptionsNearLy.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numBuyOptionsNearLy.Name = "numBuyOptionsNearLy";
            this.numBuyOptionsNearLy.Size = new System.Drawing.Size(53, 20);
            this.numBuyOptionsNearLy.TabIndex = 69;
            this.numBuyOptionsNearLy.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tipToolTips.SetToolTip(this.numBuyOptionsNearLy, "Distance to search for local system/station info.");
            this.numBuyOptionsNearLy.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // txtBuyOptionsAvoid
            // 
            this.txtBuyOptionsAvoid.ContextMenuStrip = this.mnuSetValues;
            this.txtBuyOptionsAvoid.Location = new System.Drawing.Point(70, 5);
            this.txtBuyOptionsAvoid.Name = "txtBuyOptionsAvoid";
            this.txtBuyOptionsAvoid.Size = new System.Drawing.Size(244, 20);
            this.txtBuyOptionsAvoid.TabIndex = 72;
            this.txtBuyOptionsAvoid.TabStop = false;
            this.tipToolTips.SetToolTip(this.txtBuyOptionsAvoid, "Avoids can include system/station and items delimited by comma");
            // 
            // lblBuyOptionsAvoid
            // 
            this.lblBuyOptionsAvoid.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblBuyOptionsAvoid.AutoSize = true;
            this.lblBuyOptionsAvoid.Location = new System.Drawing.Point(27, 8);
            this.lblBuyOptionsAvoid.Name = "lblBuyOptionsAvoid";
            this.lblBuyOptionsAvoid.Size = new System.Drawing.Size(37, 13);
            this.lblBuyOptionsAvoid.TabIndex = 73;
            this.lblBuyOptionsAvoid.Text = "Avoid:";
            this.tipToolTips.SetToolTip(this.lblBuyOptionsAvoid, "Avoids can include system/station and items delimited by comma");
            // 
            // lblBuyOptionsPlanetary
            // 
            this.lblBuyOptionsPlanetary.AutoSize = true;
            this.lblBuyOptionsPlanetary.Location = new System.Drawing.Point(7, 111);
            this.lblBuyOptionsPlanetary.Name = "lblBuyOptionsPlanetary";
            this.lblBuyOptionsPlanetary.Size = new System.Drawing.Size(54, 13);
            this.lblBuyOptionsPlanetary.TabIndex = 75;
            this.lblBuyOptionsPlanetary.Text = "Planetary:";
            this.tipToolTips.SetToolTip(this.lblBuyOptionsPlanetary, "Limit result to stations with one of the specified planetary. Can be Y, N and/or " +
        "?");
            // 
            // txtBuyOptionsPlanetary
            // 
            this.txtBuyOptionsPlanetary.ContextMenuStrip = this.mnuSetValues;
            this.txtBuyOptionsPlanetary.Location = new System.Drawing.Point(70, 108);
            this.txtBuyOptionsPlanetary.MaxLength = 3;
            this.txtBuyOptionsPlanetary.Name = "txtBuyOptionsPlanetary";
            this.txtBuyOptionsPlanetary.Size = new System.Drawing.Size(32, 20);
            this.txtBuyOptionsPlanetary.TabIndex = 74;
            this.txtBuyOptionsPlanetary.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tipToolTips.SetToolTip(this.txtBuyOptionsPlanetary, "Limit result to stations with one of the specified planetary. Can be Y, N and/or " +
        "?");
            this.txtBuyOptionsPlanetary.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Planetary_KeyPress);
            // 
            // chkBuyOptionsBlkMkt
            // 
            this.chkBuyOptionsBlkMkt.AutoSize = true;
            this.chkBuyOptionsBlkMkt.Location = new System.Drawing.Point(7, 82);
            this.chkBuyOptionsBlkMkt.Name = "chkBuyOptionsBlkMkt";
            this.chkBuyOptionsBlkMkt.Size = new System.Drawing.Size(51, 17);
            this.chkBuyOptionsBlkMkt.TabIndex = 76;
            this.chkBuyOptionsBlkMkt.TabStop = false;
            this.chkBuyOptionsBlkMkt.Text = "BMkt";
            this.tipToolTips.SetToolTip(this.chkBuyOptionsBlkMkt, "Require stations with a black market");
            this.chkBuyOptionsBlkMkt.UseVisualStyleBackColor = true;
            // 
            // lblBuyOptionsPads
            // 
            this.lblBuyOptionsPads.AutoSize = true;
            this.lblBuyOptionsPads.Location = new System.Drawing.Point(112, 112);
            this.lblBuyOptionsPads.Name = "lblBuyOptionsPads";
            this.lblBuyOptionsPads.Size = new System.Drawing.Size(34, 13);
            this.lblBuyOptionsPads.TabIndex = 78;
            this.lblBuyOptionsPads.Text = "Pads:";
            this.tipToolTips.SetToolTip(this.lblBuyOptionsPads, "Minimum pad sizes to consider a hop, can be M, L, and/or ?");
            // 
            // txtBuyOptionsPads
            // 
            this.txtBuyOptionsPads.ContextMenuStrip = this.mnuSetValues;
            this.txtBuyOptionsPads.Location = new System.Drawing.Point(152, 109);
            this.txtBuyOptionsPads.MaxLength = 3;
            this.txtBuyOptionsPads.Name = "txtBuyOptionsPads";
            this.txtBuyOptionsPads.Size = new System.Drawing.Size(32, 20);
            this.txtBuyOptionsPads.TabIndex = 77;
            this.txtBuyOptionsPads.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tipToolTips.SetToolTip(this.txtBuyOptionsPads, "Minimum pad sizes to consider a hop, can be M, L, and/or ?");
            this.txtBuyOptionsPads.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.PadSize_KeyPress);
            // 
            // chkBuyOptionsOneStop
            // 
            this.chkBuyOptionsOneStop.AutoSize = true;
            this.chkBuyOptionsOneStop.Location = new System.Drawing.Point(7, 59);
            this.chkBuyOptionsOneStop.Name = "chkBuyOptionsOneStop";
            this.chkBuyOptionsOneStop.Size = new System.Drawing.Size(57, 17);
            this.chkBuyOptionsOneStop.TabIndex = 79;
            this.chkBuyOptionsOneStop.TabStop = false;
            this.chkBuyOptionsOneStop.Text = "1-Stop";
            this.tipToolTips.SetToolTip(this.chkBuyOptionsOneStop, "Filters stations that don\'t contain all commodities searched for (volatile)");
            this.chkBuyOptionsOneStop.UseVisualStyleBackColor = true;
            this.chkBuyOptionsOneStop.CheckedChanged += new System.EventHandler(this.BuyOptionsOneStop_CheckedChanged);
            // 
            // numBuyOptionsAbove
            // 
            this.numBuyOptionsAbove.ContextMenuStrip = this.mnuSetValues;
            this.numBuyOptionsAbove.Location = new System.Drawing.Point(259, 84);
            this.numBuyOptionsAbove.Maximum = new decimal(new int[] {
            50000,
            0,
            0,
            0});
            this.numBuyOptionsAbove.Name = "numBuyOptionsAbove";
            this.numBuyOptionsAbove.Size = new System.Drawing.Size(55, 20);
            this.numBuyOptionsAbove.TabIndex = 80;
            this.numBuyOptionsAbove.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numBuyOptionsAbove.ThousandsSeparator = true;
            this.tipToolTips.SetToolTip(this.numBuyOptionsAbove, "Commodities above this price are filtered out");
            // 
            // lblBuyOptionsAbove
            // 
            this.lblBuyOptionsAbove.AutoSize = true;
            this.lblBuyOptionsAbove.Location = new System.Drawing.Point(212, 86);
            this.lblBuyOptionsAbove.Name = "lblBuyOptionsAbove";
            this.lblBuyOptionsAbove.Size = new System.Drawing.Size(41, 13);
            this.lblBuyOptionsAbove.TabIndex = 81;
            this.lblBuyOptionsAbove.Text = "Above:";
            this.tipToolTips.SetToolTip(this.lblBuyOptionsAbove, "Commodities above this price are filtered out");
            // 
            // numBuyOptionsBelow
            // 
            this.numBuyOptionsBelow.ContextMenuStrip = this.mnuSetValues;
            this.numBuyOptionsBelow.Location = new System.Drawing.Point(259, 110);
            this.numBuyOptionsBelow.Maximum = new decimal(new int[] {
            50000,
            0,
            0,
            0});
            this.numBuyOptionsBelow.Name = "numBuyOptionsBelow";
            this.numBuyOptionsBelow.Size = new System.Drawing.Size(55, 20);
            this.numBuyOptionsBelow.TabIndex = 82;
            this.numBuyOptionsBelow.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numBuyOptionsBelow.ThousandsSeparator = true;
            this.tipToolTips.SetToolTip(this.numBuyOptionsBelow, "Commodities above this price are filtered out");
            // 
            // lblBuyOptionsBelow
            // 
            this.lblBuyOptionsBelow.AutoSize = true;
            this.lblBuyOptionsBelow.Location = new System.Drawing.Point(214, 112);
            this.lblBuyOptionsBelow.Name = "lblBuyOptionsBelow";
            this.lblBuyOptionsBelow.Size = new System.Drawing.Size(39, 13);
            this.lblBuyOptionsBelow.TabIndex = 83;
            this.lblBuyOptionsBelow.Text = "Below:";
            this.tipToolTips.SetToolTip(this.lblBuyOptionsBelow, "Commodities below this price are filtered out");
            // 
            // cboBuyOptionsCommodities
            // 
            this.cboBuyOptionsCommodities.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cboBuyOptionsCommodities.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboBuyOptionsCommodities.Location = new System.Drawing.Point(70, 31);
            this.cboBuyOptionsCommodities.Name = "cboBuyOptionsCommodities";
            this.cboBuyOptionsCommodities.Size = new System.Drawing.Size(244, 21);
            this.cboBuyOptionsCommodities.TabIndex = 84;
            this.tipToolTips.SetToolTip(this.cboBuyOptionsCommodities, "Commodities to search for using Buy/Sell, and can be delimited by comma");
            // 
            // lblBuyOptionsCommodity
            // 
            this.lblBuyOptionsCommodity.AutoSize = true;
            this.lblBuyOptionsCommodity.Location = new System.Drawing.Point(3, 34);
            this.lblBuyOptionsCommodity.Name = "lblBuyOptionsCommodity";
            this.lblBuyOptionsCommodity.Size = new System.Drawing.Size(61, 13);
            this.lblBuyOptionsCommodity.TabIndex = 85;
            this.lblBuyOptionsCommodity.Text = "Commodity:";
            this.tipToolTips.SetToolTip(this.lblBuyOptionsCommodity, "Commodities to search for using Buy/Sell, and can be delimited by comma");
            // 
            // optBuyOptionsDistance
            // 
            this.optBuyOptionsDistance.AutoSize = true;
            this.optBuyOptionsDistance.Location = new System.Drawing.Point(88, 6);
            this.optBuyOptionsDistance.Name = "optBuyOptionsDistance";
            this.optBuyOptionsDistance.Size = new System.Drawing.Size(67, 17);
            this.optBuyOptionsDistance.TabIndex = 66;
            this.optBuyOptionsDistance.TabStop = true;
            this.optBuyOptionsDistance.Text = "Distance";
            this.tipToolTips.SetToolTip(this.optBuyOptionsDistance, "Sort items by distance and then price.");
            this.optBuyOptionsDistance.UseVisualStyleBackColor = true;
            // 
            // optBuyOptionsPrice
            // 
            this.optBuyOptionsPrice.AutoSize = true;
            this.optBuyOptionsPrice.Location = new System.Drawing.Point(161, 6);
            this.optBuyOptionsPrice.Name = "optBuyOptionsPrice";
            this.optBuyOptionsPrice.Size = new System.Drawing.Size(49, 17);
            this.optBuyOptionsPrice.TabIndex = 67;
            this.optBuyOptionsPrice.TabStop = true;
            this.optBuyOptionsPrice.Text = "Price";
            this.tipToolTips.SetToolTip(this.optBuyOptionsPrice, "Keeps items sorted by price when using --near\r\n (otherwise items are listed by di" +
        "stance and then price)");
            this.optBuyOptionsPrice.UseVisualStyleBackColor = true;
            this.optBuyOptionsPrice.CheckedChanged += new System.EventHandler(this.BuyOptionsPrice_CheckedChanged);
            // 
            // optBuyOptionsSupply
            // 
            this.optBuyOptionsSupply.AutoSize = true;
            this.optBuyOptionsSupply.Location = new System.Drawing.Point(216, 6);
            this.optBuyOptionsSupply.Name = "optBuyOptionsSupply";
            this.optBuyOptionsSupply.Size = new System.Drawing.Size(57, 17);
            this.optBuyOptionsSupply.TabIndex = 68;
            this.optBuyOptionsSupply.TabStop = true;
            this.optBuyOptionsSupply.Text = "Supply";
            this.tipToolTips.SetToolTip(this.optBuyOptionsSupply, "Sorts items by supply available first and then price");
            this.optBuyOptionsSupply.UseVisualStyleBackColor = true;
            // 
            // lblBuyOptionsSort
            // 
            this.lblBuyOptionsSort.AutoSize = true;
            this.lblBuyOptionsSort.Location = new System.Drawing.Point(6, 10);
            this.lblBuyOptionsSort.Name = "lblBuyOptionsSort";
            this.lblBuyOptionsSort.Size = new System.Drawing.Size(76, 13);
            this.lblBuyOptionsSort.TabIndex = 86;
            this.lblBuyOptionsSort.Text = "Sort results by:";
            this.tipToolTips.SetToolTip(this.lblBuyOptionsSort, "Limit result to stations with one of the specified planetary. Can be Y, N and/or " +
        "?");
            // 
            // lblSellOptionsSort
            // 
            this.lblSellOptionsSort.AutoSize = true;
            this.lblSellOptionsSort.Location = new System.Drawing.Point(6, 10);
            this.lblSellOptionsSort.Name = "lblSellOptionsSort";
            this.lblSellOptionsSort.Size = new System.Drawing.Size(76, 13);
            this.lblSellOptionsSort.TabIndex = 86;
            this.lblSellOptionsSort.Text = "Sort results by:";
            this.tipToolTips.SetToolTip(this.lblSellOptionsSort, "Limit result to stations with one of the specified planetary. Can be Y, N and/or " +
        "?");
            // 
            // optSellOptionsSupply
            // 
            this.optSellOptionsSupply.AutoSize = true;
            this.optSellOptionsSupply.Location = new System.Drawing.Point(88, 6);
            this.optSellOptionsSupply.Name = "optSellOptionsSupply";
            this.optSellOptionsSupply.Size = new System.Drawing.Size(57, 17);
            this.optSellOptionsSupply.TabIndex = 66;
            this.optSellOptionsSupply.TabStop = true;
            this.optSellOptionsSupply.Text = "Supply";
            this.tipToolTips.SetToolTip(this.optSellOptionsSupply, "Sorts items by supply available first and then price");
            this.optSellOptionsSupply.UseVisualStyleBackColor = true;
            // 
            // optSellOptionsPrice
            // 
            this.optSellOptionsPrice.AutoSize = true;
            this.optSellOptionsPrice.Location = new System.Drawing.Point(161, 6);
            this.optSellOptionsPrice.Name = "optSellOptionsPrice";
            this.optSellOptionsPrice.Size = new System.Drawing.Size(49, 17);
            this.optSellOptionsPrice.TabIndex = 67;
            this.optSellOptionsPrice.TabStop = true;
            this.optSellOptionsPrice.Text = "Price";
            this.tipToolTips.SetToolTip(this.optSellOptionsPrice, "Keeps items sorted by price when using --near\r\n (otherwise items are listed by di" +
        "stance and then price)");
            this.optSellOptionsPrice.UseVisualStyleBackColor = true;
            // 
            // cboSellOptionsCommodities
            // 
            this.cboSellOptionsCommodities.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cboSellOptionsCommodities.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboSellOptionsCommodities.Location = new System.Drawing.Point(70, 31);
            this.cboSellOptionsCommodities.Name = "cboSellOptionsCommodities";
            this.cboSellOptionsCommodities.Size = new System.Drawing.Size(244, 21);
            this.cboSellOptionsCommodities.TabIndex = 84;
            this.tipToolTips.SetToolTip(this.cboSellOptionsCommodities, "Commodities to search for using Buy/Sell, and can be delimited by comma");
            // 
            // lblSellOptionsCommodity
            // 
            this.lblSellOptionsCommodity.AutoSize = true;
            this.lblSellOptionsCommodity.Location = new System.Drawing.Point(3, 34);
            this.lblSellOptionsCommodity.Name = "lblSellOptionsCommodity";
            this.lblSellOptionsCommodity.Size = new System.Drawing.Size(61, 13);
            this.lblSellOptionsCommodity.TabIndex = 85;
            this.lblSellOptionsCommodity.Text = "Commodity:";
            this.tipToolTips.SetToolTip(this.lblSellOptionsCommodity, "Commodities to search for using Buy/Sell, and can be delimited by comma");
            // 
            // numSellOptionsBelow
            // 
            this.numSellOptionsBelow.ContextMenuStrip = this.mnuSetValues;
            this.numSellOptionsBelow.Location = new System.Drawing.Point(259, 110);
            this.numSellOptionsBelow.Maximum = new decimal(new int[] {
            50000,
            0,
            0,
            0});
            this.numSellOptionsBelow.Name = "numSellOptionsBelow";
            this.numSellOptionsBelow.Size = new System.Drawing.Size(55, 20);
            this.numSellOptionsBelow.TabIndex = 82;
            this.numSellOptionsBelow.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numSellOptionsBelow.ThousandsSeparator = true;
            this.tipToolTips.SetToolTip(this.numSellOptionsBelow, "Commodities above this price are filtered out");
            // 
            // lblSellOptionsBelow
            // 
            this.lblSellOptionsBelow.AutoSize = true;
            this.lblSellOptionsBelow.Location = new System.Drawing.Point(214, 112);
            this.lblSellOptionsBelow.Name = "lblSellOptionsBelow";
            this.lblSellOptionsBelow.Size = new System.Drawing.Size(39, 13);
            this.lblSellOptionsBelow.TabIndex = 83;
            this.lblSellOptionsBelow.Text = "Below:";
            this.tipToolTips.SetToolTip(this.lblSellOptionsBelow, "Commodities below this price are filtered out");
            // 
            // numSellOptionsAbove
            // 
            this.numSellOptionsAbove.ContextMenuStrip = this.mnuSetValues;
            this.numSellOptionsAbove.Location = new System.Drawing.Point(259, 84);
            this.numSellOptionsAbove.Maximum = new decimal(new int[] {
            50000,
            0,
            0,
            0});
            this.numSellOptionsAbove.Name = "numSellOptionsAbove";
            this.numSellOptionsAbove.Size = new System.Drawing.Size(55, 20);
            this.numSellOptionsAbove.TabIndex = 80;
            this.numSellOptionsAbove.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numSellOptionsAbove.ThousandsSeparator = true;
            this.tipToolTips.SetToolTip(this.numSellOptionsAbove, "Commodities above this price are filtered out");
            // 
            // lblSellOptionsAbove
            // 
            this.lblSellOptionsAbove.AutoSize = true;
            this.lblSellOptionsAbove.Location = new System.Drawing.Point(212, 86);
            this.lblSellOptionsAbove.Name = "lblSellOptionsAbove";
            this.lblSellOptionsAbove.Size = new System.Drawing.Size(41, 13);
            this.lblSellOptionsAbove.TabIndex = 81;
            this.lblSellOptionsAbove.Text = "Above:";
            this.tipToolTips.SetToolTip(this.lblSellOptionsAbove, "Commodities above this price are filtered out");
            // 
            // lblSellOptionsPads
            // 
            this.lblSellOptionsPads.AutoSize = true;
            this.lblSellOptionsPads.Location = new System.Drawing.Point(112, 112);
            this.lblSellOptionsPads.Name = "lblSellOptionsPads";
            this.lblSellOptionsPads.Size = new System.Drawing.Size(34, 13);
            this.lblSellOptionsPads.TabIndex = 78;
            this.lblSellOptionsPads.Text = "Pads:";
            this.tipToolTips.SetToolTip(this.lblSellOptionsPads, "Minimum pad sizes to consider a hop, can be M, L, and/or ?");
            // 
            // numSellOptionsNearLy
            // 
            this.numSellOptionsNearLy.ContextMenuStrip = this.mnuSetValues;
            this.numSellOptionsNearLy.DecimalPlaces = 2;
            this.numSellOptionsNearLy.Location = new System.Drawing.Point(139, 57);
            this.numSellOptionsNearLy.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            131072});
            this.numSellOptionsNearLy.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numSellOptionsNearLy.Name = "numSellOptionsNearLy";
            this.numSellOptionsNearLy.Size = new System.Drawing.Size(53, 20);
            this.numSellOptionsNearLy.TabIndex = 69;
            this.numSellOptionsNearLy.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tipToolTips.SetToolTip(this.numSellOptionsNearLy, "Distance to search for local system/station info.");
            this.numSellOptionsNearLy.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // txtSellOptionsPads
            // 
            this.txtSellOptionsPads.ContextMenuStrip = this.mnuSetValues;
            this.txtSellOptionsPads.Location = new System.Drawing.Point(152, 109);
            this.txtSellOptionsPads.MaxLength = 3;
            this.txtSellOptionsPads.Name = "txtSellOptionsPads";
            this.txtSellOptionsPads.Size = new System.Drawing.Size(32, 20);
            this.txtSellOptionsPads.TabIndex = 77;
            this.txtSellOptionsPads.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tipToolTips.SetToolTip(this.txtSellOptionsPads, "Minimum pad sizes to consider a hop, can be M, L, and/or ?");
            this.txtSellOptionsPads.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.PadSize_KeyPress);
            // 
            // lblSellOptionsLimit
            // 
            this.lblSellOptionsLimit.AutoSize = true;
            this.lblSellOptionsLimit.Location = new System.Drawing.Point(88, 85);
            this.lblSellOptionsLimit.Name = "lblSellOptionsLimit";
            this.lblSellOptionsLimit.Size = new System.Drawing.Size(45, 13);
            this.lblSellOptionsLimit.TabIndex = 67;
            this.lblSellOptionsLimit.Text = "Results:";
            this.tipToolTips.SetToolTip(this.lblSellOptionsLimit, "Limit the number of results shown.");
            // 
            // lblSellOptionsPlanetary
            // 
            this.lblSellOptionsPlanetary.AutoSize = true;
            this.lblSellOptionsPlanetary.Location = new System.Drawing.Point(7, 111);
            this.lblSellOptionsPlanetary.Name = "lblSellOptionsPlanetary";
            this.lblSellOptionsPlanetary.Size = new System.Drawing.Size(54, 13);
            this.lblSellOptionsPlanetary.TabIndex = 75;
            this.lblSellOptionsPlanetary.Text = "Planetary:";
            this.tipToolTips.SetToolTip(this.lblSellOptionsPlanetary, "Limit result to stations with one of the specified planetary. Can be Y, N and/or " +
        "?");
            // 
            // txtSellOptionsPlanetary
            // 
            this.txtSellOptionsPlanetary.ContextMenuStrip = this.mnuSetValues;
            this.txtSellOptionsPlanetary.Location = new System.Drawing.Point(70, 108);
            this.txtSellOptionsPlanetary.MaxLength = 3;
            this.txtSellOptionsPlanetary.Name = "txtSellOptionsPlanetary";
            this.txtSellOptionsPlanetary.Size = new System.Drawing.Size(32, 20);
            this.txtSellOptionsPlanetary.TabIndex = 74;
            this.txtSellOptionsPlanetary.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tipToolTips.SetToolTip(this.txtSellOptionsPlanetary, "Limit result to stations with one of the specified planetary. Can be Y, N and/or " +
        "?");
            this.txtSellOptionsPlanetary.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Planetary_KeyPress);
            // 
            // numSellOptionsLimit
            // 
            this.numSellOptionsLimit.ContextMenuStrip = this.mnuSetValues;
            this.numSellOptionsLimit.Location = new System.Drawing.Point(139, 83);
            this.numSellOptionsLimit.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numSellOptionsLimit.Name = "numSellOptionsLimit";
            this.numSellOptionsLimit.Size = new System.Drawing.Size(53, 20);
            this.numSellOptionsLimit.TabIndex = 66;
            this.numSellOptionsLimit.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tipToolTips.SetToolTip(this.numSellOptionsLimit, "Limit each commodity purchased to this amount on a hop");
            // 
            // txtSellOptionsAvoid
            // 
            this.txtSellOptionsAvoid.ContextMenuStrip = this.mnuSetValues;
            this.txtSellOptionsAvoid.Location = new System.Drawing.Point(70, 5);
            this.txtSellOptionsAvoid.Name = "txtSellOptionsAvoid";
            this.txtSellOptionsAvoid.Size = new System.Drawing.Size(244, 20);
            this.txtSellOptionsAvoid.TabIndex = 72;
            this.txtSellOptionsAvoid.TabStop = false;
            this.tipToolTips.SetToolTip(this.txtSellOptionsAvoid, "Avoids can include system/station and items delimited by comma");
            // 
            // lblSellOptionsAvoid
            // 
            this.lblSellOptionsAvoid.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSellOptionsAvoid.AutoSize = true;
            this.lblSellOptionsAvoid.Location = new System.Drawing.Point(27, 8);
            this.lblSellOptionsAvoid.Name = "lblSellOptionsAvoid";
            this.lblSellOptionsAvoid.Size = new System.Drawing.Size(37, 13);
            this.lblSellOptionsAvoid.TabIndex = 73;
            this.lblSellOptionsAvoid.Text = "Avoid:";
            this.tipToolTips.SetToolTip(this.lblSellOptionsAvoid, "Avoids can include system/station and items delimited by comma");
            // 
            // lblSellOptionsNearLy
            // 
            this.lblSellOptionsNearLy.AutoSize = true;
            this.lblSellOptionsNearLy.Location = new System.Drawing.Point(84, 59);
            this.lblSellOptionsNearLy.Name = "lblSellOptionsNearLy";
            this.lblSellOptionsNearLy.Size = new System.Drawing.Size(49, 13);
            this.lblSellOptionsNearLy.TabIndex = 68;
            this.lblSellOptionsNearLy.Text = "Near LY:";
            this.tipToolTips.SetToolTip(this.lblSellOptionsNearLy, "Distance to search for local system/station info.");
            // 
            // numSellOptionsDemand
            // 
            this.numSellOptionsDemand.ContextMenuStrip = this.mnuSetValues;
            this.numSellOptionsDemand.Location = new System.Drawing.Point(259, 58);
            this.numSellOptionsDemand.Maximum = new decimal(new int[] {
            9999999,
            0,
            0,
            0});
            this.numSellOptionsDemand.Name = "numSellOptionsDemand";
            this.numSellOptionsDemand.Size = new System.Drawing.Size(55, 20);
            this.numSellOptionsDemand.TabIndex = 61;
            this.numSellOptionsDemand.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numSellOptionsDemand.ThousandsSeparator = true;
            this.tipToolTips.SetToolTip(this.numSellOptionsDemand, "Limit to stations known to have at least this much supply");
            // 
            // lblSellOptionsDemand
            // 
            this.lblSellOptionsDemand.AutoSize = true;
            this.lblSellOptionsDemand.Location = new System.Drawing.Point(202, 60);
            this.lblSellOptionsDemand.Name = "lblSellOptionsDemand";
            this.lblSellOptionsDemand.Size = new System.Drawing.Size(50, 13);
            this.lblSellOptionsDemand.TabIndex = 60;
            this.lblSellOptionsDemand.Text = "Demand:";
            this.tipToolTips.SetToolTip(this.lblSellOptionsDemand, "Limit to stations known to have at least this much supply");
            // 
            // lblRaresOptionsSort
            // 
            this.lblRaresOptionsSort.AutoSize = true;
            this.lblRaresOptionsSort.Location = new System.Drawing.Point(6, 9);
            this.lblRaresOptionsSort.Name = "lblRaresOptionsSort";
            this.lblRaresOptionsSort.Size = new System.Drawing.Size(76, 13);
            this.lblRaresOptionsSort.TabIndex = 86;
            this.lblRaresOptionsSort.Text = "Sort results by:";
            this.tipToolTips.SetToolTip(this.lblRaresOptionsSort, "Limit result to stations with one of the specified planetary. Can be Y, N and/or " +
        "?");
            // 
            // optRaresOptionsDistance
            // 
            this.optRaresOptionsDistance.AutoSize = true;
            this.optRaresOptionsDistance.Location = new System.Drawing.Point(88, 7);
            this.optRaresOptionsDistance.Name = "optRaresOptionsDistance";
            this.optRaresOptionsDistance.Size = new System.Drawing.Size(67, 17);
            this.optRaresOptionsDistance.TabIndex = 66;
            this.optRaresOptionsDistance.TabStop = true;
            this.optRaresOptionsDistance.Text = "Distance";
            this.tipToolTips.SetToolTip(this.optRaresOptionsDistance, "Sort by proximity rather than price");
            this.optRaresOptionsDistance.UseVisualStyleBackColor = true;
            // 
            // optRaresOptionsPrice
            // 
            this.optRaresOptionsPrice.AutoSize = true;
            this.optRaresOptionsPrice.Location = new System.Drawing.Point(161, 7);
            this.optRaresOptionsPrice.Name = "optRaresOptionsPrice";
            this.optRaresOptionsPrice.Size = new System.Drawing.Size(49, 17);
            this.optRaresOptionsPrice.TabIndex = 67;
            this.optRaresOptionsPrice.TabStop = true;
            this.optRaresOptionsPrice.Text = "Price";
            this.tipToolTips.SetToolTip(this.optRaresOptionsPrice, "Sort by price rather than proximity");
            this.optRaresOptionsPrice.UseVisualStyleBackColor = true;
            // 
            // lblRaresOptionsPads
            // 
            this.lblRaresOptionsPads.AutoSize = true;
            this.lblRaresOptionsPads.Location = new System.Drawing.Point(242, 59);
            this.lblRaresOptionsPads.Name = "lblRaresOptionsPads";
            this.lblRaresOptionsPads.Size = new System.Drawing.Size(34, 13);
            this.lblRaresOptionsPads.TabIndex = 78;
            this.lblRaresOptionsPads.Text = "Pads:";
            this.tipToolTips.SetToolTip(this.lblRaresOptionsPads, "Minimum pad sizes to consider a hop, can be M, L, and/or ?");
            // 
            // numRaresOptionsLy
            // 
            this.numRaresOptionsLy.ContextMenuStrip = this.mnuSetValues;
            this.numRaresOptionsLy.DecimalPlaces = 2;
            this.numRaresOptionsLy.Location = new System.Drawing.Point(142, 55);
            this.numRaresOptionsLy.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            131072});
            this.numRaresOptionsLy.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numRaresOptionsLy.Name = "numRaresOptionsLy";
            this.numRaresOptionsLy.Size = new System.Drawing.Size(63, 20);
            this.numRaresOptionsLy.TabIndex = 69;
            this.numRaresOptionsLy.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tipToolTips.SetToolTip(this.numRaresOptionsLy, "Maximum distance to search from center system.");
            this.numRaresOptionsLy.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // txtRaresOptionsPads
            // 
            this.txtRaresOptionsPads.ContextMenuStrip = this.mnuSetValues;
            this.txtRaresOptionsPads.Location = new System.Drawing.Point(282, 56);
            this.txtRaresOptionsPads.MaxLength = 3;
            this.txtRaresOptionsPads.Name = "txtRaresOptionsPads";
            this.txtRaresOptionsPads.Size = new System.Drawing.Size(32, 20);
            this.txtRaresOptionsPads.TabIndex = 77;
            this.txtRaresOptionsPads.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tipToolTips.SetToolTip(this.txtRaresOptionsPads, "Minimum pad sizes to consider a hop, can be M, L, and/or ?");
            this.txtRaresOptionsPads.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.PadSize_KeyPress);
            // 
            // lblRaresOptionsLimit
            // 
            this.lblRaresOptionsLimit.AutoSize = true;
            this.lblRaresOptionsLimit.Location = new System.Drawing.Point(93, 31);
            this.lblRaresOptionsLimit.Name = "lblRaresOptionsLimit";
            this.lblRaresOptionsLimit.Size = new System.Drawing.Size(45, 13);
            this.lblRaresOptionsLimit.TabIndex = 67;
            this.lblRaresOptionsLimit.Text = "Results:";
            this.tipToolTips.SetToolTip(this.lblRaresOptionsLimit, "Maximum number of results to show");
            // 
            // lblRaresOptionsPlanetary
            // 
            this.lblRaresOptionsPlanetary.AutoSize = true;
            this.lblRaresOptionsPlanetary.Location = new System.Drawing.Point(219, 33);
            this.lblRaresOptionsPlanetary.Name = "lblRaresOptionsPlanetary";
            this.lblRaresOptionsPlanetary.Size = new System.Drawing.Size(54, 13);
            this.lblRaresOptionsPlanetary.TabIndex = 75;
            this.lblRaresOptionsPlanetary.Text = "Planetary:";
            this.tipToolTips.SetToolTip(this.lblRaresOptionsPlanetary, "Limit result to stations with one of the specified planetary. Can be Y, N and/or " +
        "?");
            // 
            // chkRaresOptionsReverse
            // 
            this.chkRaresOptionsReverse.AutoSize = true;
            this.chkRaresOptionsReverse.Enabled = false;
            this.chkRaresOptionsReverse.Location = new System.Drawing.Point(7, 30);
            this.chkRaresOptionsReverse.Name = "chkRaresOptionsReverse";
            this.chkRaresOptionsReverse.Size = new System.Drawing.Size(66, 17);
            this.chkRaresOptionsReverse.TabIndex = 79;
            this.chkRaresOptionsReverse.TabStop = false;
            this.chkRaresOptionsReverse.Text = "Reverse";
            this.tipToolTips.SetToolTip(this.chkRaresOptionsReverse, "Reverse the order, can be used with \"--ly\" and \"--limit\" to find the furthest-awa" +
        "y rares.");
            this.chkRaresOptionsReverse.UseVisualStyleBackColor = true;
            // 
            // txtRaresOptionsPlanetary
            // 
            this.txtRaresOptionsPlanetary.ContextMenuStrip = this.mnuSetValues;
            this.txtRaresOptionsPlanetary.Location = new System.Drawing.Point(282, 30);
            this.txtRaresOptionsPlanetary.MaxLength = 3;
            this.txtRaresOptionsPlanetary.Name = "txtRaresOptionsPlanetary";
            this.txtRaresOptionsPlanetary.Size = new System.Drawing.Size(32, 20);
            this.txtRaresOptionsPlanetary.TabIndex = 74;
            this.txtRaresOptionsPlanetary.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tipToolTips.SetToolTip(this.txtRaresOptionsPlanetary, "Limit result to stations with one of the specified planetary. Can be Y, N and/or " +
        "?\r\n");
            this.txtRaresOptionsPlanetary.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Planetary_KeyPress);
            // 
            // chkRaresOptionsQuiet
            // 
            this.chkRaresOptionsQuiet.AutoSize = true;
            this.chkRaresOptionsQuiet.Location = new System.Drawing.Point(7, 53);
            this.chkRaresOptionsQuiet.Name = "chkRaresOptionsQuiet";
            this.chkRaresOptionsQuiet.Size = new System.Drawing.Size(51, 17);
            this.chkRaresOptionsQuiet.TabIndex = 76;
            this.chkRaresOptionsQuiet.TabStop = false;
            this.chkRaresOptionsQuiet.Text = "Quiet";
            this.tipToolTips.SetToolTip(this.chkRaresOptionsQuiet, "Don\'t include the header lines");
            this.chkRaresOptionsQuiet.UseVisualStyleBackColor = true;
            // 
            // numRaresOptionsLimit
            // 
            this.numRaresOptionsLimit.ContextMenuStrip = this.mnuSetValues;
            this.numRaresOptionsLimit.Location = new System.Drawing.Point(144, 29);
            this.numRaresOptionsLimit.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numRaresOptionsLimit.Name = "numRaresOptionsLimit";
            this.numRaresOptionsLimit.Size = new System.Drawing.Size(63, 20);
            this.numRaresOptionsLimit.TabIndex = 66;
            this.numRaresOptionsLimit.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tipToolTips.SetToolTip(this.numRaresOptionsLimit, "Maximum number of results to show");
            // 
            // txtRaresOptionsFrom
            // 
            this.txtRaresOptionsFrom.ContextMenuStrip = this.mnuSetValues;
            this.txtRaresOptionsFrom.Location = new System.Drawing.Point(141, 3);
            this.txtRaresOptionsFrom.Name = "txtRaresOptionsFrom";
            this.txtRaresOptionsFrom.Size = new System.Drawing.Size(173, 20);
            this.txtRaresOptionsFrom.TabIndex = 72;
            this.txtRaresOptionsFrom.TabStop = false;
            this.tipToolTips.SetToolTip(this.txtRaresOptionsFrom, "Limits results to systems that are at least a given distance away from additional" +
        " systems.");
            // 
            // lblRaresOptionsFrom
            // 
            this.lblRaresOptionsFrom.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblRaresOptionsFrom.AutoSize = true;
            this.lblRaresOptionsFrom.Location = new System.Drawing.Point(61, 6);
            this.lblRaresOptionsFrom.Name = "lblRaresOptionsFrom";
            this.lblRaresOptionsFrom.Size = new System.Drawing.Size(74, 13);
            this.lblRaresOptionsFrom.TabIndex = 73;
            this.lblRaresOptionsFrom.Text = "LY away from:";
            this.tipToolTips.SetToolTip(this.lblRaresOptionsFrom, "Limits results to systems that are at least a given distance away from additional" +
        " systems.");
            // 
            // lblRaresOptionsLy
            // 
            this.lblRaresOptionsLy.AutoSize = true;
            this.lblRaresOptionsLy.Location = new System.Drawing.Point(113, 59);
            this.lblRaresOptionsLy.Name = "lblRaresOptionsLy";
            this.lblRaresOptionsLy.Size = new System.Drawing.Size(23, 13);
            this.lblRaresOptionsLy.TabIndex = 68;
            this.lblRaresOptionsLy.Text = "LY:";
            this.tipToolTips.SetToolTip(this.lblRaresOptionsLy, "Maximum distance to search from center system.");
            // 
            // numRaresOptionsAway
            // 
            this.numRaresOptionsAway.ContextMenuStrip = this.mnuSetValues;
            this.numRaresOptionsAway.DecimalPlaces = 2;
            this.numRaresOptionsAway.Location = new System.Drawing.Point(7, 4);
            this.numRaresOptionsAway.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            131072});
            this.numRaresOptionsAway.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numRaresOptionsAway.Name = "numRaresOptionsAway";
            this.numRaresOptionsAway.Size = new System.Drawing.Size(53, 20);
            this.numRaresOptionsAway.TabIndex = 69;
            this.numRaresOptionsAway.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tipToolTips.SetToolTip(this.numRaresOptionsAway, "Limits results to systems that are at least a given distance away  from additiona" +
        "l systems.");
            this.numRaresOptionsAway.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // lblRaresOptionsType
            // 
            this.lblRaresOptionsType.AutoSize = true;
            this.lblRaresOptionsType.Location = new System.Drawing.Point(6, 9);
            this.lblRaresOptionsType.Name = "lblRaresOptionsType";
            this.lblRaresOptionsType.Size = new System.Drawing.Size(61, 13);
            this.lblRaresOptionsType.TabIndex = 86;
            this.lblRaresOptionsType.Text = "Rares type:";
            this.tipToolTips.SetToolTip(this.lblRaresOptionsType, "Only list items known to be either legal or illegal.");
            // 
            // optRaresOptionsLegal
            // 
            this.optRaresOptionsLegal.AutoSize = true;
            this.optRaresOptionsLegal.Location = new System.Drawing.Point(88, 7);
            this.optRaresOptionsLegal.Name = "optRaresOptionsLegal";
            this.optRaresOptionsLegal.Size = new System.Drawing.Size(51, 17);
            this.optRaresOptionsLegal.TabIndex = 66;
            this.optRaresOptionsLegal.TabStop = true;
            this.optRaresOptionsLegal.Text = "Legal";
            this.tipToolTips.SetToolTip(this.optRaresOptionsLegal, "Only list items known to be legal.");
            this.optRaresOptionsLegal.UseVisualStyleBackColor = true;
            // 
            // optRaresOptionsIllegal
            // 
            this.optRaresOptionsIllegal.AutoSize = true;
            this.optRaresOptionsIllegal.Location = new System.Drawing.Point(161, 7);
            this.optRaresOptionsIllegal.Name = "optRaresOptionsIllegal";
            this.optRaresOptionsIllegal.Size = new System.Drawing.Size(52, 17);
            this.optRaresOptionsIllegal.TabIndex = 67;
            this.optRaresOptionsIllegal.TabStop = true;
            this.optRaresOptionsIllegal.Text = "Illegal";
            this.tipToolTips.SetToolTip(this.optRaresOptionsIllegal, "Only list items known to be illegal.");
            this.optRaresOptionsIllegal.UseVisualStyleBackColor = true;
            // 
            // optRaresOptionsAll
            // 
            this.optRaresOptionsAll.AutoSize = true;
            this.optRaresOptionsAll.Location = new System.Drawing.Point(219, 7);
            this.optRaresOptionsAll.Name = "optRaresOptionsAll";
            this.optRaresOptionsAll.Size = new System.Drawing.Size(36, 17);
            this.optRaresOptionsAll.TabIndex = 87;
            this.optRaresOptionsAll.TabStop = true;
            this.optRaresOptionsAll.Text = "All";
            this.tipToolTips.SetToolTip(this.optRaresOptionsAll, "List all rares.");
            this.optRaresOptionsAll.UseVisualStyleBackColor = true;
            // 
            // btnTradeOptionsSwap
            // 
            this.btnTradeOptionsSwap.Location = new System.Drawing.Point(3, 3);
            this.btnTradeOptionsSwap.Name = "btnTradeOptionsSwap";
            this.btnTradeOptionsSwap.Size = new System.Drawing.Size(23, 23);
            this.btnTradeOptionsSwap.TabIndex = 70;
            this.btnTradeOptionsSwap.TabStop = false;
            this.btnTradeOptionsSwap.Text = "S";
            this.tipToolTips.SetToolTip(this.btnTradeOptionsSwap, "Swap the contents of Source/Destination");
            this.btnTradeOptionsSwap.UseVisualStyleBackColor = true;
            this.btnTradeOptionsSwap.Click += new System.EventHandler(this.SwapSourceAndDestination);
            // 
            // lblTradeOptionDestination
            // 
            this.lblTradeOptionDestination.AllowDrop = true;
            this.lblTradeOptionDestination.AutoSize = true;
            this.lblTradeOptionDestination.Location = new System.Drawing.Point(32, 8);
            this.lblTradeOptionDestination.Name = "lblTradeOptionDestination";
            this.lblTradeOptionDestination.Size = new System.Drawing.Size(63, 13);
            this.lblTradeOptionDestination.TabIndex = 1;
            this.lblTradeOptionDestination.Text = "Destination:";
            this.tipToolTips.SetToolTip(this.lblTradeOptionDestination, "Destination point in the form of system or system/station");
            // 
            // cboTradeOptionDestination
            // 
            this.cboTradeOptionDestination.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cboTradeOptionDestination.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.cboTradeOptionDestination.ContextMenuStrip = this.mnuSetValues;
            this.cboTradeOptionDestination.Location = new System.Drawing.Point(101, 5);
            this.cboTradeOptionDestination.Name = "cboTradeOptionDestination";
            this.cboTradeOptionDestination.Size = new System.Drawing.Size(212, 21);
            this.cboTradeOptionDestination.TabIndex = 0;
            this.tipToolTips.SetToolTip(this.cboTradeOptionDestination, "Destination point in the form of system or system/station\r\nCtrl+Enter adds a Syst" +
        "em/Station to the favorites\r\nShift+Enter removes a System/Station from the favor" +
        "ites");
            this.cboTradeOptionDestination.SelectedIndexChanged += new System.EventHandler(this.DestinationChanged);
            this.cboTradeOptionDestination.KeyDown += new System.Windows.Forms.KeyEventHandler(this.DestSystemComboBox_KeyDown);
            // 
            // lblMarketOptionsType
            // 
            this.lblMarketOptionsType.AutoSize = true;
            this.lblMarketOptionsType.Location = new System.Drawing.Point(6, 10);
            this.lblMarketOptionsType.Name = "lblMarketOptionsType";
            this.lblMarketOptionsType.Size = new System.Drawing.Size(63, 13);
            this.lblMarketOptionsType.TabIndex = 86;
            this.lblMarketOptionsType.Text = "Market type";
            this.tipToolTips.SetToolTip(this.lblMarketOptionsType, "Limit result to stations with one of the specified planetary. Can be Y, N and/or " +
        "?");
            // 
            // optMarketOptionsBuy
            // 
            this.optMarketOptionsBuy.AutoSize = true;
            this.optMarketOptionsBuy.Location = new System.Drawing.Point(88, 8);
            this.optMarketOptionsBuy.Name = "optMarketOptionsBuy";
            this.optMarketOptionsBuy.Size = new System.Drawing.Size(43, 17);
            this.optMarketOptionsBuy.TabIndex = 66;
            this.optMarketOptionsBuy.TabStop = true;
            this.optMarketOptionsBuy.Text = "Buy";
            this.tipToolTips.SetToolTip(this.optMarketOptionsBuy, "List only items bought by the station (listed as \'SELL\' in-game)");
            this.optMarketOptionsBuy.UseVisualStyleBackColor = true;
            // 
            // optMarketOptionsSell
            // 
            this.optMarketOptionsSell.AutoSize = true;
            this.optMarketOptionsSell.Location = new System.Drawing.Point(161, 8);
            this.optMarketOptionsSell.Name = "optMarketOptionsSell";
            this.optMarketOptionsSell.Size = new System.Drawing.Size(42, 17);
            this.optMarketOptionsSell.TabIndex = 67;
            this.optMarketOptionsSell.TabStop = true;
            this.optMarketOptionsSell.Text = "Sell";
            this.tipToolTips.SetToolTip(this.optMarketOptionsSell, "List only items sold by the station (listed as \'BUY\' in-game)");
            this.optMarketOptionsSell.UseVisualStyleBackColor = true;
            // 
            // optMarketOptionsAll
            // 
            this.optMarketOptionsAll.AutoSize = true;
            this.optMarketOptionsAll.Location = new System.Drawing.Point(216, 8);
            this.optMarketOptionsAll.Name = "optMarketOptionsAll";
            this.optMarketOptionsAll.Size = new System.Drawing.Size(36, 17);
            this.optMarketOptionsAll.TabIndex = 87;
            this.optMarketOptionsAll.TabStop = true;
            this.optMarketOptionsAll.Text = "All";
            this.tipToolTips.SetToolTip(this.optMarketOptionsAll, "All items.");
            this.optMarketOptionsAll.UseVisualStyleBackColor = true;
            // 
            // cboShipVendorOptionShips
            // 
            this.cboShipVendorOptionShips.Location = new System.Drawing.Point(70, 5);
            this.cboShipVendorOptionShips.Name = "cboShipVendorOptionShips";
            this.cboShipVendorOptionShips.Size = new System.Drawing.Size(244, 21);
            this.cboShipVendorOptionShips.TabIndex = 13;
            this.tipToolTips.SetToolTip(this.cboShipVendorOptionShips, "Ships sold at this ship vendor (delimited by space or comma)");
            // 
            // lblShipVendorOptionShips
            // 
            this.lblShipVendorOptionShips.AutoSize = true;
            this.lblShipVendorOptionShips.Location = new System.Drawing.Point(4, 8);
            this.lblShipVendorOptionShips.Name = "lblShipVendorOptionShips";
            this.lblShipVendorOptionShips.Size = new System.Drawing.Size(60, 13);
            this.lblShipVendorOptionShips.TabIndex = 14;
            this.lblShipVendorOptionShips.Text = "Ships Sold:";
            this.tipToolTips.SetToolTip(this.lblShipVendorOptionShips, "Ships sold at this ship vendor (delimited by space or comma)");
            // 
            // btnNavOptionsSwap
            // 
            this.btnNavOptionsSwap.Location = new System.Drawing.Point(3, 3);
            this.btnNavOptionsSwap.Name = "btnNavOptionsSwap";
            this.btnNavOptionsSwap.Size = new System.Drawing.Size(23, 23);
            this.btnNavOptionsSwap.TabIndex = 70;
            this.btnNavOptionsSwap.TabStop = false;
            this.btnNavOptionsSwap.Text = "S";
            this.tipToolTips.SetToolTip(this.btnNavOptionsSwap, "Swap the contents of Source/Destination");
            this.btnNavOptionsSwap.UseVisualStyleBackColor = true;
            this.btnNavOptionsSwap.Click += new System.EventHandler(this.SwapSourceAndDestination);
            // 
            // chkNavOptionsStations
            // 
            this.chkNavOptionsStations.AutoSize = true;
            this.chkNavOptionsStations.Location = new System.Drawing.Point(6, 86);
            this.chkNavOptionsStations.Name = "chkNavOptionsStations";
            this.chkNavOptionsStations.Size = new System.Drawing.Size(64, 17);
            this.chkNavOptionsStations.TabIndex = 62;
            this.chkNavOptionsStations.TabStop = false;
            this.chkNavOptionsStations.Text = "Stations";
            this.tipToolTips.SetToolTip(this.chkNavOptionsStations, "Lists stations at each stop");
            this.chkNavOptionsStations.UseVisualStyleBackColor = true;
            // 
            // lblNavOptionsDestination
            // 
            this.lblNavOptionsDestination.AllowDrop = true;
            this.lblNavOptionsDestination.AutoSize = true;
            this.lblNavOptionsDestination.Location = new System.Drawing.Point(32, 8);
            this.lblNavOptionsDestination.Name = "lblNavOptionsDestination";
            this.lblNavOptionsDestination.Size = new System.Drawing.Size(63, 13);
            this.lblNavOptionsDestination.TabIndex = 1;
            this.lblNavOptionsDestination.Text = "Destination:";
            this.tipToolTips.SetToolTip(this.lblNavOptionsDestination, "Destination point in the form of system or system/station");
            // 
            // lblNavOptionsRefuelJumps
            // 
            this.lblNavOptionsRefuelJumps.AutoSize = true;
            this.lblNavOptionsRefuelJumps.Location = new System.Drawing.Point(64, 109);
            this.lblNavOptionsRefuelJumps.Name = "lblNavOptionsRefuelJumps";
            this.lblNavOptionsRefuelJumps.Size = new System.Drawing.Size(74, 13);
            this.lblNavOptionsRefuelJumps.TabIndex = 43;
            this.lblNavOptionsRefuelJumps.Text = "Refuel Jumps:";
            this.tipToolTips.SetToolTip(this.lblNavOptionsRefuelJumps, resources.GetString("lblNavOptionsRefuelJumps.ToolTip"));
            // 
            // numNavOptionsRefuelJumps
            // 
            this.numNavOptionsRefuelJumps.ContextMenuStrip = this.mnuSetValues;
            this.numNavOptionsRefuelJumps.Location = new System.Drawing.Point(144, 107);
            this.numNavOptionsRefuelJumps.Name = "numNavOptionsRefuelJumps";
            this.numNavOptionsRefuelJumps.Size = new System.Drawing.Size(53, 20);
            this.numNavOptionsRefuelJumps.TabIndex = 2;
            this.numNavOptionsRefuelJumps.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tipToolTips.SetToolTip(this.numNavOptionsRefuelJumps, resources.GetString("numNavOptionsRefuelJumps.ToolTip"));
            // 
            // cboNavOptionsDestination
            // 
            this.cboNavOptionsDestination.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cboNavOptionsDestination.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.cboNavOptionsDestination.ContextMenuStrip = this.mnuSetValues;
            this.cboNavOptionsDestination.Location = new System.Drawing.Point(101, 5);
            this.cboNavOptionsDestination.Name = "cboNavOptionsDestination";
            this.cboNavOptionsDestination.Size = new System.Drawing.Size(212, 21);
            this.cboNavOptionsDestination.TabIndex = 0;
            this.tipToolTips.SetToolTip(this.cboNavOptionsDestination, "Destination point in the form of system or system/station\r\nCtrl+Enter adds a Syst" +
        "em/Station to the favorites\r\nShift+Enter removes a System/Station from the favor" +
        "ites");
            this.cboNavOptionsDestination.SelectedIndexChanged += new System.EventHandler(this.DestinationChanged);
            this.cboNavOptionsDestination.KeyDown += new System.Windows.Forms.KeyEventHandler(this.DestSystemComboBox_KeyDown);
            // 
            // txtNavOptionsAvoid
            // 
            this.txtNavOptionsAvoid.ContextMenuStrip = this.mnuSetValues;
            this.txtNavOptionsAvoid.Location = new System.Drawing.Point(46, 32);
            this.txtNavOptionsAvoid.Name = "txtNavOptionsAvoid";
            this.txtNavOptionsAvoid.Size = new System.Drawing.Size(267, 20);
            this.txtNavOptionsAvoid.TabIndex = 71;
            this.txtNavOptionsAvoid.TabStop = false;
            this.tipToolTips.SetToolTip(this.txtNavOptionsAvoid, "Avoids can include system/station and items delimited by comma");
            // 
            // lblNavOptionsAvoid
            // 
            this.lblNavOptionsAvoid.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblNavOptionsAvoid.AutoSize = true;
            this.lblNavOptionsAvoid.Location = new System.Drawing.Point(3, 33);
            this.lblNavOptionsAvoid.Name = "lblNavOptionsAvoid";
            this.lblNavOptionsAvoid.Size = new System.Drawing.Size(37, 13);
            this.lblNavOptionsAvoid.TabIndex = 73;
            this.lblNavOptionsAvoid.Text = "Avoid:";
            this.tipToolTips.SetToolTip(this.lblNavOptionsAvoid, "Avoids can include system/station and items delimited by comma");
            // 
            // txtNavOptionsVia
            // 
            this.txtNavOptionsVia.ContextMenuStrip = this.mnuSetValues;
            this.txtNavOptionsVia.Location = new System.Drawing.Point(46, 58);
            this.txtNavOptionsVia.Name = "txtNavOptionsVia";
            this.txtNavOptionsVia.Size = new System.Drawing.Size(267, 20);
            this.txtNavOptionsVia.TabIndex = 72;
            this.txtNavOptionsVia.TabStop = false;
            this.tipToolTips.SetToolTip(this.txtNavOptionsVia, "Attempt to route through these systems, delimited by comma");
            // 
            // lblNavOptionsVia
            // 
            this.lblNavOptionsVia.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblNavOptionsVia.AutoSize = true;
            this.lblNavOptionsVia.Location = new System.Drawing.Point(11, 59);
            this.lblNavOptionsVia.Name = "lblNavOptionsVia";
            this.lblNavOptionsVia.Size = new System.Drawing.Size(25, 13);
            this.lblNavOptionsVia.TabIndex = 74;
            this.lblNavOptionsVia.Text = "Via:";
            this.tipToolTips.SetToolTip(this.lblNavOptionsVia, "Attempt to route through these systems, delimited by comma");
            // 
            // lblNavOptionsPads
            // 
            this.lblNavOptionsPads.AutoSize = true;
            this.lblNavOptionsPads.Location = new System.Drawing.Point(241, 113);
            this.lblNavOptionsPads.Name = "lblNavOptionsPads";
            this.lblNavOptionsPads.Size = new System.Drawing.Size(34, 13);
            this.lblNavOptionsPads.TabIndex = 82;
            this.lblNavOptionsPads.Text = "Pads:";
            this.tipToolTips.SetToolTip(this.lblNavOptionsPads, "Minimum pad sizes to consider a hop, can be M, L, and/or ?");
            // 
            // txtNavOptionsPads
            // 
            this.txtNavOptionsPads.ContextMenuStrip = this.mnuSetValues;
            this.txtNavOptionsPads.Location = new System.Drawing.Point(281, 110);
            this.txtNavOptionsPads.MaxLength = 3;
            this.txtNavOptionsPads.Name = "txtNavOptionsPads";
            this.txtNavOptionsPads.Size = new System.Drawing.Size(32, 20);
            this.txtNavOptionsPads.TabIndex = 81;
            this.txtNavOptionsPads.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tipToolTips.SetToolTip(this.txtNavOptionsPads, "Minimum pad sizes to consider a hop, can be M, L, and/or ?");
            this.txtNavOptionsPads.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.PadSize_KeyPress);
            // 
            // lblNavOptionsPlanetary
            // 
            this.lblNavOptionsPlanetary.AutoSize = true;
            this.lblNavOptionsPlanetary.Location = new System.Drawing.Point(218, 87);
            this.lblNavOptionsPlanetary.Name = "lblNavOptionsPlanetary";
            this.lblNavOptionsPlanetary.Size = new System.Drawing.Size(54, 13);
            this.lblNavOptionsPlanetary.TabIndex = 80;
            this.lblNavOptionsPlanetary.Text = "Planetary:";
            this.tipToolTips.SetToolTip(this.lblNavOptionsPlanetary, "Limit result to stations with one of the specified planetary. Can be Y, N and/or " +
        "?");
            // 
            // txtNavOptionsPlanetary
            // 
            this.txtNavOptionsPlanetary.ContextMenuStrip = this.mnuSetValues;
            this.txtNavOptionsPlanetary.Location = new System.Drawing.Point(281, 84);
            this.txtNavOptionsPlanetary.MaxLength = 3;
            this.txtNavOptionsPlanetary.Name = "txtNavOptionsPlanetary";
            this.txtNavOptionsPlanetary.Size = new System.Drawing.Size(32, 20);
            this.txtNavOptionsPlanetary.TabIndex = 79;
            this.txtNavOptionsPlanetary.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tipToolTips.SetToolTip(this.txtNavOptionsPlanetary, "Limit result to stations with one of the specified planetary. Can be Y, N and/or " +
        "?");
            this.txtNavOptionsPlanetary.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Planetary_KeyPress);
            // 
            // numNavOptionsLy
            // 
            this.numNavOptionsLy.ContextMenuStrip = this.mnuSetValues;
            this.numNavOptionsLy.DecimalPlaces = 2;
            this.numNavOptionsLy.Location = new System.Drawing.Point(144, 82);
            this.numNavOptionsLy.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            131072});
            this.numNavOptionsLy.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numNavOptionsLy.Name = "numNavOptionsLy";
            this.numNavOptionsLy.Size = new System.Drawing.Size(53, 20);
            this.numNavOptionsLy.TabIndex = 84;
            this.numNavOptionsLy.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tipToolTips.SetToolTip(this.numNavOptionsLy, "Constrains jumps to a maximum ly distance");
            this.numNavOptionsLy.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // lblNavOptionsLy
            // 
            this.lblNavOptionsLy.AutoSize = true;
            this.lblNavOptionsLy.Location = new System.Drawing.Point(115, 84);
            this.lblNavOptionsLy.Name = "lblNavOptionsLy";
            this.lblNavOptionsLy.Size = new System.Drawing.Size(23, 13);
            this.lblNavOptionsLy.TabIndex = 83;
            this.lblNavOptionsLy.Text = "LY:";
            this.tipToolTips.SetToolTip(this.lblNavOptionsLy, "Constrains jumps to a maximum ly distance");
            // 
            // numOldDataOptionsNearLy
            // 
            this.numOldDataOptionsNearLy.ContextMenuStrip = this.mnuSetValues;
            this.numOldDataOptionsNearLy.DecimalPlaces = 2;
            this.numOldDataOptionsNearLy.Location = new System.Drawing.Point(147, 7);
            this.numOldDataOptionsNearLy.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            131072});
            this.numOldDataOptionsNearLy.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numOldDataOptionsNearLy.Name = "numOldDataOptionsNearLy";
            this.numOldDataOptionsNearLy.Size = new System.Drawing.Size(53, 20);
            this.numOldDataOptionsNearLy.TabIndex = 2;
            this.numOldDataOptionsNearLy.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tipToolTips.SetToolTip(this.numOldDataOptionsNearLy, "[Requires --near] Systems within this range of --near.");
            this.numOldDataOptionsNearLy.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // lblOldDataOptionsLimit
            // 
            this.lblOldDataOptionsLimit.AutoSize = true;
            this.lblOldDataOptionsLimit.Location = new System.Drawing.Point(97, 61);
            this.lblOldDataOptionsLimit.Name = "lblOldDataOptionsLimit";
            this.lblOldDataOptionsLimit.Size = new System.Drawing.Size(45, 13);
            this.lblOldDataOptionsLimit.TabIndex = 67;
            this.lblOldDataOptionsLimit.Text = "Results:";
            this.tipToolTips.SetToolTip(this.lblOldDataOptionsLimit, "Maximum number of results to show");
            // 
            // numOldDataOptionsLimit
            // 
            this.numOldDataOptionsLimit.ContextMenuStrip = this.mnuSetValues;
            this.numOldDataOptionsLimit.Location = new System.Drawing.Point(148, 59);
            this.numOldDataOptionsLimit.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numOldDataOptionsLimit.Name = "numOldDataOptionsLimit";
            this.numOldDataOptionsLimit.Size = new System.Drawing.Size(53, 20);
            this.numOldDataOptionsLimit.TabIndex = 4;
            this.numOldDataOptionsLimit.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tipToolTips.SetToolTip(this.numOldDataOptionsLimit, "Maximum number of results to show");
            // 
            // lblOldDataOptionsNearLy
            // 
            this.lblOldDataOptionsNearLy.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblOldDataOptionsNearLy.AutoSize = true;
            this.lblOldDataOptionsNearLy.Location = new System.Drawing.Point(92, 12);
            this.lblOldDataOptionsNearLy.Name = "lblOldDataOptionsNearLy";
            this.lblOldDataOptionsNearLy.Size = new System.Drawing.Size(49, 13);
            this.lblOldDataOptionsNearLy.TabIndex = 73;
            this.lblOldDataOptionsNearLy.Text = "Near LY:";
            this.tipToolTips.SetToolTip(this.lblOldDataOptionsNearLy, "[Requires --near] Systems within this range of --near.");
            // 
            // chkOldDataOptionsRoute
            // 
            this.chkOldDataOptionsRoute.AutoSize = true;
            this.chkOldDataOptionsRoute.Location = new System.Drawing.Point(103, 86);
            this.chkOldDataOptionsRoute.Name = "chkOldDataOptionsRoute";
            this.chkOldDataOptionsRoute.Size = new System.Drawing.Size(55, 17);
            this.chkOldDataOptionsRoute.TabIndex = 5;
            this.chkOldDataOptionsRoute.Text = "Route";
            this.tipToolTips.SetToolTip(this.chkOldDataOptionsRoute, "Sort the results of OldData into a route by distance");
            this.chkOldDataOptionsRoute.UseVisualStyleBackColor = true;
            // 
            // lblOldDataOptionsMinAge
            // 
            this.lblOldDataOptionsMinAge.AutoSize = true;
            this.lblOldDataOptionsMinAge.Location = new System.Drawing.Point(96, 37);
            this.lblOldDataOptionsMinAge.Name = "lblOldDataOptionsMinAge";
            this.lblOldDataOptionsMinAge.Size = new System.Drawing.Size(46, 13);
            this.lblOldDataOptionsMinAge.TabIndex = 75;
            this.lblOldDataOptionsMinAge.Text = "MinAge:";
            this.tipToolTips.SetToolTip(this.lblOldDataOptionsMinAge, "Minimum age in days to filter by");
            // 
            // numOldDataOptionsMinAge
            // 
            this.numOldDataOptionsMinAge.ContextMenuStrip = this.mnuSetValues;
            this.numOldDataOptionsMinAge.Location = new System.Drawing.Point(148, 33);
            this.numOldDataOptionsMinAge.Maximum = new decimal(new int[] {
            365,
            0,
            0,
            0});
            this.numOldDataOptionsMinAge.Name = "numOldDataOptionsMinAge";
            this.numOldDataOptionsMinAge.Size = new System.Drawing.Size(53, 20);
            this.numOldDataOptionsMinAge.TabIndex = 3;
            this.numOldDataOptionsMinAge.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tipToolTips.SetToolTip(this.numOldDataOptionsMinAge, "Minimum age in days to filter by");
            // 
            // chkLocalOptionsTrading
            // 
            this.chkLocalOptionsTrading.AutoSize = true;
            this.chkLocalOptionsTrading.Location = new System.Drawing.Point(114, 74);
            this.chkLocalOptionsTrading.Name = "chkLocalOptionsTrading";
            this.chkLocalOptionsTrading.Size = new System.Drawing.Size(62, 17);
            this.chkLocalOptionsTrading.TabIndex = 103;
            this.chkLocalOptionsTrading.TabStop = false;
            this.chkLocalOptionsTrading.Text = "Trading";
            this.tipToolTips.SetToolTip(this.chkLocalOptionsTrading, "Limit stations to those which which have markets or trade data.");
            this.chkLocalOptionsTrading.UseVisualStyleBackColor = true;
            this.chkLocalOptionsTrading.Click += new System.EventHandler(this.LocalFilterCheckBoxChanged);
            // 
            // numLocalOptionsLy
            // 
            this.numLocalOptionsLy.ContextMenuStrip = this.mnuSetValues;
            this.numLocalOptionsLy.DecimalPlaces = 2;
            this.numLocalOptionsLy.Location = new System.Drawing.Point(259, 5);
            this.numLocalOptionsLy.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            131072});
            this.numLocalOptionsLy.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numLocalOptionsLy.Name = "numLocalOptionsLy";
            this.numLocalOptionsLy.Size = new System.Drawing.Size(53, 20);
            this.numLocalOptionsLy.TabIndex = 102;
            this.numLocalOptionsLy.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tipToolTips.SetToolTip(this.numLocalOptionsLy, "Constrains jumps to a maximum ly distance");
            this.numLocalOptionsLy.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // lblLocalOptionsLy
            // 
            this.lblLocalOptionsLy.AutoSize = true;
            this.lblLocalOptionsLy.Location = new System.Drawing.Point(230, 7);
            this.lblLocalOptionsLy.Name = "lblLocalOptionsLy";
            this.lblLocalOptionsLy.Size = new System.Drawing.Size(23, 13);
            this.lblLocalOptionsLy.TabIndex = 101;
            this.lblLocalOptionsLy.Text = "LY:";
            this.tipToolTips.SetToolTip(this.lblLocalOptionsLy, "Constrains jumps to a maximum ly distance");
            // 
            // lblLocalOptionsPads
            // 
            this.lblLocalOptionsPads.AutoSize = true;
            this.lblLocalOptionsPads.Location = new System.Drawing.Point(240, 60);
            this.lblLocalOptionsPads.Name = "lblLocalOptionsPads";
            this.lblLocalOptionsPads.Size = new System.Drawing.Size(34, 13);
            this.lblLocalOptionsPads.TabIndex = 100;
            this.lblLocalOptionsPads.Text = "Pads:";
            this.tipToolTips.SetToolTip(this.lblLocalOptionsPads, "Minimum pad sizes to consider a hop, can be M, L, and/or ?");
            // 
            // txtLocalOptionsPads
            // 
            this.txtLocalOptionsPads.ContextMenuStrip = this.mnuSetValues;
            this.txtLocalOptionsPads.Location = new System.Drawing.Point(280, 57);
            this.txtLocalOptionsPads.MaxLength = 3;
            this.txtLocalOptionsPads.Name = "txtLocalOptionsPads";
            this.txtLocalOptionsPads.Size = new System.Drawing.Size(32, 20);
            this.txtLocalOptionsPads.TabIndex = 99;
            this.txtLocalOptionsPads.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tipToolTips.SetToolTip(this.txtLocalOptionsPads, "Minimum pad sizes to consider a hop, can be M, L, and/or ?");
            this.txtLocalOptionsPads.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.PadSize_KeyPress);
            // 
            // lblLocalOptionsPlanetary
            // 
            this.lblLocalOptionsPlanetary.AutoSize = true;
            this.lblLocalOptionsPlanetary.Location = new System.Drawing.Point(226, 34);
            this.lblLocalOptionsPlanetary.Name = "lblLocalOptionsPlanetary";
            this.lblLocalOptionsPlanetary.Size = new System.Drawing.Size(54, 13);
            this.lblLocalOptionsPlanetary.TabIndex = 98;
            this.lblLocalOptionsPlanetary.Text = "Planetary:";
            this.tipToolTips.SetToolTip(this.lblLocalOptionsPlanetary, "Limit result to stations with one of the specified planetary. Can be Y, N and/or " +
        "?");
            // 
            // txtLocalOptionsPlanetary
            // 
            this.txtLocalOptionsPlanetary.ContextMenuStrip = this.mnuSetValues;
            this.txtLocalOptionsPlanetary.Location = new System.Drawing.Point(280, 31);
            this.txtLocalOptionsPlanetary.MaxLength = 3;
            this.txtLocalOptionsPlanetary.Name = "txtLocalOptionsPlanetary";
            this.txtLocalOptionsPlanetary.Size = new System.Drawing.Size(32, 20);
            this.txtLocalOptionsPlanetary.TabIndex = 97;
            this.txtLocalOptionsPlanetary.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tipToolTips.SetToolTip(this.txtLocalOptionsPlanetary, "Limit result to stations with one of the specified planetary. Can be Y, N and/or " +
        "?");
            this.txtLocalOptionsPlanetary.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Planetary_KeyPress);
            // 
            // btnLocalOptionsReset
            // 
            this.btnLocalOptionsReset.Enabled = false;
            this.btnLocalOptionsReset.Location = new System.Drawing.Point(291, 88);
            this.btnLocalOptionsReset.Name = "btnLocalOptionsReset";
            this.btnLocalOptionsReset.Size = new System.Drawing.Size(21, 21);
            this.btnLocalOptionsReset.TabIndex = 95;
            this.btnLocalOptionsReset.Text = "R";
            this.tipToolTips.SetToolTip(this.btnLocalOptionsReset, "Reset the station filters.");
            this.btnLocalOptionsReset.UseVisualStyleBackColor = true;
            this.btnLocalOptionsReset.Click += new System.EventHandler(this.ResetLocalFilters);
            // 
            // chkLocalOptionsStations
            // 
            this.chkLocalOptionsStations.AutoSize = true;
            this.chkLocalOptionsStations.Location = new System.Drawing.Point(114, 28);
            this.chkLocalOptionsStations.Name = "chkLocalOptionsStations";
            this.chkLocalOptionsStations.Size = new System.Drawing.Size(64, 17);
            this.chkLocalOptionsStations.TabIndex = 96;
            this.chkLocalOptionsStations.Text = "Stations";
            this.tipToolTips.SetToolTip(this.chkLocalOptionsStations, "Filter destinations that don\'t contain stations");
            this.chkLocalOptionsStations.UseVisualStyleBackColor = true;
            this.chkLocalOptionsStations.Click += new System.EventHandler(this.LocalFilterCheckBoxChanged);
            // 
            // chkLocalOptionsOutfitting
            // 
            this.chkLocalOptionsOutfitting.AutoSize = true;
            this.chkLocalOptionsOutfitting.Location = new System.Drawing.Point(114, 5);
            this.chkLocalOptionsOutfitting.Name = "chkLocalOptionsOutfitting";
            this.chkLocalOptionsOutfitting.Size = new System.Drawing.Size(68, 17);
            this.chkLocalOptionsOutfitting.TabIndex = 94;
            this.chkLocalOptionsOutfitting.TabStop = false;
            this.chkLocalOptionsOutfitting.Text = "Outfitting";
            this.tipToolTips.SetToolTip(this.chkLocalOptionsOutfitting, "Does the station have outfitting facilities? (Default is unknown)");
            this.chkLocalOptionsOutfitting.UseVisualStyleBackColor = true;
            this.chkLocalOptionsOutfitting.Click += new System.EventHandler(this.LocalFilterCheckBoxChanged);
            // 
            // chkLocalOptionsRearm
            // 
            this.chkLocalOptionsRearm.AutoSize = true;
            this.chkLocalOptionsRearm.Location = new System.Drawing.Point(3, 51);
            this.chkLocalOptionsRearm.Name = "chkLocalOptionsRearm";
            this.chkLocalOptionsRearm.Size = new System.Drawing.Size(57, 17);
            this.chkLocalOptionsRearm.TabIndex = 91;
            this.chkLocalOptionsRearm.TabStop = false;
            this.chkLocalOptionsRearm.Text = "Rearm";
            this.tipToolTips.SetToolTip(this.chkLocalOptionsRearm, "Does the station have rearm facilities? (Default is unknown)");
            this.chkLocalOptionsRearm.UseVisualStyleBackColor = true;
            this.chkLocalOptionsRearm.Click += new System.EventHandler(this.LocalFilterCheckBoxChanged);
            // 
            // chkLocalOptionsCommodities
            // 
            this.chkLocalOptionsCommodities.AutoSize = true;
            this.chkLocalOptionsCommodities.Location = new System.Drawing.Point(3, 28);
            this.chkLocalOptionsCommodities.Name = "chkLocalOptionsCommodities";
            this.chkLocalOptionsCommodities.Size = new System.Drawing.Size(85, 17);
            this.chkLocalOptionsCommodities.TabIndex = 90;
            this.chkLocalOptionsCommodities.TabStop = false;
            this.chkLocalOptionsCommodities.Text = "Commodities";
            this.tipToolTips.SetToolTip(this.chkLocalOptionsCommodities, "Does the station have a commodities market? (Default is unknown)");
            this.chkLocalOptionsCommodities.UseVisualStyleBackColor = true;
            this.chkLocalOptionsCommodities.Click += new System.EventHandler(this.LocalFilterCheckBoxChanged);
            // 
            // chkLocalOptionsRepair
            // 
            this.chkLocalOptionsRepair.AutoSize = true;
            this.chkLocalOptionsRepair.Location = new System.Drawing.Point(3, 97);
            this.chkLocalOptionsRepair.Name = "chkLocalOptionsRepair";
            this.chkLocalOptionsRepair.Size = new System.Drawing.Size(57, 17);
            this.chkLocalOptionsRepair.TabIndex = 93;
            this.chkLocalOptionsRepair.TabStop = false;
            this.chkLocalOptionsRepair.Text = "Repair";
            this.tipToolTips.SetToolTip(this.chkLocalOptionsRepair, "Does the station have repair facilities? (Default is unknown)");
            this.chkLocalOptionsRepair.UseVisualStyleBackColor = true;
            this.chkLocalOptionsRepair.Click += new System.EventHandler(this.LocalFilterCheckBoxChanged);
            // 
            // chkLocalOptionsBlkMkt
            // 
            this.chkLocalOptionsBlkMkt.AutoSize = true;
            this.chkLocalOptionsBlkMkt.Location = new System.Drawing.Point(3, 5);
            this.chkLocalOptionsBlkMkt.Name = "chkLocalOptionsBlkMkt";
            this.chkLocalOptionsBlkMkt.Size = new System.Drawing.Size(89, 17);
            this.chkLocalOptionsBlkMkt.TabIndex = 89;
            this.chkLocalOptionsBlkMkt.TabStop = false;
            this.chkLocalOptionsBlkMkt.Text = "Black Market";
            this.tipToolTips.SetToolTip(this.chkLocalOptionsBlkMkt, "Does the station have a black market? (Default is unknown)");
            this.chkLocalOptionsBlkMkt.UseVisualStyleBackColor = true;
            this.chkLocalOptionsBlkMkt.Click += new System.EventHandler(this.LocalFilterCheckBoxChanged);
            // 
            // chkLocalOptionsRefuel
            // 
            this.chkLocalOptionsRefuel.AutoSize = true;
            this.chkLocalOptionsRefuel.Location = new System.Drawing.Point(3, 74);
            this.chkLocalOptionsRefuel.Name = "chkLocalOptionsRefuel";
            this.chkLocalOptionsRefuel.Size = new System.Drawing.Size(57, 17);
            this.chkLocalOptionsRefuel.TabIndex = 92;
            this.chkLocalOptionsRefuel.TabStop = false;
            this.chkLocalOptionsRefuel.Text = "Refuel";
            this.tipToolTips.SetToolTip(this.chkLocalOptionsRefuel, "Does the station have refueling facilities? (Default is unknown)");
            this.chkLocalOptionsRefuel.UseVisualStyleBackColor = true;
            this.chkLocalOptionsRefuel.Click += new System.EventHandler(this.LocalFilterCheckBoxChanged);
            // 
            // chkLocalOptionsShipyard
            // 
            this.chkLocalOptionsShipyard.AutoSize = true;
            this.chkLocalOptionsShipyard.Location = new System.Drawing.Point(114, 51);
            this.chkLocalOptionsShipyard.Name = "chkLocalOptionsShipyard";
            this.chkLocalOptionsShipyard.Size = new System.Drawing.Size(67, 17);
            this.chkLocalOptionsShipyard.TabIndex = 88;
            this.chkLocalOptionsShipyard.TabStop = false;
            this.chkLocalOptionsShipyard.Text = "Shipyard";
            this.tipToolTips.SetToolTip(this.chkLocalOptionsShipyard, "Does the station have a shipyard? (Default is unknown)");
            this.chkLocalOptionsShipyard.UseVisualStyleBackColor = true;
            this.chkLocalOptionsShipyard.Click += new System.EventHandler(this.LocalFilterCheckBoxChanged);
            // 
            // btnLocalOptionsAll
            // 
            this.btnLocalOptionsAll.Location = new System.Drawing.Point(264, 88);
            this.btnLocalOptionsAll.Name = "btnLocalOptionsAll";
            this.btnLocalOptionsAll.Size = new System.Drawing.Size(21, 21);
            this.btnLocalOptionsAll.TabIndex = 104;
            this.btnLocalOptionsAll.Text = "A";
            this.tipToolTips.SetToolTip(this.btnLocalOptionsAll, "Set all the station filters.");
            this.btnLocalOptionsAll.UseVisualStyleBackColor = true;
            this.btnLocalOptionsAll.Click += new System.EventHandler(this.SetLocalFilters);
            // 
            // chkSellOptionsBlkMkt
            // 
            this.chkSellOptionsBlkMkt.AutoSize = true;
            this.chkSellOptionsBlkMkt.Location = new System.Drawing.Point(7, 82);
            this.chkSellOptionsBlkMkt.Name = "chkSellOptionsBlkMkt";
            this.chkSellOptionsBlkMkt.Size = new System.Drawing.Size(51, 17);
            this.chkSellOptionsBlkMkt.TabIndex = 87;
            this.chkSellOptionsBlkMkt.TabStop = false;
            this.chkSellOptionsBlkMkt.Text = "BMkt";
            this.tipToolTips.SetToolTip(this.chkSellOptionsBlkMkt, "Require stations with a black market");
            this.chkSellOptionsBlkMkt.UseVisualStyleBackColor = true;
            // 
            // backgroundWorker3
            // 
            this.backgroundWorker3.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BackgroundWorker3_DoWork);
            this.backgroundWorker3.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.BackgroundWorker3_RunWorkerCompleted);
            // 
            // backgroundWorker4
            // 
            this.backgroundWorker4.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BackgroundWorker4_DoWork);
            this.backgroundWorker4.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.BackgroundWorker4_RunWorkerCompleted);
            // 
            // panRunOptions
            // 
            this.panRunOptions.Controls.Add(this.btnRunOptionsSwap);
            this.panRunOptions.Controls.Add(this.lblRunOptionsLoopInt);
            this.panRunOptions.Controls.Add(this.lblRunOptionsPlanetary);
            this.panRunOptions.Controls.Add(this.txtRunOptionsPlanetary);
            this.panRunOptions.Controls.Add(this.numRunOptionsLoopInt);
            this.panRunOptions.Controls.Add(this.chkRunOptionsJumps);
            this.panRunOptions.Controls.Add(this.chkRunOptionsShorten);
            this.panRunOptions.Controls.Add(this.lblRunOptionsDestination);
            this.panRunOptions.Controls.Add(this.chkRunOptionsBlkMkt);
            this.panRunOptions.Controls.Add(this.lblRunOptionsEndJumps);
            this.panRunOptions.Controls.Add(this.lblRunOptionsStartJumps);
            this.panRunOptions.Controls.Add(this.chkRunOptionsDirect);
            this.panRunOptions.Controls.Add(this.numRunOptionsRoutes);
            this.panRunOptions.Controls.Add(this.chkRunOptionsLoop);
            this.panRunOptions.Controls.Add(this.numRunOptionsEndJumps);
            this.panRunOptions.Controls.Add(this.numRunOptionsStartJumps);
            this.panRunOptions.Controls.Add(this.chkRunOptionsTowards);
            this.panRunOptions.Controls.Add(this.lblRunOptionsRoutes);
            this.panRunOptions.Controls.Add(this.cboRunOptionsDestination);
            this.panRunOptions.Controls.Add(this.chkRunOptionsUnique);
            this.panRunOptions.Location = new System.Drawing.Point(1, 1);
            this.panRunOptions.Name = "panRunOptions";
            this.panRunOptions.Size = new System.Drawing.Size(320, 140);
            this.panRunOptions.TabIndex = 1;
            // 
            // lblUpdateNotify
            // 
            this.lblUpdateNotify.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblUpdateNotify.AutoSize = true;
            this.lblUpdateNotify.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUpdateNotify.ForeColor = System.Drawing.Color.Red;
            this.lblUpdateNotify.Location = new System.Drawing.Point(201, 656);
            this.lblUpdateNotify.Name = "lblUpdateNotify";
            this.lblUpdateNotify.Size = new System.Drawing.Size(421, 15);
            this.lblUpdateNotify.TabIndex = 60;
            this.lblUpdateNotify.Text = "TDHelper has been updated. Please restart to complete process.";
            this.lblUpdateNotify.Visible = false;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.pagOutput);
            this.tabControl1.Controls.Add(this.tabSavedPage1);
            this.tabControl1.Controls.Add(this.tabSavedPage2);
            this.tabControl1.Controls.Add(this.tabSavedPage3);
            this.tabControl1.Controls.Add(this.tabNotesPage);
            this.tabControl1.Controls.Add(this.tabLogPage);
            this.tabControl1.Location = new System.Drawing.Point(11, 236);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(855, 410);
            this.tabControl1.TabIndex = 23;
            this.tabControl1.TabStop = false;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.TabControl1_SelectedIndexChanged);
            // 
            // pagOutput
            // 
            this.pagOutput.Controls.Add(this.rtbOutput);
            this.pagOutput.Location = new System.Drawing.Point(4, 22);
            this.pagOutput.Name = "pagOutput";
            this.pagOutput.Padding = new System.Windows.Forms.Padding(3);
            this.pagOutput.Size = new System.Drawing.Size(847, 384);
            this.pagOutput.TabIndex = 0;
            this.pagOutput.Text = "Output";
            this.pagOutput.UseVisualStyleBackColor = true;
            // 
            // rtbOutput
            // 
            this.rtbOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rtbOutput.ContextMenuStrip = this.mnuStrip1;
            this.rtbOutput.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbOutput.Location = new System.Drawing.Point(3, 6);
            this.rtbOutput.Name = "rtbOutput";
            this.rtbOutput.ReadOnly = true;
            this.rtbOutput.Size = new System.Drawing.Size(841, 375);
            this.rtbOutput.TabIndex = 0;
            this.rtbOutput.TabStop = false;
            this.rtbOutput.Text = "";
            this.rtbOutput.TextChanged += new System.EventHandler(this.Td_outputBox_TextChanged);
            // 
            // tabSavedPage1
            // 
            this.tabSavedPage1.Controls.Add(this.rtbSaved1);
            this.tabSavedPage1.Location = new System.Drawing.Point(4, 22);
            this.tabSavedPage1.Name = "tabSavedPage1";
            this.tabSavedPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabSavedPage1.Size = new System.Drawing.Size(847, 384);
            this.tabSavedPage1.TabIndex = 1;
            this.tabSavedPage1.Text = "Saved #1";
            this.tabSavedPage1.UseVisualStyleBackColor = true;
            // 
            // rtbSaved1
            // 
            this.rtbSaved1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rtbSaved1.ContextMenuStrip = this.mnuStrip1;
            this.rtbSaved1.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbSaved1.Location = new System.Drawing.Point(3, 3);
            this.rtbSaved1.Name = "rtbSaved1";
            this.rtbSaved1.ReadOnly = true;
            this.rtbSaved1.Size = new System.Drawing.Size(834, 381);
            this.rtbSaved1.TabIndex = 2;
            this.rtbSaved1.TabStop = false;
            this.rtbSaved1.Text = "";
            // 
            // tabSavedPage2
            // 
            this.tabSavedPage2.Controls.Add(this.rtbSaved2);
            this.tabSavedPage2.Location = new System.Drawing.Point(4, 22);
            this.tabSavedPage2.Name = "tabSavedPage2";
            this.tabSavedPage2.Size = new System.Drawing.Size(847, 384);
            this.tabSavedPage2.TabIndex = 2;
            this.tabSavedPage2.Text = "Saved #2";
            this.tabSavedPage2.UseVisualStyleBackColor = true;
            // 
            // rtbSaved2
            // 
            this.rtbSaved2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rtbSaved2.ContextMenuStrip = this.mnuStrip1;
            this.rtbSaved2.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbSaved2.Location = new System.Drawing.Point(3, 3);
            this.rtbSaved2.Name = "rtbSaved2";
            this.rtbSaved2.ReadOnly = true;
            this.rtbSaved2.Size = new System.Drawing.Size(832, 380);
            this.rtbSaved2.TabIndex = 2;
            this.rtbSaved2.TabStop = false;
            this.rtbSaved2.Text = "";
            // 
            // tabSavedPage3
            // 
            this.tabSavedPage3.Controls.Add(this.rtbSaved3);
            this.tabSavedPage3.Location = new System.Drawing.Point(4, 22);
            this.tabSavedPage3.Name = "tabSavedPage3";
            this.tabSavedPage3.Size = new System.Drawing.Size(847, 384);
            this.tabSavedPage3.TabIndex = 3;
            this.tabSavedPage3.Text = "Saved #3";
            this.tabSavedPage3.UseVisualStyleBackColor = true;
            // 
            // rtbSaved3
            // 
            this.rtbSaved3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rtbSaved3.ContextMenuStrip = this.mnuStrip1;
            this.rtbSaved3.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbSaved3.Location = new System.Drawing.Point(3, 3);
            this.rtbSaved3.Name = "rtbSaved3";
            this.rtbSaved3.ReadOnly = true;
            this.rtbSaved3.Size = new System.Drawing.Size(834, 380);
            this.rtbSaved3.TabIndex = 2;
            this.rtbSaved3.TabStop = false;
            this.rtbSaved3.Text = "";
            // 
            // tabNotesPage
            // 
            this.tabNotesPage.Controls.Add(this.txtNotes);
            this.tabNotesPage.Location = new System.Drawing.Point(4, 22);
            this.tabNotesPage.Name = "tabNotesPage";
            this.tabNotesPage.Padding = new System.Windows.Forms.Padding(3);
            this.tabNotesPage.Size = new System.Drawing.Size(847, 384);
            this.tabNotesPage.TabIndex = 4;
            this.tabNotesPage.Text = "Notes";
            this.tabNotesPage.UseVisualStyleBackColor = true;
            // 
            // txtNotes
            // 
            this.txtNotes.AcceptsTab = true;
            this.txtNotes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtNotes.ContextMenuStrip = this.mnuStrip1;
            this.txtNotes.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtNotes.Location = new System.Drawing.Point(3, 3);
            this.txtNotes.Name = "txtNotes";
            this.txtNotes.Size = new System.Drawing.Size(832, 380);
            this.txtNotes.TabIndex = 3;
            this.txtNotes.TabStop = false;
            this.txtNotes.Text = "";
            // 
            // tabLogPage
            // 
            this.tabLogPage.Controls.Add(this.grdPilotsLog);
            this.tabLogPage.Location = new System.Drawing.Point(4, 22);
            this.tabLogPage.Name = "tabLogPage";
            this.tabLogPage.Padding = new System.Windows.Forms.Padding(3);
            this.tabLogPage.Size = new System.Drawing.Size(847, 384);
            this.tabLogPage.TabIndex = 5;
            this.tabLogPage.Text = "Pilot\'s Log";
            this.tabLogPage.UseVisualStyleBackColor = true;
            // 
            // grdPilotsLog
            // 
            this.grdPilotsLog.AllowUserToAddRows = false;
            this.grdPilotsLog.AllowUserToResizeRows = false;
            this.grdPilotsLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grdPilotsLog.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.grdPilotsLog.BackgroundColor = System.Drawing.SystemColors.ControlLightLight;
            this.grdPilotsLog.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.grdPilotsLog.ContextMenuStrip = this.mnuStrip2;
            this.grdPilotsLog.Location = new System.Drawing.Point(3, 3);
            this.grdPilotsLog.Name = "grdPilotsLog";
            this.grdPilotsLog.Size = new System.Drawing.Size(834, 383);
            this.grdPilotsLog.TabIndex = 0;
            this.grdPilotsLog.VirtualMode = true;
            this.grdPilotsLog.CellContextMenuStripNeeded += new System.Windows.Forms.DataGridViewCellContextMenuStripNeededEventHandler(this.PilotsLogDataGrid_CellContextMenuStripNeeded);
            this.grdPilotsLog.CellValueNeeded += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.PilotsLogDataGrid_CellValueNeeded);
            this.grdPilotsLog.CellValuePushed += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.PilotsLogDataGrid_CellValuePushed);
            this.grdPilotsLog.UserDeletingRow += new System.Windows.Forms.DataGridViewRowCancelEventHandler(this.PilotsLogDataGrid_UserDeletingRow);
            // 
            // mnuStrip2
            // 
            this.mnuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuInsertAtGridRow,
            this.mnuRemoveAtGridRow,
            this.sepSeparator4,
            this.mnuForceRefreshGridView,
            this.mnuForceResortMenuItem,
            this.sepSeparator5,
            this.mnuCopySystemToSrc,
            this.mnuCopySystemToDest});
            this.mnuStrip2.Name = "contextMenuStrip2";
            this.mnuStrip2.Size = new System.Drawing.Size(184, 148);
            // 
            // mnuInsertAtGridRow
            // 
            this.mnuInsertAtGridRow.Name = "mnuInsertAtGridRow";
            this.mnuInsertAtGridRow.Size = new System.Drawing.Size(183, 22);
            this.mnuInsertAtGridRow.Text = "Insert Row";
            this.mnuInsertAtGridRow.Click += new System.EventHandler(this.InsertAtGridRow_Click);
            // 
            // mnuRemoveAtGridRow
            // 
            this.mnuRemoveAtGridRow.Name = "mnuRemoveAtGridRow";
            this.mnuRemoveAtGridRow.Size = new System.Drawing.Size(183, 22);
            this.mnuRemoveAtGridRow.Text = "Remove Row";
            this.mnuRemoveAtGridRow.Click += new System.EventHandler(this.RemoveAtGridRow_Click);
            // 
            // sepSeparator4
            // 
            this.sepSeparator4.Name = "sepSeparator4";
            this.sepSeparator4.Size = new System.Drawing.Size(180, 6);
            // 
            // mnuForceRefreshGridView
            // 
            this.mnuForceRefreshGridView.Name = "mnuForceRefreshGridView";
            this.mnuForceRefreshGridView.Size = new System.Drawing.Size(183, 22);
            this.mnuForceRefreshGridView.Text = "Force Refresh";
            this.mnuForceRefreshGridView.Click += new System.EventHandler(this.ForceRefreshGridView_Click);
            // 
            // mnuForceResortMenuItem
            // 
            this.mnuForceResortMenuItem.Name = "mnuForceResortMenuItem";
            this.mnuForceResortMenuItem.Size = new System.Drawing.Size(183, 22);
            this.mnuForceResortMenuItem.Text = "Force Re-sort";
            this.mnuForceResortMenuItem.Click += new System.EventHandler(this.ForceResortMenuItem_Click);
            // 
            // sepSeparator5
            // 
            this.sepSeparator5.Name = "sepSeparator5";
            this.sepSeparator5.Size = new System.Drawing.Size(180, 6);
            // 
            // mnuCopySystemToSrc
            // 
            this.mnuCopySystemToSrc.Name = "mnuCopySystemToSrc";
            this.mnuCopySystemToSrc.Size = new System.Drawing.Size(183, 22);
            this.mnuCopySystemToSrc.Text = "Copy System to Src";
            this.mnuCopySystemToSrc.Click += new System.EventHandler(this.CopySystemToSrc_Click);
            // 
            // mnuCopySystemToDest
            // 
            this.mnuCopySystemToDest.Name = "mnuCopySystemToDest";
            this.mnuCopySystemToDest.Size = new System.Drawing.Size(183, 22);
            this.mnuCopySystemToDest.Text = "Copy System to Dest";
            this.mnuCopySystemToDest.Click += new System.EventHandler(this.CopySystemToDest_Click);
            // 
            // panSellOptions
            // 
            this.panSellOptions.Controls.Add(this.chkSellOptionsBlkMkt);
            this.panSellOptions.Controls.Add(this.grpSellOptionsSort);
            this.panSellOptions.Controls.Add(this.cboSellOptionsCommodities);
            this.panSellOptions.Controls.Add(this.lblSellOptionsCommodity);
            this.panSellOptions.Controls.Add(this.numSellOptionsBelow);
            this.panSellOptions.Controls.Add(this.lblSellOptionsBelow);
            this.panSellOptions.Controls.Add(this.numSellOptionsAbove);
            this.panSellOptions.Controls.Add(this.lblSellOptionsAbove);
            this.panSellOptions.Controls.Add(this.lblSellOptionsPads);
            this.panSellOptions.Controls.Add(this.numSellOptionsNearLy);
            this.panSellOptions.Controls.Add(this.txtSellOptionsPads);
            this.panSellOptions.Controls.Add(this.lblSellOptionsLimit);
            this.panSellOptions.Controls.Add(this.lblSellOptionsPlanetary);
            this.panSellOptions.Controls.Add(this.txtSellOptionsPlanetary);
            this.panSellOptions.Controls.Add(this.numSellOptionsLimit);
            this.panSellOptions.Controls.Add(this.txtSellOptionsAvoid);
            this.panSellOptions.Controls.Add(this.lblSellOptionsAvoid);
            this.panSellOptions.Controls.Add(this.lblSellOptionsNearLy);
            this.panSellOptions.Controls.Add(this.numSellOptionsDemand);
            this.panSellOptions.Controls.Add(this.lblSellOptionsDemand);
            this.panSellOptions.Location = new System.Drawing.Point(1, 1);
            this.panSellOptions.Name = "panSellOptions";
            this.panSellOptions.Size = new System.Drawing.Size(320, 160);
            this.panSellOptions.TabIndex = 4;
            this.panSellOptions.EnabledChanged += new System.EventHandler(this.OptionsPanel_StateChanged);
            this.panSellOptions.VisibleChanged += new System.EventHandler(this.OptionsPanel_StateChanged);
            // 
            // grpSellOptionsSort
            // 
            this.grpSellOptionsSort.Controls.Add(this.lblSellOptionsSort);
            this.grpSellOptionsSort.Controls.Add(this.optSellOptionsSupply);
            this.grpSellOptionsSort.Controls.Add(this.optSellOptionsPrice);
            this.grpSellOptionsSort.Location = new System.Drawing.Point(4, 130);
            this.grpSellOptionsSort.Name = "grpSellOptionsSort";
            this.grpSellOptionsSort.Size = new System.Drawing.Size(310, 26);
            this.grpSellOptionsSort.TabIndex = 86;
            this.grpSellOptionsSort.TabStop = false;
            // 
            // panBuyOptions
            // 
            this.panBuyOptions.Controls.Add(this.grpBuyOptionsSort);
            this.panBuyOptions.Controls.Add(this.cboBuyOptionsCommodities);
            this.panBuyOptions.Controls.Add(this.lblBuyOptionsCommodity);
            this.panBuyOptions.Controls.Add(this.numBuyOptionsBelow);
            this.panBuyOptions.Controls.Add(this.lblBuyOptionsBelow);
            this.panBuyOptions.Controls.Add(this.numBuyOptionsAbove);
            this.panBuyOptions.Controls.Add(this.lblBuyOptionsAbove);
            this.panBuyOptions.Controls.Add(this.lblBuyOptionsPads);
            this.panBuyOptions.Controls.Add(this.numBuyOptionsNearLy);
            this.panBuyOptions.Controls.Add(this.txtBuyOptionsPads);
            this.panBuyOptions.Controls.Add(this.lblBuyOptionsLimit);
            this.panBuyOptions.Controls.Add(this.lblBuyOptionsPlanetary);
            this.panBuyOptions.Controls.Add(this.chkBuyOptionsOneStop);
            this.panBuyOptions.Controls.Add(this.txtBuyOptionsPlanetary);
            this.panBuyOptions.Controls.Add(this.chkBuyOptionsBlkMkt);
            this.panBuyOptions.Controls.Add(this.numBuyOptionsLimit);
            this.panBuyOptions.Controls.Add(this.txtBuyOptionsAvoid);
            this.panBuyOptions.Controls.Add(this.lblBuyOptionsAvoid);
            this.panBuyOptions.Controls.Add(this.lblBuyOptionsNearLy);
            this.panBuyOptions.Controls.Add(this.numBuyOptionsSupply);
            this.panBuyOptions.Controls.Add(this.lblBuyOptionsSupply);
            this.panBuyOptions.Location = new System.Drawing.Point(1, 1);
            this.panBuyOptions.Name = "panBuyOptions";
            this.panBuyOptions.Size = new System.Drawing.Size(320, 160);
            this.panBuyOptions.TabIndex = 3;
            this.panBuyOptions.Visible = false;
            this.panBuyOptions.EnabledChanged += new System.EventHandler(this.OptionsPanel_StateChanged);
            this.panBuyOptions.VisibleChanged += new System.EventHandler(this.OptionsPanel_StateChanged);
            // 
            // grpBuyOptionsSort
            // 
            this.grpBuyOptionsSort.Controls.Add(this.lblBuyOptionsSort);
            this.grpBuyOptionsSort.Controls.Add(this.optBuyOptionsSupply);
            this.grpBuyOptionsSort.Controls.Add(this.optBuyOptionsDistance);
            this.grpBuyOptionsSort.Controls.Add(this.optBuyOptionsPrice);
            this.grpBuyOptionsSort.Location = new System.Drawing.Point(4, 130);
            this.grpBuyOptionsSort.Name = "grpBuyOptionsSort";
            this.grpBuyOptionsSort.Size = new System.Drawing.Size(310, 26);
            this.grpBuyOptionsSort.TabIndex = 86;
            this.grpBuyOptionsSort.TabStop = false;
            // 
            // lblRouteOptionsJumps
            // 
            this.lblRouteOptionsJumps.AutoSize = true;
            this.lblRouteOptionsJumps.Location = new System.Drawing.Point(5, 30);
            this.lblRouteOptionsJumps.Name = "lblRouteOptionsJumps";
            this.lblRouteOptionsJumps.Size = new System.Drawing.Size(40, 13);
            this.lblRouteOptionsJumps.TabIndex = 31;
            this.lblRouteOptionsJumps.Text = "Jumps:";
            // 
            // lblRouteOptionsMaxLS
            // 
            this.lblRouteOptionsMaxLS.AutoSize = true;
            this.lblRouteOptionsMaxLS.Location = new System.Drawing.Point(26, 53);
            this.lblRouteOptionsMaxLS.Name = "lblRouteOptionsMaxLS";
            this.lblRouteOptionsMaxLS.Size = new System.Drawing.Size(46, 13);
            this.lblRouteOptionsMaxLS.TabIndex = 10;
            this.lblRouteOptionsMaxLS.Text = "Max LS:";
            // 
            // lblRouteOptionsPruneHops
            // 
            this.lblRouteOptionsPruneHops.AutoSize = true;
            this.lblRouteOptionsPruneHops.Location = new System.Drawing.Point(6, 30);
            this.lblRouteOptionsPruneHops.Name = "lblRouteOptionsPruneHops";
            this.lblRouteOptionsPruneHops.Size = new System.Drawing.Size(66, 13);
            this.lblRouteOptionsPruneHops.TabIndex = 15;
            this.lblRouteOptionsPruneHops.Text = "Prune Hops:";
            // 
            // lblRouteOptionsPruneScore
            // 
            this.lblRouteOptionsPruneScore.AutoSize = true;
            this.lblRouteOptionsPruneScore.Location = new System.Drawing.Point(3, 7);
            this.lblRouteOptionsPruneScore.Name = "lblRouteOptionsPruneScore";
            this.lblRouteOptionsPruneScore.Size = new System.Drawing.Size(69, 13);
            this.lblRouteOptionsPruneScore.TabIndex = 49;
            this.lblRouteOptionsPruneScore.Text = "Prune Score:";
            // 
            // lblRouteOptionsLsPenalty
            // 
            this.lblRouteOptionsLsPenalty.AutoSize = true;
            this.lblRouteOptionsLsPenalty.Location = new System.Drawing.Point(11, 76);
            this.lblRouteOptionsLsPenalty.Name = "lblRouteOptionsLsPenalty";
            this.lblRouteOptionsLsPenalty.Size = new System.Drawing.Size(61, 13);
            this.lblRouteOptionsLsPenalty.TabIndex = 8;
            this.lblRouteOptionsLsPenalty.Text = "LS Penalty:";
            // 
            // lblShipInsurance
            // 
            this.lblShipInsurance.AutoSize = true;
            this.lblShipInsurance.Location = new System.Drawing.Point(358, 8);
            this.lblShipInsurance.Name = "lblShipInsurance";
            this.lblShipInsurance.Size = new System.Drawing.Size(57, 13);
            this.lblShipInsurance.TabIndex = 51;
            this.lblShipInsurance.Text = "Insurance:";
            // 
            // backgroundWorker5
            // 
            this.backgroundWorker5.WorkerReportsProgress = true;
            this.backgroundWorker5.WorkerSupportsCancellation = true;
            this.backgroundWorker5.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BackgroundWorker5_DoWork);
            this.backgroundWorker5.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.BackgroundWorker5_RunWorkerCompleted);
            // 
            // backgroundWorker6
            // 
            this.backgroundWorker6.WorkerReportsProgress = true;
            this.backgroundWorker6.WorkerSupportsCancellation = true;
            this.backgroundWorker6.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BackgroundWorker6_DoWork);
            this.backgroundWorker6.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.BackgroundWorker6_RunWorkerCompleted);
            // 
            // lblTrackerLink
            // 
            this.lblTrackerLink.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTrackerLink.AutoSize = true;
            this.lblTrackerLink.Location = new System.Drawing.Point(692, 654);
            this.lblTrackerLink.Name = "lblTrackerLink";
            this.lblTrackerLink.Size = new System.Drawing.Size(99, 13);
            this.lblTrackerLink.TabIndex = 66;
            this.lblTrackerLink.TabStop = true;
            this.lblTrackerLink.Text = "Report bugs/issues";
            this.lblTrackerLink.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblTrackerLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.TrackerLinkLabel_LinkClicked);
            // 
            // lblFaqLink
            // 
            this.lblFaqLink.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblFaqLink.AutoSize = true;
            this.lblFaqLink.Location = new System.Drawing.Point(805, 654);
            this.lblFaqLink.Name = "lblFaqLink";
            this.lblFaqLink.Size = new System.Drawing.Size(55, 13);
            this.lblFaqLink.TabIndex = 67;
            this.lblFaqLink.TabStop = true;
            this.lblFaqLink.Text = "Help/FAQ";
            this.lblFaqLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.FaqLinkLabel_LinkClicked);
            // 
            // backgroundWorker7
            // 
            this.backgroundWorker7.WorkerReportsProgress = true;
            this.backgroundWorker7.WorkerSupportsCancellation = true;
            this.backgroundWorker7.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BackgroundWorker7_DoWork);
            this.backgroundWorker7.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.BackgroundWorker7_RunWorkerCompleted);
            // 
            // panShipData
            // 
            this.panShipData.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panShipData.Controls.Add(this.cboCommandersShips);
            this.panShipData.Controls.Add(this.numShipInsurance);
            this.panShipData.Controls.Add(this.lblCommandersCredits);
            this.panShipData.Controls.Add(this.numCommandersCredits);
            this.panShipData.Controls.Add(this.lblPadSize);
            this.panShipData.Controls.Add(this.numRouteOptionsShipCapacity);
            this.panShipData.Controls.Add(this.numUnladenLy);
            this.panShipData.Controls.Add(this.numLadenLy);
            this.panShipData.Controls.Add(this.lblRouteOptionsShipCapacity);
            this.panShipData.Controls.Add(this.lblUnladenLy);
            this.panShipData.Controls.Add(this.lblShipInsurance);
            this.panShipData.Controls.Add(this.lblLadenLy);
            this.panShipData.Controls.Add(this.txtPadSize);
            this.panShipData.Location = new System.Drawing.Point(0, 0);
            this.panShipData.Name = "panShipData";
            this.panShipData.Size = new System.Drawing.Size(528, 56);
            this.panShipData.TabIndex = 1;
            // 
            // panStock
            // 
            this.panStock.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panStock.Controls.Add(this.lblRouteOptionsDemand);
            this.panStock.Controls.Add(this.numRouteOptionsDemand);
            this.panStock.Controls.Add(this.numRouteOptionsLimit);
            this.panStock.Controls.Add(this.numRouteOptionsStock);
            this.panStock.Controls.Add(this.lblStock);
            this.panStock.Controls.Add(this.lblRouteOptionsCargoLimit);
            this.panStock.Location = new System.Drawing.Point(0, 63);
            this.panStock.Name = "panStock";
            this.panStock.Size = new System.Drawing.Size(145, 73);
            this.panStock.TabIndex = 1;
            // 
            // panHops
            // 
            this.panHops.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panHops.Controls.Add(this.numRouteOptionsHops);
            this.panHops.Controls.Add(this.lblRouteOptionsHops);
            this.panHops.Controls.Add(this.numRouteOptionsJumps);
            this.panHops.Controls.Add(this.lblRouteOptionsJumps);
            this.panHops.Location = new System.Drawing.Point(285, 63);
            this.panHops.Name = "panHops";
            this.panHops.Size = new System.Drawing.Size(92, 73);
            this.panHops.TabIndex = 2;
            // 
            // panProfit
            // 
            this.panProfit.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panProfit.Controls.Add(this.numRouteOptionsMargin);
            this.panProfit.Controls.Add(this.numRouteOptionsGpt);
            this.panProfit.Controls.Add(this.lblRouteOptionsMaxGpt);
            this.panProfit.Controls.Add(this.lblRouteOptionsGpt);
            this.panProfit.Controls.Add(this.numRouteOptionsMaxGpt);
            this.panProfit.Controls.Add(this.lblRouteOptionsMargin);
            this.panProfit.Location = new System.Drawing.Point(151, 63);
            this.panProfit.Name = "panProfit";
            this.panProfit.Size = new System.Drawing.Size(128, 73);
            this.panProfit.TabIndex = 3;
            // 
            // panOther
            // 
            this.panOther.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panOther.Controls.Add(this.numRouteOptionsAge);
            this.panOther.Controls.Add(this.lblRouteOptionsAge);
            this.panOther.Controls.Add(this.numRouteOptionsMaxLSDistance);
            this.panOther.Controls.Add(this.lblRouteOptionsMaxLS);
            this.panOther.Controls.Add(this.numRouteOptionsPruneScore);
            this.panOther.Controls.Add(this.numRouteOptionsLsPenalty);
            this.panOther.Controls.Add(this.lblRouteOptionsPruneScore);
            this.panOther.Controls.Add(this.lblRouteOptionsLsPenalty);
            this.panOther.Controls.Add(this.numRouteOptionsPruneHops);
            this.panOther.Controls.Add(this.lblRouteOptionsPruneHops);
            this.panOther.Location = new System.Drawing.Point(383, 63);
            this.panOther.Name = "panOther";
            this.panOther.Size = new System.Drawing.Size(145, 132);
            this.panOther.TabIndex = 5;
            // 
            // panMisc
            // 
            this.panMisc.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panMisc.Controls.Add(this.txtAvoid);
            this.panMisc.Controls.Add(this.lblAvoid);
            this.panMisc.Controls.Add(this.txtVia);
            this.panMisc.Controls.Add(this.lblVia);
            this.panMisc.Location = new System.Drawing.Point(0, 142);
            this.panMisc.Name = "panMisc";
            this.panMisc.Size = new System.Drawing.Size(377, 53);
            this.panMisc.TabIndex = 1;
            // 
            // panRouteOptions
            // 
            this.panRouteOptions.Controls.Add(this.panMisc);
            this.panRouteOptions.Controls.Add(this.panHops);
            this.panRouteOptions.Controls.Add(this.panStock);
            this.panRouteOptions.Controls.Add(this.panShipData);
            this.panRouteOptions.Controls.Add(this.panProfit);
            this.panRouteOptions.Controls.Add(this.panOther);
            this.panRouteOptions.Location = new System.Drawing.Point(338, 34);
            this.panRouteOptions.Name = "panRouteOptions";
            this.panRouteOptions.Size = new System.Drawing.Size(528, 196);
            this.panRouteOptions.TabIndex = 1;
            // 
            // panMethods
            // 
            this.panMethods.Controls.Add(this.btnSaveSettings);
            this.panMethods.Controls.Add(this.btnCmdrProfile);
            this.panMethods.Controls.Add(this.btnMiniMode);
            this.panMethods.Controls.Add(this.btnStart);
            this.panMethods.Controls.Add(this.btnDbUpdate);
            this.panMethods.Controls.Add(this.cboMethod);
            this.panMethods.Controls.Add(this.lblSourceSystem);
            this.panMethods.Controls.Add(this.btnGetSystem);
            this.panMethods.Controls.Add(this.cboSourceSystem);
            this.panMethods.Controls.Add(this.btnSettings);
            this.panMethods.Location = new System.Drawing.Point(10, 6);
            this.panMethods.Name = "panMethods";
            this.panMethods.Size = new System.Drawing.Size(856, 23);
            this.panMethods.TabIndex = 0;
            // 
            // panRaresOptions
            // 
            this.panRaresOptions.Controls.Add(this.grpRaresOptionsType);
            this.panRaresOptions.Controls.Add(this.grpRaresOptionsSort);
            this.panRaresOptions.Controls.Add(this.lblRaresOptionsPads);
            this.panRaresOptions.Controls.Add(this.numRaresOptionsAway);
            this.panRaresOptions.Controls.Add(this.numRaresOptionsLy);
            this.panRaresOptions.Controls.Add(this.txtRaresOptionsPads);
            this.panRaresOptions.Controls.Add(this.lblRaresOptionsLimit);
            this.panRaresOptions.Controls.Add(this.lblRaresOptionsPlanetary);
            this.panRaresOptions.Controls.Add(this.chkRaresOptionsReverse);
            this.panRaresOptions.Controls.Add(this.txtRaresOptionsPlanetary);
            this.panRaresOptions.Controls.Add(this.chkRaresOptionsQuiet);
            this.panRaresOptions.Controls.Add(this.numRaresOptionsLimit);
            this.panRaresOptions.Controls.Add(this.txtRaresOptionsFrom);
            this.panRaresOptions.Controls.Add(this.lblRaresOptionsFrom);
            this.panRaresOptions.Controls.Add(this.lblRaresOptionsLy);
            this.panRaresOptions.Location = new System.Drawing.Point(1, 1);
            this.panRaresOptions.Name = "panRaresOptions";
            this.panRaresOptions.Size = new System.Drawing.Size(320, 144);
            this.panRaresOptions.TabIndex = 5;
            this.panRaresOptions.Visible = false;
            this.panRaresOptions.EnabledChanged += new System.EventHandler(this.OptionsPanel_StateChanged);
            this.panRaresOptions.VisibleChanged += new System.EventHandler(this.OptionsPanel_StateChanged);
            // 
            // grpRaresOptionsType
            // 
            this.grpRaresOptionsType.Controls.Add(this.optRaresOptionsAll);
            this.grpRaresOptionsType.Controls.Add(this.lblRaresOptionsType);
            this.grpRaresOptionsType.Controls.Add(this.optRaresOptionsLegal);
            this.grpRaresOptionsType.Controls.Add(this.optRaresOptionsIllegal);
            this.grpRaresOptionsType.Location = new System.Drawing.Point(7, 81);
            this.grpRaresOptionsType.Name = "grpRaresOptionsType";
            this.grpRaresOptionsType.Size = new System.Drawing.Size(310, 26);
            this.grpRaresOptionsType.TabIndex = 87;
            this.grpRaresOptionsType.TabStop = false;
            // 
            // grpRaresOptionsSort
            // 
            this.grpRaresOptionsSort.Controls.Add(this.lblRaresOptionsSort);
            this.grpRaresOptionsSort.Controls.Add(this.optRaresOptionsDistance);
            this.grpRaresOptionsSort.Controls.Add(this.optRaresOptionsPrice);
            this.grpRaresOptionsSort.Location = new System.Drawing.Point(7, 113);
            this.grpRaresOptionsSort.Name = "grpRaresOptionsSort";
            this.grpRaresOptionsSort.Size = new System.Drawing.Size(310, 26);
            this.grpRaresOptionsSort.TabIndex = 86;
            this.grpRaresOptionsSort.TabStop = false;
            // 
            // panTradeOptions
            // 
            this.panTradeOptions.Controls.Add(this.btnTradeOptionsSwap);
            this.panTradeOptions.Controls.Add(this.lblTradeOptionDestination);
            this.panTradeOptions.Controls.Add(this.cboTradeOptionDestination);
            this.panTradeOptions.Location = new System.Drawing.Point(1, 1);
            this.panTradeOptions.Name = "panTradeOptions";
            this.panTradeOptions.Size = new System.Drawing.Size(320, 33);
            this.panTradeOptions.TabIndex = 3;
            this.panTradeOptions.Visible = false;
            this.panTradeOptions.EnabledChanged += new System.EventHandler(this.OptionsPanel_StateChanged);
            this.panTradeOptions.VisibleChanged += new System.EventHandler(this.OptionsPanel_StateChanged);
            // 
            // panMarketOptions
            // 
            this.panMarketOptions.Controls.Add(this.grpMarketOptionsType);
            this.panMarketOptions.Location = new System.Drawing.Point(1, 1);
            this.panMarketOptions.Name = "panMarketOptions";
            this.panMarketOptions.Size = new System.Drawing.Size(320, 47);
            this.panMarketOptions.TabIndex = 5;
            this.panMarketOptions.Visible = false;
            this.panMarketOptions.EnabledChanged += new System.EventHandler(this.OptionsPanel_StateChanged);
            this.panMarketOptions.VisibleChanged += new System.EventHandler(this.OptionsPanel_StateChanged);
            // 
            // grpMarketOptionsType
            // 
            this.grpMarketOptionsType.Controls.Add(this.optMarketOptionsAll);
            this.grpMarketOptionsType.Controls.Add(this.lblMarketOptionsType);
            this.grpMarketOptionsType.Controls.Add(this.optMarketOptionsBuy);
            this.grpMarketOptionsType.Controls.Add(this.optMarketOptionsSell);
            this.grpMarketOptionsType.Location = new System.Drawing.Point(4, 7);
            this.grpMarketOptionsType.Name = "grpMarketOptionsType";
            this.grpMarketOptionsType.Size = new System.Drawing.Size(310, 29);
            this.grpMarketOptionsType.TabIndex = 86;
            this.grpMarketOptionsType.TabStop = false;
            // 
            // panShipVendorOptions
            // 
            this.panShipVendorOptions.Controls.Add(this.cboShipVendorOptionShips);
            this.panShipVendorOptions.Controls.Add(this.lblShipVendorOptionShips);
            this.panShipVendorOptions.Location = new System.Drawing.Point(1, 1);
            this.panShipVendorOptions.Name = "panShipVendorOptions";
            this.panShipVendorOptions.Size = new System.Drawing.Size(320, 35);
            this.panShipVendorOptions.TabIndex = 6;
            this.panShipVendorOptions.Visible = false;
            this.panShipVendorOptions.EnabledChanged += new System.EventHandler(this.OptionsPanel_StateChanged);
            this.panShipVendorOptions.VisibleChanged += new System.EventHandler(this.OptionsPanel_StateChanged);
            // 
            // panNavOptions
            // 
            this.panNavOptions.Controls.Add(this.numNavOptionsLy);
            this.panNavOptions.Controls.Add(this.lblNavOptionsLy);
            this.panNavOptions.Controls.Add(this.lblNavOptionsPads);
            this.panNavOptions.Controls.Add(this.txtNavOptionsPads);
            this.panNavOptions.Controls.Add(this.lblNavOptionsPlanetary);
            this.panNavOptions.Controls.Add(this.txtNavOptionsPlanetary);
            this.panNavOptions.Controls.Add(this.txtNavOptionsAvoid);
            this.panNavOptions.Controls.Add(this.lblNavOptionsAvoid);
            this.panNavOptions.Controls.Add(this.txtNavOptionsVia);
            this.panNavOptions.Controls.Add(this.lblNavOptionsVia);
            this.panNavOptions.Controls.Add(this.btnNavOptionsSwap);
            this.panNavOptions.Controls.Add(this.chkNavOptionsStations);
            this.panNavOptions.Controls.Add(this.lblNavOptionsDestination);
            this.panNavOptions.Controls.Add(this.lblNavOptionsRefuelJumps);
            this.panNavOptions.Controls.Add(this.numNavOptionsRefuelJumps);
            this.panNavOptions.Controls.Add(this.cboNavOptionsDestination);
            this.panNavOptions.Location = new System.Drawing.Point(1, 1);
            this.panNavOptions.Name = "panNavOptions";
            this.panNavOptions.Size = new System.Drawing.Size(320, 139);
            this.panNavOptions.TabIndex = 3;
            this.panNavOptions.Visible = false;
            this.panNavOptions.EnabledChanged += new System.EventHandler(this.OptionsPanel_StateChanged);
            this.panNavOptions.VisibleChanged += new System.EventHandler(this.OptionsPanel_StateChanged);
            // 
            // panOldDataOptions
            // 
            this.panOldDataOptions.Controls.Add(this.chkOldDataOptionsRoute);
            this.panOldDataOptions.Controls.Add(this.lblOldDataOptionsMinAge);
            this.panOldDataOptions.Controls.Add(this.numOldDataOptionsMinAge);
            this.panOldDataOptions.Controls.Add(this.numOldDataOptionsNearLy);
            this.panOldDataOptions.Controls.Add(this.lblOldDataOptionsLimit);
            this.panOldDataOptions.Controls.Add(this.numOldDataOptionsLimit);
            this.panOldDataOptions.Controls.Add(this.lblOldDataOptionsNearLy);
            this.panOldDataOptions.Location = new System.Drawing.Point(1, 1);
            this.panOldDataOptions.Name = "panOldDataOptions";
            this.panOldDataOptions.Size = new System.Drawing.Size(320, 109);
            this.panOldDataOptions.TabIndex = 6;
            this.panOldDataOptions.Visible = false;
            this.panOldDataOptions.EnabledChanged += new System.EventHandler(this.OptionsPanel_StateChanged);
            this.panOldDataOptions.VisibleChanged += new System.EventHandler(this.OptionsPanel_StateChanged);
            // 
            // panLocalOptions
            // 
            this.panLocalOptions.Controls.Add(this.btnLocalOptionsAll);
            this.panLocalOptions.Controls.Add(this.chkLocalOptionsTrading);
            this.panLocalOptions.Controls.Add(this.numLocalOptionsLy);
            this.panLocalOptions.Controls.Add(this.lblLocalOptionsLy);
            this.panLocalOptions.Controls.Add(this.lblLocalOptionsPads);
            this.panLocalOptions.Controls.Add(this.txtLocalOptionsPads);
            this.panLocalOptions.Controls.Add(this.lblLocalOptionsPlanetary);
            this.panLocalOptions.Controls.Add(this.txtLocalOptionsPlanetary);
            this.panLocalOptions.Controls.Add(this.btnLocalOptionsReset);
            this.panLocalOptions.Controls.Add(this.chkLocalOptionsStations);
            this.panLocalOptions.Controls.Add(this.chkLocalOptionsOutfitting);
            this.panLocalOptions.Controls.Add(this.chkLocalOptionsRearm);
            this.panLocalOptions.Controls.Add(this.chkLocalOptionsCommodities);
            this.panLocalOptions.Controls.Add(this.chkLocalOptionsRepair);
            this.panLocalOptions.Controls.Add(this.chkLocalOptionsBlkMkt);
            this.panLocalOptions.Controls.Add(this.chkLocalOptionsRefuel);
            this.panLocalOptions.Controls.Add(this.chkLocalOptionsShipyard);
            this.panLocalOptions.Location = new System.Drawing.Point(1, 1);
            this.panLocalOptions.Name = "panLocalOptions";
            this.panLocalOptions.Size = new System.Drawing.Size(320, 116);
            this.panLocalOptions.TabIndex = 5;
            this.panLocalOptions.Visible = false;
            this.panLocalOptions.EnabledChanged += new System.EventHandler(this.OptionsPanel_StateChanged);
            this.panLocalOptions.VisibleChanged += new System.EventHandler(this.OptionsPanel_StateChanged);
            // 
            // panOptions
            // 
            this.panOptions.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panOptions.Controls.Add(this.panRunOptions);
            this.panOptions.Controls.Add(this.panBuyOptions);
            this.panOptions.Controls.Add(this.panSellOptions);
            this.panOptions.Controls.Add(this.panRaresOptions);
            this.panOptions.Controls.Add(this.panMarketOptions);
            this.panOptions.Controls.Add(this.panNavOptions);
            this.panOptions.Controls.Add(this.panOldDataOptions);
            this.panOptions.Controls.Add(this.panLocalOptions);
            this.panOptions.Controls.Add(this.panShipVendorOptions);
            this.panOptions.Controls.Add(this.panTradeOptions);
            this.panOptions.Location = new System.Drawing.Point(10, 33);
            this.panOptions.Name = "panOptions";
            this.panOptions.Size = new System.Drawing.Size(322, 197);
            this.panOptions.TabIndex = 1;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(872, 676);
            this.Controls.Add(this.panOptions);
            this.Controls.Add(this.panMethods);
            this.Controls.Add(this.panRouteOptions);
            this.Controls.Add(this.lblUpdateNotify);
            this.Controls.Add(this.icoUpdateNotify);
            this.Controls.Add(this.lblFaqLink);
            this.Controls.Add(this.lblTrackerLink);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.lblStopWatch);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(882, 714);
            this.Name = "MainForm";
            this.Text = "Trade Dangerous Helper";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            this.LocationChanged += new System.EventHandler(this.MainForm_LocationChanged);
            this.mnuSetValues.ResumeLayout(false);
            this.mnuStrip1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numRunOptionsRoutes)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRunOptionsEndJumps)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRunOptionsStartJumps)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRouteOptionsLimit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRouteOptionsPruneHops)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRouteOptionsPruneScore)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRouteOptionsLsPenalty)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRouteOptionsMaxLSDistance)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRouteOptionsGpt)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numShipInsurance)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRouteOptionsShipCapacity)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRouteOptionsJumps)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRouteOptionsHops)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCommandersCredits)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUnladenLy)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numLadenLy)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRouteOptionsAge)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRouteOptionsMargin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRouteOptionsMaxGpt)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.icoUpdateNotify)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRouteOptionsStock)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRunOptionsLoopInt)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRouteOptionsDemand)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBuyOptionsSupply)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBuyOptionsLimit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBuyOptionsNearLy)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBuyOptionsAbove)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBuyOptionsBelow)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSellOptionsBelow)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSellOptionsAbove)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSellOptionsNearLy)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSellOptionsLimit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSellOptionsDemand)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRaresOptionsLy)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRaresOptionsLimit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRaresOptionsAway)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numNavOptionsRefuelJumps)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numNavOptionsLy)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numOldDataOptionsNearLy)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numOldDataOptionsLimit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numOldDataOptionsMinAge)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numLocalOptionsLy)).EndInit();
            this.panRunOptions.ResumeLayout(false);
            this.panRunOptions.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.pagOutput.ResumeLayout(false);
            this.tabSavedPage1.ResumeLayout(false);
            this.tabSavedPage2.ResumeLayout(false);
            this.tabSavedPage3.ResumeLayout(false);
            this.tabNotesPage.ResumeLayout(false);
            this.tabLogPage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdPilotsLog)).EndInit();
            this.mnuStrip2.ResumeLayout(false);
            this.panSellOptions.ResumeLayout(false);
            this.panSellOptions.PerformLayout();
            this.grpSellOptionsSort.ResumeLayout(false);
            this.grpSellOptionsSort.PerformLayout();
            this.panBuyOptions.ResumeLayout(false);
            this.panBuyOptions.PerformLayout();
            this.grpBuyOptionsSort.ResumeLayout(false);
            this.grpBuyOptionsSort.PerformLayout();
            this.panShipData.ResumeLayout(false);
            this.panShipData.PerformLayout();
            this.panStock.ResumeLayout(false);
            this.panStock.PerformLayout();
            this.panHops.ResumeLayout(false);
            this.panHops.PerformLayout();
            this.panProfit.ResumeLayout(false);
            this.panProfit.PerformLayout();
            this.panOther.ResumeLayout(false);
            this.panOther.PerformLayout();
            this.panMisc.ResumeLayout(false);
            this.panMisc.PerformLayout();
            this.panRouteOptions.ResumeLayout(false);
            this.panMethods.ResumeLayout(false);
            this.panMethods.PerformLayout();
            this.panRaresOptions.ResumeLayout(false);
            this.panRaresOptions.PerformLayout();
            this.grpRaresOptionsType.ResumeLayout(false);
            this.grpRaresOptionsType.PerformLayout();
            this.grpRaresOptionsSort.ResumeLayout(false);
            this.grpRaresOptionsSort.PerformLayout();
            this.panTradeOptions.ResumeLayout(false);
            this.panTradeOptions.PerformLayout();
            this.panMarketOptions.ResumeLayout(false);
            this.grpMarketOptionsType.ResumeLayout(false);
            this.grpMarketOptionsType.PerformLayout();
            this.panShipVendorOptions.ResumeLayout(false);
            this.panShipVendorOptions.PerformLayout();
            this.panNavOptions.ResumeLayout(false);
            this.panNavOptions.PerformLayout();
            this.panOldDataOptions.ResumeLayout(false);
            this.panOldDataOptions.PerformLayout();
            this.panLocalOptions.ResumeLayout(false);
            this.panLocalOptions.PerformLayout();
            this.panOptions.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Label lblRunOptionsDestination;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.ComboBox cboRunOptionsDestination;
        private System.Windows.Forms.Label lblStopWatch;
        private System.ComponentModel.BackgroundWorker backgroundWorker2;
        private System.Windows.Forms.Label lblUnladenLy;
        private System.Windows.Forms.Label lblLadenLy;
        private System.Windows.Forms.Label lblRouteOptionsShipCapacity;
        private System.Windows.Forms.Label lblCommandersCredits;
        private System.Windows.Forms.CheckBox chkRunOptionsTowards;
        private System.Windows.Forms.CheckBox chkRunOptionsLoop;
        private System.Windows.Forms.ToolTip tipToolTips;
        private System.Windows.Forms.Label lblRouteOptionsHops;
        private System.Windows.Forms.Button btnDbUpdate;
        private System.Windows.Forms.CheckBox chkRunOptionsBlkMkt;
        private System.Windows.Forms.Label lblAvoid;
        private System.Windows.Forms.TextBox txtAvoid;
        private System.Windows.Forms.Label lblRunOptionsStartJumps;
        private System.Windows.Forms.Label lblRunOptionsEndJumps;
        private System.Windows.Forms.TextBox txtVia;
        private System.Windows.Forms.Label lblVia;
        private System.Windows.Forms.ComboBox cboMethod;
        private System.Windows.Forms.ContextMenuStrip mnuStrip1;
        private System.Windows.Forms.ToolStripMenuItem mnuCut;
        private System.Windows.Forms.ToolStripMenuItem mnuCopy;
        private System.Windows.Forms.ToolStripMenuItem mnuPaste;
        private System.Windows.Forms.ToolStripSeparator sepSeparator1;
        private System.Windows.Forms.ToolStripMenuItem mnuSelectAll;
        private System.Windows.Forms.ToolStripSeparator sepSeparator2;
        private System.Windows.Forms.ToolStripMenuItem mnuPushNotes;
        private System.Windows.Forms.ToolStripMenuItem mnuNotesClear;
        private System.Windows.Forms.Label lblRunOptionsRoutes;
        private System.Windows.Forms.NumericUpDown numUnladenLy;
        private System.Windows.Forms.NumericUpDown numLadenLy;
        private System.Windows.Forms.NumericUpDown numRouteOptionsShipCapacity;
        private System.Windows.Forms.NumericUpDown numRouteOptionsJumps;
        private System.Windows.Forms.NumericUpDown numRouteOptionsHops;
        private System.Windows.Forms.NumericUpDown numCommandersCredits;
        private System.Windows.Forms.NumericUpDown numShipInsurance;
        private System.Windows.Forms.CheckBox chkRunOptionsUnique;
        private System.Windows.Forms.Button btnGetSystem;
        private System.Windows.Forms.ComboBox cboSourceSystem;
        private System.Windows.Forms.Label lblSourceSystem;
        private System.Windows.Forms.Label lblPadSize;
        private System.Windows.Forms.TextBox txtPadSize;
        private System.Windows.Forms.Label lblRouteOptionsAge;
        private System.Windows.Forms.NumericUpDown numRouteOptionsMaxLSDistance;
        private System.Windows.Forms.NumericUpDown numRouteOptionsGpt;
        private System.Windows.Forms.NumericUpDown numRouteOptionsLimit;
        private System.Windows.Forms.NumericUpDown numRouteOptionsPruneHops;
        private System.Windows.Forms.NumericUpDown numRouteOptionsPruneScore;
        private System.Windows.Forms.NumericUpDown numRouteOptionsLsPenalty;
        private System.Windows.Forms.NumericUpDown numRouteOptionsAge;
        private System.Windows.Forms.NumericUpDown numRunOptionsRoutes;
        private System.Windows.Forms.NumericUpDown numRunOptionsEndJumps;
        private System.Windows.Forms.NumericUpDown numRunOptionsStartJumps;
        private System.Windows.Forms.ToolStripMenuItem mnuDelete;
        private System.ComponentModel.BackgroundWorker backgroundWorker3;
        private System.ComponentModel.BackgroundWorker backgroundWorker4;
        private System.Windows.Forms.CheckBox chkRunOptionsDirect;
        private System.Windows.Forms.Panel panRunOptions;
        private System.Windows.Forms.ToolStripSeparator sepSeparator3;
        private System.Windows.Forms.ToolStripMenuItem mnuSavePage1;
        private System.Windows.Forms.ToolStripMenuItem mnuSavePage2;
        private System.Windows.Forms.ToolStripMenuItem mnuSavePage3;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage pagOutput;
        private System.Windows.Forms.RichTextBox rtbOutput;
        private System.Windows.Forms.TabPage tabSavedPage1;
        private System.Windows.Forms.RichTextBox rtbSaved1;
        private System.Windows.Forms.TabPage tabSavedPage2;
        private System.Windows.Forms.RichTextBox rtbSaved2;
        private System.Windows.Forms.TabPage tabSavedPage3;
        private System.Windows.Forms.RichTextBox rtbSaved3;
        private System.Windows.Forms.TabPage tabNotesPage;
        private System.Windows.Forms.RichTextBox txtNotes;
        private System.Windows.Forms.Label lblRouteOptionsJumps;
        private System.Windows.Forms.Label lblRouteOptionsMaxLS;
        private System.Windows.Forms.Label lblRouteOptionsPruneHops;
        private System.Windows.Forms.Label lblRouteOptionsPruneScore;
        private System.Windows.Forms.Label lblRouteOptionsLsPenalty;
        private System.Windows.Forms.Label lblShipInsurance;
        private System.Windows.Forms.Label lblRouteOptionsCargoLimit;
        private System.Windows.Forms.Label lblRouteOptionsGpt;
        private System.Windows.Forms.ToolStripMenuItem nmuClearSaved1;
        private System.Windows.Forms.ToolStripMenuItem mnuClearSaved2;
        private System.Windows.Forms.ToolStripMenuItem mnuClearSaved3;
        private System.Windows.Forms.Label lblRouteOptionsMargin;
        private System.Windows.Forms.NumericUpDown numRouteOptionsMargin;
        private System.Windows.Forms.Label lblRouteOptionsMaxGpt;
        private System.Windows.Forms.NumericUpDown numRouteOptionsMaxGpt;
        private System.Windows.Forms.Button btnMiniMode;
        private System.Windows.Forms.ComboBox cboCommandersShips;
        private System.Windows.Forms.Label lblUpdateNotify;
        private System.Windows.Forms.PictureBox icoUpdateNotify;
        private System.ComponentModel.BackgroundWorker backgroundWorker5;
        private System.Windows.Forms.NumericUpDown numRouteOptionsStock;
        private System.Windows.Forms.Label lblStock;
        private System.ComponentModel.BackgroundWorker backgroundWorker6;
        private System.Windows.Forms.Label lblRunOptionsLoopInt;
        private System.Windows.Forms.NumericUpDown numRunOptionsLoopInt;
        private System.Windows.Forms.CheckBox chkRunOptionsShorten;
        private System.Windows.Forms.NumericUpDown numRouteOptionsDemand;
        private System.Windows.Forms.Label lblRouteOptionsDemand;
        private System.Windows.Forms.CheckBox chkRunOptionsJumps;
        private System.Windows.Forms.TabPage tabLogPage;
        private System.Windows.Forms.DataGridView grdPilotsLog;
        private System.Windows.Forms.LinkLabel lblTrackerLink;
        private System.Windows.Forms.LinkLabel lblFaqLink;
        private System.Windows.Forms.ContextMenuStrip mnuStrip2;
        private System.Windows.Forms.ToolStripMenuItem mnuInsertAtGridRow;
        private System.Windows.Forms.ToolStripMenuItem mnuForceRefreshGridView;
        private System.Windows.Forms.ToolStripSeparator sepSeparator4;
        private System.Windows.Forms.ToolStripSeparator sepSeparator5;
        private System.Windows.Forms.ToolStripMenuItem mnuCopySystemToSrc;
        private System.Windows.Forms.ToolStripMenuItem mnuCopySystemToDest;
        private System.Windows.Forms.ToolStripMenuItem mnuRemoveAtGridRow;
        private System.Windows.Forms.ToolStripMenuItem mnuForceResortMenuItem;
        private System.Windows.Forms.Button btnCmdrProfile;
        private System.ComponentModel.BackgroundWorker backgroundWorker7;
        private System.Windows.Forms.Button btnSaveSettings;
        private System.Windows.Forms.Panel panShipData;
        private System.Windows.Forms.Panel panMisc;
        private System.Windows.Forms.Panel panOther;
        private System.Windows.Forms.Panel panHops;
        private System.Windows.Forms.Panel panProfit;
        private System.Windows.Forms.Panel panStock;
        private System.Windows.Forms.Label lblRunOptionsPlanetary;
        private System.Windows.Forms.TextBox txtRunOptionsPlanetary;
        private System.Windows.Forms.Button btnSettings;
        private System.Windows.Forms.Panel panRouteOptions;
        private System.Windows.Forms.Panel panMethods;
        private System.Windows.Forms.Button btnRunOptionsSwap;
        private System.Windows.Forms.Panel panBuyOptions;
        private System.Windows.Forms.GroupBox grpBuyOptionsSort;
        private System.Windows.Forms.Label lblBuyOptionsSort;
        private System.Windows.Forms.RadioButton optBuyOptionsSupply;
        private System.Windows.Forms.RadioButton optBuyOptionsDistance;
        private System.Windows.Forms.RadioButton optBuyOptionsPrice;
        private System.Windows.Forms.ComboBox cboBuyOptionsCommodities;
        private System.Windows.Forms.Label lblBuyOptionsCommodity;
        private System.Windows.Forms.NumericUpDown numBuyOptionsBelow;
        private System.Windows.Forms.Label lblBuyOptionsBelow;
        private System.Windows.Forms.NumericUpDown numBuyOptionsAbove;
        private System.Windows.Forms.Label lblBuyOptionsAbove;
        private System.Windows.Forms.Label lblBuyOptionsPads;
        private System.Windows.Forms.NumericUpDown numBuyOptionsNearLy;
        private System.Windows.Forms.TextBox txtBuyOptionsPads;
        private System.Windows.Forms.Label lblBuyOptionsLimit;
        private System.Windows.Forms.Label lblBuyOptionsPlanetary;
        private System.Windows.Forms.CheckBox chkBuyOptionsOneStop;
        private System.Windows.Forms.TextBox txtBuyOptionsPlanetary;
        private System.Windows.Forms.CheckBox chkBuyOptionsBlkMkt;
        private System.Windows.Forms.NumericUpDown numBuyOptionsLimit;
        private System.Windows.Forms.TextBox txtBuyOptionsAvoid;
        private System.Windows.Forms.Label lblBuyOptionsAvoid;
        private System.Windows.Forms.Label lblBuyOptionsNearLy;
        private System.Windows.Forms.NumericUpDown numBuyOptionsSupply;
        private System.Windows.Forms.Label lblBuyOptionsSupply;
        private System.Windows.Forms.Panel panSellOptions;
        private System.Windows.Forms.GroupBox grpSellOptionsSort;
        private System.Windows.Forms.Label lblSellOptionsSort;
        private System.Windows.Forms.RadioButton optSellOptionsSupply;
        private System.Windows.Forms.RadioButton optSellOptionsPrice;
        private System.Windows.Forms.ComboBox cboSellOptionsCommodities;
        private System.Windows.Forms.Label lblSellOptionsCommodity;
        private System.Windows.Forms.NumericUpDown numSellOptionsBelow;
        private System.Windows.Forms.Label lblSellOptionsBelow;
        private System.Windows.Forms.NumericUpDown numSellOptionsAbove;
        private System.Windows.Forms.Label lblSellOptionsAbove;
        private System.Windows.Forms.Label lblSellOptionsPads;
        private System.Windows.Forms.NumericUpDown numSellOptionsNearLy;
        private System.Windows.Forms.TextBox txtSellOptionsPads;
        private System.Windows.Forms.Label lblSellOptionsLimit;
        private System.Windows.Forms.Label lblSellOptionsPlanetary;
        private System.Windows.Forms.TextBox txtSellOptionsPlanetary;
        private System.Windows.Forms.NumericUpDown numSellOptionsLimit;
        private System.Windows.Forms.TextBox txtSellOptionsAvoid;
        private System.Windows.Forms.Label lblSellOptionsAvoid;
        private System.Windows.Forms.Label lblSellOptionsNearLy;
        private System.Windows.Forms.NumericUpDown numSellOptionsDemand;
        private System.Windows.Forms.Label lblSellOptionsDemand;
        private System.Windows.Forms.Panel panRaresOptions;
        private System.Windows.Forms.GroupBox grpRaresOptionsType;
        private System.Windows.Forms.RadioButton optRaresOptionsAll;
        private System.Windows.Forms.Label lblRaresOptionsType;
        private System.Windows.Forms.RadioButton optRaresOptionsLegal;
        private System.Windows.Forms.RadioButton optRaresOptionsIllegal;
        private System.Windows.Forms.GroupBox grpRaresOptionsSort;
        private System.Windows.Forms.Label lblRaresOptionsSort;
        private System.Windows.Forms.RadioButton optRaresOptionsDistance;
        private System.Windows.Forms.RadioButton optRaresOptionsPrice;
        private System.Windows.Forms.Label lblRaresOptionsPads;
        private System.Windows.Forms.NumericUpDown numRaresOptionsAway;
        private System.Windows.Forms.NumericUpDown numRaresOptionsLy;
        private System.Windows.Forms.TextBox txtRaresOptionsPads;
        private System.Windows.Forms.Label lblRaresOptionsLimit;
        private System.Windows.Forms.Label lblRaresOptionsPlanetary;
        private System.Windows.Forms.CheckBox chkRaresOptionsReverse;
        private System.Windows.Forms.TextBox txtRaresOptionsPlanetary;
        private System.Windows.Forms.CheckBox chkRaresOptionsQuiet;
        private System.Windows.Forms.NumericUpDown numRaresOptionsLimit;
        private System.Windows.Forms.TextBox txtRaresOptionsFrom;
        private System.Windows.Forms.Label lblRaresOptionsFrom;
        private System.Windows.Forms.Label lblRaresOptionsLy;
        private System.Windows.Forms.Panel panTradeOptions;
        private System.Windows.Forms.Button btnTradeOptionsSwap;
        private System.Windows.Forms.Label lblTradeOptionDestination;
        private System.Windows.Forms.ComboBox cboTradeOptionDestination;
        private System.Windows.Forms.Panel panMarketOptions;
        private System.Windows.Forms.GroupBox grpMarketOptionsType;
        private System.Windows.Forms.Label lblMarketOptionsType;
        private System.Windows.Forms.RadioButton optMarketOptionsBuy;
        private System.Windows.Forms.RadioButton optMarketOptionsSell;
        private System.Windows.Forms.RadioButton optMarketOptionsAll;
        private System.Windows.Forms.Panel panNavOptions;
        private System.Windows.Forms.Button btnNavOptionsSwap;
        private System.Windows.Forms.CheckBox chkNavOptionsStations;
        private System.Windows.Forms.Label lblNavOptionsDestination;
        private System.Windows.Forms.Label lblNavOptionsRefuelJumps;
        private System.Windows.Forms.NumericUpDown numNavOptionsRefuelJumps;
        private System.Windows.Forms.ComboBox cboNavOptionsDestination;
        private System.Windows.Forms.Panel panShipVendorOptions;
        private System.Windows.Forms.ComboBox cboShipVendorOptionShips;
        private System.Windows.Forms.Label lblShipVendorOptionShips;
        private System.Windows.Forms.Label lblNavOptionsPads;
        private System.Windows.Forms.TextBox txtNavOptionsPads;
        private System.Windows.Forms.Label lblNavOptionsPlanetary;
        private System.Windows.Forms.TextBox txtNavOptionsPlanetary;
        private System.Windows.Forms.TextBox txtNavOptionsAvoid;
        private System.Windows.Forms.Label lblNavOptionsAvoid;
        private System.Windows.Forms.TextBox txtNavOptionsVia;
        private System.Windows.Forms.Label lblNavOptionsVia;
        private System.Windows.Forms.NumericUpDown numNavOptionsLy;
        private System.Windows.Forms.Label lblNavOptionsLy;
        private System.Windows.Forms.Panel panOldDataOptions;
        private System.Windows.Forms.NumericUpDown numOldDataOptionsNearLy;
        private System.Windows.Forms.Label lblOldDataOptionsLimit;
        private System.Windows.Forms.NumericUpDown numOldDataOptionsLimit;
        private System.Windows.Forms.Label lblOldDataOptionsNearLy;
        private System.Windows.Forms.CheckBox chkOldDataOptionsRoute;
        private System.Windows.Forms.Label lblOldDataOptionsMinAge;
        private System.Windows.Forms.NumericUpDown numOldDataOptionsMinAge;
        private System.Windows.Forms.Panel panLocalOptions;
        private System.Windows.Forms.CheckBox chkLocalOptionsTrading;
        private System.Windows.Forms.NumericUpDown numLocalOptionsLy;
        private System.Windows.Forms.Label lblLocalOptionsLy;
        private System.Windows.Forms.Label lblLocalOptionsPads;
        private System.Windows.Forms.TextBox txtLocalOptionsPads;
        private System.Windows.Forms.Label lblLocalOptionsPlanetary;
        private System.Windows.Forms.TextBox txtLocalOptionsPlanetary;
        private System.Windows.Forms.Button btnLocalOptionsReset;
        private System.Windows.Forms.CheckBox chkLocalOptionsStations;
        private System.Windows.Forms.CheckBox chkLocalOptionsOutfitting;
        private System.Windows.Forms.CheckBox chkLocalOptionsRearm;
        private System.Windows.Forms.CheckBox chkLocalOptionsCommodities;
        private System.Windows.Forms.CheckBox chkLocalOptionsRepair;
        private System.Windows.Forms.CheckBox chkLocalOptionsBlkMkt;
        private System.Windows.Forms.CheckBox chkLocalOptionsRefuel;
        private System.Windows.Forms.CheckBox chkLocalOptionsShipyard;
        private System.Windows.Forms.Panel panOptions;
        private System.Windows.Forms.Button btnLocalOptionsAll;
        private System.Windows.Forms.CheckBox chkSellOptionsBlkMkt;
        private System.Windows.Forms.ContextMenuStrip mnuSetValues;
        private System.Windows.Forms.ToolStripMenuItem mnuLadenLY;
        private System.Windows.Forms.ToolStripMenuItem mnuUnladenLY;
        private System.Windows.Forms.ToolStripSeparator mnuSep2;
        private System.Windows.Forms.ToolStripMenuItem mnuCapacity;
        private System.Windows.Forms.ToolStripSeparator mnuSep3;
        private System.Windows.Forms.ToolStripMenuItem mnuReset;
    }
}

