//﻿#define Trace

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

        private XPathDocument xpathDoc;
        private XPathNavigator nav;
        private Dictionary<String, String> _xmlnsPrefixes;
        private int originalGroupBoxMinHeight;
        private int originalPanel1MinSize;
        private System.Threading.ManualResetEvent wantFormat = new System.Threading.ManualResetEvent(false);
        private DateTime _originDateTime = new System.DateTime(0);
        private System.DateTime _lastRtbKeyPress;
        private XPathParser<XElement> xpathParser = new XPathParser<XElement>();
        private List<Tuple<int, int>> matchPositions;
        private int currentMatch;
        private int numVisibleLines;
        private int totalLinesInDoc;

        public XPathVisualizerTool()
        {
            SetupDebugConsole(); // for debugging purposes
            InitializeComponent();
            FixupTitle();
            RememberSizes();
            AdjustSplitterSize();
            SetupAutocompletes();
            KickoffColorizer();
            DisableMatchButtons();

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
            // CANNOT DO AutoComplete for RichTextBoxn
            //this.tbXpath.AutoCompleteMode = AutoCompleteMode.Suggest;
            //this.tbXpath.AutoCompleteSource = AutoCompleteSource.CustomSource;
            //this.tbXpath.AutoCompleteCustomSource = _xpathExpressionMruList;

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
                xpathDoc = null; // invalidate the cached doc 
                matchPositions = null;
                DisableMatchButtons();
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
            xpathDoc = null; // invalidate the cached doc 
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
                PreloadXmlns();
            }
            catch (Exception exc1)
            {
                this.lblStatus.Text = String.Format("Cannot process that XML. ({0})", exc1.Message);
            }
        }



        private static System.Text.RegularExpressions.Regex unnamedXmlnsRegex =
            new System.Text.RegularExpressions.Regex("\\sxmlns\\s*=\\s*['\"](.+?)['\"]");

        private static System.Text.RegularExpressions.Regex namedXmlnsRegex =
            new System.Text.RegularExpressions.Regex("\\sxmlns:([^\\s]+)\\s*=\\s*['\"](.+?)['\"]");


        private void PreloadXmlns()
        {
            xmlNamespaces.Clear();
            int c = 1;
            var regexi = new System.Text.RegularExpressions.Regex[] { namedXmlnsRegex, unnamedXmlnsRegex };

            for (int i = 0; i < regexi.Length; i++)
            {
                // check for xmlnamespaces in the loaded document
                var matches = regexi[i].Matches(this.richTextBox1.Text);

                if (matches != null && matches.Count != 0)
                {
                    foreach (System.Text.RegularExpressions.Match m in matches)
                    {
                        string ns = m.Groups[2 - i].Value.ToString();
                        if (!xmlNamespaces.Values.Contains(ns))
                        {
                            // get the prefix - it's either explicit or contrived
                            string origPrefix = (i == 1)
                                ? String.Format("ns{0}", c++)    // contrived
                                : m.Groups[1].Value.ToString();  // explicit

                            // make sure the prefix is unique
                            int dupes = 0;
                            string actualPrefix = origPrefix;
                            while (xmlNamespaces.Keys.Contains(actualPrefix))
                            {
                                actualPrefix = (i == 1)
                                    ? String.Format("ns{0}", c++)
                                    : String.Format("{0}-{1}", origPrefix, dupes++);
                            }

                            xmlNamespaces.Add(actualPrefix, ns);
                        }
                    }
                }
            }
            DisplayXmlPrefixList();
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
            matchPositions = null;
            DisableMatchButtons();
            string xpathExpression = this.tbXpath.Text;
            if (String.IsNullOrEmpty(xpathExpression))
            {
                this.lblStatus.Text = "Cannot evaluate: There is no XPath expression.";
                return;
            }

            string rtbText = this.richTextBox1.Text;
            if (String.IsNullOrEmpty(rtbText))
            {
                this.lblStatus.Text = "Cannot evaluate: There is no XML document.";
                return;
            }

            IntPtr mask = IntPtr.Zero;
            try
            {
                // reset highlighting 
                this.richTextBox1.SelectAll();
                this.richTextBox1.SelectionBackColor = Color.White;
                this.richTextBox1.Update();

                mask = this.richTextBox1.BeginUpdate();
                this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
                this.tbXpath.BackColor = this.tbXmlns.BackColor; // just in case

                // load the Xml doc
                if (xpathDoc == null)
                {
                    xpathDoc = new XPathDocument(new StringReader(rtbText));
                    nav = xpathDoc.CreateNavigator();
                }
                XmlNamespaceManager xmlns = new XmlNamespaceManager(nav.NameTable);
                foreach (string prefix in xmlNamespaces.Keys)
                    xmlns.AddNamespace(prefix, xmlNamespaces[prefix]);

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
                RememberInMruList(_xpathExpressionMruList, xpathExpression);
            }
            catch (Exception exc1)
            {
                string brokenPrefix = IsUnknownNamespacePrefix(exc1);
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
                    MessageBox.Show("Exception: " + exc1.Message,
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

            EnableMatchButtons();
            currentMatch = 0;
            scrollToCurrentMatch();
        }


        private void RememberInMruList(System.Windows.Forms.AutoCompleteStringCollection list, string value)
        {
            if (list.Contains(value))
            {
                list.Remove(value);
            }
            else if (list.Count >= _MaxMruListSize)
            {
                list.RemoveAt(0);
            }
            list.Add(value);
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
            ComputePositionsOfSelection(selection, xmlns);

            foreach (var t in matchPositions)
            {
                // do the highlight
                this.richTextBox1.Select(t.V1, t.V2 - t.V1 + 1);
                this.richTextBox1.SelectionBackColor =
                    Color.FromArgb(Color.Red.A, 0x98, 0xFb, 0x98);
            }
        }




                
        /// <summary>
        /// Computes the positions of the selected nodes in the XML RichTextBox,
        /// given the XPathNodeIterator. 
        /// </summary>
        /// <remarks>
        /// This finishes pretty quickly, no need to do it asynchronously. 
        /// </remarks>
        /// <param name="selection">the node-set selection</param>
        /// <param name="xmlns">you know</param>
        private void ComputePositionsOfSelection(XPathNodeIterator selection, XmlNamespaceManager xmlns)
        {
            var lc = new LineCalculator(this.richTextBox1);
            matchPositions = new List<Tuple<int, int>>();

            // get Text once (it's expensive)
            string rtbText = this.richTextBox1.Text;
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
                        while (rtbText[ix2] != '\'' && rtbText[ix2] != '"')
                            ix2++;
                        c = rtbText[ix2];
                        ix2++;
                        while (rtbText[ix2] != c)
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
                            string subs1 = rtbText.Substring(ix2, 1);
                            while (subs1 != ">" && ix2 > ix)
                            {
                                ix2--;
                                subs1 = rtbText.Substring(ix2, 1);
                            }
                        }
                        else
                        {
                            // Manual Labor. Since there is no XPathNavigator.MoveToEndOfElement(),
                            // we look for the EndElement in the text.  First, advance past the 
                            // original node name.  If the succeeding char is not / (meaning 
                            // an empty element), then look for the </NodeName> string.  

                            ix2 = ix + node.Name.Length + 1;
                            //string subs1 = rtbText.Substring(ix2, 1);
                            if (rtbText[ix2] == '/')
                            {
                                // we're at the end-element
                                ix2++;
                            }
                            else
                            {
                                string subs1 = String.Format("</{0}>", node.Name);
                                int ix3 = rtbText.IndexOf(subs1, ix2);
                                if (ix3 > 0)
                                {
                                    ix2 = ix3 + subs1.Length;
                                }
                                else
                                {
                                    ix2 = rtbText.IndexOf('>', ix2);
                                }
                            }
                        }
                    }

                    // do we need to remember this one?
                    if (ix2 > ix)
                    {
                        // Record the location of the match within the doc.
                        matchPositions.Add(Tuple.New(ix, ix2));
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
                var settings = new System.Xml.XmlWriterSettings
                    {
                        // OmitXmlDeclaration = true,
                        Indent = true,
                        IndentChars= "  "
                            };
                
                using (var writer = System.Xml.XmlWriter.Create(builder, settings))
                {
                    doc.Save(writer);
                }
                this.richTextBox1.Text = builder.ToString();
                wantFormat.Set();
                xpathDoc = null; // invalidate the cached doc 
                matchPositions = null;
                DisableMatchButtons();
                PreloadXmlns();
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

        private string IsUnknownNamespacePrefix(Exception exc1)
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
                if (xmlNamespaces.Keys.Contains(tbPrefix.Text))
                {
                    // Bzzt!
                    this.tbPrefix.SelectAll();
                    this.tbPrefix.BackColor = Color.FromArgb((Color.Red.A << 24) | 0xFFDEAD);
                    this.tbPrefix.Focus();
                }
                else
                {
                    // add it to the list of prefixes, and display the list
                    xmlNamespaces.Add(tbPrefix.Text, tbXmlns.Text);
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
            if (xmlNamespaces.Keys.Contains(k))
            {
                xmlNamespaces.Remove(k);
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


        private Dictionary<String, String> xmlNamespaces
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
                //this.BeginUpdate();
                this.SuspendLayout();
                this.pnlPrefixList.SuspendLayout();
                
                this.pnlPrefixList.Controls.Clear();

                int count = 0;
                if (xmlNamespaces.Keys.Count > 0)
                {
                    foreach (var k in xmlNamespaces.Keys)
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
                                Text = xmlNamespaces[k],
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

                this.splitContainer3.Panel1MinSize = originalPanel1MinSize + (deltaY * count);
                this.splitContainer3.SplitterDistance = this.splitContainer3.Panel1MinSize;

                // We don't need to explicitly set the size of the groupbox.  Groupbox1
                // is docked at the bottom of SplitContainer3.Panel1, so it grows as we
                // move the splitter.

                this.pnlPrefixList.ResumeLayout();
                this.ResumeLayout();
            }
            catch (Exception exc1)
            {
                MessageBox.Show(String.Format("There was a problem ! [problem={0}]",
                                              exc1.Message), "Whoops!", MessageBoxButtons.OK);
            }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            IndentXml();
        }

        private void stripNamespacesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StripXmlNamespaces();
        }

        /// <summary>
        /// Strips namespaces from the XML in the XML RichTextBox
        /// </summary>
        /// <remarks>
        /// This finishes pretty quickly, no need to do it asynchronously. 
        /// </remarks>
        private void StripXmlNamespaces()
        {
            try
            {
                System.Xml.XmlDocument doc= new System.Xml.XmlDocument();
                doc.LoadXml(this.richTextBox1.Text);

                var builder = new System.Text.StringBuilder();
                var settings = new System.Xml.XmlWriterSettings
                    {
                        //OmitXmlDeclaration = true,
                        Indent = true,
                        IndentChars= "  "
                            };
                
                using (var writer = new NoNamespaceXmlTextWriter(builder, settings))
                {
                    doc.Save(writer);
                }
                this.richTextBox1.Text = builder.ToString();
                wantFormat.Set();
                xpathDoc = null; // invalidate the cached doc 
                matchPositions = null;
                DisableMatchButtons();
                PreloadXmlns();
            }
            catch (System.Exception)
            {
                // illegal xml... do nothing
            }
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


        private void DisableMatchButtons()
        {
            this.matchPanel.Visible = false;
            //matchPositions = null;
            this.lblMatch.Text = "";
            this.btn_NextMatch.Enabled = false;
            this.btn_PrevMatch.Enabled = false;
        }

        private void EnableMatchButtons()
        {
            if (matchPositions != null && matchPositions.Count > 0)
            {
                this.btn_NextMatch.Enabled = true;
                this.btn_PrevMatch.Enabled = true;
                currentMatch = 0;
                numVisibleLines = this.rtbe.NumberOfVisibleLines;
                totalLinesInDoc = this.richTextBox1.Lines.Count();
                this.matchPanel.Visible = true;
            }
        }

        private void scrollToCurrentMatch()
        {
            if (matchPositions == null) return;
            Tuple<int,int> position = matchPositions[currentMatch];

            Trace("scrollToPosition(match({0}) position({1}))",
                  currentMatch, position.V1);
            
            int startLine = this.richTextBox1.GetLineFromCharIndex(position.V1);
            
            Trace("scrollToPosition::startLine({0}) numVisibleLines({1})",
                  startLine, numVisibleLines);

            this.lblMatch.Text = String.Format("{0}/{1}",
                                               currentMatch + 1, matchPositions.Count);

            // If the start line is in the middle of the doc... 
            //if (startLine > totalLinesInDoc)
            if (startLine > numVisibleLines - 2)
            {
                // scroll so that the first line is 1/3 the way from the top
                int cix = this.richTextBox1.GetFirstCharIndexFromLine(startLine - numVisibleLines / 3 + 1);
                this.richTextBox1.Select(cix, cix + 1);
            }
            else
            {
                // set the selection at the very beginning
                this.richTextBox1.Select(0, 1);
            }
            this.richTextBox1.ScrollToCaret();

            // restore selection:
            this.richTextBox1.Select(position.V1, 0);
        }


        private void btn_NextMatch_Click(object sender, EventArgs e)
        {
            if (matchPositions == null) return;
            currentMatch++;
            if (currentMatch == matchPositions.Count)
                currentMatch = 0;
            scrollToCurrentMatch();
        }

        private void btn_PrevMatch_Click(object sender, EventArgs e)
        {
            if (matchPositions == null) return;
            currentMatch--;
            if (currentMatch < 0)
                currentMatch = matchPositions.Count - 1;
            Trace("currentMatch = {0}", currentMatch);
            scrollToCurrentMatch();
        }


        private void tbXpath_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.btnEvalXpath_Click(sender, null);
                e.Handled = true;
            }
            else if (e.Control && e.KeyCode == Keys.N)
            {
                btn_NextMatch_Click(sender, null);
                e.Handled = true;
            }
            else if (e.Control && e.KeyCode == Keys.P)
            {
                btn_PrevMatch_Click(sender, null);
                e.Handled = true;
            }
        }

        private void form_KeyDown(object sender, KeyEventArgs e)
        {
            // Because Form.KeyPreview is true, this method gets invoked before
            // the KeyDown event is passed to the control with focus.  This way we 
            // can handle keydown events on a form-wide basis. 
            if (e.Control && e.KeyCode == Keys.N)
            {
                btn_NextMatch_Click(sender, null);
                e.Handled = true;
            }
            else if (e.Control && e.KeyCode == Keys.P)
            {
                btn_PrevMatch_Click(sender, null);
                e.Handled = true;
            }
        }


        /// <summary>
        /// Deletes the selected nodes in the XML RichTextBox, given the XPathNodeIterator. 
        /// </summary>
        /// <remarks>
        /// This finishes pretty quickly, no need to do it asynchronously. 
        /// </remarks>
        private void DeleteSelection()
        {
            if (matchPositions == null) return;
            //ComputePositionsOfSelection(selection, xmlns);
            int totalRemoved = 0; 
            Trace("DeleteSelection(count({0}))", matchPositions.Count);

            foreach (var t in matchPositions)
            {
                // do the deletion
                Trace("DeleteSelection(match({0},{1}))", t.V1, t.V2);
            
                this.richTextBox1.Select(t.V1 - totalRemoved, t.V2 - t.V1 + 1);
                this.richTextBox1.SelectedText = "";
                totalRemoved += (t.V2 - t.V1 + 1);
                
                Trace("DeleteSelection(total({0})", totalRemoved);
            }

            currentMatch = 0;
            matchPositions = null;
            DisableMatchButtons();
        }


        
        
        private void removeSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (matchPositions == null) return;
            DisableMatchButtons();
            
            IntPtr mask = IntPtr.Zero;
            try 
            {
                mask = this.richTextBox1.BeginUpdate();
                this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

                DeleteSelection();

                // re-format (re-indent) the result
                IndentXml();
            }

            finally
            {
                this.Cursor = System.Windows.Forms.Cursors.Default;
                this.richTextBox1.EndUpdate(mask);
            }
            
        }
 

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
         {
            try 
            {
                var dlg1 = new SaveFileDialog
                    {
                        FileName = System.IO.Path.GetFileName(this.tbXmlDoc.Text),
                        InitialDirectory = System.IO.Path.GetDirectoryName(this.tbXmlDoc.Text),
                        OverwritePrompt = true,
                        Title = "Where would you like to save the XML?",
                        Filter = "XML files|*.xml|All files (*.*)|*.*"
                    };

                var result = dlg1.ShowDialog();
                if (result == DialogResult.OK)
                {
                    this.tbXmlDoc.Text = dlg1.FileName;

                    File.WriteAllText(this.tbXmlDoc.Text,
                                      this.richTextBox1.Text);
                }
              
            }
            catch (System.Exception exc1)
            {
                MessageBox.Show("Exception: " + exc1.Message,
                                "Exception while saving",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Exclamation);

            }            
        }
       
    }

}
