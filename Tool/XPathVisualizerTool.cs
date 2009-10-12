// XPathVisualizerTool.cs
// ------------------------------------------------------------------
//
// Copyright (c) 2009 Dino Chiesa.
// All rights reserved.
//
// This file is part of the source code disribution for Ionic's
// XPath Visualizer Tool.
//
// ------------------------------------------------------------------
//
// This code is licensed under the Microsoft Public License. 
// See the file License.rtf or License.txt for the license details.
// More info on: http://XPathVisualizer.codeplex.com
//
// ------------------------------------------------------------------
//


using System;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;                    // for the Contains extension
using System.Xml.Linq;                // XElement
using System.Windows.Forms;
using CodePlex.XPathParser;

namespace XPathVisualizer
{
    public partial class XPathVisualizerTool : Form
    {
        public XPathVisualizerTool()
        {
            InitializeComponent();
            FixupTitle();
            RememberSizes();
            AdjustSplitterSize();
            SetupAutocompletes();
            KickoffColorizer();

            this.progressBar1.Visible = false;
            this.lblStatus.Text = "Ready";
        }

        private void RememberSizes()
        {
            originalGroupBoxMinHeight = this.groupBox1.MinimumSize.Height;
            originalPanel1MinSize = this.splitContainer3.Panel1MinSize;
        }


        private void SetupAutocompletes()
        {
            // setup the autocomplete for the xpath expressions
            FillFormFromRegistry();
            //this.tbXpath.AutoCompleteMode = AutoCompleteMode.Suggest;
            //this.tbXpath.AutoCompleteSource = AutoCompleteSource.CustomSource;
            //this.tbXpath.AutoCompleteCustomSource = _xpathExpressionMruList;

            // insert the most recent expression into the box?  
            //this.tbXpath.Text = _xpathExpressionMruList[0];

            // setup the autocomplete for the file
            this.tbXmlDoc.AutoCompleteMode = AutoCompleteMode.Suggest;
            this.tbXmlDoc.AutoCompleteSource = AutoCompleteSource.FileSystem;
        }

        private void FixupTitle()
        {
            var a = System.Reflection.Assembly.GetExecutingAssembly();
            object[] attr = a.GetCustomAttributes(typeof(System.Reflection.AssemblyDescriptionAttribute), true);
            var desc = attr[0] as System.Reflection.AssemblyDescriptionAttribute;

            this.Text = desc.Description + " v" + a.GetName().Version.ToString();
        }

        private static System.Text.RegularExpressions.Regex re2 =
            new System.Text.RegularExpressions.Regex("\\sxmlns\\s*=\\s*['\"](.+?)['\"]");


        private void btnLoadXml_Click(object sender, EventArgs e)
        {
            try
            {
                this.richTextBox1.Text = "";
                this.richTextBox1.Update();
                _lastRtbKeyPress = _originDateTime;
                this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
                this.richTextBox1.Text = File.ReadAllText(this.tbXmlDoc.Text);
                wantFormat.Set();

                PreloadXmlns();
            }
            catch (Exception exc1)
            {
                this.richTextBox1.Text = "file read error:  " + exc1.ToString();
                this.lblStatus.Text = "Cannot read that file.";
            }
            finally
            {
                this.Cursor = System.Windows.Forms.Cursors.Default;
            }
        }



        //int priorTextLength = -1; 
        private void richTextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            xpathDoc = null;
            nav = null;
            _lastRtbKeyPress = System.DateTime.Now;
            if (this.richTextBox1.Text.Length == 0) return;
            // assume no length change means format change only
            //if (priorTextLength == this.richTextBox1.Text.Length) return;
            wantFormat.Set();
            //priorTextLength = this.richTextBox1.Text.Length;
        }




        private void richTextBox1_Leave(object sender, EventArgs e)
        {
            IntPtr mask = IntPtr.Zero;
            try
            {
                //stopWatch.Reset();
                //stopWatch.Start();

                //this.richTextBox1.Update();
                //mask = this.richTextBox1.BeginUpdate();
                //this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

                //ColorizeXml(this.richTextBox1);

                PreloadXmlns();
                //this.lblStatus.Text = "Highlighting...";

            }
            catch (Exception exc1)
            {
                this.lblStatus.Text = String.Format("Cannot process that XML. ({0})", exc1.Message);
            }
            finally
            {
                //this.Cursor = System.Windows.Forms.Cursors.Default;
                //this.richTextBox1.EndUpdate(mask);
            }
        }



