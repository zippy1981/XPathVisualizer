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
// See the file License.rtffor the license details.
// More info on: http://dotnetzip.codeplex.com
//
// ------------------------------------------------------------------
//
//

using System;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace XPathTester
{
    public partial class XPathVisualizerTool : Form
    {
        public XPathVisualizerTool()
        {
            InitializeComponent();
            FixupTitle();
            RememberSizes();
            AdjustSplitterSize();
        }

        private void RememberSizes()
        {
            originalGroupBoxMinHeight = this.groupBox1.MinimumSize.Height;
            originalPanel1MinSize = this.splitContainer3.Panel1MinSize;
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
                IntPtr mask = this.richTextBox1.BeginUpdate();
                this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
                this.richTextBox1.Text = File.ReadAllText(this.tbXmlDoc.Text);
                this.richTextBox1.ColorizeXml();
                this.Cursor = System.Windows.Forms.Cursors.Default;
                this.richTextBox1.EndUpdate(mask);

                PreloadXmlns();
            }
            catch (Exception exc1)
            {
                this.richTextBox1.Text = "file read error:  " + exc1.ToString();
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



        private void btnEvalXpath_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(this.tbXpath.Text))
            {
                this.toolStripStatusLabel1.Text = "Cannot evaluate: There is no XPath expression.";
            }
            else if (String.IsNullOrEmpty(this.richTextBox1.Text))
            {
                this.toolStripStatusLabel1.Text = "Cannot evaluate: There is no XML document.";
            }
            else
            {
                try
                {
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

                    // reset highlighting 
                    this.richTextBox1.SelectAll();
                    //this.richTextBox1.SelectionColor = Color.Black;
                    this.richTextBox1.SelectionBackColor = Color.White;
                    if (selection == null || selection.Count == 0)
                    {
                        this.toolStripStatusLabel1.Text = String.Format("{0}: Zero nodes selected", xpathExpression);
                    }
                    else
                    {
                        this.toolStripStatusLabel1.Text = String.Format("{0}: {1} {2} selected",
                                                                        xpathExpression,
                                                                        selection.Count, (selection.Count == 1) ? "node" : "nodes");
                        HighlightSelection(selection, xmlns);
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
                        this.toolStripStatusLabel1.Text = "Exception: " + exc1.Message;
                    }
                    else if (BadExpression(exc1))
                    {
                        this.tbXpath.SelectAll();
                        this.tbXpath.BackColor = Color.FromArgb((Color.Red.A << 24) | 0xFFDEAD);
                        this.tbXpath.Focus();
                        this.toolStripStatusLabel1.Text = "Exception: " + exc1.Message;
                    }
                    else
                    {
                        MessageBox.Show("Exception: " + exc1.ToString(),
                            "Exception while evaluating XPath",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Exclamation);
                    }
                }
            }
        }


        private void HighlightSelection(XPathNodeIterator selection, XmlNamespaceManager xmlns)
        {
            bool scrolled = false;
            foreach (XPathNavigator node in selection)
            {
                IXmlLineInfo lineInfo = node as IXmlLineInfo;
                if (lineInfo == null || !lineInfo.HasLineInfo()) continue;

                int ix = this.richTextBox1.MyGetCharIndexFromLine(lineInfo.LineNumber - 1) + lineInfo.LinePosition - 1 - 1;

                if (ix >= 0)
                {
                    int ix2 = 0;

                    //string sub = this.richTextBox1.Text.Substring(ix);
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
                        ix2 = ix + node.Name.Length + s.Length + 1 + 2;
                    }
                    else if (node.NodeType == XPathNodeType.Element)
                    {
                        if (node.MoveToNext())
                        {
                            // The navigator moved to the succeeding element. Now backup 
                            // through the text to find the ending square bracket for *this* element.
                            ix2 = this.richTextBox1.MyGetCharIndexFromLine(lineInfo.LineNumber - 1) +
                                lineInfo.LinePosition - 1;
                            string subs1 = this.richTextBox1.Text.Substring(ix2, 1);
                            while (subs1 != ">" && ix2 > ix)
                            {
                                ix2--;
                                subs1 = this.richTextBox1.Text.Substring(ix2, 1);
                            }
                        }
                        else
                        {
                            // Manual Labor. Since there is no XPathNavigator.MoveToEndElement(), we 
                            // look for the EndElement in the text.  First, advance past the 
                            // original node name.  If the succeeding char is not / (meaning 
                            // an empty element), then look for the </NodeName> string.  

                            ix2 = ix + node.Name.Length + 1;
                            string subs1 = this.richTextBox1.Text.Substring(ix2, 1);
                            if (subs1 == "/")
                            {
                                // we're at the end-element
                                ix2++;
                            }
                            else
                            {
                                subs1 = String.Format("</{0}>", node.Name);
                                int ix3 = this.richTextBox1.Text.IndexOf(subs1);
                                if (ix3 > 0)
                                {
                                    ix2 = ix3 + subs1.Length;
                                }
                                else
                                {
                                    subs1 = this.richTextBox1.Text.Substring(ix2, 1);
                                    while (subs1 != ">")
                                    {
                                        ix2++;
                                        subs1 = this.richTextBox1.Text.Substring(ix2, 1);
                                    }
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

        private void linkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (sender as LinkLabel != null)
                System.Diagnostics.Process.Start((sender as LinkLabel).Text);
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


        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            xpathDoc = null;
            nav = null;
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
    }





    public static class Extensions
    {
        public static string XmlEscapeQuotes(this String s)
        {
            while (s.Contains("\""))
            {
                s = s.Replace("\"", "&quot;");
            }
            return s;
        }


        public static void ColorizeXml(this System.Windows.Forms.RichTextBox rtb)
        {
            var sr = new StringReader(rtb.Text);
            XmlReader reader = XmlReader.Create(sr);

            if ((reader as IXmlLineInfo) != null)
            {
                IXmlLineInfo rinfo = (IXmlLineInfo)reader;
                if (rinfo.HasLineInfo())
                {
                    int ix = 0;
                    while (reader.Read())
                    {
                        switch (reader.NodeType)
                        {
                            case XmlNodeType.Element: // The node is an element.
                                ix = rtb.MyGetCharIndexFromLine(rinfo.LineNumber - 1) +
                                    +rinfo.LinePosition - 1;
                                rtb.Select(ix - 1, 1);
                                rtb.SelectionColor = Color.Blue;
                                rtb.Select(ix, reader.Name.Length);
                                rtb.SelectionColor = Color.DarkRed;

                                if (reader.HasAttributes)
                                {
                                    reader.MoveToFirstAttribute();
                                    do
                                    {
                                        //string s = reader.Value;
                                        ix = rtb.MyGetCharIndexFromLine(rinfo.LineNumber - 1) +
                                            +rinfo.LinePosition - 1;
                                        rtb.Select(ix, reader.Name.Length);
                                        rtb.SelectionColor = Color.Red;

                                        ix += reader.Name.Length;
                                        while (rtb.Text.Substring(ix, 1) != "=")
                                            ix++;

                                        // make the equals sign blue
                                        rtb.Select(ix + reader.Name.Length, 1);
                                        rtb.SelectionColor = Color.Blue;

                                        // skip over the quote char (it remains black)
                                        while (rtb.Text.Substring(ix, 1)[0] != reader.QuoteChar)
                                            ix++;
                                        ix++;

                                        // highlight the value of the attribute as blue
                                        if (rtb.Text.Substring(ix).StartsWith(reader.Value))
                                        {
                                            rtb.Select(ix, reader.Value.Length);
                                        }
                                        else
                                        {
                                            // Difference in escaping.  The InnerXml may include
                                            // \" where &quot; is in the doc.
                                            string s = reader.Value.XmlEscapeQuotes();
                                            int delta = s.Length - reader.Value.Length;
                                            rtb.Select(ix, reader.Value.Length + delta);
                                        }
                                        rtb.SelectionColor = Color.Blue;

                                    }
                                    while (reader.MoveToNextAttribute());

                                    while (rtb.Text.Substring(ix, 1)[0] != '>')
                                        ix++;

                                    // the close-angle-bracket
                                    rtb.Select(ix, 1);
                                    rtb.SelectionColor = Color.Blue;

                                }
                                break;

                            case XmlNodeType.Text: // Display the text in each element.
                                //ix = rtb.MyGetCharIndexFromLine(rinfo.LineNumber-1) +
                                //    + rinfo.LinePosition-1;
                                //rtb.Select(ix, reader.Value.Length);
                                //rtb.SelectionColor = Color.Black;
                                break;

                            case XmlNodeType.EndElement: // Display the end of the element.
                                ix = rtb.MyGetCharIndexFromLine(rinfo.LineNumber - 1) +
                                    +rinfo.LinePosition - 1;
                                rtb.Select(ix - 2, 2);
                                rtb.SelectionColor = Color.Blue;
                                rtb.Select(ix, reader.Name.Length);
                                rtb.SelectionColor = Color.DarkRed;
                                rtb.Select(ix + reader.Name.Length, 1);
                                rtb.SelectionColor = Color.Blue;
                                break;

                            case XmlNodeType.Attribute:
                                ix = rtb.MyGetCharIndexFromLine(rinfo.LineNumber - 1) +
                                    +rinfo.LinePosition - 1;
                                rtb.Select(ix, reader.Name.Length);
                                rtb.SelectionColor = Color.Green;
                                break;

                            case XmlNodeType.Comment:
                                ix = rtb.MyGetCharIndexFromLine(rinfo.LineNumber - 1) +
                                    +rinfo.LinePosition - 1;
                                string comment = reader.Value;
                                rtb.Select(ix, comment.Length);
                                rtb.SelectionColor = Color.Green;
                                break;
                        }
                    }
                }
            }
        }

        private const int WM_SETREDRAW = 0x000B;
        private const int WM_USER = 0x400;
        private const int EM_GETEVENTMASK = (WM_USER + 59);
        private const int EM_SETEVENTMASK = (WM_USER + 69);

        [DllImport("user32", CharSet = CharSet.Auto)]
        private extern static IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, IntPtr lParam);

        public static IntPtr BeginUpdate(this System.Windows.Forms.RichTextBox rtb)
        {
            // Stop redrawing:
            SendMessage(rtb.Handle, WM_SETREDRAW, 0, IntPtr.Zero);
            // Stop sending of events:
            IntPtr eventMask = SendMessage(rtb.Handle, EM_GETEVENTMASK, 0, IntPtr.Zero);

            return eventMask;
        }

        public static void EndUpdate(this System.Windows.Forms.RichTextBox rtb, IntPtr eventMask)
        {
            // turn on events
            SendMessage(rtb.Handle, EM_SETEVENTMASK, 0, eventMask);
            // turn on redrawing
            SendMessage(rtb.Handle, WM_SETREDRAW, 1, IntPtr.Zero);
            rtb.Invalidate();
        }


        public static int MyGetCharIndexFromLine(this System.Windows.Forms.RichTextBox rtb, int line)
        {
            // The built-in GetFirstCharIndexFromLine does not work for me. 
            // Not sure why.

            line++;
            if (line <= 1) return 0;
            int ix = 0;
            int xline = 0;
            do
            {
                int delta = rtb.Text.Substring(ix).IndexOf('\n');
                if (delta < 0) return -1;
                ix += delta + 1;
                xline++;
            }
            while (xline + 1 < line);

            return ix;
        }

    }

}
