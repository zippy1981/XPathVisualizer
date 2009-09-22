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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(XPathVisualizerTool));
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.pnlPrefixList = new System.Windows.Forms.Panel();
            this.tbXmlns = new System.Windows.Forms.TextBox();
            this.tbPrefix = new System.Windows.Forms.TextBox();
            this.btnAddNsPrefix = new System.Windows.Forms.Button();
            this.btnEvalXpath = new System.Windows.Forms.Button();
            this.btnLoadXml = new System.Windows.Forms.Button();
            this.tbXpath = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.tbXmlDoc = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.progressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.linkToCodeplex = new System.Windows.Forms.ToolStripStatusLabel();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Name = "splitContainer3";
            this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.groupBox1);
            this.splitContainer3.Panel1.Controls.Add(this.btnEvalXpath);
            this.splitContainer3.Panel1.Controls.Add(this.btnLoadXml);
            this.splitContainer3.Panel1.Controls.Add(this.tbXpath);
            this.splitContainer3.Panel1.Controls.Add(this.label2);
            this.splitContainer3.Panel1.Controls.Add(this.btnBrowse);
            this.splitContainer3.Panel1.Controls.Add(this.tbXmlDoc);
            this.splitContainer3.Panel1.Controls.Add(this.label1);
            this.splitContainer3.Panel1MinSize = 140;
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.richTextBox1);
            this.splitContainer3.Size = new System.Drawing.Size(530, 300);
            this.splitContainer3.SplitterDistance = 175;
            this.splitContainer3.SplitterWidth = 6;
            this.splitContainer3.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.pnlPrefixList);
            this.groupBox1.Controls.Add(this.tbXmlns);
            this.groupBox1.Controls.Add(this.tbPrefix);
            this.groupBox1.Controls.Add(this.btnAddNsPrefix);
            this.groupBox1.Location = new System.Drawing.Point(12, 67);
            this.groupBox1.MinimumSize = new System.Drawing.Size(0, 70);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(506, 104);
            this.groupBox1.TabIndex = 48;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "namespaces and prefixes";
            // 
            // pnlPrefixList
            // 
            this.pnlPrefixList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlPrefixList.Location = new System.Drawing.Point(2, 42);
            this.pnlPrefixList.Name = "pnlPrefixList";
            this.pnlPrefixList.Size = new System.Drawing.Size(502, 56);
            this.pnlPrefixList.TabIndex = 51;
            // 
            // tbXmlns
            // 
            this.tbXmlns.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbXmlns.Location = new System.Drawing.Point(96, 16);
            this.tbXmlns.Name = "tbXmlns";
            this.tbXmlns.Size = new System.Drawing.Size(364, 20);
            this.tbXmlns.TabIndex = 55;
            // 
            // tbPrefix
            // 
            this.tbPrefix.Location = new System.Drawing.Point(6, 16);
            this.tbPrefix.Name = "tbPrefix";
            this.tbPrefix.Size = new System.Drawing.Size(74, 20);
            this.tbPrefix.TabIndex = 50;
            this.tbPrefix.TextChanged += new System.EventHandler(this.tbPrefix_TextChanged);
            // 
            // btnAddNsPrefix
            // 
            this.btnAddNsPrefix.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddNsPrefix.Location = new System.Drawing.Point(466, 16);
            this.btnAddNsPrefix.Name = "btnAddNsPrefix";
            this.btnAddNsPrefix.Size = new System.Drawing.Size(28, 20);
            this.btnAddNsPrefix.TabIndex = 60;
            this.btnAddNsPrefix.Text = "+";
            this.btnAddNsPrefix.UseVisualStyleBackColor = true;
            this.btnAddNsPrefix.Click += new System.EventHandler(this.btnAddNsPrefix_Click);
            // 
            // btnEvalXpath
            // 
            this.btnEvalXpath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEvalXpath.Location = new System.Drawing.Point(478, 41);
            this.btnEvalXpath.Name = "btnEvalXpath";
            this.btnEvalXpath.Size = new System.Drawing.Size(40, 23);
            this.btnEvalXpath.TabIndex = 45;
            this.btnEvalXpath.Text = "Eval";
            this.btnEvalXpath.UseVisualStyleBackColor = true;
            this.btnEvalXpath.Click += new System.EventHandler(this.btnEvalXpath_Click);
            // 
            // btnLoadXml
            // 
            this.btnLoadXml.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLoadXml.Location = new System.Drawing.Point(478, 12);
            this.btnLoadXml.Name = "btnLoadXml";
            this.btnLoadXml.Size = new System.Drawing.Size(40, 23);
            this.btnLoadXml.TabIndex = 30;
            this.btnLoadXml.Text = "Load";
            this.btnLoadXml.UseVisualStyleBackColor = true;
            this.btnLoadXml.Click += new System.EventHandler(this.btnLoadXml_Click);
            // 
            // tbXpath
            // 
            this.tbXpath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbXpath.Location = new System.Drawing.Point(108, 42);
            this.tbXpath.Name = "tbXpath";
            this.tbXpath.Size = new System.Drawing.Size(364, 20);
            this.tbXpath.TabIndex = 40;
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
            this.btnBrowse.Location = new System.Drawing.Point(443, 10);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(29, 26);
            this.btnBrowse.TabIndex = 20;
            this.btnBrowse.Text = "...";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // tbXmlDoc
            // 
            this.tbXmlDoc.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbXmlDoc.Location = new System.Drawing.Point(108, 13);
            this.tbXmlDoc.Name = "tbXmlDoc";
            this.tbXmlDoc.Size = new System.Drawing.Size(329, 20);
            this.tbXmlDoc.TabIndex = 10;
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
            // richTextBox1
            // 
            this.richTextBox1.BackColor = System.Drawing.SystemColors.Window;
            this.richTextBox1.DetectUrls = false;
            this.richTextBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBox1.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBox1.Location = new System.Drawing.Point(0, 0);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(530, 119);
            this.richTextBox1.TabIndex = 80;
            this.richTextBox1.Text = "";
            this.richTextBox1.Leave += new System.EventHandler(this.richTextBox1_Leave);
            this.richTextBox1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.richTextBox1_KeyPress);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus,
            this.progressBar1,
            this.linkToCodeplex});
            this.statusStrip1.Location = new System.Drawing.Point(0, 300);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(530, 22);
            this.statusStrip1.TabIndex = 0;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // lblStatus
            // 
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(214, 17);
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
            // XPathVisualizerTool
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(530, 322);
            this.Controls.Add(this.splitContainer3);
            this.Controls.Add(this.statusStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(440, 320);
            this.Name = "XPathVisualizerTool";
            this.Text = "XPathVisualizer";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Resize += new System.EventHandler(this.XPathVisualizerTool_Resize);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel1.PerformLayout();
            this.splitContainer3.Panel2.ResumeLayout(false);
            this.splitContainer3.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
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
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Button btnEvalXpath;
        private System.Windows.Forms.TextBox tbXpath;
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
    }
}