        private void PreloadXmlns()
        {
            xmlnsPrefixes.Clear();
            // check for xmlnamespaces in the loaded document
            var matches = re2.Matches(this.richTextBox1.Text);

            if (matches != null && matches.Count != 0)
            {
                xmlnsPrefixes.Clear();
                int x = 1;
                foreach (System.Text.RegularExpressions.Match m in matches)
                {
                    xmlnsPrefixes.Add(String.Format("ns{0}", x),
                                      m.Groups[1].Value.ToString());
                    x++;
                }
                DisplayXmlPrefixList();
            }
        }

        private void AdjustSplitterSize()
        {
            this.splitContainer3.SplitterDistance = this.splitContainer3.Panel1MinSize;
        }


        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.SaveFormToRegistry();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.FillFormFromRegistry();
        }


        private void btnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.InitialDirectory = System.IO.File.Exists(this.tbXmlDoc.Text)
                ? System.IO.Path.GetDirectoryName(this.tbXmlDoc.Text)
                : this.tbXmlDoc.Text;
            dlg.Filter = "xml files|*.xml|All Files|*.*";
            dlg.FilterIndex = 1;
            dlg.RestoreDirectory = true;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                this.tbXmlDoc.Text = dlg.FileName;
                if (System.IO.File.Exists(this.tbXmlDoc.Text))
                    btnLoadXml_Click(sender, e);
            }
        }



        private void tbXpath_TextChanged(object sender, EventArgs e)
        {
            int ss = this.tbXpath.SelectionStart;
            int sl = this.tbXpath.SelectionLength;
            // get the text.
            string xpathExpr = this.tbXpath.Text;
            // put it back.
            // why? because it's possible to paste in RTF, which won't
            // show up correctly in that one-line RichTextBox. 
            this.tbXpath.Text = xpathExpr;
            this.tbXpath.SelectAll();
            this.tbXpath.SelectionColor = Color.Black;
            this.tbXpath.SelectionFont = this.Font;
            this.tbXpath.Select(ss, sl);
            try
            {
                XElement xe = xpathParser.Parse(xpathExpr, new XPathTreeBuilder());
                this.toolTip1.SetToolTip(this.tbXpath, "XPath expression");
            }
            catch (XPathParserException exc1)
            {
                this.tbXpath.Select(exc1.ErrorStart, exc1.ErrorEnd - exc1.ErrorStart);
                this.tbXpath.SelectionColor = Color.Red;
                this.tbXpath.Select(ss, sl);
                this.toolTip1.SetToolTip(this.tbXpath, exc1.Message);
            }
        }

        private void btnEvalXpath_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(this.tbXpath.Text))
            {
                this.lblStatus.Text = "Cannot evaluate: There is no XPath expression.";
                return;
            }

            if (String.IsNullOrEmpty(this.richTextBox1.Text))
            {
                this.lblStatus.Text = "Cannot evaluate: There is no XML document.";
                return;
            }

            IntPtr mask = IntPtr.Zero;
            try
            {
                // reset highlighting 
                this.richTextBox1.SelectAll();
                //this.richTextBox1.SelectionColor = Color.Black;
                this.richTextBox1.SelectionBackColor = Color.White;
                this.richTextBox1.Update();

                mask = this.richTextBox1.BeginUpdate();
                this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
                this.tbXpath.BackColor = this.tbXmlns.BackColor; // just in case
                string xpathExpression = this.tbXpath.Text;
                //load the Xml doc
                if (xpathDoc == null) xpathDoc = new XPathDocument(new StringReader(this.richTextBox1.Text));
                if (nav == null) nav = xpathDoc.CreateNavigator();
                XmlNamespaceManager xmlns = new XmlNamespaceManager(nav.NameTable);
                foreach (string k in xmlnsPrefixes.Keys)
                {
                    xmlns.AddNamespace(k, xmlnsPrefixes[k]);
                }
                XPathNodeIterator selection = nav.Select(xpathExpression, xmlns);

                if (selection == null || selection.Count == 0)
                {
                    this.lblStatus.Text = String.Format("{0}: Zero nodes selected", xpathExpression);
                }
                else
                {
                    this.lblStatus.Text = String.Format("{0}: {1} {2} selected",
                                                                    xpathExpression,
                                                                    selection.Count, (selection.Count == 1) ? "node" : "nodes");
                    HighlightSelection(selection, xmlns);
                }

                // remember the successful xpath queries
                if (!_xpathExpressionMruList.Contains(xpathExpression))
                {
                    if (_xpathExpressionMruList.Count >= _MaxMruListSize)
                        _xpathExpressionMruList.RemoveAt(0);
                    _xpathExpressionMruList.Add(xpathExpression);
                }
            }
            catch (Exception exc1)
            {
                string brokenPrefix = IsUnkownNamespacePrefix(exc1);
                if (brokenPrefix != null)
                {
                    int ix = this.tbXpath.Text.IndexOf(brokenPrefix);
                    this.tbXpath.Select(ix, brokenPrefix.Length);
                    this.tbXpath.BackColor = Color.FromArgb((Color.Red.A << 24) | 0xFFDEAD);
                    this.tbXpath.Focus();
                    this.lblStatus.Text = "Exception: " + exc1.Message;
                }
                else if (BadExpression(exc1))
                {
                    this.tbXpath.SelectAll();
                    this.tbXpath.BackColor = Color.FromArgb((Color.Red.A << 24) | 0xFFDEAD);
                    this.tbXpath.Focus();
                    this.lblStatus.Text = "Exception: " + exc1.Message;
                }
                else
                {
                    MessageBox.Show("Exception: " + exc1.ToString(),
                                    "Exception while evaluating XPath",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Exclamation);
                }
            }

            finally
            {
                this.Cursor = System.Windows.Forms.Cursors.Default;
                this.richTextBox1.EndUpdate(mask);
            }
        }




        /// <summary>
        /// Highlights the selected nodes in the XML RichTextBox, given the XPathNodeIterator. 
        /// </summary>
        /// <remarks>
        /// This finishes pretty quickly, no need to do it asynchronously. 
        /// </remarks>
        /// <param name="selection">the node-set selection</param>
        /// <param name="xmlns">you know</param>
        private void HighlightSelection(XPathNodeIterator selection, XmlNamespaceManager xmlns)
        {
            bool scrolled = false;
            var lc = new LineCalculator(this.richTextBox1);
            string txt = this.richTextBox1.Text;
            foreach (XPathNavigator node in selection)
            {
                IXmlLineInfo lineInfo = node as IXmlLineInfo;
                if (lineInfo == null || !lineInfo.HasLineInfo()) continue;

                int ix = lc.GetCharIndexFromLine(lineInfo.LineNumber - 1) +
                    lineInfo.LinePosition - 1 - 1;

                if (ix >= 0)
                {
                    int ix2 = 0;

                    if (node.NodeType == XPathNodeType.Comment)
                    {
                        ix2 = ix + node.Value.Length;
                        ix++;
                    }

                    else if (node.NodeType == XPathNodeType.Text)
                    {
                        string s = node.Value.XmlEscapeQuotes();
                        ix2 = ix + s.Length;
                        ix++;
                    }
                    else if (node.NodeType == XPathNodeType.Attribute)
                    {
                        string s = node.Value.XmlEscapeQuotes();
                        ix++;
                        ix2 = ix + node.Name.Length + 1;
                        char c = ' ';
                        while (txt[ix2] != '\'' && txt[ix2] != '"')
                            ix2++;
                        c = txt[ix2];
                        ix2++;
                        while (txt[ix2] != c)
                            ix2++;
                    }
                    else if (node.NodeType == XPathNodeType.Element)
                    {
                        if (node.MoveToNext())
                        {
                            // The navigator moved to the succeeding element. Now backup 
                            // through the text to find the ending square bracket for *this* element.
                            ix2 = lc.GetCharIndexFromLine(lineInfo.LineNumber - 1) +
                                lineInfo.LinePosition - 1;
                            string subs1 = txt.Substring(ix2, 1);
                            while (subs1 != ">" && ix2 > ix)
                            {
                                ix2--;
                                subs1 = txt.Substring(ix2, 1);
                            }
                        }
                        else
                        {
                            // Manual Labor. Since there is no XPathNavigator.MoveToEndOfElement(),
                            // we look for the EndElement in the text.  First, advance past the 
                            // original node name.  If the succeeding char is not / (meaning 
                            // an empty element), then look for the </NodeName> string.  

                            ix2 = ix + node.Name.Length + 1;
                            //string subs1 = txt.Substring(ix2, 1);
                            if (txt[ix2] == '/')
                            {
                                // we're at the end-element
                                ix2++;
                            }
                            else
                            {
                                string subs1 = String.Format("</{0}>", node.Name);
                                int ix3 = txt.IndexOf(subs1, ix2);
                                if (ix3 > 0)
                                {
                                    ix2 = ix3 + subs1.Length;
                                }
                                else
                                {
                                    ix2 = txt.IndexOf('>', ix2);
                                }
                            }
                        }
                    }

                    if (ix2 > ix)
                    {
                        this.richTextBox1.Select(ix, ix2 - ix + 1);
                        this.richTextBox1.SelectionBackColor =
                            Color.FromArgb(Color.Red.A, 0x98, 0xFb, 0x98);
                        if (!scrolled)
                        {
                            this.richTextBox1.ScrollToCaret();
                            scrolled = true;
                        }
                    }
                }
            }
        }




        /// <summary>
        /// Re-formats (Indents) the text in the XML RichTextBox
        /// </summary>
        /// <remarks>
        /// This finishes pretty quickly, no need to do it asynchronously. 
        /// </remarks>
        private void IndentXml()
        {
            try
            {
                System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                doc.LoadXml(this.richTextBox1.Text);
                var builder = new System.Text.StringBuilder();
                var settings = new System.Xml.XmlWriterSettings { OmitXmlDeclaration = true, Indent = true };
                using (var writer = System.Xml.XmlWriter.Create(builder, settings))
                {
                    doc.Save(writer);
                }
                this.richTextBox1.Text = builder.ToString();
                wantFormat.Set();
            }
            catch (System.Exception)
            {
                // maybe invalid XML, so... just do nothing
            }
        }



        private void linkToCodeplex_Click(object sender, EventArgs e)
        {
            if (sender as ToolStripStatusLabel != null)
                System.Diagnostics.Process.Start((sender as ToolStripStatusLabel).Text);
        }



        private static System.Text.RegularExpressions.Regex re1 =
            new System.Text.RegularExpressions.Regex("Namespace prefix '(.+)' is not defined");
        private string IsUnkownNamespacePrefix(Exception exc1)
        {
            var match = re1.Match(exc1.ToString());
            if (match != null && match.Captures != null && match.Captures.Count != 0)
                return match.Groups[1].Value.ToString();
            return null;
        }


        private bool BadExpression(Exception exc1)
        {
            return exc1.Message.Contains("Expression must evaluate to a node-set");
        }



        private void btnAddNsPrefix_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(this.tbPrefix.Text) && !String.IsNullOrEmpty(this.tbXmlns.Text))
            {
                if (xmlnsPrefixes.Keys.Contains(tbPrefix.Text))
                {
                    // Bzzt!
                    this.tbPrefix.SelectAll();
                    this.tbPrefix.BackColor = Color.FromArgb((Color.Red.A << 24) | 0xFFDEAD);
                    this.tbPrefix.Focus();
                }
                else
                {
                    // add it to the list of prefixes, and display the list
                    xmlnsPrefixes.Add(tbPrefix.Text, tbXmlns.Text);
                    DisplayXmlPrefixList();
                    this.tbPrefix.Text = "";
                    this.tbXmlns.Text = "";
                    this.tbPrefix.Focus();
                    this.tbXpath.BackColor = this.tbXmlns.BackColor;
                }
            }
        }

        private void RemovePrefix(string k)
        {
            if (xmlnsPrefixes.Keys.Contains(k))
            {
                xmlnsPrefixes.Remove(k);
                DisplayXmlPrefixList();
            }
        }

        private void XPathVisualizerTool_Resize(object sender, EventArgs e)
        {
            AdjustSplitterSize();
        }


        private void tbPrefix_TextChanged(object sender, EventArgs e)
        {
            if (this.tbPrefix.BackColor != this.tbXmlns.BackColor)
            {
                this.tbPrefix.BackColor = this.tbXmlns.BackColor;
            }
        }


        private Dictionary<String, String> xmlnsPrefixes
        {
            get
            {
                if (_xmlnsPrefixes == null)
                    _xmlnsPrefixes = new Dictionary<String, String>();
                return _xmlnsPrefixes;
            }
        }


        private void DisplayXmlPrefixList()
        {
            int offsetX = 2;   // left
            int offsetY = 16;  // positive is up
            int deltaY = 20;
            try
            {
                this.pnlPrefixList.Controls.Clear();

                int count = 0;
                if (xmlnsPrefixes.Keys.Count > 0)
                {

                    foreach (var k in xmlnsPrefixes.Keys)
                    {
                        // add a set of controls to the panel for each key/value pair in the list
                        var tb1 = new System.Windows.Forms.TextBox
                            {
                                Anchor = (System.Windows.Forms.AnchorStyles)
                                    (System.Windows.Forms.AnchorStyles.Top |
                                     System.Windows.Forms.AnchorStyles.Left),
                                Location = new System.Drawing.Point(this.tbPrefix.Location.X - offsetX,
                                                                    this.tbPrefix.Location.Y - offsetY + (count * deltaY)),
                                Size = new System.Drawing.Size(this.tbPrefix.Size.Width, this.tbPrefix.Size.Height),
                                Text = k,
                                ReadOnly = true,
                                TabStop = false,
                            };
                        this.pnlPrefixList.Controls.Add(tb1);
                        var lbl1 = new System.Windows.Forms.Label
                            {
                                AutoSize = true,
                                Location = new System.Drawing.Point(this.tbXmlns.Location.X - offsetX - 18,
                                                                    this.tbXmlns.Location.Y - offsetY + (count * deltaY)),
                                Size = new System.Drawing.Size(24, 13),
                                Text = ":=",
                            };
                        this.pnlPrefixList.Controls.Add(lbl1);

                        var tb2 = new System.Windows.Forms.TextBox
                            {
                                Anchor = (System.Windows.Forms.AnchorStyles)
                                    (System.Windows.Forms.AnchorStyles.Top |
                                     System.Windows.Forms.AnchorStyles.Left |
                                     System.Windows.Forms.AnchorStyles.Right),
                                Location = new System.Drawing.Point(this.tbXmlns.Location.X - offsetX,
                                                                    this.tbXmlns.Location.Y - offsetY + (count * deltaY)),
                                Size = new System.Drawing.Size(this.tbXmlns.Size.Width, this.tbXmlns.Size.Height),
                                Text = xmlnsPrefixes[k],
                                ReadOnly = true,
                                TabStop = false,
                            };
                        this.pnlPrefixList.Controls.Add(tb2);
                        var btn1 = new System.Windows.Forms.Button
                            {
                                Anchor = (System.Windows.Forms.AnchorStyles)
                                    (System.Windows.Forms.AnchorStyles.Top |
                                     System.Windows.Forms.AnchorStyles.Right),
                                Location = new System.Drawing.Point(this.btnAddNsPrefix.Location.X - offsetX,
                                                                    this.btnAddNsPrefix.Location.Y - offsetY + (count * deltaY)),
                                Size = new System.Drawing.Size(this.btnAddNsPrefix.Size.Width,
                                                               this.btnAddNsPrefix.Size.Height),
                                Text = "X",
                                UseVisualStyleBackColor = true,
                                TabStop = false,
                            };
                        btn1.Click += (src, e) => { RemovePrefix(k); };
                        this.pnlPrefixList.Controls.Add(btn1);
                        count++;
                    }
                }
                else
                    count = 1;

                // we don't need the groupbox to get any larger.
                //                 var size = new System.Drawing.Size(0, originalGroupBoxMinHeight + (deltaY * (count-1)) );
                //                 this.groupBox1.MinimumSize = size;
                //                 this.groupBox1.MaximumSize = size;
                this.splitContainer3.Panel1MinSize = originalPanel1MinSize + (deltaY * (count - 1));
                this.splitContainer3.SplitterDistance = this.splitContainer3.Panel1MinSize;
            }
            catch (Exception exc1)
            {
                MessageBox.Show(String.Format("There was a problem ! [problem={0}]",
                                              exc1.Message), "Whoops!", MessageBoxButtons.OK);
            }
        }

        private XPathDocument xpathDoc;
        private XPathNavigator nav;
        private Dictionary<String, String> _xmlnsPrefixes;
        private int originalGroupBoxMinHeight;
        private int originalPanel1MinSize;
        private System.Threading.ManualResetEvent wantFormat = new System.Threading.ManualResetEvent(false);
        private DateTime _originDateTime = new System.DateTime(0);
        private System.DateTime _lastRtbKeyPress;
        private XPathParser<XElement> xpathParser = new XPathParser<XElement>();

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            IndentXml();
        }

        private void copyAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string txt = this.richTextBox1.Text;
            Clipboard.SetDataObject(txt, true);
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string txt = this.richTextBox1.SelectedText;
            Clipboard.SetDataObject(txt, true);
        }

        
        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string o = Clipboard.GetData(DataFormats.Text) as String;
            if (o != null)
            {
                this.richTextBox1.SelectedText = o;
                wantFormat.Set();
            }
        }
    }



}
