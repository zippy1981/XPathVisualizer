namespace XPathVisualizer
{
    partial class XPathVisualizerTool
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(XPathVisualizerTool));
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.btnExpandCollapse = new System.Windows.Forms.Button();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.pnlInput = new System.Windows.Forms.Panel();
            this.tbXmlns = new System.Windows.Forms.TextBox();
            this.tbPrefix = new System.Windows.Forms.TextBox();
            this.btnAddNsPrefix = new System.Windows.Forms.Button();
            this.pnlPrefixList = new System.Windows.Forms.Panel();
            this.btnEvalXpath = new System.Windows.Forms.Button();
            this.btnLoadXml = new System.Windows.Forms.Button();
            this.tbXpath = new System.Windows.Forms.RichTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.tbXmlDoc = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.matchPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.btn_NextMatch = new Ionic.WinForms.RepeatButton();
            this.lblMatch = new System.Windows.Forms.Label();
            this.btn_PrevMatch = new Ionic.WinForms.RepeatButton();
            this.customTabControl1 = new Ionic.WinForms.CustomTabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.richTextBox1 = new Ionic.WinForms.RichTextBoxEx();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toggleLineNumbersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stripNamespacesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.extractHighlightedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeSelectedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.progressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.linkToCodeplex = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.label3 = new System.Windows.Forms.Label();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.pnlInput.SuspendLayout();
            this.matchPanel.SuspendLayout();
            this.customTabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            //
            // splitContainer3
            //
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.IsSplitterFixed = true;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Name = "splitContainer3";
            this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
            //
            // splitContainer3.Panel1
            //
            this.splitContainer3.Panel1.Controls.Add(this.btnExpandCollapse);
            this.splitContainer3.Panel1.Controls.Add(this.groupBox1);
            this.splitContainer3.Panel1.Controls.Add(this.btnEvalXpath);
            this.splitContainer3.Panel1.Controls.Add(this.btnLoadXml);
            this.splitContainer3.Panel1.Controls.Add(this.tbXpath);
            this.splitContainer3.Panel1.Controls.Add(this.label2);
            this.splitContainer3.Panel1.Controls.Add(this.btnBrowse);
            this.splitContainer3.Panel1.Controls.Add(this.tbXmlDoc);
            this.splitContainer3.Panel1.Controls.Add(this.label1);
            this.splitContainer3.Panel1MinSize = 114;
            //
            // splitContainer3.Panel2
            //
            this.splitContainer3.Panel2.Controls.Add(this.matchPanel);
            this.splitContainer3.Panel2.Controls.Add(this.customTabControl1);
            this.splitContainer3.Size = new System.Drawing.Size(538, 354);
            this.splitContainer3.SplitterDistance = 152;
            this.splitContainer3.SplitterWidth = 6;
            this.splitContainer3.TabIndex = 0;
            //
            // btnExpandCollapse
            //
            this.btnExpandCollapse.BackColor = System.Drawing.Color.Transparent;
            this.btnExpandCollapse.FlatAppearance.BorderSize = 0;
            this.btnExpandCollapse.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExpandCollapse.ImageIndex = 0;
            this.btnExpandCollapse.ImageList = this.imageList1;
            this.btnExpandCollapse.Location = new System.Drawing.Point(148, 68);
            this.btnExpandCollapse.Name = "btnExpandCollapse";
            this.btnExpandCollapse.Size = new System.Drawing.Size(12, 12);
            this.btnExpandCollapse.TabIndex = 61;
            this.btnExpandCollapse.TabStop = false;
            this.toolTip1.SetToolTip(this.btnExpandCollapse, "Collapse");
            this.btnExpandCollapse.UseVisualStyleBackColor = false;
            this.btnExpandCollapse.Click += new System.EventHandler(this.button1_Click);
            //
            // imageList1
            //
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Fuchsia;
            this.imageList1.Images.SetKeyName(0, "Collapse_small.bmp");
            this.imageList1.Images.SetKeyName(1, "Expand_small.bmp");
            //
            // groupBox1
            //
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBox1.Controls.Add(this.pnlInput);
            this.groupBox1.Controls.Add(this.pnlPrefixList);
            this.groupBox1.Location = new System.Drawing.Point(12, 67);
            this.groupBox1.MinimumSize = new System.Drawing.Size(0, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(517, 86);
            this.groupBox1.TabIndex = 48;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "namespaces and prefixes";
            //
            // pnlInput
            //
            this.pnlInput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlInput.Controls.Add(this.tbXmlns);
            this.pnlInput.Controls.Add(this.tbPrefix);
            this.pnlInput.Controls.Add(this.btnAddNsPrefix);
            this.pnlInput.Location = new System.Drawing.Point(2, 14);
            this.pnlInput.Name = "pnlInput";
            this.pnlInput.Size = new System.Drawing.Size(513, 24);
            this.pnlInput.TabIndex = 62;
            //
            // tbXmlns
            //
            this.tbXmlns.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbXmlns.Location = new System.Drawing.Point(92, 2);
            this.tbXmlns.Name = "tbXmlns";
            this.tbXmlns.Size = new System.Drawing.Size(373, 20);
            this.tbXmlns.TabIndex = 55;
            this.toolTip1.SetToolTip(this.tbXmlns, "enter an xml namespace");
            //
            // tbPrefix
            //
            this.tbPrefix.Location = new System.Drawing.Point(2, 2);
            this.tbPrefix.Name = "tbPrefix";
            this.tbPrefix.Size = new System.Drawing.Size(78, 20);
            this.tbPrefix.TabIndex = 50;
            this.toolTip1.SetToolTip(this.tbPrefix, "enter a unique xmlns prefix");
            this.tbPrefix.TextChanged += new System.EventHandler(this.tbPrefix_TextChanged);
            this.tbPrefix.CausesValidation = true;
            this.tbPrefix.Validating += this.textBox1_Validating;
            //
            // btnAddNsPrefix
            //
            this.btnAddNsPrefix.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddNsPrefix.Location = new System.Drawing.Point(473, 2);
            this.btnAddNsPrefix.Name = "btnAddNsPrefix";
            this.btnAddNsPrefix.Size = new System.Drawing.Size(28, 20);
            this.btnAddNsPrefix.TabIndex = 60;
            this.btnAddNsPrefix.Text = "+";
            this.toolTip1.SetToolTip(this.btnAddNsPrefix, "add the specified namespace+prefix");
            this.btnAddNsPrefix.UseVisualStyleBackColor = true;
            this.btnAddNsPrefix.Click += new System.EventHandler(this.btnAddNsPrefix_Click);
            //
            // pnlPrefixList
            //
            this.pnlPrefixList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlPrefixList.Location = new System.Drawing.Point(2, 42);
            this.pnlPrefixList.Name = "pnlPrefixList";
            this.pnlPrefixList.Size = new System.Drawing.Size(513, 38);
            this.pnlPrefixList.TabIndex = 51;
            //
            // btnEvalXpath
            //
            this.btnEvalXpath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEvalXpath.Location = new System.Drawing.Point(486, 41);
            this.btnEvalXpath.Name = "btnEvalXpath";
            this.btnEvalXpath.Size = new System.Drawing.Size(40, 23);
            this.btnEvalXpath.TabIndex = 45;
            this.btnEvalXpath.Text = "Eval";
            this.toolTip1.SetToolTip(this.btnEvalXpath, "evaluate the expression");
            this.btnEvalXpath.UseVisualStyleBackColor = true;
            this.btnEvalXpath.Click += new System.EventHandler(this.btnEvalXpath_Click);
            //
            // btnLoadXml
            //
            this.btnLoadXml.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLoadXml.Location = new System.Drawing.Point(486, 12);
            this.btnLoadXml.Name = "btnLoadXml";
            this.btnLoadXml.Size = new System.Drawing.Size(40, 23);
            this.btnLoadXml.TabIndex = 30;
            this.btnLoadXml.Text = "Load";
            this.toolTip1.SetToolTip(this.btnLoadXml, "load the document");
            this.btnLoadXml.UseVisualStyleBackColor = true;
            this.btnLoadXml.Click += new System.EventHandler(this.btnLoadXml_Click);
            //
            // tbXpath
            //
            this.tbXpath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbXpath.BackColor = System.Drawing.SystemColors.Window;
            this.tbXpath.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbXpath.DetectUrls = false;
            this.tbXpath.Location = new System.Drawing.Point(108, 42);
            this.tbXpath.Multiline = false;
            this.tbXpath.Name = "tbXpath";
            this.tbXpath.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.tbXpath.Size = new System.Drawing.Size(372, 20);
            this.tbXpath.TabIndex = 40;
            this.tbXpath.Text = "";
            this.toolTip1.SetToolTip(this.tbXpath, "enter an XPath expression");
            this.tbXpath.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbXpath_KeyDown);
            this.tbXpath.MouseUp += new System.Windows.Forms.MouseEventHandler(this.tbXpath_MouseUp);
            this.tbXpath.TextChanged += new System.EventHandler(this.tbXpath_TextChanged);
            //
            // label2
            //
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 46);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(90, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "XPath Expression";
            //
            // btnBrowse
            //
            this.btnBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowse.Location = new System.Drawing.Point(451, 10);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(29, 26);
            this.btnBrowse.TabIndex = 20;
            this.btnBrowse.Text = "...";
            this.toolTip1.SetToolTip(this.btnBrowse, "browse the filesystem...");
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            //
            // tbXmlDoc
            //
            this.tbXmlDoc.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbXmlDoc.Location = new System.Drawing.Point(108, 13);
            this.tbXmlDoc.Name = "tbXmlDoc";
            this.tbXmlDoc.Size = new System.Drawing.Size(337, 20);
            this.tbXmlDoc.TabIndex = 10;
            this.toolTip1.SetToolTip(this.tbXmlDoc, "a file path or URL to load from");
            //
            // label1
            //
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "XML Doc";
            //
            // matchPanel
            //
            this.matchPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.matchPanel.AutoSize = true;
            this.matchPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.matchPanel.BackColor = System.Drawing.SystemColors.Window;
            this.matchPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.matchPanel.Controls.Add(this.btn_NextMatch);
            this.matchPanel.Controls.Add(this.lblMatch);
            this.matchPanel.Controls.Add(this.btn_PrevMatch);
            this.matchPanel.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.matchPanel.Location = new System.Drawing.Point(420, 24);
            this.matchPanel.Name = "matchPanel";
            this.matchPanel.Size = new System.Drawing.Size(92, 31);
            this.matchPanel.TabIndex = 84;
            this.matchPanel.WrapContents = false;
            //
            // btn_NextMatch
            //
            this.btn_NextMatch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_NextMatch.DelayTicks = 3;
            this.btn_NextMatch.Interval = 150;
            this.btn_NextMatch.Location = new System.Drawing.Point(65, 3);
            this.btn_NextMatch.Name = "btn_NextMatch";
            this.btn_NextMatch.Size = new System.Drawing.Size(22, 23);
            this.btn_NextMatch.TabIndex = 81;
            this.btn_NextMatch.Text = ">";
            this.toolTip1.SetToolTip(this.btn_NextMatch, "next match");
            this.btn_NextMatch.UseVisualStyleBackColor = true;
            this.btn_NextMatch.Click += new System.EventHandler(this.btn_NextMatch_Click);
            //
            // lblMatch
            //
            this.lblMatch.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblMatch.AutoSize = true;
            this.lblMatch.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblMatch.Location = new System.Drawing.Point(29, 4);
            this.lblMatch.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.lblMatch.Name = "lblMatch";
            this.lblMatch.Padding = new System.Windows.Forms.Padding(3);
            this.lblMatch.Size = new System.Drawing.Size(32, 21);
            this.lblMatch.TabIndex = 83;
            this.lblMatch.Text = "0/0";
            this.lblMatch.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // btn_PrevMatch
            //
            this.btn_PrevMatch.AccessibleName = "s";
            this.btn_PrevMatch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_PrevMatch.DelayTicks = 3;
            this.btn_PrevMatch.Interval = 150;
            this.btn_PrevMatch.Location = new System.Drawing.Point(3, 3);
            this.btn_PrevMatch.Name = "btn_PrevMatch";
            this.btn_PrevMatch.Size = new System.Drawing.Size(22, 23);
            this.btn_PrevMatch.TabIndex = 82;
            this.btn_PrevMatch.Text = "<";
            this.toolTip1.SetToolTip(this.btn_PrevMatch, "previous match");
            this.btn_PrevMatch.UseVisualStyleBackColor = true;
            this.btn_PrevMatch.Click += new System.EventHandler(this.btn_PrevMatch_Click);
            //
            // customTabControl1
            //
            this.customTabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.customTabControl1.Controls.Add(this.tabPage1);
            this.customTabControl1.ItemSize = new System.Drawing.Size(0, 15);
            this.customTabControl1.Location = new System.Drawing.Point(0, 1);
            this.customTabControl1.Name = "customTabControl1";
            this.customTabControl1.Padding = new System.Drawing.Point(18, 0);
            this.customTabControl1.SelectedIndex = 0;
            this.customTabControl1.Size = new System.Drawing.Size(538, 195);
            this.customTabControl1.TabIndex = 86;
            this.customTabControl1.TabStop = false;
            this.customTabControl1.SelectedIndexChanged += new System.EventHandler(this.customTabControl1_SelectedIndexChanged);
            //
            // tabPage1
            //
            this.tabPage1.Controls.Add(this.richTextBox1);
            this.tabPage1.Location = new System.Drawing.Point(4, 19);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(530, 172);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "tabPage1   ";
            this.tabPage1.UseVisualStyleBackColor = true;
            //
            // richTextBox1
            //
            this.richTextBox1.BackColor = System.Drawing.SystemColors.Window;
            this.richTextBox1.ContextMenuStrip = this.contextMenuStrip1;
            this.richTextBox1.DetectUrls = false;
            this.richTextBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBox1.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBox1.Location = new System.Drawing.Point(3, 3);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.NumberAlignment = System.Drawing.StringAlignment.Center;
            this.richTextBox1.NumberBackground1 = System.Drawing.SystemColors.ControlLight;
            this.richTextBox1.NumberBackground2 = System.Drawing.SystemColors.Window;
            this.richTextBox1.NumberBorder = System.Drawing.SystemColors.ControlDark;
            this.richTextBox1.NumberBorderThickness = 1F;
            this.richTextBox1.NumberColor = System.Drawing.Color.DarkGray;
            this.richTextBox1.NumberFont = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBox1.NumberLeadingZeroes = false;
            this.richTextBox1.NumberLineCounting = Ionic.WinForms.RichTextBoxEx.LineCounting.CRLF;
            this.richTextBox1.NumberPadding = 2;
            this.richTextBox1.ShowLineNumbers = true;
            this.richTextBox1.Size = new System.Drawing.Size(524, 166);
            this.richTextBox1.TabIndex = 80;
            this.richTextBox1.Text = "";
            this.richTextBox1.Leave += new System.EventHandler(this.richTextBox1_Leave);
            this.richTextBox1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.richTextBox1_KeyPress);
            //
            // contextMenuStrip1
            //
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.toggleLineNumbersToolStripMenuItem,
            this.stripNamespacesToolStripMenuItem,
            this.extractHighlightedToolStripMenuItem,
            this.removeSelectedToolStripMenuItem,
            this.copyToolStripMenuItem,
            this.copyAllToolStripMenuItem,
            this.pasteToolStripMenuItem,
            this.saveAsToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(189, 202);
            //
            // toolStripMenuItem1
            //
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(188, 22);
            this.toolStripMenuItem1.Text = "Reformat";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
            //
            // toggleLineNumbersToolStripMenuItem
            //
            this.toggleLineNumbersToolStripMenuItem.Name = "toggleLineNumbersToolStripMenuItem";
            this.toggleLineNumbersToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.toggleLineNumbersToolStripMenuItem.Text = "Toggle Line Numbers";
            this.toggleLineNumbersToolStripMenuItem.Click += new System.EventHandler(this.toggleLineNumbersToolStripMenuItem_Click);
            //
            // stripNamespacesToolStripMenuItem
            //
            this.stripNamespacesToolStripMenuItem.Name = "stripNamespacesToolStripMenuItem";
            this.stripNamespacesToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.stripNamespacesToolStripMenuItem.Text = "Strip Namespaces";
            this.stripNamespacesToolStripMenuItem.Click += new System.EventHandler(this.stripNamespacesToolStripMenuItem_Click);
            //
            // extractHighlightedToolStripMenuItem
            //
            this.extractHighlightedToolStripMenuItem.Name = "extractHighlightedToolStripMenuItem";
            this.extractHighlightedToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.extractHighlightedToolStripMenuItem.Text = "Extract highlighted";
            this.extractHighlightedToolStripMenuItem.Click += new System.EventHandler(this.extractHighlightedToolStripMenuItem_Click);
            //
            // removeSelectedToolStripMenuItem
            //
            this.removeSelectedToolStripMenuItem.Name = "removeSelectedToolStripMenuItem";
            this.removeSelectedToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.removeSelectedToolStripMenuItem.Text = "Remove highlighted";
            this.removeSelectedToolStripMenuItem.Click += new System.EventHandler(this.removeSelectedToolStripMenuItem_Click);
            //
            // copyToolStripMenuItem
            //
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.copyToolStripMenuItem.Text = "Copy";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            //
            // copyAllToolStripMenuItem
            //
            this.copyAllToolStripMenuItem.Name = "copyAllToolStripMenuItem";
            this.copyAllToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.copyAllToolStripMenuItem.Text = "Copy All";
            this.copyAllToolStripMenuItem.Click += new System.EventHandler(this.copyAllToolStripMenuItem_Click);
            //
            // pasteToolStripMenuItem
            //
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            this.pasteToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.pasteToolStripMenuItem.Text = "Paste";
            this.pasteToolStripMenuItem.Click += new System.EventHandler(this.pasteToolStripMenuItem_Click);
            //
            // saveAsToolStripMenuItem
            //
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.saveAsToolStripMenuItem.Text = "Save As...";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            //
            // statusStrip1
            //
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus,
            this.progressBar1,
            this.linkToCodeplex});
            this.statusStrip1.Location = new System.Drawing.Point(0, 354);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(538, 22);
            this.statusStrip1.TabIndex = 0;
            this.statusStrip1.Text = "statusStrip1";
            //
            // lblStatus
            //
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(222, 17);
            this.lblStatus.Spring = true;
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // progressBar1
            //
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(100, 16);
            this.progressBar1.ToolTipText = "Highlight progress";
            //
            // linkToCodeplex
            //
            this.linkToCodeplex.IsLink = true;
            this.linkToCodeplex.LinkBehavior = System.Windows.Forms.LinkBehavior.AlwaysUnderline;
            this.linkToCodeplex.Name = "linkToCodeplex";
            this.linkToCodeplex.Size = new System.Drawing.Size(199, 17);
            this.linkToCodeplex.Text = "http://XPathVisualizer.codeplex.com";
            this.linkToCodeplex.Click += new System.EventHandler(this.linkToCodeplex_Click);
            //
            // contextMenuStrip2
            //
            this.contextMenuStrip2.Name = "contextMenuStrip2";
            this.contextMenuStrip2.Size = new System.Drawing.Size(61, 4);
            //
            // label3
            //
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.label3.Location = new System.Drawing.Point(18, 131);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(478, 31);
            this.label3.TabIndex = 81;
            this.label3.Text = "ctrl-N to create a new, blank document";
            //
            // XPathVisualizerTool
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(538, 376);
            this.Controls.Add(this.splitContainer3);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.label3);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MinimumSize = new System.Drawing.Size(440, 320);
            this.Name = "XPathVisualizerTool";
            this.Text = "XPathVisualizer";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Resize += new System.EventHandler(this.XPathVisualizerTool_Resize);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.form_KeyDown);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel1.PerformLayout();
            this.splitContainer3.Panel2.ResumeLayout(false);
            this.splitContainer3.Panel2.PerformLayout();
            this.splitContainer3.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.pnlInput.ResumeLayout(false);
            this.pnlInput.PerformLayout();
            this.matchPanel.ResumeLayout(false);
            this.matchPanel.PerformLayout();
            this.customTabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.contextMenuStrip1.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbXmlDoc;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.Button btnLoadXml;
        private System.Windows.Forms.Button btnBrowse;
        private Ionic.WinForms.RichTextBoxEx richTextBox1;
        private System.Windows.Forms.Button btnEvalXpath;
        private System.Windows.Forms.RichTextBox tbXpath;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
        private System.Windows.Forms.Button btnAddNsPrefix;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox tbXmlns;
        private System.Windows.Forms.TextBox tbPrefix;
        private System.Windows.Forms.Panel pnlPrefixList;
        private System.Windows.Forms.ToolStripProgressBar progressBar1;
        private System.Windows.Forms.ToolStripStatusLabel linkToCodeplex;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem copyAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
        private System.Windows.Forms.FlowLayoutPanel matchPanel;
        private System.Windows.Forms.Label lblMatch;
        private System.Windows.Forms.ToolStripMenuItem stripNamespacesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeSelectedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.Button btnExpandCollapse;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Panel pnlInput;
        private Ionic.WinForms.RepeatButton btn_PrevMatch;
        private Ionic.WinForms.RepeatButton btn_NextMatch;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip2;
        private Ionic.WinForms.CustomTabControl customTabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.ToolStripMenuItem extractHighlightedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toggleLineNumbersToolStripMenuItem;
        private System.Windows.Forms.Label label3;
    }
}

