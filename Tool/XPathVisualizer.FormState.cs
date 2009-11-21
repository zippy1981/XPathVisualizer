// XPathVisualizerTool.FormState.cs
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
using System.Windows.Forms;

namespace XPathVisualizer
{
    public partial class XPathVisualizerTool 
    {
        private void SaveFormToRegistry()
        {
            if (AppCuKey == null) return;

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

            // workitem 3392
            // store the position of splitter
            // AppCuKey.SetValue(_rvn_Splitter, this.splitContainer3.SplitterDistance.ToString());

            // the Xpath expression MRU list
            var converted = _xpathExpressionMruList.ToList().ConvertAll(x => x.XmlEscapeIexcl());
            string history = String.Join("¡", converted.ToArray());
            AppCuKey.SetValue(_rvn_History, history);

        }


        private void FillFormFromRegistry()
        {
            if (!stateLoaded)
            {
                if (AppCuKey != null)
                {
                    // fill the various textboxes
                    var s = (string)AppCuKey.GetValue(_rvn_XmlDoc);
                    if (s != null) this.tbXmlDoc.Text = s;

                    s = (string)AppCuKey.GetValue(_rvn_XPathExpression);
                    if (s != null) this.tbXpath.Text = s;

                    s = (string)AppCuKey.GetValue(_rvn_Prefix);
                    if (s != null) this.tbPrefix.Text = s;

                    s = (string)AppCuKey.GetValue(_rvn_Xmlns);
                    if (s != null) this.tbXmlns.Text = s;


                    // get the MRU list of XPath expressions
                    _xpathExpressionMruList = new System.Windows.Forms.AutoCompleteStringCollection();
                    string historyString = (string)AppCuKey.GetValue(_rvn_History, "");
                    if (!String.IsNullOrEmpty(historyString))
                    {
                        string[] items = historyString.Split('¡');
                        if (items != null && items.Length > 0)
                        {
                            //_xpathExpressionMruList.AddRange(items);
                            foreach (string item in items)
                                _xpathExpressionMruList.Add(item.XmlUnescapeIexcl());
                            
                            // insert the most recent expression into the box?  
                            this.tbXpath.Text = _xpathExpressionMruList[_xpathExpressionMruList.Count-1];
                        }
                    }

                
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


                    // workitem 3392 - don't need this
                    // set the splitter
                    // s = (string)AppCuKey.GetValue(_rvn_Splitter);
                    // if (!String.IsNullOrEmpty(s))
                    //   {
                    //     try
                    //     {
                    //         int x = Int32.Parse(s);
                    //         this.splitContainer3.SplitterDistance = x;
                    //     }
                    //     catch { }
                    // }

                    stateLoaded = true;

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


        private Microsoft.Win32.RegistryKey _appCuKey;
        private static string _AppRegyPath = "Software\\Dino Chiesa\\XPathVisualizer";
        private string _rvn_XmlDoc = "XML File";
        private string _rvn_XPathExpression = "XPathExpression";
        private string _rvn_Prefix = "Prefix";
        private string _rvn_Xmlns = "Xmlns";
        private string _rvn_Geometry = "Geometry";
        //private string _rvn_Splitter = "Splitter";
        private string _rvn_History = "History";
        private readonly int _MaxMruListSize = 14;
        //private Ionic.Utils.MruList<String> _xpathExpressionMruList;
        private System.Windows.Forms.AutoCompleteStringCollection _xpathExpressionMruList;
        private bool stateLoaded;
    }
}
