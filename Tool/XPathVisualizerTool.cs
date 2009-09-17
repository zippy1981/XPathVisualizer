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



        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.SaveFormToRegistry();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.FillFormFromRegistry();
        }



        private void SaveFormToRegistry()
        {
            if (AppCuKey != null)
            {
                if (!String.IsNullOrEmpty(this.tbXmlDoc.Text))
                    AppCuKey.SetValue(_rvn_XmlDoc, this.tbXmlDoc.Text);
                if (!String.IsNullOrEmpty(this.tbXpath.Text))
                    AppCuKey.SetValue(_rvn_XPathExpression, this.tbXpath.Text);

                if (!String.IsNullOrEmpty(this.tbPrefix.Text))
                    AppCuKey.SetValue(_rvn_Prefix, this.tbPrefix.Text);

                if (!String.IsNullOrEmpty(this.tbXmlns.Text))
                    AppCuKey.SetValue(_rvn_Xmlns, this.tbXmlns.Text);

                // store the size of the form
                int w = 0, h = 0, left = 0, top = 0;
                if (this.Bounds.Width < this.MinimumSize.Width || this.Bounds.Height < this.MinimumSize.Height)
                {
                    // RestoreBounds is the size of the window prior to last minimize action.
                    // But the form may have been resized since then!
                    w = this.RestoreBounds.Width;
                    h = this.RestoreBounds.Height;
                    left = this.RestoreBounds.Location.X;
                    top = this.RestoreBounds.Location.Y;
                }
                else
                {
                    w = this.Bounds.Width;
                    h = this.Bounds.Height;
                    left = this.Location.X;
                    top = this.Location.Y;
                }
                AppCuKey.SetValue(_rvn_Geometry,
                                  String.Format("{0},{1},{2},{3},{4}",
                                                left, top, w, h, (int)this.WindowState));
            }

            // store the position of splitter
            AppCuKey.SetValue(_rvn_Splitter, this.splitContainer3.SplitterDistance.ToString());

        }


        private void FillFormFromRegistry()
        {
            if (AppCuKey != null)
            {
                var s = (string)AppCuKey.GetValue(_rvn_XmlDoc);
                if (s != null) this.tbXmlDoc.Text = s;

                s = (string)AppCuKey.GetValue(_rvn_XPathExpression);
                if (s != null) this.tbXpath.Text = s;

                s = (string)AppCuKey.GetValue(_rvn_Prefix);
                if (s != null) this.tbPrefix.Text = s;

                s = (string)AppCuKey.GetValue(_rvn_Xmlns);
                if (s != null) this.tbXmlns.Text = s;

                // set the geometry of the form
                s = (string)AppCuKey.GetValue(_rvn_Geometry);
                if (!String.IsNullOrEmpty(s))
                {
                    int[] p = Array.ConvertAll<string, int>(s.Split(','),
                                                            new Converter<string, int>((t) => { return Int32.Parse(t); }));
                    if (p != null && p.Length == 5)
                    {
                        this.Bounds = ConstrainToScreen(new System.Drawing.Rectangle(p[0], p[1], p[2], p[3]));
                    }
                }


                // set the splitter
                s = (string)AppCuKey.GetValue(_rvn_Splitter);
                if (!String.IsNullOrEmpty(s))
                {
                    try
                    {
                        int x = Int32.Parse(s);
                        this.splitContainer3.SplitterDistance = x;
                    }
                    catch { }
                }

            }
        }


        private System.Drawing.Rectangle ConstrainToScreen(System.Drawing.Rectangle bounds)
        {
            Screen screen = Screen.FromRectangle(bounds);
            System.Drawing.Rectangle workingArea = screen.WorkingArea;
            int width = Math.Min(bounds.Width, workingArea.Width);
            int height = Math.Min(bounds.Height, workingArea.Height);
            // mmm....minimax            
            int left = Math.Min(workingArea.Right - width, Math.Max(bounds.Left, workingArea.Left));
            int top = Math.Min(workingArea.Bottom - height, Math.Max(bounds.Top, workingArea.Top));
            return new System.Drawing.Rectangle(left, top, width, height);
        }


        public Microsoft.Win32.RegistryKey AppCuKey
        {
            get
            {
                if (_appCuKey == null)
                {
                    _appCuKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(_AppRegyPath, true);
                    if (_appCuKey == null)
                        _appCuKey = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(_AppRegyPath);
                }
                return _appCuKey;
            }
            set { _appCuKey = null; }
        }


        private void btnEvalXpath_Click(object sender, EventArgs e)
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
                    this.toolStripStatusLabel1.Text = "Exception: " + exc1.ToString();
            }
        }



        private void HighlightSelection(XPathNodeIterator selection, XmlNamespaceManager xmlns)
        {
            bool scrolled = false;
            foreach (XPathNavigator node in selection)
            {
                IXmlLineInfo lineInfo = node as IXmlLineInfo;
                if (lineInfo == null || !lineInfo.HasLineInfo()) continue;

                //int ix = this.richTextBox1.GetFirstCharIndexFromLine(lineInfo.LineNumber - 1) +
                //    lineInfo.LinePosition - 1 - 1;

                int ix = this.richTextBox1.MyGetCharIndexFromLine(lineInfo.LineNumber - 1) + lineInfo.LinePosition - 1 - 1;

                if (ix >= 0)
                {
                    int ix2 = 0;

                    //Object o = node.Evaluate("string-length()");
                    string sub = this.richTextBox1.Text.Substring(ix);
                    if (node.NodeType == XPathNodeType.Comment)
                    {
                        ix2 = ix + node.Value.Length;
                        ix++;
                    }

                    else if (node.NodeType == XPathNodeType.Text)
                    {
                        string s = node.Value.XmlEscapeQuotes();
                        ix2 = ix + s.Length  ;
                        ix++;
                    }
                    else if (node.NodeType == XPathNodeType.Attribute)
                    {
                        string s = node.Value.XmlEscapeQuotes();
                        ix2 = ix + node.Name.Length + s.Length + 1 + 2;
                    }
                    else if (node.NodeType == XPathNodeType.Element)
                    {
                        var reader = node.ReadSubtree();
                        //do
                        //{
                        //    reader.Read();                             
                        //} while (reader.NodeType != XmlNodeType.EndElement && reader.NodeType != XmlNodeType.None);


                        if (node.MoveToNext())
                        {
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
                                //while (this.richTextBox1.Text.Substring(ix2, 1) != ">") ix2++;
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


#if JUNK
        // NB:

        // node.OuterXml can include an explicit namespace declaration, which is
        // not actually present in the original document. You cannot get the raw
        // string of the Xml doc using the XPathNavigator.

        // To do this you need an XmlReader  (maybe XpathNavigatorReader.)

        string s2 = node.GetOuterXml(false);

        if (sub.StartsWith(s2))
        {
            // Highlight the matched element
            this.richTextBox1.Select(ix, s2.Length);
        }
        else
        {
            // Difference in escaping.  The InnerXml may include \" where &quot;
            // is in the doc. Must escape the InnerXml to be comparable. 
            string s = node.InnerXml.XmlEscapeQuotes();
            int delta = s.Length - node.InnerXml.Length;
            this.richTextBox1.Select(ix, node.OuterXml.Length + delta);
        }
#endif


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
            int dX = 2;
            int dY = 16;
            try
            {
                this.pnlPrefixList.Controls.Clear();

                int startY = 0;
                foreach (var k in xmlnsPrefixes.Keys)
                {
                    // add a set of controls to the panel for each key/value pair in the list
                    var tb1 = new System.Windows.Forms.TextBox
                        {
                            Anchor = (System.Windows.Forms.AnchorStyles)
                                (System.Windows.Forms.AnchorStyles.Top |
                                 System.Windows.Forms.AnchorStyles.Left),
                            Location = new System.Drawing.Point(this.tbPrefix.Location.X - dX,
                                                                this.tbPrefix.Location.Y - dY + startY),
                            Size = new System.Drawing.Size(this.tbPrefix.Size.Width, this.tbPrefix.Size.Height),
                            Text = k,
                            ReadOnly = true,
                            TabStop = false,
                        };
                    this.pnlPrefixList.Controls.Add(tb1);
                    var lbl1 = new System.Windows.Forms.Label
                        {
                            AutoSize = true,
                            Location = new System.Drawing.Point(this.tbXmlns.Location.X - dX - 18,
                                                                this.tbXmlns.Location.Y - dY + startY),
                            Size = new System.Drawing.Size(24, 13),
                            Text = ":=",
                        };
                    this.pnlPrefixList.Controls.Add(lbl1);

                    var tb2 = new System.Windows.Forms.TextBox
                        {
                            Anchor = (System.Windows.Forms.AnchorStyles)
                                (System.Windows.Forms.AnchorStyles.Top |
                                 System.Windows.Forms.AnchorStyles.Left),
                            Location = new System.Drawing.Point(this.tbXmlns.Location.X - dX,
                                                                this.tbXmlns.Location.Y - dY + startY),
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
                            Location = new System.Drawing.Point(this.btnAddNsPrefix.Location.X - dX,
                                                                this.btnAddNsPrefix.Location.Y - dY + startY),
                            Size = new System.Drawing.Size(this.btnAddNsPrefix.Size.Width,
                                                           this.btnAddNsPrefix.Size.Height),
                            Text = "X",
                            UseVisualStyleBackColor = true,
                            TabStop = false,
                        };
                    btn1.Click += (src, e) => { RemovePrefix(k); };
                    this.pnlPrefixList.Controls.Add(btn1);
                    startY += 20;
                }
            }
            catch (Exception exc1)
            {
                MessageBox.Show(String.Format("There was a problem ! [problem={0}]",
                                              exc1.Message), "Whoops!", MessageBoxButtons.OK);
            }
        }

        private Microsoft.Win32.RegistryKey _appCuKey;
        private static string _AppRegyPath = "Software\\Dino Chiesa\\XPathTester";
        private string _rvn_XmlDoc = "XML File";
        private string _rvn_XPathExpression = "XPathExpression";
        private string _rvn_Prefix = "Prefix";
        private string _rvn_Xmlns = "Xmlns";
        private string _rvn_Geometry = "Geometry";
        private string _rvn_Splitter = "Splitter";
        private XPathDocument xpathDoc;
        private XPathNavigator nav;
        private Dictionary<String, String> _xmlnsPrefixes;
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


#if NONSENSE
        public static string GetOuterXml(this XPathNavigator nav, bool withNamespaces)
        {
            StringWriter output = new StringWriter(System.Globalization.CultureInfo.InvariantCulture);
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.OmitXmlDeclaration = true;
            settings.ConformanceLevel = ConformanceLevel.Auto;
            XmlWriter writer2 = XmlWriter.Create(output, settings);
            try
            {
                writer2.WriteNode(nav, withNamespaces);
            }
            finally
            {
                writer2.Close();
            }
            return output.ToString();
        } 
#endif

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
            //             int ix = 0;
            //             int xline = 0;
            //             do
            //             {
            //                 xline = rtb.GetLineFromCharIndex(ix);
            //                 if (xline == line) return ix;
            //                 int delta = rtb.Text.Substring(ix).IndexOf('\n');
            //                 if (delta < 0) return -1;
            //                 ix += delta + 1;
            //             }
            //             while (ix < rtb.Text.Length);
            //             return -1;

            line++;
            if (line == 0 || line == 1) return 0;
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
            //xline = rtb.GetLineFromCharIndex(ix);
        }

    }

}
